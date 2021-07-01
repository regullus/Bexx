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
    public class Anuncio : ControllerBase
    {

        #region base
        
        private readonly IUnitOfWork unitOfWork;
        public Anuncio(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        #endregion

        #region Custom

        [HttpPost]
        [Route("getAnuncioByIdUsuario")]
        public async Task<IActionResult> GetAnuncioByIdUsuario([FromBody] IDUsuarioModel model)
        {
            try
            {
                IReadOnlyList<AnuncioSaldoModel> ret = await unitOfWork.Anuncio.GetbyIdUsuarioAsync(model.idUsuario);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("getByidAnuncio")]
        public async Task<IActionResult> GetByIdAnuncio([FromBody] IdModel model)
        {
            try
            {
                AnuncioSaldoModel ret = await unitOfWork.Anuncio.GetWithSaldoByIdAsync(model.id);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("getAnuncioMidia")]
        public async Task<IActionResult> GetAnuncioMidia()
        {
            try
            {
                IReadOnlyList<AnuncioMidiaModel> ret = await unitOfWork.Anuncio.GetAnuncioMidiaAsync();
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("getAnuncioSituacao")]
        public async Task<IActionResult> GetAnuncioSituacao()
        {
            try
            {
                IReadOnlyList<AnuncioSituacaoModel> ret = await unitOfWork.Anuncio.GetAnuncioSituacaoAsync();
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("getAnuncioTipo")]
        public async Task<IActionResult> GetAnuncioTipo()
        {
            try
            {
                IReadOnlyList<AnuncioTipoModel> ret = await unitOfWork.Anuncio.GetAnuncioTipoAsync();
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("getAnuncioTipoById")]
        public async Task<IActionResult> GetAnuncioTipoById([FromBody] IdModel model)
        {
            try
            {
                AnuncioTipoModel ret = await unitOfWork.Anuncio.GetAnuncioTipoByIdAsync(model.id);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("getForStreamerAnuncioTipo")]
        public async Task<IActionResult> GetForStreamerAnuncioTipo()
        {
            try
            {
                IReadOnlyList<AnuncioTipoModel> ret = await unitOfWork.Anuncio.GetForStreamerAnuncioTipoAsync();
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("getAnuncioTempo")]
        public async Task<IActionResult> GetAnuncioTempo()
        {
            try
            {
                IReadOnlyList<AnuncioTempoModel> ret = await unitOfWork.Anuncio.GetAnuncioTempoAsync();
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("getAnuncioQuantidade")]
        public async Task<IActionResult> GetAnuncioQuantidade()
        {
            try
            {
                IReadOnlyList<AnuncioQuantidadeModel> ret = await unitOfWork.Anuncio.GetAnuncioQuantidadeAsync();
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("getAnuncioUsuario")]
        public async Task<IActionResult> GetAnuncioUsuario([FromBody] IdUsuarioIdAnuncioTipoIdRedeSocialModel model)
        {
            try
            {
                IReadOnlyList<AnuncioSelecionadoModel> ret = await unitOfWork.Anuncio.GetAnuncioSelecionadoAsync(model.idUsuario, model.idAnuncioTipo, model.idRedeSocial);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("delAnuncioUsuario")]
        public async Task<IActionResult> DelAnuncioUsuario([FromBody] IdUsuarioIdAnuncioTipoIdRedeSocialModel model)
        {
            try
            {
                int ret = await unitOfWork.AnuncioUsuario.DeleteAsync(model.idUsuario, model.idAnuncioTipo, model.idRedeSocial);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("addAnuncioUsuario")]
        public async Task<IActionResult> AddAnuncioUsuario([FromBody] AnuncioUsuarioModel model)
        {
            try
            {
                int ret = await unitOfWork.AnuncioUsuario.AddAsync(model);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("getAnuncioGrupo")]
        public async Task<IActionResult> GetAnuncioGrupo()
        {
            try
            {
                IReadOnlyList<AnuncioGrupoModel> ret = await unitOfWork.Anuncio.GetAnuncioGrupoAsync();
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("getAnuncioPrecoByIdUsuario")]
        public async Task<IActionResult> GetAnuncioPrecoByIdUsuario([FromBody] IdModel model)
        {
            try
            {
                IReadOnlyList<AnuncioPrecoModel> ret = await unitOfWork.Anuncio.GetAnuncioPrecoByIdUsuarioAsync(model.id);
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
        [Route("createAnuncio")]
        public async Task<IActionResult> CreateAnuncio([FromBody] AnuncioSaldoModel model)
        {
            try
            {
                int ret = await unitOfWork.Anuncio.AddWithSaldoAsync(model);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("updateAnuncio")]
        public async Task<IActionResult> UpdateAnuncio([FromBody] AnuncioSaldoModel model)
        {
            try
            {
                int ret = await unitOfWork.Anuncio.UpdateWithSaldoAsync(model);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("deleteLogicAnuncio")]
        public async Task<IActionResult> DeleteLogicAnuncio([FromBody] IdModel model)
        {
            try
            {
                int ret = await unitOfWork.Anuncio.DeleteLogicAsync(model.id);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("createAnuncioRedeSocial")]
        public async Task<IActionResult> CreateAnuncioRedeSocial([FromBody] AnuncioRedeSocialModel model)
        {
            try
            {
                int ret = await unitOfWork.AnuncioRedeSocial.AddAsync(model);
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
