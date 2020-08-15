using AsyncHttpAzureFunc.LongTaskProcessor;
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
        private readonly ILongTaskHandler longTaskHandler;
        private readonly CancelationTokenStore cancelationTokenStore;

        public SubmitController(ILongTaskHandler longTaskHandler, CancelationTokenStore cancelationTokenStore)
        {
            this.longTaskHandler = longTaskHandler;
            this.cancelationTokenStore = cancelationTokenStore;
        }

        [HttpGet]
        public IEnumerable<Guid> Get()
        {
            var message = new MessageSubmit(Guid.NewGuid(), DateTime.Now);
            longTaskHandler.Publish(message);
            return new Guid[] { message.MessageId };
        }

        [HttpGet]
        [Route("cancel/{id}")]
        public IEnumerable<Guid> Cancel(Guid id)
        {
            cancelationTokenStore.Cancel(id);
            return new Guid[] { id };
        }
    }
}
