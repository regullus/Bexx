#region using

using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;
using RestSharp;
using System.Net;
using Newtonsoft.Json;
using Models;
using Utils;
using System.Web;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using System.Threading;

#endregion

namespace WebApp
{
    public static class Local
    {
        public static string Configuracao(string chave)
        {
            string valor = "";

            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile($"appsettings.json").Build();
            valor = builder.GetSection("Configuration").GetValue<string>(chave);

            return valor;
        }
    }

    public class appController : Controller
    {
        #region Base

        public readonly RestClient _clientAPI;

        public appController()
        {
            _clientAPI = new RestClient(Local.Configuracao("URL_API"));
        }

        #endregion

        #region Local

        public static void LogFile(string strFileName, string strMensagem)
        {
            strFileName = Helpers.DataAtual() + "_" + strFileName + ".txt";

            string strPath = Local.Configuracao("PATH_CDN") + "\\Log";
            System.IO.StreamWriter sw;
            try
            {
                sw = new System.IO.StreamWriter(strPath + strFileName, true);
                sw.WriteLine(strMensagem);
                sw.Close();
            }
            catch (Exception ex)
            {
                string strErro = ex.Message;
            }
            finally
            {
                sw = null;
            }
        }

        #endregion

        #region API

        public async Task<string> CheckToken()
        {
            string strRet = "";

            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/DashBoard/versao", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "Bearer " + User.Identity.Token());
                request.RequestFormat = DataFormat.Json;
                var response = _clientAPI.Execute(request);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        var _retorno = JsonConvert.DeserializeObject<StatusModel>(response.Content);
                        if (String.IsNullOrEmpty(_retorno.message))
                        {
                            strRet = "";
                        }
                        else
                        {
                            strRet = _retorno.message;
                            return strRet;
                        }
                        break;
                    case HttpStatusCode.Unauthorized:
                        bool retRefresh = await Refresh();
                        if (retRefresh)
                        {
                            return "RefreshToken";
                        }
                        break;
                    default:
                        strRet = "";
                        break;
                }
            }
            catch (Exception ex)
            {
                strRet = ex.Message;
            }
            //LogErro strRet
            LogErro("appController", "CheckToken", strRet);
            return strRet;
        }

        public UsuarioModel UsuarioApi()
        {
            UsuarioModel usuario;
            string strRet = "Um erro inesperado ocorreu!";
            try
            {
                int id = User.Identity.Id();
                //Cria a transação.
                var request = new RestRequest("/api/Usuario/getById", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "Bearer " + token);
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { id = id });
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    usuario = JsonConvert.DeserializeObject<UsuarioModel>(response.Content);
                    return usuario;
                }
                else
                {
                    strRet = TrataErro(response);
                }
            }
            catch (Exception ex)
            {
                strRet = ex.Message;
            }

            LogErro("Geral", "UsuarioApi", strRet);

            return null;
        }

        public async Task<bool> Refresh()
        {
            RestClient _clientAPI = new RestClient(Local.Configuracao("URL_API"));
            UsuarioClaimModel usuarioClaim;
            bool ret = false;
            try
            {
                TokenRequest tokenRequest = new TokenRequest()
                {
                    idUsuario = User.Identity.Id(),
                    Token = User.Identity.Token(),
                    RefreshToken = User.Identity.RefreshToken()
                };

                //Chamar Refresh
                var request = new RestRequest("/api/authenticate/refreshtoken", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.RequestFormat = DataFormat.Json;
                var json = JsonConvert.SerializeObject(tokenRequest);
                request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
                var response = _clientAPI.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //Atualizar Clains
                    usuarioClaim = JsonConvert.DeserializeObject<UsuarioClaimModel>(response.Content);
                    string teste = User.Identity.Token();
                    User.Identity.TokenUpdate(usuarioClaim.Token);
                    User.Identity.AvatarUpdate(usuarioClaim.Avatar);
                    teste = User.Identity.Token();
                    if (usuarioClaim != null)
                    {
                        ret = await SetClaim(usuarioClaim, User.Identity.Login(), User.Identity.Lembrar());
                    }
                }
                else
                {
                    //Verificar erro
                    string strRet = TrataErro(response);
                }
            }
            catch (Exception)
            {
                ret = false;
            }
            return ret;
        }

        public async Task<bool> SetClaim(UsuarioClaimModel usuarioClaim, string name, string lembrar)
        {
            try
            {
                if (HttpContext.Request.Cookies.Count > 0)
                {
                    var siteCookies = HttpContext.Request.Cookies.Where(c => c.Key.Contains("TronarUserLoginCookie"));
                    foreach (var cookie in siteCookies)
                    {
                        Response.Cookies.Delete(cookie.Key);
                    }
                }

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
                HttpContext.Session.Clear();

                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuarioClaim.Email));
                identity.AddClaim(new Claim(ClaimTypes.Name, name));
                identity.AddClaim(new Claim(type: "Id", value: usuarioClaim.Id.ToString()));
                identity.AddClaim(new Claim(type: "idAtivo", value: usuarioClaim.idAtivo.ToString()));
                identity.AddClaim(new Claim(type: "Nome", value: usuarioClaim.Nome));
                identity.AddClaim(new Claim(type: "Email", value: usuarioClaim.Email));
                identity.AddClaim(new Claim(type: "Senha", value: usuarioClaim.Senha));
                identity.AddClaim(new Claim(type: "Token", value: usuarioClaim.Token));
                identity.AddClaim(new Claim(type: "RefreshToken", value: usuarioClaim.RefreshToken));
                identity.AddClaim(new Claim(type: "Lembrar", value: lembrar));
                identity.AddClaim(new Claim(type: "Expiracao", value: usuarioClaim.Expiracao.ToString()));
                identity.AddClaim(new Claim(type: "Idioma", value: usuarioClaim.Idioma.ToString()));
                identity.AddClaim(new Claim(type: "Avatar", value: usuarioClaim.Avatar.ToString()));
                identity.AddClaim(new Claim(type: "idFinanceiroConta", value: usuarioClaim.idFinanceiroConta.ToString()));
                identity.AddClaim(new Claim(type: "DoisFatoresHabilitado", value: usuarioClaim.DoisFatoresHabilitado.ToString()));
                identity.AddClaim(new Claim(type: "AutenticadorGoogleChaveSecreta", value: usuarioClaim.AutenticadorGoogleChaveSecreta.ToString()));
                identity.AddClaim(new Claim(type: "idPais", value: usuarioClaim.idPais.ToString()));

                string[] roles = usuarioClaim.Roles.Split(',');
                foreach (var role in roles)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
                }

                var principal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.Now.AddDays(1),
                    IsPersistent = true,
                };

                await HttpContext.SignOutAsync();
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(principal), authProperties);

            }
            catch (Exception)
            {
                return false;
                // throw;
            }
            return true;
        }

        public async Task<string> TokenToApi()
        {
            string strRet = User.Identity.Token();

            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/DashBoard/versao", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "Bearer " + strRet);
                request.RequestFormat = DataFormat.Json;
                var response = _clientAPI.Execute(request);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    bool retRefresh = await Refresh();
                    if (retRefresh)
                    {
                        strRet = User.Identity.Token();
                    }
                }
            }
            catch (Exception ex)
            {
                LogErro("appController", "TokenToApi", ex.Message);
            }
           
            return "Bearer " + strRet;
        }

        public UsuarioPaisModel PaisUsuarioApi()
        {
            UsuarioPaisModel retorno;
            string strRet = "Um erro inesperado ocorreu!";
            try
            {
                int id = User.Identity.Id();
                //Cria a transação.
                var request = new RestRequest("/api/Usuario/getUsuarioPaisbyIdUsuario", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "Bearer " + token);
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { id = id });
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    retorno = JsonConvert.DeserializeObject<UsuarioPaisModel>(response.Content);
                    return retorno;
                }
                else
                {
                    strRet = TrataErro(response);
                }
            }
            catch (Exception ex)
            {
                strRet = ex.Message;
            }

            LogErro("Geral", "PaisUsuarioApi", strRet);

            return null;
        }

        #endregion

        #region Propriedades

        public int idUsuario
        {
            get
            {
                return User.Identity.Id();
            }
        }

        public string token
        {
            get
            {
                return User.Identity.Token();
            }
        }

        public bool usuarioAtivo
        {
            get
            {
                return User.Identity.idAtivo() == 1;
            }
        }

        public void setSession(string chave, string valor)
        {
            HttpContext.Session.SetString(chave, valor);
        }

        public string getSession(string chave)
        {
            return HttpContext.Session.GetString(chave);
        }

        public string idioma
        {
            get
            {
                return User.Identity.Idioma();
            }
        }

        #endregion

        #region TrataErro

        public static string TrataErro(IRestResponse resp)
        {
            string strRet;

            switch (resp.StatusCode)
            {
                case HttpStatusCode.NotFound: //404
                    strRet = "requisição não encontrada";
                    break;
                case HttpStatusCode.BadRequest: //400
                    var retBadRequest = JsonConvert.DeserializeObject<StatusModel>(resp.Content);
                    if (String.IsNullOrEmpty(retBadRequest.message))
                    {
                        strRet = "requisição inválida";
                    }
                    else
                    {
                        strRet = retBadRequest.message;
                    }
                    break;
                case HttpStatusCode.InternalServerError: //500
                    var retInternalServerError = JsonConvert.DeserializeObject<StatusModel>(resp.Content);
                    if (String.IsNullOrEmpty(retInternalServerError.message))
                    {
                        strRet = "erro interno!";
                    }
                    else
                    {
                        strRet = retInternalServerError.message;
                    }
                    break;
                case HttpStatusCode.Unauthorized:
                    strRet = "sem autorização";
                    break;
                case HttpStatusCode.NoContent:
                    strRet = ""; //sem dados - deixar em banco
                    break;
                default:
                    strRet = "um erro inesperado ocorreu!";
                    break;
            }

            return strRet;
        }

        public string LogErro(string Origem, string strBreveDescricao, string strErroMensagem)
        {
            string strTicket = Helpers.Chave();
            if (!String.IsNullOrEmpty(strErroMensagem))
            {
                string strErro = "";
                try
                {
                    TicketsModel model = new TicketsModel();
                    model.idUsuario = User.Identity.Id();
                    model.origem = Origem;
                    model.descricao = strBreveDescricao;
                    model.valor = strErroMensagem;
                    model.ticket = strTicket;
                    model.atualizacao = Helpers.DateTimeTron;
                    model.criacao = Helpers.DateTimeTron;
                    model.status = 1; //Ativo
                    Ticket(model, User.Identity.Token());
                }
                catch (Exception ex)
                {
                    strErro = ex.Message;
                }

                //Exibe mensagem na tela no modo debug
                if (Local.Configuracao("DEBUG") == "true")
                {
                    if (ViewBag.Erro != null)
                    {
                        ViewBag.erro += "<br/><br/>Local: " + strBreveDescricao + "<br/><br/>Mensagem: " + strErroMensagem;
                        if (!String.IsNullOrEmpty(strErro))
                        {
                            ViewBag.erro += "<br/><br/>ErroLocal:" + strErro;
                        }
                    }
                }
            }
            return strTicket;
        }

        public static void Ticket(TicketsModel model, string userToken)
        {
            string retorno = "ok";
            try
            {
                if (String.IsNullOrEmpty(userToken))
                {
                    LogFile("ticket_noToken", model.idUsuario + "|" + model.origem + "|" + model.descricao + "|" + model.valor + "|" + model.ticket);
                }
                else
                {
                    RestClient _clientAPI = new RestClient(Local.Configuracao("URL_API"));
                    var request = new RestRequest("/api/tickets/createticket", Method.POST);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("Authorization", "Bearer " + userToken);
                    request.RequestFormat = DataFormat.Json;
                    var json = JsonConvert.SerializeObject(model);
                    request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
                    var response = _clientAPI.Execute(request);

                    //Converte resposta com sucesso.
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        retorno = JsonConvert.DeserializeObject<string>(response.Content);
                    }
                    else
                    {
                        retorno = TrataErro(response);
                    }
                }
            }
            catch (Exception ex)
            {
                retorno = ex.Message;
            }
            if (retorno != "ok")
            {
                //Grava log em filesystem
                LogFile("ticket", retorno);
            }
        }

        #endregion

        #region Percistencia

        /// <summary>
        /// Percistencia dos parametros da pagina sem paginaçao - Itens 2
        /// </summary>
        public void Persistencia(ref string Paramentro1,
                                 ref string Paramentro2,
                                 string strPagina)
        {
            //Inicia Tela trabalhada pelo usuario
            if (getSession("Pagina") == "")
            {
                setSession("Pagina", strPagina);
            }

            //Caso tenha trocado de tela ou caso tenha clicado no botao refresh da tela
            if (getSession("Pagina") != strPagina || getSession("Limpar") != "")
            {
                setSession("Paramentro1", "");
                setSession("Paramentro2", "");
                setSession("Pagina", strPagina);
                setSession("Limpar", "");
            }

            #region Paramentro1

            if (String.IsNullOrEmpty(Paramentro1))
            {
                if (Paramentro1 != "")
                {
                    if (getSession("Parametro1") != "")
                    {
                        Paramentro1 = getSession("Parametro1").ToString();
                    }
                }
                else
                {
                    Paramentro1 = "";
                    setSession("Paramentro1", "");
                }
            }
            else
            {
                setSession("Paramentro1", Paramentro1);
            }

            #endregion

            #region Paramentro2

            if (String.IsNullOrEmpty(Paramentro2))
            {
                if (Paramentro2 != "")
                {
                    if (getSession("Parametro2") != null)
                    {
                        Paramentro2 = getSession("Parametro2").ToString();
                    }
                }
                else
                {
                    Paramentro2 = null;
                    setSession("Paramentro2", "");
                }
            }
            else
            {
                setSession("Paramentro2", Paramentro2);
            }

            #endregion

        }

        public void LimparPersistencia()
        {
            setSession("Limpar", "true");
        }

        #endregion

        #region Apoio

        public async Task<bool> Check()
        {
            bool ret = true;
            //string dataExpiracao = User.Identity.DataExpiracao();

            //Toda pagina deve conter essa validacao
            if (String.IsNullOrEmpty(await CheckToken()))
            {
                RedirectToAction("AccessDenied", "Login"); //Autenticacao do token falhou!
            }
            if (!usuarioAtivo)
            {
                RedirectToAction("Erro", "Home", new { Titulo = "Erro", Mensagem = "Usuário não esta ativo!" });
            }

            int idUsuario = User.Identity.Id();

            if (User.Identity.idAtivo() != 1)
            {
                RedirectToAction("Erro", "Home", new { Titulo = "Erro", Mensagem = "Usuário não esta ativo!" });
            }

            #region Checa se há erros

            if (TempData["erro"] != null)
            {
                ViewBag.Erro = TempData["erro"];
                TempData["erro"] = null;
            }

            #endregion

            #region Checa se há mensagens

            if (TempData["mensagem"] != null)
            {
                ViewBag.Mensagem = TempData["mensagem"];
                TempData["mensagem"] = null;
            }

            #endregion

            #region define cultura
            Localizacao();
            #endregion

            return ret;
        }

        #endregion

        #region Locate

        public void Localizacao()
        {
            //Obtem idioma preferido do usuario
            string strIdioma = User.Identity.Idioma();

            //Caso não haja a preferencia no usuario
            if (String.IsNullOrEmpty(strIdioma))
            {
                UsuarioPaisModel pais = PaisUsuarioApi();
                strIdioma = pais.cultura;
            }

            //Altera a cultura do sistema de acordo com o idioma
            var culture = new System.Globalization.CultureInfo(strIdioma);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
       
        #endregion

    }
}