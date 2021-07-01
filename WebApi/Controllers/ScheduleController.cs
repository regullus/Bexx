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
    public class Schedule : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public Schedule(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpPost]
        [Route("usuarioCalculo")]
        public async Task<IActionResult> UsuarioCalculo([FromBody] ImputUsuarioCalculoModel model)
        {
            try
            {
                int ret = await unitOfWork.Schedule.UsuarioCalculoAsync(model);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

    }
}
