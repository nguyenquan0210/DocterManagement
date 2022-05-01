using DoctorManagement.ViewModels.System.Models;
using Microsoft.AspNetCore.Mvc;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace DoctorManagement.BackendAPI.Controllers
{
    public class MessageController : Controller
    {
        private readonly ITwilioRestClient _client;
        public MessageController(ITwilioRestClient client)
        {
            _client = client;
        }
        [HttpPost("api/send-sms")]
        public IActionResult SendSms(SmsMessage model)
        {
            var message = MessageResource.Create(
                to: new PhoneNumber(model.To),
                from: new PhoneNumber(model.From),
                body: model.Message,
                client: _client); // pass in the custom client

            return Ok(message.Sid);
        }
    }
}
