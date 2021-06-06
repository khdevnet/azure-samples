using DurableFunctionsSaga.OrderProcess.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;

namespace DurableFunctionsSaga.OrderProcess
{
    public static class ProcessOrderActivity
    {
        [FunctionName(nameof(CreateOrder))]
        public static Guid CreateOrder([ActivityTrigger] object input,
            [Table(OrderEntity.Name, "AzureWebJobsStorage")] out OrderEntity order,
            ILogger log)
        {
            var id = Guid.NewGuid();
            order = new OrderEntity
            {
                OrderId = id,
                RowKey = Guid.NewGuid().ToString("N"),
                Status = OrderStatus.Created.ToString()
            };

            log.LogInformation($"Order process > Create order > id {id}.");

            return id;
        }

        [FunctionName(nameof(UpdateOrderSetStatus))]
        public static void UpdateOrderSetStatus([ActivityTrigger] UpdateOrder updateOrder,
            [Table(OrderEntity.Name, "AzureWebJobsStorage")] out OrderEntity order,
            ILogger log)
        {
            order = new OrderEntity
            {
                OrderId = updateOrder.OrderId,
                RowKey = Guid.NewGuid().ToString("N"),
                Status = updateOrder.Status.ToString()
            };

            log.LogInformation($"OrderProcess {updateOrder.OrderId} > Set status > {updateOrder.Status}.");
        }

        // write orchestration id to Azure storage table, row key is payment id
        [FunctionName(nameof(StartPayment))]
        public static void StartPayment(
            [ActivityTrigger] StartPayment paymentInfo,
            [Table(PaymentStateEntity.Name, "AzureWebJobsStorage")] out PaymentStateEntity paymentState,
            ILogger log)
        {
            log.LogInformation($"OrderProcess > StartPayment.");
            var paymentId = Guid.NewGuid().ToString("N");

            log.LogInformation($"OrderProcess > Save payment id {paymentId} to Payments storage.");

            paymentState = new PaymentStateEntity
            {
                RowKey = paymentId,
                OrchestrationId = paymentInfo.OrchestrationId,
                OrderId = paymentInfo.OrderId
            };

            var host = Helper.GetHostUrl();

            var webhookAddress = $"{host}/api/{PaymentStateConstants.WebhookAction}/{paymentId}";
            var paymentSuccessfullLink = $"{webhookAddress}?result={OrderStatus.PaymentSucessfull}";
            var paymentFailedLink = $"{webhookAddress}?result={OrderStatus.PaymentFailed}";

            log.LogInformation($"OrderProcess > Payment sucessful link {paymentSuccessfullLink}.");
            log.LogInformation($"OrderProcess > Payment failed link {paymentFailedLink}.");
        }
    }
}
