using DurableFunctionsSaga.OrderProcess.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DurableFunctionsSaga.OrderProcess
{
    public static class ProcessOrderOrchestrator
    {
        [FunctionName(nameof(ProcessOrderOrchestrator.ProcessOrder))]
        public static async Task ProcessOrder(
            [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            log = context.CreateReplaySafeLogger(log);

            var orderId = await context.CallActivityAsync<Guid>(nameof(ProcessOrderActivity.CreateOrder), null);
            await context.CallActivityAsync(nameof(ProcessOrderActivity.UpdateOrderSetStatus), new UpdateOrder { OrderId = orderId, OrchestrationId = context.InstanceId, Status = OrderStatus.WaitForPayment.ToString() });
            await context.CallActivityAsync(nameof(ProcessOrderActivity.StartPayment), new StartPayment { OrderId = orderId, OrchestrationId = context.InstanceId });

            var paymentResult = await WaitForPaymentResultEvent(context, log);

            await context.CallActivityAsync(
                nameof(ProcessOrderActivity.UpdateOrderSetStatus),
                new UpdateOrder
                {
                    OrderId = orderId,
                    OrchestrationId = context.InstanceId,
                    Status = paymentResult
                });
        }

        private static async Task<string> WaitForPaymentResultEvent(
            IDurableOrchestrationContext context,
            ILogger log)
        {
            string paymentResult = OrderStatus.PaymentFailed.ToString();
            try
            {
                log.LogInformation($"{nameof(ProcessOrderOrchestrator)} > wait for payment result event");

                paymentResult = await context.WaitForExternalEvent<string>(
                    PaymentStateConstants.PaymentResultEventName,
                    TimeSpan.FromSeconds(30));

                log.LogInformation($"{nameof(ProcessOrderOrchestrator)} > payment result > {paymentResult}");
            }
            catch (TimeoutException)
            {
                log.LogWarning("Timed out waiting for payment response.");
                paymentResult = OrderStatus.PaymentTimeout.ToString();
            }

            return paymentResult;
        }
    }
}
