using AsyncHttpAzureFunc.LongTaskProcessor;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsyncHttpAzureFunc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubmitController : ControllerBase
    {
        private readonly IPublishEndpoint publishEndpoint;

        public SubmitController(IPublishEndpoint publishEndpoint)
        {
            this.publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<IEnumerable<Guid>> Get()
        {
            var message = new MessageSubmit(Guid.NewGuid(), DateTime.Now);
            await publishEndpoint.Publish(message);
            return new Guid[] { message.MessageId };
        }
    }
}
