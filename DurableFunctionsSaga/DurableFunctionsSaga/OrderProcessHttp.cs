using System;
using System.Net.Http;
using System.Threading.Tasks;
using DurableFunctionsSaga.OrderProcess;
using DurableFunctionsSaga.OrderProcess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace DurableFunctionsSaga
{
    public static class OrderProcessHttp
    {
        [FunctionName(nameof(Http_StartOrderProcessOrchestrator))]
        public static async Task<HttpResponseMessage> Http_StartOrderProcessOrchestrator(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "process-order/{id}")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync(nameof(ProcessOrderOrchestrator.ProcessOrder), null);

            log.LogInformation($"Started order process orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        //read payment state from Azure storage table, rowkey read from request url Id
        [FunctionName(nameof(Http_PaymentStatusWebhook))]
        public static async Task<IActionResult> Http_PaymentStatusWebhook(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = PaymentStateConstants.WebhookAction + "/{id}")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient client,
            [Table(PaymentStateEntity.Name, PaymentStateEntity.Key, "{id}", Connection = "AzureWebJobsStorage")] PaymentStateEntity paymentState,
            ILogger log)
        {
            // nb if the payment state doesn't exist, framework just returns a 404 before we get here
            string result = req.RequestUri.ParseQueryString()["result"].ToString();

            if (result == null)
            {
                return new BadRequestObjectResult("Need an payment state.");
            }

            log.LogWarning($"Sending payment result to {paymentState.OrchestrationId} of {result}");

            // send the Payment state external event to this orchestration
            await client.RaiseEventAsync(paymentState.OrchestrationId, PaymentStateConstants.PaymentResultEventName, result);
            return new OkResult();
        }
    }
}