using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace _2.RabbitMq.Consumer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Consumer push mes to RabbitMq")]
    public class RabbitMqConsumerController : ControllerBase
    {
        public RabbitMqConsumerController()
        { 
        
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return Ok("Starting service");
        }
    }
}
