#region using

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using Utils;
using WebApp.Model;
using Microsoft.AspNetCore.Http;

#endregion

namespace WebApp.Controllers
{
    public class LoginController : appController
    {
        #region Variaveis

        private readonly ReCaptchaService _ReCaptchaService;
        public string gstrErro = "";

        #endregion

        #region Base

        public LoginController(ReCaptchaService _reCaptchaService)
        {
            _ReCaptchaService = _reCaptchaService;
            // _clientAPI = new RestClient(Local.Configuracao("URL_API"));
        }

        #endregion
        
        #region Actions

        #region Login

        [HttpGet]
        [AllowAnonymous]
        public ActionResult index(string ret, string id)
        {
            try
            {
                ViewBag.Message = null;
                gstrErro = null;

                var lembrar = ((ClaimsIdentity)User.Identity).FindFirst("Lembrar");
                if (lembrar != null)
                {
                    if (lembrar.Value != null && lembrar.Value.ToLower() == "true")
                    {
                        string strURL = UrlSistema(id);
                        if (String.IsNullOrEmpty(strURL))
                        {
                            ViewBag.Message = "Sistema inválido";
                        }
                        else
                        {
                            return Redirect(strURL);
                        }
                    }
                }

                if (!String.IsNullOrEmpty(ret))
                {
                    switch (ret)
                    {
                        case "SI":
                            ViewBag.Message = "Entradas inválidas...";
                            break;
                        case "ReCaptcha":
                            ViewBag.Message = "ReCaptcha inválido...";
                            break;
                        default:
                            if (String.IsNullOrEmpty(ret))
                            {
                                ViewBag.Message = null;
                            }
                            else
                            {
                                ViewBag.Message = ret;
                            }
                            break;
                    }
                }
             
                ViewBag.Termos = Local.Configuracao("URL_TERMOS_CONDICOES_USO");
            }
            catch (Exception ex)
            {
                string ticket = LogErro("LoginController", "Index", ex.Message);
                return RedirectToAction("Erro", "Home", new { Titulo = "Erro", Mensagem = "Foi aberto um ticket de n. " + ticket + " para esse incidente." });
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login([Bind] LoginInputModel _usuario)
        {
            try
            {
                #region ReCaptcha

                if (String.IsNullOrEmpty(_usuario.token))
                {
                    return RedirectToAction("Index", "Login", new { ret = "ReCaptcha" });
                }

                var _reCaptchaVerify = _ReCaptchaService.VerifyReCaptcha(_usuario.token);

                if (!_reCaptchaVerify.Result.success && _reCaptchaVerify.Result.score <= 0.5)
                {
                    return RedirectToAction("Index", "Login", new { ret = "ReCaptcha" });
                }

                #endregion

                #region SQLInjection
                //Verifica se há entradas de sqlinjection no logi e senha

                string strSQLInjection = Helpers.VerificarSQLInjection(_usuario.login);

                if (String.IsNullOrEmpty(strSQLInjection))
                {
                    strSQLInjection = Helpers.VerificarSQLInjection(_usuario.password);
                }

                if (!String.IsNullOrEmpty(strSQLInjection))
                {
                    return RedirectToAction("Index", "Login", new { ret = "SI" });
                    //ToDo - Enviar email para admin
                }

                #endregion

                #region Login

                UsuarioClaimModel usuarioClaim = UsuarioClainApi(_usuario.login, _usuario.password);

                if (usuarioClaim != null)
                {
                    bool ret = await SetClaim(usuarioClaim, _usuario.login, _usuario.lembrar.ToString());

                    string strURL = UrlSistema("1");

                    if (String.IsNullOrEmpty(strURL))
                    {
                        ViewBag.Message = "Sistema inválido";
                    }
                    else
                    {
                        //return RedirectToAction("Index", "Home");
                        return Redirect(strURL);
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(gstrErro))
                    {
                        return RedirectToAction("Index", "Login", new { ret = "Dados inválidos: " + gstrErro });
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                string ticket = LogErro("LoginController", "Login", ex.Message);
                return RedirectToAction("Erro", "Home", new { Titulo = "Erro em Anúncios", Mensagem = "Foi aberto um ticket de n. " + ticket + " para esse incidente." });
            }

            return RedirectToAction("Index", "Login", new { ret = "Dados inválidos" });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync();
                //mcr retornar para portal
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                string ticket = LogErro("LoginController", "Logout", ex.Message);
                return RedirectToAction("Erro", "Home", new { Titulo = "Erro", Mensagem = "Foi aberto um ticket de n. " + ticket + " para esse incidente." });
            }
        }

        #endregion

        #region Cadastro

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Cadastro([Bind] LoginInputModel _usuario)
        {
            #region ReCaptcha

            if (String.IsNullOrEmpty(_usuario.token))
            {
                return Json("ReCaptcha inválido!");
            }
            try
            {
                var _reCaptchaVerify = _ReCaptchaService.VerifyReCaptcha(_usuario.token);

                if (!_reCaptchaVerify.Result.success && _reCaptchaVerify.Result.score <= 0.5)
                {
                    return Json("ReCaptcha inválido!");
                }
            }
            catch (Exception)
            {
                return Json("ReCaptcha inválido!");
            }

            #endregion

            #region Consistencia

            //Verificas se login já existe
            if (!String.IsNullOrEmpty(_usuario.login))
            {
                bool ret = LoginExiste(_usuario.login);
                if (ret)
                {
                    return Json("Login escolhido já existe!");
                }
            }

            //Se houve aceite
            if (!String.IsNullOrEmpty(_usuario.aceite))
            {
                if (_usuario.aceite != "on")
                {
                    return Json("Você deve aceitar os termos e condições!");
                }
            }
            else
            {
                return Json("O aceite dos termos e condições não foi informado");
            }
            //Se senha = confirmacao senha
            if (!String.IsNullOrEmpty(_usuario.password))
            {
                if (!String.IsNullOrEmpty(_usuario.confirmarPassword))
                {
                    if (_usuario.password != _usuario.confirmarPassword)
                    {
                        return Json("A senha e a confirmação devem ser iguais!");
                    }
                }
                else
                {
                    return Json("A senha não foi informada!");
                }
            }
            else
            {
                return Json("A senha não foi informada!");
            }
            //Email
            if (String.IsNullOrEmpty(_usuario.email))
            {
                return Json("O email não foi informado!");
            }
            if (String.IsNullOrEmpty(_usuario.login))
            {
                return Json("O login não foi informado!");
            }
            if (String.IsNullOrEmpty(_usuario.nome))
            {
                return Json("O nome não foi informado!");
            }
            if (String.IsNullOrEmpty(_usuario.token))
            {
                return Json("O token não foi informado!");
            }
            #endregion

            #region SQLInjection
            //Verifica se há entradas de sqlinjection no logi e senha

            string strSQLInjection = Helpers.VerificarSQLInjection(_usuario.login);

            if (String.IsNullOrEmpty(strSQLInjection))
            {
                strSQLInjection = Helpers.VerificarSQLInjection(_usuario.password);
            }

            if (String.IsNullOrEmpty(strSQLInjection))
            {
                strSQLInjection = Helpers.VerificarSQLInjection(_usuario.confirmarPassword);
            }

            if (String.IsNullOrEmpty(strSQLInjection))
            {
                strSQLInjection = Helpers.VerificarSQLInjection(_usuario.nome);
            }

            if (!String.IsNullOrEmpty(strSQLInjection))
            {
                return Json("Dados inválidos");
                //ToDo - Enviar email para admin
            }

            #endregion

            #region Cadastro
            StatusIdModel _retorno;
            string strRet = "Um erro inesperado ocorreu!";
            try
            {
                //Cria objeto de envio.
                UsuarioCadastroModel model = new UsuarioCadastroModel
                {
                    nome = _usuario.nome,
                    userName = _usuario.login,
                    email = _usuario.email,
                    password = _usuario.password,
                    idUsuarioDados = 0,
                    idImagem = 1,
                    idEmpresa = 1,
                    idGrupo = 1,
                    idUsuarioSituacao = 2,
                    idArea = 1,
                    idPais = 1,
                    idSexo = 3,
                    dataExpiracaoSenha = DateTime.Now.AddMonths(3),
                    idIdioma = 1,
                    avatar = "000000_Avatar.png",
                    doisFatoresHabilitado = false,        
                    autenticadorGoogleChaveSecreta = string.Empty,
                    perfil = "usuario"
                };

                //Cria a transação.
                var request = new RestRequest("/api/Authenticate/register", Method.POST);

                request.AddHeader("Content-Type", "application/json");
                request.RequestFormat = DataFormat.Json;
                var json = JsonConvert.SerializeObject(model);
                request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
                IRestResponse response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    _retorno = JsonConvert.DeserializeObject<StatusIdModel>(response.Content);
                    if (_retorno.status == "Success")
                    {
                        return Json("ok");
                    }
                    else
                    {
                        return Json(_retorno.message);
                    }
                }
                else
                {
                    strRet = TrataErro(response);
                }
            }
            catch (Exception ex)
            {
                strRet = ex.Message;
                //LogErro
                LogErro("Login", "Cadastro", ex.Message);
            }

            return Json(strRet);

            #endregion
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("ConfirmarCadastro")]
        public ActionResult ConfirmarCadastro(string user)
        {
            #region Consistencia

            if (String.IsNullOrEmpty(user))
            {
                bool ret = LoginExiste(user);
                if (ret)
                {
                    return Json("Sem Usuário!");
                }
            }

            #endregion

            #region SQLInjection
            //Verifica se há entradas de sqlinjection no logi e senha

            string strSQLInjection = Helpers.VerificarSQLInjection(user);

            if (!String.IsNullOrEmpty(strSQLInjection))
            {
                return Json("Dados inválidos");
                //ToDo - Enviar email para admin
            }

            #endregion

            #region ConfirmarCadastro

            StatusModel _retorno;
            string strRet = "Um erro inesperado ocorreu!";

            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/Authenticate/confirmarcadastro", Method.POST);

                request.AddHeader("Content-Type", "application/json");
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { UserNameToken = user });
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    _retorno = JsonConvert.DeserializeObject<StatusModel>(response.Content);
                    if (_retorno.status == "Success")
                    {
                        return RedirectToAction("Sucesso", "Home", new { Titulo = "Cadastro confirmado!", Mensagem1 = _retorno.message, Mensagem2 = "...para entrar clique no botão Login.", NomeLink = "Login", Link = Local.Configuracao("URL_APP") });
                    }
                    else
                    {
                        return RedirectToAction("Erro", "Home", new { Titulo = "Erro", Mensagem = _retorno.message });
                    }
                }
                else
                {
                    strRet = TrataErro(response);
                }
            }
            catch (Exception ex)
            {
                strRet = ex.Message;
                LogErro("Login", "Confirmar Cadastro", ex.Message);
            }

            #endregion

            return RedirectToAction("Erro", "Home", new { Titulo = "Erro", Mensagem = strRet });
        }

        #endregion

        #region EsqueceuSenha

        [HttpPost]
        [AllowAnonymous]
        public ActionResult EsqueceuSenha([Bind] LoginInputModel _usuario)
        {

            #region ReCaptcha

            if (String.IsNullOrEmpty(_usuario.token))
            {
                return Json("ReCaptcha inválido!");
            }
            try
            {
                var _reCaptchaVerify = _ReCaptchaService.VerifyReCaptcha(_usuario.token);

                if (!_reCaptchaVerify.Result.success && _reCaptchaVerify.Result.score <= 0.5)
                {
                    return Json("ReCaptcha inválido!");
                }
            }
            catch (Exception)
            {
                return Json("ReCaptcha inválido!");
            }

            #endregion

            #region Consistencia

            //Email
            if (String.IsNullOrEmpty(_usuario.login))
            {
                return Json("O login não foi informado!");
            }

            #endregion

            #region SQLInjection
            //Verifica se há entradas de sqlinjection no logi e senha

            string strSQLInjection = Helpers.VerificarSQLInjection(_usuario.login);

            if (!String.IsNullOrEmpty(strSQLInjection))
            {
                return Json("Dados inválidos");
                //ToDo - Enviar email para admin
            }

            #endregion

            #region EsqueceuSenha

            StatusModel _retorno;
            string strRet = "Um erro inesperado ocorreu!";

            try
            {
                UsuarioNomeModel model = new UsuarioNomeModel
                {
                    usuarioNome = _usuario.login,
                };

                //Cria a transação.
                var request = new RestRequest("/api/Authenticate/esqueceusenha", Method.POST);

                request.AddHeader("Content-Type", "application/json");
                request.RequestFormat = DataFormat.Json;
                var json = JsonConvert.SerializeObject(model);
                request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    _retorno = JsonConvert.DeserializeObject<StatusModel>(response.Content);
                    if (_retorno.status == "Success")
                    {
                        return Json("ok");
                    }
                    else
                    {
                        return Json(_retorno.message);
                    }
                }
                else
                {
                    strRet = TrataErro(response);
                }
            }
            catch (Exception ex)
            {
                strRet = ex.Message;
                LogErro("Login", "Esqueceu Senha", ex.Message);
            }

            #endregion

            return Json(strRet);

        }

        [HttpGet]
        [Route("NovaSenha")]
        [AllowAnonymous]
        public ActionResult NovaSenha(string user)
        {

            #region Consistencia

            string strErro = "";

            string dado = Helpers.Morpho(user, TipoCriptografia.Descriptografa);
            if (String.IsNullOrEmpty(dado))
            {
                strErro = "token não é válido!";
                return RedirectToAction("Erro", "Home", new { Titulo = "Erro", Mensagem = "Não foi possível cadastrar uma nova senha, motivo: " + strErro });
            }

            string funcao = "";
            string chave = "";
            string usuario = "";
            int validade = 0;

            string[] dados = dado.Split('|');
            try
            {
                funcao = dados[0].ToString();
                chave = dados[1].ToString(); //ToDo - Salver em banco e confirmar se chave é a mesma (SEGURANCA)
                usuario = dados[2].ToString();
                validade = Convert.ToInt32(dados[3].ToString());
            }
            catch (Exception)
            {
                strErro = "token não é válido!!";
                return RedirectToAction("Erro", "Home", new { Titulo = "Erro", Mensagem = "Não foi possível cadastrar uma nova senha, motivo: " + strErro });
            }

            if (validade < Helpers.DataAtualInt) //Configuracao em WebApi tem o total em dias
            {
                strErro = "O email enviado perdeu a validade, caso não tenha se logado no MULTIPLIX, por favor faça um novo cadastro.";
                return RedirectToAction("Erro", "Home", new { Titulo = "Erro", Mensagem = "Não foi possível cadastrar uma nova senha, motivo: " + strErro });
            }

            #endregion

            #region AlterarSenha
            ViewBag.Token2 = user;
            ViewBag.Nome = usuario;

            #endregion

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult CriarSenha([Bind] NovaSenhaModel _novaSenha)
        {
            #region ReCaptcha

            if (String.IsNullOrEmpty(_novaSenha.token))
            {
                return Json("ReCaptcha inválido!");
            }
            try
            {
                var _reCaptchaVerify = _ReCaptchaService.VerifyReCaptcha(_novaSenha.token);

                if (!_reCaptchaVerify.Result.success && _reCaptchaVerify.Result.score <= 0.5)
                {
                    return Json("ReCaptcha inválido!");
                }
            }
            catch (Exception)
            {
                return Json("ReCaptcha inválido!");
            }

            #endregion

            #region Consistencia

            string strErro = "";

            if (String.IsNullOrEmpty(_novaSenha.token2))
            {
                return Json("O token não foi informado!");
            }

            string dado = Helpers.Morpho(_novaSenha.token2, TipoCriptografia.Descriptografa);
            if (String.IsNullOrEmpty(dado))
            {
                strErro = "token não é válido!";
                return RedirectToAction("Erro", "Home", new { Titulo = "Erro", Mensagem = "Não foi possível cadastrar uma nova senha, motivo: " + strErro });
            }

            string funcao = "";
            string chave = "";
            string usuario = "";
            int validade = 0;

            string[] dados = dado.Split('|');
            try
            {
                funcao = dados[0].ToString();
                chave = dados[1].ToString(); //ToDo - Salver em banco e confirmar se chave é a mesma (SEGURANCA)
                usuario = dados[2].ToString();
                validade = Convert.ToInt32(dados[3].ToString());
            }
            catch (Exception)
            {
                strErro = "token não é válido!!";
                return Json(strErro);
            }

            if (validade < Helpers.DataAtualInt) //Configuracao em WebApi tem o total em dias
            {
                strErro = "O email enviado perdeu a validade. Caso necessário clique novamente no esqueceu minha senha.";
                return Json(strErro);
            }
            if (funcao != "ES") //funcao deve ser ES - Esqueci a Senha
            {
                strErro = "função do token não é válido!!";
                return Json(strErro);
            }
            //Se senha = confirmacao senha
            if (!String.IsNullOrEmpty(_novaSenha.password))
            {
                if (!String.IsNullOrEmpty(_novaSenha.confirmarPassword))
                {
                    if (_novaSenha.password != _novaSenha.confirmarPassword)
                    {
                        return Json("A senha e a confirmação devem ser iguais!");
                    }
                    //if (!_novaSenha.Password.Any(char.IsUpper))
                    //{
                    //    return Json("A senha deve conter uma letra maiúscula!");
                    //}
                }
                else
                {
                    return Json("A senha não foi informada!");
                }
            }
            else
            {
                return Json("A senha não foi informada!");
            }

            #endregion

            #region SQLInjection
            //Verifica se há entradas de sqlinjection no logi e senha

            string strSQLInjection = Helpers.VerificarSQLInjection(_novaSenha.password);

            if (String.IsNullOrEmpty(strSQLInjection))
            {
                strSQLInjection = Helpers.VerificarSQLInjection(_novaSenha.confirmarPassword);
            }

            if (!String.IsNullOrEmpty(strSQLInjection))
            {
                return Json("Dados inválidos");
                //ToDo - Enviar email para admin
            }

            #endregion

            #region CriarSenha

            StatusModel _retorno;
            string strRet = "Um erro inesperado ocorreu!";
            try
            {
                //Cria objeto de envio.
                LoginModel model = new LoginModel
                {
                    usuario = usuario,
                    password = _novaSenha.password,
                };

                //Cria a transação.
                var request = new RestRequest("/api/Authenticate/novasenha", Method.POST);

                request.AddHeader("Content-Type", "application/json");
                request.RequestFormat = DataFormat.Json;
                var json = JsonConvert.SerializeObject(model);
                request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    _retorno = JsonConvert.DeserializeObject<StatusModel>(response.Content);
                    if (_retorno.status == "Success")
                    {
                        return Json("ok");
                    }
                    else
                    {
                        return Json(_retorno.message);
                    }
                }
                else
                {
                    strRet = TrataErro(response);
                }
            }
            catch (Exception ex)
            {
                strRet = ex.Message;
                //LogErro
                LogErro("Login", "Criar Senha", ex.Message);
            }

            return Json(strRet);

            #endregion
        }

        #endregion

        #endregion

        #region Mensagens

        [AllowAnonymous]
        [Route("Erro404")]
        public IActionResult Erro404()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> AccessDenied()
        {
            try
            {
                await HttpContext.SignOutAsync();
            }
            catch (Exception ex)
            {
                string ticket = LogErro("LoginController", "AccessDenied", ex.Message);
                return RedirectToAction("Erro", "Home", new { Titulo = "Erro", Mensagem = "Foi aberto um ticket de n. " + ticket + " para esse incidente." });
            }

            return View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> LoginExpired()
        {
            try
            {
                await HttpContext.SignOutAsync();
            }
            catch (Exception ex)
            {
                string ticket = LogErro("LoginController", "AccessDenied", ex.Message);
                return RedirectToAction("Erro", "Home", new { Titulo = "Erro em Anúncios", Mensagem = "Foi aberto um ticket de n. " + ticket + " para esse incidente." });
            }

            return View();
        }

        #endregion

        #region API

        [AllowAnonymous]
        public UsuarioClaimModel UsuarioClainApi(string login, string senha)
        {
            UsuarioClaimModel _usuario;
            string strRet = "Um erro inesperado ocorreu!";
            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/Authenticate/login", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { usuario = login, password = senha });
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    _usuario = JsonConvert.DeserializeObject<UsuarioClaimModel>(response.Content);
                    return _usuario;
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
            gstrErro = strRet;
            return null;
        }

        [AllowAnonymous]
        public bool LoginExiste(string username)
        {
            StatusModel _retorno;
            string strRet = "";
            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/Authenticate/loginexiste", Method.POST);

                request.AddHeader("Content-Type", "application/json");
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { username = username });
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    _retorno = JsonConvert.DeserializeObject<StatusModel>(response.Content);
                    if (_retorno.message == "ok")
                    {
                        return true;
                    }
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
            //log erro para strRet
            LogErro("Login", "LoginExiste", strRet);
            return false;
        }

        #endregion

        #region JsonResult

        [AllowAnonymous]
        public JsonResult UserExiste(string username)
        {
            StatusModel _retorno;
            string strRet = "";
            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/Authenticate/loginexiste", Method.POST);

                request.AddHeader("Content-Type", "application/json");
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { login = username });
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    _retorno = JsonConvert.DeserializeObject<StatusModel>(response.Content);
                    if (_retorno.message == "ok")
                    {
                        return Json("valid : true");
                    }
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
            //LogErro pra strRet
            LogErro("Login", "LoginExiste", strRet);
            return Json("valid : false");
        }

        #endregion

        #region ApiExterno

        [AllowAnonymous]
        [Route("api/isAuthenticated")]
        public async Task<IActionResult> IsAuthenticated()
        {
            await Task.FromResult(false); //Fake task
            try
            {
                return Ok(new Response { Status = "Success", Message = User.Identity.IsAuthenticated.ToString() });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "false" });
            }
        }

        [Route("api/fail")]
        public async Task<IActionResult> Fail()
        {
            await Task.FromResult(false); //Fake task
            return StatusCode(StatusCodes.Status401Unauthorized, new Response { Status = "Error", Message = "Não autorizado!" });
        }

        [AllowAnonymous]
        [Route("api/[action]")]
        public async Task<IActionResult> Denied()
        {
            await Task.FromResult(false); //Fake task
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Não autorizado!" });
        }

        [AllowAnonymous]
        [Route("api/versao")]
        public async Task<IActionResult> Versao()
        {
            string versao = "v2.4.0";
            try
            {
                await Task.FromResult(false); //Fake task
                return Ok(new Response { Status = "Success", Message = versao });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }


        }

        [Authorize]
        [Route("api/nome")]
        public async Task<IActionResult> Nome()
        {
            await Task.FromResult(false); //Fake task

            if (String.IsNullOrEmpty(User.Identity.Nome()))
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new Response { Status = "Error", Message = "Não autorizado!" });
            }
            else
            {
                return Ok(new Response { Status = "Success", Message = User.Identity.Nome() });
            }
        }

        [Authorize]
        [Route("api/token")]
        public async Task<IActionResult> Token()
        {
            await Task.FromResult(false); //Fake task
            if (String.IsNullOrEmpty(token))
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new Response { Status = "Error", Message = "Não autorizado!" });
            }
            else
            {
                return Ok(new Response { Status = "Success", Message = token });
            }
        }

        [Authorize]
        [Route("api/claims")]
        public async Task<IActionResult> Claims()
        {
            await Task.FromResult(false); //Fake task

            try
            {
                UsuarioClaimModel claim = new UsuarioClaimModel();

                claim.id = User.Identity.Id();
                claim.idAtivo = User.Identity.idAtivo();
                claim.nome = User.Identity.Nome();
                claim.email = User.Identity.Email();
                claim.senha = ""; //não retornar a senha
                claim.regras = User.Identity.Regra();
                claim.token = User.Identity.Token();
                claim.refreshToken = User.Identity.RefreshToken();
                claim.idioma = User.Identity.Idioma();
                claim.expiracao = Convert.ToDateTime(User.Identity.DataExpiracao());
                claim.avatar = User.Identity.Avatar();
                claim.doisFatoresHabilitado = User.Identity.DoisFatoresHabilitado();
                claim.autenticadorGoogleChaveSecreta = ""; //não exibir
                claim.idPais = User.Identity.idPais();

                return Ok(claim);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new Response { Status = "Error", Message = ex.Message });
            }
            
        }

        #endregion
    }
}