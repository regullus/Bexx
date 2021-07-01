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
    public class RedeSocial : ControllerBase
    {

        #region base
        
        private readonly IUnitOfWork unitOfWork;

        public RedeSocial(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        #endregion

        #region Custom

        [HttpPost]
        [Route("getByIdTipoAsync")]
        public async Task<IActionResult> getByIdTipoAsync([FromBody] IdModel model)
        {
            try
            {
                IReadOnlyList<RedeSocialModel> ret = await unitOfWork.RedeSocial.GetRedeSocialByIdTipoAsync(model.id);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("getRedeSocialTipo")]
        public async Task<IActionResult> GetRedeSocialTipo()
        {
            try
            {
                IReadOnlyList<RedeSocialTipoModel> ret = await unitOfWork.RedeSocial.GetRedeSocialTipoAsync();
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }


        [HttpPost]
        [Route("getRedeSocialSelecionado")]
        public async Task<IActionResult> GetRedeSocialSelecionado([FromBody] IdAnuncioModel model)
        {
            try
            {
                IReadOnlyList<RedeSocialSelecionadoModel> ret = await unitOfWork.RedeSocial.GetRedeSocialSelecionadoAsync(model.idAnuncio);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        //DeleteRedeSocialSelecionadoAsync

        [HttpPost]
        [Route("deleteRedeSocialSelecionado")]
        public async Task<IActionResult> DeleteRedeSocialSelecionado([FromBody] IdAnuncioModel model)
        {
            //Remove todas as associações entre o a rede social e o anuncio - tabela anuncio_rede_social
            try
            {
                int ret = await unitOfWork.RedeSocial.DeleteRedeSocialSelecionadoAsync(model.idAnuncio);
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
        [Route("createRedeSocial")]
        public async Task<IActionResult> CreateRedeSocial([FromBody] RedeSocialModel model)
        {
            try
            {
                int ret = await unitOfWork.RedeSocial.AddAsync(model);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("updateRedeSocial")]
        public async Task<IActionResult> UpdateRedeSocial([FromBody] RedeSocialModel model)
        {
            try
            {
                int ret = await unitOfWork.RedeSocial.UpdateAsync(model);
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
