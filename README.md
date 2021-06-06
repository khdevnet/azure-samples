# Axure function samples
## Durable function order process saga
![](https://github.com/khdevnet/azure-samples/blob/master/DurableFunctionsSaga/diagram.png)
* Create order events in Azure Storage Orders Table
* Save OrchestrationId in Azure Storage PaymentState Table
* Using HTTP GET request as hook from payment system to finish payment process.
* Raise Durable function event API
* Use Timer to failed uncomplete Payment requests

# General info
* [functions-overview](https://docs.microsoft.com/en-us/azure/azure-functions/functions-overview)
* [durable-functions-overview](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-overview?tabs=csharp)
* [functions-consumption-costs](https://docs.microsoft.com/en-us/azure/azure-functions/functions-consumption-costs)
* [functions-dotnet-dependency-injection](https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection)
