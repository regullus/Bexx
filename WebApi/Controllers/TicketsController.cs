#region using

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models;
using WebApi.Authentication;
using Interfaces;
using Microsoft.AspNetCore.Http;
using WebApi.Helper;

#endregion

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class Tickets : ControllerBase
    {

        #region base
        
        private readonly IUnitOfWork unitOfWork;

        public Tickets(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        #endregion

        #region Custom
                
        #endregion

        #region InsUpdDel

        [HttpPost]
        [Route("createticket")]
        public async Task<IActionResult> CreateTicket([FromBody] TicketsModel model)
        {
            string retorno = "ok";
            try
            {
                int ret = await unitOfWork.Tickets.AddAsync(model);
                return Ok(retorno);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        #endregion
    }
}
