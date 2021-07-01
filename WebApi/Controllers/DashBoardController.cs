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
    public class DashBoard : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public DashBoard(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpPost]
        [Route("streamerPublicitario")]
        public async Task<IActionResult> StreamerPublicitario([FromBody] IdModel model)
        {
            try
            {
                IList<StreamerPublicitarioModel> ret = await unitOfWork.DashBoard.StreamerPublicitarioAsync(model.id);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("dashExibicaoCampanha")]
        public async Task<IActionResult> DashExibicaoCampanha([FromBody] IdModel model)
        {
            try
            {
                IList<DashExibicaoCampanhaModel> ret = await unitOfWork.DashBoard.DashExibicaoCampanhaAsync(model.id);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        #region Versao
        [HttpPost]
        [Route("versao")]
        public async Task<IActionResult> Versao()
        {
            await Task.FromResult(false); //Fake task
            return Ok(new Response { Status = "Success", Message = Geral.Configuracao("VERSAO") });
        }
       
        #endregion

    }
}
