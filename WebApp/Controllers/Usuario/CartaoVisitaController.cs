﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Controllers.Usuario
{
    public class CartaoVisitaController : appController
    {
        public async Task<IActionResult> Index()
        {
            try
            {
                await Check();
            }
            catch (Exception ex)
            {
                string ticket = LogErro("CartaoVisitaController", "Index", ex.Message);
                return RedirectToAction("Erro", "Home", new { Titulo = "Erro em Anúncios", Mensagem = "Foi aberto um ticket de n. " + ticket + " para esse incidente." });
            }
            return View();
        }
    }
}
