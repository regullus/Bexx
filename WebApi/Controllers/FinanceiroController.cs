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
    public class Financeiro : ControllerBase
    {

        #region base

        private readonly IUnitOfWork unitOfWork;
        public Financeiro(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        #endregion

        #region Custom
        #region Saldo

        [HttpPost]
        [Route("getSaldoByIdAnuncioConta")]
        public async Task<IActionResult> GetSaldoByIdAnuncioConta([FromBody] SaldoApiModel model)
        {
            try
            {
                FinanceiroSaldoModel ret = await unitOfWork.FinanceiroSaldo.GetSaldoByIdAnuncioAsync(model.idFinanceiroConta, model.idAnuncio);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("getSaldoByIdConta")]
        public async Task<IActionResult> GetSaldoByIdConta([FromBody] IdModel model)
        {
            try
            {
                IReadOnlyList<FinanceiroSaldoModel> ret = await unitOfWork.FinanceiroSaldo.GetSaldoByIdContaAsync(model.id);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        #endregion

        [HttpPost]
        [Route("getContaByIdUsuario")]
        public async Task<IActionResult> GetContaByIdUsuario([FromBody] IdModel model)
        {
            try
            {
                FinanceiroContaModel ret = await unitOfWork.FinanceiroConta.GetByIdUsuarioAsync(model.id);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        #region FinanceiroLancamento

        [HttpPost]
        [Route("addWithSaldoAsync")]
        public async Task<IActionResult> AddWithSaldoAsync([FromBody] FinanceiroLancamentoModel model)
        {
            try
            {
                int ret = await unitOfWork.FinanceiroLancamento.AddWithSaldoAsync(model);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("getLancamentoByIdContaAsync")]
        public async Task<IActionResult> GetLancamentoByIdContaAsync([FromBody] valorIntModel model)
        {
            try
            {
                IReadOnlyList<FinanceiroLancamentoModel> ret = await unitOfWork.FinanceiroLancamento.GetLancamentoByIdContaAsync(model.id, model.valor);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        #endregion

        #region FinanceiroContaTipo

        [HttpPost]
        [Route("getFinanceiroContaTipo")]
        public async Task<IActionResult> GetFinanceiroContaTipo()
        {
            try
            {
                IReadOnlyList<FinanceiroContaTipoModel> ret = await unitOfWork.FinanceiroConta.GetFinanceiroContaTipoAsync();
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        #endregion

        #region FinanceiroMoeda

        [HttpPost]
        [Route("getFinanceiroMoedaById")]
        public async Task<IActionResult> GetFinanceiroMoedaById([FromBody] IdModel model)
        {
            try
            {
                FinanceiroMoedaModel ret = await unitOfWork.FinanceiroConta.GetFinanceiroMoedaByIdAsync(model.id);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        #endregion

        #region FinanceiroLancamentoTipo

        [HttpPost]
        [Route("getFinanceiroLancamentoTipo")]
        public async Task<IActionResult> GetFinanceiroLancamentoTipo()
        {
            try
            {
                IReadOnlyList<FinanceiroLancamentoTipoModel> ret = await unitOfWork.FinanceiroLancamento.GetFinanceiroLancamentoTipoAsync();
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        #endregion


        #endregion

        #region InsUpdDel

        [HttpPost]
        [Route("createFinanceiroConta")]
        public async Task<IActionResult> CreateFinanceiroConta([FromBody] FinanceiroContaModel model)
        {
            try
            {
                int ret = await unitOfWork.FinanceiroConta.AddAsync(model);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("updateFinanceiroConta")]
        public async Task<IActionResult> UpdateFinanceiroConta([FromBody] FinanceiroContaModel model)
        {
            try
            {
                int ret = await unitOfWork.FinanceiroConta.UpdateAsync(model);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("createFinanceiroLancamento")]
        public async Task<IActionResult> CreateFinanceiroLancamento([FromBody] FinanceiroLancamentoModel model)
        {
            try
            {
                int ret = await unitOfWork.FinanceiroLancamento.AddAsync(model);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("updateFinanceiroLancamento")]
        public async Task<IActionResult> UpdateFinanceiroLancamento([FromBody] FinanceiroLancamentoModel model)
        {
            try
            {
                int ret = await unitOfWork.FinanceiroLancamento.UpdateAsync(model);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("createFinanceiroSaldo")]
        public async Task<IActionResult> CreateFinanceiroSaldo([FromBody] FinanceiroSaldoModel model)
        {
            try
            {
                int ret = await unitOfWork.FinanceiroSaldo.AddAsync(model);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("updateFinanceiroSaldo")]
        public async Task<IActionResult> UpdateFinanceiroSaldo([FromBody] FinanceiroSaldoModel model)
        {
            try
            {
                int ret = await unitOfWork.FinanceiroSaldo.UpdateAsync(model);
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
