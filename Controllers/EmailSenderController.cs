using EmailSender.Interface;
using EmailSender.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmailSender.Model;

namespace EmailSender.Controllers
{
    public class EmailSenderController : Controller
    {
        IEmailSender _emailSender;
        public EmailSenderController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        [HttpPost, Route("SendEmail")]
        public async Task<IActionResult> SendEmailAsync([FromForm] EmailRequest request)
        {
            try
            {
                 string messageStatus = await _emailSender.SendEmailAsync(request);
                return Ok(messageStatus);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }
    }
}
