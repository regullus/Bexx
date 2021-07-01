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
    public class Tronar : ControllerBase
    {

        #region base

        private readonly IUnitOfWork unitOfWork;

        public Tronar(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        #endregion

        #region Custom

        [HttpPost]
        [Route("getById")]
        public async Task<IActionResult> GetbyId([FromBody] IdModel model)
        {
            try
            {
                TronarModel ret = await unitOfWork.Tronar.GetByIdAsync(model.id);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("getByLinkTronado")]
        public async Task<IActionResult> getByLinkTronado([FromBody] linkTronadoModel model)
        {
            try
            {
                TronarModel ret = await unitOfWork.Tronar.GetByLinkTronadoAsync(model.linkTronado);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("getByIdAnuncioTipo")]
        public async Task<IActionResult> GetbyIdAnuncioTipo([FromBody] TronarUsuarioAnuncioTipoModel model)
        {
            try
            {
                TronarModel ret = await unitOfWork.Tronar.GetByIdAnuncioTipoAsync(model.idUsuario, model.idAnuncioTipo);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("getByIdAnuncioTipoRedeSocial")]
        public async Task<IActionResult> GetbyIdAnuncioTipoRedeSocial([FromBody] TronarUsuarioAnuncioTipoRedeSocialModel model)
        {
            try
            {
                TronarModel ret = await unitOfWork.Tronar.GetByIdAnuncioTipoRedeSocialAsync(model.idUsuario, model.idAnuncioTipo, model.redeSocial);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("getUsuarioRedeSocial")]
        public async Task<IActionResult> GetUsuarioRedeSocial([FromBody] TronarUsuarioAnuncioTipoRedeSocialModel model)
        {
            try
            {
                TronarUsuarioRedeSocialModel ret = await unitOfWork.Tronar.GetUsuarioRedeSocialAsync(model.idUsuario, model.redeSocial);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("updateUsuarioRedeSocial")]
        public async Task<IActionResult> UpdateUsuarioRedeSocial([FromBody] TronarUsuarioRedeSocialModel model)
        {
            try
            {
                int ret = await unitOfWork.Tronar.UpdateUsuarioRedeSocialAsync(model.idUsuario, model.redeSocial, model.usuarioRedeSocial);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }


        [HttpPost]
        [Route("getTronarSituacao")]
        public async Task<IActionResult> GetTronarSituacao()
        {
            try
            {
                IReadOnlyList<TronarSituacaoModel> ret = await unitOfWork.Tronar.GetTronarSituacaoAsync();
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        #endregion

        #region InsUpdDel

        [HttpPost]
        [Route("createTronar")]
        public async Task<IActionResult> CreateTronar([FromBody] TronarModel model)
        {
            try
            {
                int ret = await unitOfWork.Tronar.AddAsync(model);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("updateTronar")]
        public async Task<IActionResult> UpdateTronar([FromBody] TronarModel model)
        {
            try
            {
                int ret = await unitOfWork.Tronar.UpdateAsync(model);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("deleteLogicTronar")]
        public async Task<IActionResult> DeleteLogicTronar([FromBody] IdModel model)
        {
            try
            {
                int ret = await unitOfWork.Tronar.DeleteLogicAsync(model.id);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        #endregion

    }
}
