using _1.RabbitMq.Producer.Api.Infrastructure.Application.Interfaces;
using _3.RabbitMq.Service.Bus.Events;
using _3.RabbitMq.Service.Bus.RabbitMQ;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _1.RabbitMq.Producer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Producer push mes to RabbitMq")]
    public class RabbitMqProducerController : ControllerBase
    {
        private readonly IUserEmailService _emailService;

        public RabbitMqProducerController(IUserEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> SendEmailAsync()
        {
            await _emailService.SendEmailAsync();
            return Ok("Success!!!");
        }
    }
}