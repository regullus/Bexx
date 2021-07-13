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
    public class Base : ControllerBase
    {

        #region base
        
        private readonly IUnitOfWork unitOfWork;

        public Base(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        #endregion

        #region Custom

        #region Pais

        [HttpPost]
        [Route("getPaisAsync")]
        public async Task<IActionResult> GetPaisAsync()
        {
            try
            {
                IReadOnlyList<PaisModel> ret = await unitOfWork.Base.GetPaisAsync(); 
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


        #endregion

    }
}
