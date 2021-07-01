#region using

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;
using OtpSharp;
using RestSharp;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebApp.Model;
using Wiry.Base32;

#endregion

namespace WebApp.Controllers.Usuario
{
    public class SegurancaController : appController
    {
        #region Actions

        public async Task<IActionResult> Index()
        {
            try
            {
                await Check();

                ViewBag.TabCorrente = "1";
                ViewBag.SenhaAtual = "";
                ViewBag.SenhaNova = "";
                ViewBag.SenhaConfirmar = "";

                ViewBag.Acao = "";
                ViewBag.Secretkey = "";
                ViewBag.DoisFatoresHabilitado = User.Identity.DoisFatoresHabilitado();

                var response = new GoogleAuthenticatorResponse { twoFactorEnabled = User.Identity.DoisFatoresHabilitado() }; //  user.TwoFactorEnabled };

                return View(response);

            }
            catch (Exception ex)
            {
                string ticket = LogErro("SegurancaController", "GoogleAuthenticator", ex.Message);
                return RedirectToAction("Erro", "Home", new { Titulo = "Erro em Seguranca", Mensagem = "Foi aberto um ticket de n. " + ticket + " para esse incidente." });
            }
            //return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(IFormCollection form)
        {
            try
            {
                await Check();

                ViewBag.TabCorrente = form["tabCorrente"];
                ViewBag.SenhaAtual = form["senhaAtual"];
                ViewBag.SenhaNova = form["senhaNova"];
                ViewBag.SenhaConfirmar = form["senhaConfirmar"];

                ViewBag.Acao = form["acao"];
                ViewBag.Token = form["token"];
                ViewBag.Secretkey = form["secretkey"];

                var response = new GoogleAuthenticatorResponse { twoFactorEnabled = User.Identity.DoisFatoresHabilitado() };

                if (form["tabCorrente"] == "1")
                {
                    #region Validacao

                    string strMsgErro = string.Empty;

                    if (string.IsNullOrEmpty(form["senhaAtual"]))
                        strMsgErro = "A senha atual deve ser informada.";
                    else if (string.IsNullOrEmpty(form["senhaNova"]))
                        strMsgErro = "A nova senha deve ser informada.";
                    else if (string.IsNullOrEmpty(form["senhaConfirmar"]))
                        strMsgErro = "A senha de confirmação deve ser informada.";

                    if (!string.IsNullOrEmpty(form["senhaAtual"]) &&
                        !string.IsNullOrEmpty(form["senhaNova"]) &&
                        !string.IsNullOrEmpty(form["senhaConfirmar"]))
                    {
                        if (form["senhaAtual"] == form["senhaNova"])
                            strMsgErro = strMsgErro + "A nova senha não pode ser igual a senha atual.";
                        else if (form["senhaNova"] != form["senhaConfirmar"])
                            strMsgErro = "A nova senha e a confirmação da senha estão divergentes.";
                        else if (!Utils.Validacao.ValidaSenhaForte(form["senhaNova"]))
                            strMsgErro = "Sua senha deve ter no mínimo seis e no máximo doze caracteres, sendo eles letras maiúscular e minúsculas, números e simbolos especiais como @ # $ %.";
                    }

                    if (string.IsNullOrEmpty(strMsgErro))
                    {
                        //Checa Autenticação        
                        string strPassord = Utils.Helpers.Morpho(form["senhaAtual"], Utils.TipoCriptografia.Criptografa);
                        if (strPassord != User.Identity.Senha())
                            strMsgErro = strMsgErro + "A senha atual não conferi.";
                    }

                    #endregion

                    if (!string.IsNullOrEmpty(strMsgErro))
                    {
                        //AlertErro                  
                        ViewBag.Erro = strMsgErro;
                    }
                    else
                    {
                        LoginModel loginModel = new LoginModel
                        {
                            userName = User.Identity.Nome(),
                            password = ViewBag.SenhaNova
                        };

                        string strRet = await NovaSenhaApi(loginModel);
                        if (strRet != "ok")
                        {
                            //AlertErro
                            ViewBag.Erro = strRet;
                        }
                        else
                        {
                            #region Mensagem
                            ViewBag.MensagemVoltarTexto = "Sua senha foi alterado com sucesso!";
                            ViewBag.MensagemVoltarLink = Url.Action("Index", "Home");
                            #endregion
                        }
                    }
                }
                else
                {
                    if (ViewBag.Acao == "habilitar")
                    {
                        byte[] secretKey = Base32Encoding.ZBase32.ToBytes(ViewBag.Secretkey);
                        long timeStepMatched = 0;
                        var otp = new Totp(secretKey);
                        if (otp.VerifyTotp(ViewBag.Token, out timeStepMatched, new VerificationWindow(10, 10)))
                        {
                            DoisFatoresModel usuarioDoisFatores = new DoisFatoresModel
                            {
                                Id = User.Identity.Id(),
                                DoisFatoresHabilitado = true,
                                AutenticadorGoogleChaveSecreta = form["secretkey"].ToString()
                            };

                            if (await UsuarioDoisFatoresGravarApi(usuarioDoisFatores) > 0)
                            {            
                                await Refresh();
                                #region Mensagem
                                ViewBag.MensagemVoltarTexto = "Sua verificação em duas etapas foi habilitada com sucesso!";
                                ViewBag.MensagemVoltarLink = Url.Action("Index", "Home");
                                #endregion
                                ViewBag.DoisFatoresHabilitado = true;
                            }
                            else
                            {
                                //AlertErro
                                ViewBag.Erro = "Ocorreu um erro ao habilitar sua verificação em duas etapas!";
                                ViewBag.DoisFatoresHabilitado = false;
                            }
                        }
                        else
                        {
                            //AlertErro
                            ViewBag.Erro = "O Token informado está invalido, tente novamente!";
                            ViewBag.DoisFatoresHabilitado = false;
                        }
                    }

                    if (ViewBag.Acao == "desabilitar")
                    {
                        byte[] secretKey = Base32Encoding.ZBase32.ToBytes(User.Identity.AutenticadorGoogleChaveSecreta());
                        long timeStepMatched = 0;
                        var otp = new Totp(secretKey);
                        if (otp.VerifyTotp(ViewBag.Token, out timeStepMatched, new VerificationWindow(10, 10)))
                        {
                            DoisFatoresModel usuarioDoisFatores = new DoisFatoresModel
                            {
                                Id = User.Identity.Id(),
                                DoisFatoresHabilitado = false,
                                AutenticadorGoogleChaveSecreta = User.Identity.AutenticadorGoogleChaveSecreta()
                            };

                            if (await UsuarioDoisFatoresGravarApi(usuarioDoisFatores) > 0)
                            {
                                if (await Refresh())
                                {
                                    #region Mensagem
                                    ViewBag.MensagemVoltarTexto = "Sua verificação em duas etapas foi desabilitada com sucesso!";
                                    ViewBag.MensagemVoltarLink = Url.Action("Index", "Home");
                                    #endregion
                                    ViewBag.DoisFatoresHabilitado = false;
                                }
                                else
                                {
                                    //AlertErro
                                    ViewBag.Erro = "Ops! um erro inesperado ocorreu.Seu status da autenticação não foi atualizado";
                                    ViewBag.DoisFatoresHabilitado = true;
                                }                             
                            }
                            else
                            {
                                //AlertErro
                                ViewBag.Erro = "Ocorreu um erro ao desabilitar sua verificação em duas etapas!";
                                ViewBag.DoisFatoresHabilitado = true;
                            }
                        }
                        else
                        {
                            //AlertErro
                            ViewBag.Erro = "O Token informado está invalido, tente novamente!";
                            ViewBag.DoisFatoresHabilitado = true;
                        }
                    }
                }
                return View(response);

            }
            catch (Exception ex)
            {
                string ticket = LogErro("SegurancaController", "GoogleAuthenticator", ex.Message);
                return RedirectToAction("Erro", "Home", new { Titulo = "Erro em Seguranca", Mensagem = "Foi aberto um ticket de n. " + ticket + " para esse incidente." });
            }
        }
        
        #endregion

        #region API

        public async Task<int> UsuarioDoisFatoresGravarApi(DoisFatoresModel model)
        {
            string strRet;
            int ret = 0;
            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/Usuario/UpdateDoisFatoresAsync", Method.POST);

                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                var json = JsonConvert.SerializeObject(model);
                request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ret = JsonConvert.DeserializeObject<int>(response.Content);

                    return ret;
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
            //LogErro
            LogErro("DadosPessoaisController", "UsuarioDadosGravarApi", strRet);
            return ret;
        }

        public async Task<string> NovaSenhaApi(LoginModel model)
        {
            //string strRet;
            StatusModel _retorno;
            string strRet = "Um erro inesperado ocorreu!";
            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/Authenticate/novaSenha", Method.POST);

                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
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
                        return "ok";
                    }
                    else
                    {
                        return _retorno.message;
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

            return strRet;
        }

        #endregion

        #region ViewBag

        #endregion

        #region Function

        private string GetTotpUrl(byte[] secretKey, string user)
        {
            var url = string.Empty;
            url = KeyUrl.GetTotpUrl(secretKey, user) + "&issuer=" + "Multiplix Admin";

            return url;
        }

        #endregion

        #region JsonResult

        [HttpPost]
        public ActionResult EsqueceuSenha(string _usuario)
        {
            #region Consistencia

            //Email
            if (String.IsNullOrEmpty(_usuario))
            {
                return Json("O login não foi informado!");
            }

            #endregion

            #region SQLInjection
            //Verifica se há entradas de sqlinjection no logi e senha

            string strSQLInjection = Utils.Helpers.VerificarSQLInjection(_usuario);
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
                UserNameModel model = new UserNameModel
                {
                    UserName = _usuario,
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
        public JsonResult HabilitarGoogleAuthenticator()
        {
            string strSecretKey = User.Identity.AutenticadorGoogleChaveSecreta();
            byte[] secretKey;

            if (string.IsNullOrEmpty(strSecretKey))
            {
                secretKey = KeyGeneration.GenerateRandomKey(20);
                strSecretKey = Base32Encoding.ZBase32.GetString(secretKey);
            }
            else
            {
                secretKey = Base32Encoding.ZBase32.ToBytes(strSecretKey);
            }

            string barcodeUrl = GetTotpUrl(secretKey, User.Identity.Login());

            var response = new GoogleAuthenticatorResponse()
            {
                twoFactorEnabled = User.Identity.DoisFatoresHabilitado(), 
                ok = true,            
                secretkey = strSecretKey, 
                qrCodeImage = HttpUtility.UrlEncode(barcodeUrl)
            };

            return Json(response);
        }

        #endregion
    }
}
