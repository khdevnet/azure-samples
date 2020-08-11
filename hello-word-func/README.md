
# General info
* [functions-overview](https://docs.microsoft.com/en-us/azure/azure-functions/functions-overview)
* [durable-functions-overview](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-overview?tabs=csharp)
* [functions-consumption-costs](https://docs.microsoft.com/en-us/azure/azure-functions/functions-consumption-costs)
* [functions-dotnet-dependency-injection](https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection)
* [third-party-logging-providers](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-3.1#third-party-logging-providers)
* [log4net-with-aspnetcore-for-logging](https://dotnetthoughts.net/how-to-use-log4net-with-aspnetcore-for-logging/)
* [Log4NetAdoNetAppender](https://github.com/microknights/Log4NetAdoNetAppender)
# Testing
* [azure-functions-tests](https://github.com/Azure-Samples/azure-functions-tests)
* [azure-functions-tests-queue-trigger](https://github.com/Azure-Samples/azure-functions-tests)
* [azure-functions-tests-v3](https://docs.microsoft.com/en-us/azure/azure-functions/functions-test-a-function)




# Log4net database logging-providers
Create database Logs
```sql
  	 CREATE TABLE [dbo].[Log] ( 
	   [ID] [int] IDENTITY (1, 1) NOT NULL ,
	   [Date] [datetime] NOT NULL ,
	   [Thread] [varchar] (255) NOT NULL ,
	   [Level] [varchar] (20) NOT NULL ,
	   [Logger] [varchar] (255) NOT NULL ,
	   [Message] [varchar] (4000) NOT NULL,
	   [Exception] [varchar] (4000) NOT NULL 
	 ) ON [PRIMARY]
```
