﻿#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Model;
using WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Helper;

#endregion

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IMailService mailService;
        private const string cLocal = "MailController";

        public MailController(IMailService mailService)
        {
            this.mailService = mailService;
        }
        [HttpPost("send")]
        public async Task<IActionResult> SendMail([FromForm] MailRequest request)
        {
            try
            {
                await mailService.SendEmailAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                Geral.LogErro(cLocal, "UpdatePassword - " + request.ToEmail, ex.Message, 1);
                throw;
            }

        }
        [HttpPost("welcome")]
        public async Task<IActionResult> SendWelcomeMail([FromForm] WelcomeRequest request)
        {
            try
            {
                await mailService.SendWelcomeEmailAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                Geral.LogErro(cLocal, "UpdatePassword - " + request.ToEmail, ex.Message, 1);
                throw;
            }

        }

    }
}