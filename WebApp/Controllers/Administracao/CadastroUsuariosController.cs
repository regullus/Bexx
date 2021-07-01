#region using

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using RestSharp;
using System.Net;
using Newtonsoft.Json;
using Models;
using WebApp.Model;
using Microsoft.AspNetCore.Mvc.Rendering;
using Utils;
using System.IO;
using Microsoft.AspNetCore.Http;

#endregion

namespace WebApp.Controllers.Usuario
{
    public class CadastroUsuariosController : appController
    {
        const string nomeController = "CadastroUsuariosController";

        #region Actions
        public async Task<IActionResult> Index()
        {
            CadastroUsuario cadastroUsuario = new CadastroUsuario();
            try
            {
                //Toda action deve conter essa validacao
                await Check();

                cadastroUsuario.idPais = 31;
                cadastroUsuario.idSexo = 3;
                cadastroUsuario.idIdioma = 1;
                cadastroUsuario.doisFatoresHabilitados = false;
                cadastroUsuario.idInstituicaoFinanceira = 1;
                cadastroUsuario.dataExpiracaoSenha = DateTime.Now.AddDays(90);
                cadastroUsuario.perfil = "Streamer";

                //Preenche as ViewBag
                await SetViewBags(cadastroUsuario);
            }
            catch (Exception ex)
            {
                string ticket = LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), ex.Message);
                return RedirectToAction("Erro", "Home", new { Titulo = "Erro em Dados Pessoais", Mensagem = "Foi aberto um ticket de n. " + ticket + " para esse incidente." });
            }
            return View(cadastroUsuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(CadastroUsuario cadastroUsuario, IFormFile imagem)
        {
            //Toda action deve conter essa validacao
            await Check();

            Boolean blnCriar = false;

            string extencao = "jpg";
            string strArquivo = "";

            if (ModelState.IsValid)
            {
                #region Validacao

                string strMsgErro = string.Empty;

                if (string.IsNullOrEmpty(cadastroUsuario.nome))
                    strMsgErro = "O Nome deve ser informado.";
                else if (string.IsNullOrEmpty(cadastroUsuario.cpf))
                    strMsgErro = "O CPF deve ser informado.";
                else if (!Utils.Validacao.ValidaCPF(cadastroUsuario.cpf))
                    strMsgErro = "o CPF está invalido.";
                else if (string.IsNullOrEmpty(cadastroUsuario.email))
                    strMsgErro = "O Email deve ser informado.";
                else if (!Utils.Validacao.ValidarEmail(cadastroUsuario.email))
                    strMsgErro = "O Email está invalido.";
                else if (string.IsNullOrEmpty(cadastroUsuario.celular))
                    strMsgErro = "O Celular deve ser informado.";
                else if (cadastroUsuario.dataExpiracaoSenha < DateTime.Now)
                    strMsgErro = "A data de expiração invalida.";
                else if (cadastroUsuario.celular.Length < 9)
                    strMsgErro = "A data de expiracao da senha está invalido.";
                else if (cadastroUsuario.idInstituicaoFinanceira > 1 ||
                         !string.IsNullOrEmpty(cadastroUsuario.agencia) ||
                         !string.IsNullOrEmpty(cadastroUsuario.agenciaDigito) ||
                         !string.IsNullOrEmpty(cadastroUsuario.conta) ||
                         !string.IsNullOrEmpty(cadastroUsuario.conta))
                {
                    if (cadastroUsuario.idInstituicaoFinanceira == 1)
                        strMsgErro = "A instituição financeira deve ser informada.";
                    else if (string.IsNullOrEmpty(cadastroUsuario.agencia))
                        strMsgErro = "A agencia deve ser informada.";
                    else if (string.IsNullOrEmpty(cadastroUsuario.conta))
                        strMsgErro = "A conta  deve ser informada.";
                    else if (string.IsNullOrEmpty(cadastroUsuario.contaDigito))
                        strMsgErro = "O digito da conta deve ser informada.";
                }

                if (string.IsNullOrEmpty(cadastroUsuario.login))
                    strMsgErro = "O login deve ser informado.";
                else
                {
                    if (cadastroUsuario.id == 0 && await LoginExiste(cadastroUsuario.login))
                    {
                        strMsgErro = "O login escolhido já existe.";
                    }
                }

                if (string.IsNullOrEmpty(cadastroUsuario.senha))
                    strMsgErro = "A senha deve ser informada.";
                else
                {
                    if (!Utils.Validacao.ValidaSenhaForte(cadastroUsuario.senha))
                        strMsgErro = "A senha deve ter no mínimo seis e no máximo doze caracteres, sendo eles letras maiúscular e minúsculas, números e simbolos especiais como @ # $ %.";
                }

                if (!string.IsNullOrEmpty(strMsgErro))
                {
                    //AlertErro
                    await SetViewBags(cadastroUsuario);
                    ViewBag.Erro = strMsgErro;
                    return View(cadastroUsuario);
                }

                //Caso não seja enviada uma imagem, não efetua alteração na mesma
                if ((imagem != null && imagem.Length > 0))
                {
                    //verifica se é png, jpg ou jpeg no nome do arquivo
                    string strExtencao = imagem.FileName.Substring(imagem.FileName.IndexOf(".") + 1).ToLower();
                    if (!(strExtencao == "jpg" || strExtencao == "png" || strExtencao == "jpeg"))
                    {
                        //Não é uma imagem valida
                        //AlertErro
                        await SetViewBags(cadastroUsuario);
                        ViewBag.Erro = "A imagem deve ser do tipo jpg, jpeg ou png";
                        return View(cadastroUsuario);
                    }
                    //verifica se é png, jpg ou jpeg no conteudo do arquivo
                    string[] contentType = imagem.ContentType.Split('/');
                    if (contentType.Length > 1 && (contentType[1] == "png" || contentType[1] == "jpg" || contentType[1] == "jpeg"))
                    {
                        extencao = contentType[1];
                        if (extencao == "jpeg")
                        {
                            extencao = "jpg";
                        }
                    }
                    if (!(extencao == "png" || extencao == "jpg"))
                    {
                        //Não é uma imagem valida
                        //AlertErro
                        await SetViewBags(cadastroUsuario);
                        ViewBag.Erro = "A imagem deve ser do tipo jpg, jpeg ou png";
                        return View(cadastroUsuario);
                    }
                }

                #endregion

                #region Cria Usuario

                try
                {
                    if (cadastroUsuario.id == 0)
                    {
                        blnCriar = true;
                        cadastroUsuario.id = await CriarUsuarioApi(cadastroUsuario);
                        if (cadastroUsuario.id == 0)
                        {
                            //AlertErro
                            await SetViewBags(cadastroUsuario);
                            ViewBag.Erro = "Ops! um erro ocorreu. Não foi possível criar o usuário! (Erro. 00)";
                            return View(cadastroUsuario);
                        }

                        #region Criar Conta Contabil
                        if (cadastroUsuario.perfil == "Publicitario" || cadastroUsuario.perfil == "Streamer")
                        {

                            FinanceiroContaModel conta = new FinanceiroContaModel
                            {
                                idUsuario = cadastroUsuario.id,
                                idFinanceiroSituacao = 1,
                                idMoeda = 1,
                                idFinanceiroContaTipo = 1,
                                nome = "Principal",
                                token = "",
                                observacao = "Criação automatica"
                            };

                            conta.id = await CriarContaContabilApi(conta);
                            if (conta.id == 0)
                            {
                                //AlertErro
                                await SetViewBags(cadastroUsuario);
                                ViewBag.Erro = "Ops! um erro ocorreu. Não foi possível criar a conta contábil! (Erro. 01)";
                                return View(cadastroUsuario);
                            }
                        }
                        #endregion                    
                    }

                }
                catch (Exception ex)
                {
                    #region Erro
                    //LogErro
                    LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName() + " - Criar.Usuario.Catch", ex.Message);
                    //AlertErro
                    await SetViewBags(cadastroUsuario);
                    ViewBag.Erro = "Ops! um erro ocorreu. Não foi possível criar seus dados! (Erro. 02)";
                    return View(cadastroUsuario);
                    #endregion
                }

                #endregion

                #region Salvar Imagem
                //Caso não seja enviada uma imagem, não efetua alteração na mesma
                if ((imagem != null && imagem.Length > 0))
                {
                    string filePath = "";
                    try
                    {
                        strArquivo = cadastroUsuario.id.ToString("D6") + "_Avatar." + extencao;
                        cadastroUsuario.avatar = strArquivo;
                        filePath = Local.Configuracao("PATH_CDN") + @"\\avatar\" + strArquivo;
                        using (var fileSteam = new FileStream(filePath, FileMode.Create))
                        {
                            await imagem.CopyToAsync(fileSteam);
                        }
                    }
                    catch (Exception ex)
                    {
                        #region Erro
                        //AlertErro
                        await SetViewBags(cadastroUsuario);
                        ViewBag.Erro = "Ops! um erro ocorreu. Não foi possível salvar a imagem!";
                        //LogErro
                        LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName() + " - Salvar.Imagem.Catch", ex.Message);
                        return View(cadastroUsuario);
                        #endregion
                    }
                }
                #endregion

                #region Salvar Usuario

                try
                {
                    UsuarioModel usuario = await ObterUsuarioApi(cadastroUsuario.id);

                    UsuarioDadosModel usuarioDados = new UsuarioDadosModel();
                    if (cadastroUsuario.idUsuarioDados != 0)
                        usuarioDados = await ObterUsuarioDadosApi(cadastroUsuario.idUsuarioDados);
                    else
                    {
                        usuarioDados.DataUltimoAcesso = Helpers.DataAtualInt;
                        usuarioDados.ExibeBoasVindas = true;
                    }

                    usuarioDados.Atualizacao = Helpers.DataAtualInt;
                    usuarioDados.SocialTwitch = cadastroUsuario.SocialTwitch;
                    usuarioDados.SocialFacebook = cadastroUsuario.SocialFacebook;
                    usuarioDados.SocialTwitter = cadastroUsuario.SocialTwitter;
                    usuarioDados.SocialYouTube = cadastroUsuario.SocialYouTube;
                    usuarioDados.SocialInstagram = cadastroUsuario.SocialInstagram;
                    usuarioDados.cpf = cadastroUsuario.cpf;
                    usuarioDados.idInstituicaoFinanceira = cadastroUsuario.idInstituicaoFinanceira;
                    usuarioDados.agencia = Helpers.Morpho(cadastroUsuario.agencia, Utils.TipoCriptografia.Criptografa);
                    usuarioDados.agenciaDigito = Helpers.Morpho(cadastroUsuario.agenciaDigito, Utils.TipoCriptografia.Criptografa);
                    usuarioDados.conta = Helpers.Morpho(cadastroUsuario.conta, Utils.TipoCriptografia.Criptografa);
                    usuarioDados.contaDigito = Helpers.Morpho(cadastroUsuario.contaDigito, Utils.TipoCriptografia.Criptografa);
                    usuarioDados.chavePix = Helpers.Morpho(cadastroUsuario.chavePix, Utils.TipoCriptografia.Criptografa);

                    usuarioDados.id = await GravarUsuarioDadosApi(usuarioDados);
                    cadastroUsuario.idUsuarioDados = usuarioDados.id;

                    usuario.idUsuarioDados = cadastroUsuario.idUsuarioDados;
                    usuario.atualizacao = Helpers.DataAtualInt;
                    usuario.idUserSituacao = cadastroUsuario.idUserSituacao;
                    usuario.idPais = cadastroUsuario.idPais;
                    usuario.idSexo = cadastroUsuario.idSexo;
                    //usuario.dataVencimentoPlano = cadastroUsuario.dataVencimentoPlano;
                    usuario.dataExpiracaoSenha = cadastroUsuario.dataExpiracaoSenha;
                    usuario.idIdioma = cadastroUsuario.idIdioma;
                    usuario.avatar = string.IsNullOrEmpty(strArquivo) ? usuario.avatar : strArquivo;
                    usuario.nome = cadastroUsuario.nome;
                    usuario.senha = Utils.Helpers.Morpho(cadastroUsuario.senha, Utils.TipoCriptografia.Criptografa);
                    usuario.login = cadastroUsuario.login;
                    usuario.email = cadastroUsuario.email;
                    usuario.celular = cadastroUsuario.celular;
                    usuario.doisFatoresHabilitados = cadastroUsuario.doisFatoresHabilitados;
                    usuario.idUsuarioDados = cadastroUsuario.idUsuarioDados;

                    await SalvarUsuarioApi(usuario);

                    //User.Identity.AvatarUpdate(usuario.avatar);
                    //Refresh Token, Identity e Claim
                    await Refresh();
                }
                catch (Exception ex)
                {
                    #region Erro
                    //LogErro
                    LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName() + " - Salvar.Usuario.Catch", ex.Message);
                    //AlertErro
                    await SetViewBags(cadastroUsuario);
                    ViewBag.Erro = "Ops! um erro ocorreu. Não foi possível salvar seus dados! (Erro. 01)";
                    return View(cadastroUsuario);
                    #endregion
                }

                #endregion
            }
            else
            {
                #region Erro inesperado
                //AlertErro
                await SetViewBags(cadastroUsuario);
                ViewBag.Erro = "Ops! um erro inesperado ocorreu. Não foi possível salvar seus dados!";
                //LogErro
                LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName() + " - Edit.ModelState", "");
                return View(cadastroUsuario);
                #endregion
            }

            #region Mensagem
            if (blnCriar)
                ViewBag.MensagemVoltarTexto = "O cadastro do usuario " + cadastroUsuario.login + " foi criado com sucesso!";
            else
                ViewBag.MensagemVoltarTexto = "O cadastro do usuario " + cadastroUsuario.login + " foi alterado com sucesso!";
            ViewBag.MensagemVoltarLink = Url.Action("Index", "Home");
            #endregion

            await SetViewBags(cadastroUsuario);

            return View(cadastroUsuario);
        }

        [HttpPost]
        public async Task<ActionResult> GetUsuario(int? id)
        {
            CadastroUsuario cadastroUsuario = new CadastroUsuario();
            if (id != null)
            {
                try
                {
                    UsuarioModel usuario = await ObterUsuarioApi((int)id);
                    if (usuario == null)
                    {
                        #region Erro inesperado
                        //LogErro
                        LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), "Problemas com cadastro do usuario ( " + id + " ).");

                        usuario = new UsuarioModel();
                        #endregion
                    }
                    else
                    {
                        cadastroUsuario.id = usuario.id;
                        cadastroUsuario.idUserSituacao = usuario.idUserSituacao;
                        cadastroUsuario.idPais = usuario.idPais;
                        cadastroUsuario.idSexo = usuario.idSexo;
                        cadastroUsuario.dataVencimentoPlano = usuario.dataVencimentoPlano;
                        cadastroUsuario.dataExpiracaoSenha = usuario.dataExpiracaoSenha;
                        cadastroUsuario.idIdioma = usuario.idIdioma;
                        cadastroUsuario.avatar = usuario.avatar;
                        cadastroUsuario.avatarUrl = Local.Configuracao("URL_CDN") + @"avatar/" + usuario.avatar + "?id=" + Utils.Helpers.Chave();
                        cadastroUsuario.nome = usuario.nome;
                        cadastroUsuario.senha = Utils.Helpers.Morpho(usuario.senha, Utils.TipoCriptografia.Descriptografa);
                        cadastroUsuario.login = usuario.login;
                        cadastroUsuario.email = usuario.email;
                        cadastroUsuario.celular = usuario.celular;
                        cadastroUsuario.doisFatoresHabilitados = usuario.doisFatoresHabilitados;
                        cadastroUsuario.idUsuarioDados = usuario.idUsuarioDados;

                        switch (await ObterPerfilUsuarioApi(cadastroUsuario.id))
                        {
                            case 1:
                                cadastroUsuario.perfil = "Master";
                                break;
                            case 2:
                                cadastroUsuario.perfil = "Administrador";
                                break;
                            case 3:
                                cadastroUsuario.perfil = "Usuario";
                                break;
                            case 4:
                                cadastroUsuario.perfil = "Streamer";
                                break;
                            case 5:
                                cadastroUsuario.perfil = "Publicitario";
                                break;
                            default:
                                cadastroUsuario.perfil = "";
                                break;
                        }
                    }

                    UsuarioDadosModel usuarioDados = new UsuarioDadosModel();
                    if (usuario.idUsuarioDados > 0)
                    {
                        usuarioDados = await ObterUsuarioDadosApi(usuario.idUsuarioDados);
                        if (usuarioDados != null)
                        {
                            cadastroUsuario.SocialTwitch = usuarioDados.SocialTwitch;
                            cadastroUsuario.SocialFacebook = usuarioDados.SocialFacebook;
                            cadastroUsuario.SocialTwitter = usuarioDados.SocialTwitter;
                            cadastroUsuario.SocialYouTube = usuarioDados.SocialYouTube;
                            cadastroUsuario.SocialInstagram = usuarioDados.SocialInstagram;
                            cadastroUsuario.cpf = usuarioDados.cpf;
                            cadastroUsuario.idInstituicaoFinanceira = usuarioDados.idInstituicaoFinanceira;
                            cadastroUsuario.agencia = Helpers.Morpho(usuarioDados.agencia, Utils.TipoCriptografia.Descriptografa);
                            cadastroUsuario.agenciaDigito = Helpers.Morpho(usuarioDados.agenciaDigito, Utils.TipoCriptografia.Descriptografa);
                            cadastroUsuario.conta = Helpers.Morpho(usuarioDados.conta, Utils.TipoCriptografia.Descriptografa);
                            cadastroUsuario.contaDigito = Helpers.Morpho(usuarioDados.contaDigito, Utils.TipoCriptografia.Descriptografa);
                            cadastroUsuario.chavePix = Helpers.Morpho(usuarioDados.chavePix, Utils.TipoCriptografia.Descriptografa);
                        }
                    }
                    else
                    {
                        cadastroUsuario.idInstituicaoFinanceira = 1;
                    }
                }
                catch (Exception ex)
                {
                    #region Erro
                    //LogErro          
                    LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName() + " - Catch", ex.Message);
                    cadastroUsuario.id = 0;
                    #endregion
                }
            }
            else
            {
                #region Erro inesperado
                //LogErro
                LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), "Id não informado");
                //AlertErro
                cadastroUsuario.id = 0;
                #endregion
            }
            return Json(cadastroUsuario);
        }

        #endregion

        #region API

        private async Task<IList<UsuarioListaCrudModel>> ObterListaUsuarioCrudApi()
        {
            string strRet;
            IList<UsuarioListaCrudModel> lista = new List<UsuarioListaCrudModel>();
            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/Usuario/getListaUsuarioCrudAsync", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    lista = JsonConvert.DeserializeObject<IList<UsuarioListaCrudModel>>(response.Content);
                    return lista;
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
            LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), strRet);
            return null;
        }
        private async Task<UsuarioModel> ObterUsuarioApi(int id)
        {
            string strRet;
            UsuarioModel usuario = new UsuarioModel();
            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/Usuario/GetbyId", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
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
            //LogErro
            LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), strRet);
            return null;
        }
        private async Task<int> ObterPerfilUsuarioApi(int id)
        {
            string strRet;
            int idPerfil = 0;
            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/Usuario/getUsuarioPerfilAsync", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { id = id });
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    idPerfil = JsonConvert.DeserializeObject<int>(response.Content);
                    return idPerfil;
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
            LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), strRet);
            return 0;
        }
        private async Task<int> CriarUsuarioApi(CadastroUsuario usuario)
        {
            StatusIdModel _retorno;
            string strRet = "Um erro inesperado ocorreu!";
            try
            {
                //Cria objeto de envio.
                UsuarioCadastroModel model = new UsuarioCadastroModel
                {
                    nome = usuario.nome,
                    userName = usuario.login,
                    email = usuario.email,
                    password = usuario.senha,
                    idUsuarioDados = 0,
                    idImagem = 1,
                    idEmpresa = 1,
                    idFilial = 1,
                    idUserSituacao = 2,
                    idPlano = 1,
                    idPais = 31,
                    idSexo = 3,
                    dataVencimentoPlano = DateTime.Now.AddYears(2),
                    dataExpiracaoSenha = DateTime.Now.AddMonths(3),
                    idIdioma = 1,
                    avatar = "000000_Avatar.png",
                    DoisFatoresHabilitado = false,
                    AutenticadorGoogleChaveSecreta = string.Empty,
                    perfil = usuario.perfil
                };

                //Cria a transação.
                var request = new RestRequest("/api/Authenticate/register", Method.POST);

                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
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
                        return _retorno.id;
                    }
                    else
                    {
                        return 0;
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
                LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), ex.Message);
            }

            return 0;
        }

        private async Task<int> CriarContaContabilApi(FinanceiroContaModel model)
        {
            int _retorno;
            string strRet = "Um erro inesperado ocorreu!";
            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/Financeiro/CreateFinanceiroConta", Method.POST);

                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                var json = JsonConvert.SerializeObject(model);
                request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
                IRestResponse response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    _retorno = JsonConvert.DeserializeObject<int>(response.Content);
                    return _retorno;
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
                LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), ex.Message);
            }

            return 0;
        }
        private async Task<bool> LoginExiste(string username)
        {
            StatusModel _retorno;
            string strRet = "";
            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/Authenticate/loginexiste", Method.POST);

                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
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

        private async Task<IList<InstituicaoFinanceiraModel>> ObterInstituicaoFinanceiraApi(int idAtivo)
        {
            string strRet;
            IList<InstituicaoFinanceiraModel> lista = new List<InstituicaoFinanceiraModel>();
            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/Base/GetInstituicaoFinanceiraAsync", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { id = idAtivo });
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    lista = JsonConvert.DeserializeObject<IList<InstituicaoFinanceiraModel>>(response.Content);
                    return lista;
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
            LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), strRet);
            return null;
        }
        private async Task<IList<PaisModel>> ObterPaisApi()
        {
            string strRet;
            IList<PaisModel> lista = new List<PaisModel>();
            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/Base/GetPaisAsync", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    lista = JsonConvert.DeserializeObject<IList<PaisModel>>(response.Content);
                    return lista;
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
            LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), strRet);
            return null;
        }
        private async Task<IList<IdiomaModel>> ObterIdiomaApi()
        {
            string strRet;
            IList<IdiomaModel> lista = new List<IdiomaModel>();
            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/Fixo/GetIdiomaAllAsync", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    lista = JsonConvert.DeserializeObject<IList<IdiomaModel>>(response.Content);
                    return lista;
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
            LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), strRet);
            return null;
        }
        private async Task<IList<UsuarioSituacaoModel>> ObterSituacaoApi()
        {
            string strRet;
            IList<UsuarioSituacaoModel> lista = new List<UsuarioSituacaoModel>();
            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/Usuario/GetUsuarioSituacao", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    lista = JsonConvert.DeserializeObject<IList<UsuarioSituacaoModel>>(response.Content);
                    return lista;
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
            LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), strRet);
            return null;
        }

        private async Task<IList<UsuarioSexoModel>> ObterUsuarioSexoApi()
        {
            string strRet;
            IList<UsuarioSexoModel> lista = new List<UsuarioSexoModel>();
            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/Usuario/getUsuarioSexo", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    lista = JsonConvert.DeserializeObject<IList<UsuarioSexoModel>>(response.Content);
                    return lista;
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
            LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), strRet);
            return null;
        }
        private async Task<int> SalvarUsuarioApi(UsuarioModel model)
        {
            string strRet;
            int ret = 0;
            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/Usuario/updateUsuario", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                var json = JsonConvert.SerializeObject(model);
                request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ret = model.id;  //JsonConvert.DeserializeObject<int>(response.Content);
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
            LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), strRet);
            return ret;
        }
        private async Task<UsuarioDadosModel> ObterUsuarioDadosApi(int idUsuarioDados)
        {
            string strRet;
            UsuarioDadosModel ret = new UsuarioDadosModel();
            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/Usuario/GetUsuarioDados", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { id = idUsuarioDados });
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ret = JsonConvert.DeserializeObject<UsuarioDadosModel>(response.Content);
                    return ret ?? new UsuarioDadosModel();
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
            LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), strRet);
            return null;
        }
        private async Task<int> GravarUsuarioDadosApi(UsuarioDadosModel model)
        {
            string strRet;
            int ret = 0;
            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/Usuario/UpdateUsuarioDados", Method.POST);
                if (model.id == 0)
                    request = new RestRequest("/api/Usuario/CreateUsuarioDados", Method.POST);

                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                var json = JsonConvert.SerializeObject(model);
                request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (model.id == 0)
                        model.id = JsonConvert.DeserializeObject<int>(response.Content);

                    return model.id;
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
            LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), strRet);
            return ret;
        }

        #endregion

        #region ViewBag

        public async Task<bool> SetViewBags(CadastroUsuario model)
        {
            //Obtem a lista de usuarios existentes
            ViewBag.Usuarios = await ObterListaUsuarioCrudApi();

            ViewBag.dataExpiracaoSenha = model.dataExpiracaoSenha;
            ViewBag.UrlImageDefault = Local.Configuracao("URL_CDN") + @"avatar/" + "000000_avatar.png" + "?id=" + Utils.Helpers.Chave();

            if (string.IsNullOrEmpty(model.avatar))
                model.avatarUrl = Local.Configuracao("URL_CDN") + @"avatar/" + "000000_avatar.png" + "?id=" + Utils.Helpers.Chave();
            else
                model.avatarUrl = Local.Configuracao("URL_CDN") + @"avatar/" + model.avatar + "?id=" + Utils.Helpers.Chave();

            IList<InstituicaoFinanceiraModel> instituicoes = await ObterInstituicaoFinanceiraApi(1); //== 1 ativo
            ViewBag.idInstituicaoFinanceira = new SelectList(instituicoes.ToList(), "id", "descricao", model.idInstituicaoFinanceira != 0 ? model.idInstituicaoFinanceira : 1);

            IList<PaisModel> paises = await ObterPaisApi();
            ViewBag.idPais = new SelectList(paises.ToList(), "id", "nome", model.idPais != 0 ? model.idPais : 31);

            IList<IdiomaModel> idiomas = await ObterIdiomaApi();
            ViewBag.idIdioma = new SelectList(idiomas.ToList(), "id", "nome", model.idIdioma != 0 ? model.idIdioma : 1);

            IList<UsuarioSexoModel> usuarioSexo = await ObterUsuarioSexoApi();
            ViewBag.idSexo = new SelectList(usuarioSexo.ToList(), "id", "nome", model.idSexo != 0 ? model.idSexo : 3);

            IList<UsuarioSituacaoModel> usuarioSituacao = await ObterSituacaoApi();
            ViewBag.idUserSituacao = new SelectList(usuarioSituacao.ToList(), "id", "nome", model.idUserSituacao != 0 ? model.idUserSituacao : 1);



            return true;
        }

        #endregion
    }
}
