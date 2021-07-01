#region using
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace WebApp.Controllers.Comum
{
    [Authorize]
    public class SobreController : appController
    {
        public async Task<IActionResult> Index()
        {
            try
            {
                await Check();
            }
            catch (Exception ex)
            {
                string ticket = LogErro("SobreController", "Index", ex.Message);
                return RedirectToAction("Erro", "Home", new { Titulo = "Erro em Anúncios", Mensagem = "Foi aberto um ticket de n. " + ticket + " para esse incidente." });
            }
            return View();
        }
    }
}
