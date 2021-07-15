#region using

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Security.Claims;
using RestSharp;
using System.Net;
using Newtonsoft.Json;
using Utils;

#endregion

namespace WebApp.Controllers
{
    [Authorize]
    public class HomeController : appController
    {

        #region Base

        public HomeController()
        {
        }

        #endregion

        #region Actions

        public async Task<IActionResult> Index()
        {
            try
            {
                //Toda action deve conter essa validacao
                await Check();

                switch (User.Identity.Role())
                {
                    case "Streamer":
                        ViewBag.Destaque = "true";
                        ViewBag.DestaqueTitulo = "Novas campanhas...";
                        ViewBag.DestaqueMensagem = "...verifique as novas campanhas incluidas.";
                        ViewBag.DestaqueLinkOk = "true";
                        ViewBag.DestaqueLinkTitulo = "campanhas";
                        ViewBag.DestaqueLink = Url.Content("~/Campanhas");
                        break;
                    case "Usuario":
                        ViewBag.Destaque = "true";
                        ViewBag.DestaqueTitulo = "Não se esqueça...";
                        ViewBag.DestaqueMensagem = "...de atualizar seus contatos.";
                        ViewBag.DestaqueLinkOk = "true";
                        ViewBag.DestaqueLinkTitulo = "contatos";
                        ViewBag.DestaqueLink = Url.Content("~/Leads");
                        break;
                    case "Administrador":
                        ViewBag.Destaque = "false";
                        break;
                    case "Publicitario":
                        ViewBag.Destaque = "true";
                        ViewBag.DestaqueTitulo = "Conte com nossos designers";
                        ViewBag.DestaqueMensagem = "Caso necessite para fazer suas artes graficas.";
                        ViewBag.DestaqueLinkOk = "true";
                        ViewBag.DestaqueLinkTitulo = "Entre em contato";
                        ViewBag.DestaqueLink = "https://tronar.com";
                        break;
                    case "Master":
                        ViewBag.Destaque = "true";
                        ViewBag.DestaqueTitulo = "Nova Funcionalidade:";
                        ViewBag.DestaqueMensagem = "Email Prospecção";
                        ViewBag.DestaqueLinkOk = "true";
                        ViewBag.DestaqueLinkTitulo = "acesse";
                        ViewBag.DestaqueLink = Url.Content("~/EmailProspeccao");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                string ticket = LogErro("HomeController", "Index", ex.Message);
                return RedirectToAction("Erro", "Home", new { Titulo = "Erro em Anúncios", Mensagem = "Foi aberto um ticket de n. " + ticket + " para esse incidente." });
            }

            return View();
        }

        #endregion

        #region utils

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #endregion

        #region Mensagens

        [AllowAnonymous]
        public IActionResult Erro(string Titulo, string Mensagem)
        {
            ViewData["Titulo"] = Titulo;
            ViewData["Mensagem"] = Mensagem;
            return View();
        }

        [AllowAnonymous]
        public IActionResult Mensagem(string Titulo, string Mensagem)
        {
            ViewData["Titulo"] = Titulo;
            ViewData["Mensagem"] = Mensagem;
            return View();
        }

        [AllowAnonymous]
        public IActionResult Sucesso(string Titulo, string Mensagem1, string Mensagem2, string NomeLink, string Link)
        {
            ViewData["Titulo"] = Titulo;
            ViewData["Mensagem1"] = Mensagem1;
            ViewData["Mensagem2"] = Mensagem2;
            ViewData["NomeLink"] = NomeLink;
            ViewData["Link"] = Link;
            return View();
        }

        #endregion

        #region Teste

        [Authorize(Roles = "Administrador, Todos")]
        public ActionResult Usuarios()
        {
            //var usuario = new UsuarioClaim();
            return View();
        }

        #endregion

        #region API

        #endregion

    }
}
