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
    public class DadosPessoaisController : appController
    {
        #region Actions
        public async Task<IActionResult> Index()
        {
            DadosPessoais dadosPessoais = new DadosPessoais();
            try
            {
                await Check();

                #region Obtem Dados Pessoais do Usuario Logado

                UsuarioModel usuario = UsuarioApi();
                dadosPessoais.id = usuario.id;
                dadosPessoais.idPais = usuario.idPais;
                dadosPessoais.idSexo = usuario.idSexo;
                dadosPessoais.nome = usuario.nome;
                dadosPessoais.email = usuario.email;
                dadosPessoais.idIdioma = usuario.idIdioma;
                dadosPessoais.avatar = usuario.avatar;
                dadosPessoais.celular = usuario.celular;
                dadosPessoais.idUsuarioDados = usuario.idUsuarioDados;

                UsuarioDadosModel usuarioDados = new UsuarioDadosModel();
                if (usuario.idUsuarioDados > 0)
                {
                    usuarioDados = await UsuarioDadosObterApi(usuario.idUsuarioDados);
                    if (usuarioDados != null)
                    {
                        dadosPessoais.SocialTwitch = usuarioDados.SocialTwitch;
                        dadosPessoais.SocialFacebook = usuarioDados.SocialFacebook;
                        dadosPessoais.SocialTwitter = usuarioDados.SocialTwitter;
                        dadosPessoais.SocialYouTube = usuarioDados.SocialYouTube;
                        dadosPessoais.SocialInstagram = usuarioDados.SocialInstagram;
                        dadosPessoais.cpf = usuarioDados.cpf;
                        dadosPessoais.idInstituicaoFinanceira = usuarioDados.idInstituicaoFinanceira;
                        dadosPessoais.agencia = Helpers.Morpho(usuarioDados.agencia, Utils.TipoCriptografia.Descriptografa);
                        dadosPessoais.agenciaDigito = Helpers.Morpho(usuarioDados.agenciaDigito, Utils.TipoCriptografia.Descriptografa);
                        dadosPessoais.conta = Helpers.Morpho(usuarioDados.conta, Utils.TipoCriptografia.Descriptografa);
                        dadosPessoais.contaDigito = Helpers.Morpho(usuarioDados.contaDigito, Utils.TipoCriptografia.Descriptografa);
                        dadosPessoais.chavePix = Helpers.Morpho(usuarioDados.chavePix, Utils.TipoCriptografia.Descriptografa);
                    }
                }

                await SetViewBags(dadosPessoais);

                #endregion
            }
            catch (Exception ex)
            {
                string ticket = LogErro("DadosPessoaisController", "Index", ex.Message);
                return RedirectToAction("Erro", "Home", new { Titulo = "Erro em Dados Pessoais", Mensagem = "Foi aberto um ticket de n. " + ticket + " para esse incidente." });
            }
            return View(dadosPessoais);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(DadosPessoais dadosPessoais, IFormFile imagem)
        {
            await Check();

            string extencao = "jpg";
            string strArquivo = string.Empty;

            if (ModelState.IsValid)
            {
                #region Validacao

                string strMsgErro = string.Empty;

                if (string.IsNullOrEmpty(dadosPessoais.nome))
                    strMsgErro = "O Nome deve ser informado.";
                else if (string.IsNullOrEmpty(dadosPessoais.cpf))
                    strMsgErro = "O CPF deve ser informado.";
                else if (!Utils.Validacao.ValidaCPF(dadosPessoais.cpf))
                    strMsgErro = "o CPF está invalido.";
                else if (string.IsNullOrEmpty(dadosPessoais.email))
                    strMsgErro = "O Email deve ser informado.";
                else if (!Utils.Validacao.ValidarEmail(dadosPessoais.email))
                    strMsgErro = "O Email está invalido.";
                else if (string.IsNullOrEmpty(dadosPessoais.celular))
                    strMsgErro = "O Celular deve ser informado.";
                else if (dadosPessoais.celular.Length < 9)
                    strMsgErro = "O Celular está invalido.";
                else if (dadosPessoais.idInstituicaoFinanceira > 1 ||
                         !string.IsNullOrEmpty(dadosPessoais.agencia) ||
                         !string.IsNullOrEmpty(dadosPessoais.agenciaDigito) ||
                         !string.IsNullOrEmpty(dadosPessoais.conta) ||
                         !string.IsNullOrEmpty(dadosPessoais.conta))
                {
                    if (dadosPessoais.idInstituicaoFinanceira == 1)
                        strMsgErro = "A instituição financeira deve ser informada.";
                    else if (string.IsNullOrEmpty(dadosPessoais.agencia))
                        strMsgErro = "A agencia deve ser informada.";
                    else if (string.IsNullOrEmpty(dadosPessoais.conta))
                        strMsgErro = "A conta  deve ser informada.";
                    else if (string.IsNullOrEmpty(dadosPessoais.contaDigito))
                        strMsgErro = "O digito da conta deve ser informada.";
                }

                if (!string.IsNullOrEmpty(strMsgErro))
                {
                    //AlertErro
                    await SetViewBags(dadosPessoais);
                    ViewBag.Erro = strMsgErro;
                    return View(dadosPessoais);
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
                        await SetViewBags(dadosPessoais);
                        ViewBag.Erro = "A imagem deve ser do tipo jpg, jpeg ou png";
                        return View(dadosPessoais);
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
                        await SetViewBags(dadosPessoais);
                        ViewBag.Erro = "A imagem deve ser do tipo jpg, jpeg ou png";
                        return View(dadosPessoais);
                    }
                }

                #endregion

                #region SalvarImagem
                //Caso não seja enviada uma imagem, não efetua alteração na mesma
                if ((imagem != null && imagem.Length > 0))
                {
                    string filePath = "";
                    try
                    {
                        strArquivo = dadosPessoais.id.ToString("D6") + "_Avatar." + extencao;
                        dadosPessoais.avatar = strArquivo;
                        filePath = Local.Configuracao("PATH_CDN") + @"\\avatar\\" + strArquivo;
                        using (var fileSteam = new FileStream(filePath, FileMode.Create))
                        {
                            await imagem.CopyToAsync(fileSteam);
                        }
                    }
                    catch (Exception ex)
                    {
                        #region Erro
                        //AlertErro
                        await SetViewBags(dadosPessoais);
                        ViewBag.Erro = "Ops! um erro ocorreu. Não foi possível salvar a imagem!";
                        //LogErro
                        LogErro("DadosPessoaisController", "Edit - Gravar Imagem - Catch", ex.Message);
                        return View(dadosPessoais);
                        #endregion
                    }
                }
                #endregion

                #region Salvar

                try
                {
                    UsuarioModel usuario = UsuarioApi();

                    UsuarioDadosModel usuarioDados = new UsuarioDadosModel();
                    if (dadosPessoais.idUsuarioDados != 0)
                        usuarioDados = await UsuarioDadosObterApi(dadosPessoais.idUsuarioDados);
                    else
                    {
                        usuarioDados.DataUltimoAcesso = Helpers.DataAtualInt;
                        usuarioDados.ExibeBoasVindas = true;
                    }

                    usuarioDados.Atualizacao = Helpers.DataAtualInt;
                    usuarioDados.SocialTwitch = dadosPessoais.SocialTwitch;
                    usuarioDados.SocialFacebook = dadosPessoais.SocialFacebook;
                    usuarioDados.SocialTwitter = dadosPessoais.SocialTwitter;
                    usuarioDados.SocialYouTube = dadosPessoais.SocialYouTube;
                    usuarioDados.SocialInstagram = dadosPessoais.SocialInstagram;
                    usuarioDados.cpf = dadosPessoais.cpf;
                    usuarioDados.idInstituicaoFinanceira = dadosPessoais.idInstituicaoFinanceira;
                    usuarioDados.agencia = Helpers.Morpho(dadosPessoais.agencia, Utils.TipoCriptografia.Criptografa);
                    usuarioDados.agenciaDigito = Helpers.Morpho(dadosPessoais.agenciaDigito, Utils.TipoCriptografia.Criptografa);
                    usuarioDados.conta = Helpers.Morpho(dadosPessoais.conta, Utils.TipoCriptografia.Criptografa);
                    usuarioDados.contaDigito = Helpers.Morpho(dadosPessoais.contaDigito, Utils.TipoCriptografia.Criptografa);
                    usuarioDados.chavePix = Helpers.Morpho(dadosPessoais.chavePix, Utils.TipoCriptografia.Criptografa);

                    usuarioDados.id = await UsuarioDadosGravarApi(usuarioDados);
                    dadosPessoais.idUsuarioDados = usuarioDados.id;

                    usuario.idUsuarioDados = usuarioDados.id;
                    usuario.atualizacao = Helpers.DataAtualInt;
                    usuario.idPais = dadosPessoais.idPais;
                    usuario.idSexo = dadosPessoais.idSexo;
                    usuario.nome = dadosPessoais.nome;
                    usuario.email = dadosPessoais.email;
                    usuario.idIdioma = dadosPessoais.idIdioma;
                    usuario.avatar = string.IsNullOrEmpty(strArquivo) ? usuario.avatar : strArquivo;
                    usuario.celular = dadosPessoais.celular;

                    await UsuarioSalvarApi(usuario);

                    //User.Identity.AvatarUpdate(usuario.avatar);
                    //Refresh Token, Identity e Claim
                    await Refresh();
                }
                catch (Exception ex)
                {
                    #region Erro
                    //LogErro
                    LogErro("DadosPessoaisController", "Edit - Salvar - Catch", ex.Message);
                    //AlertErro
                    await SetViewBags(dadosPessoais);
                    ViewBag.Erro = "Ops! um erro ocorreu. Não foi possível salvar seus dados! (Erro. 01)";
                    return View(dadosPessoais);
                    #endregion
                }

                #endregion
            }
            else
            {
                #region Erro inesperado
                //AlertErro
                await SetViewBags(dadosPessoais);
                ViewBag.Erro = "Ops! um erro inesperado ocorreu. Não foi possível salvar seus dados!";
                //LogErro
                LogErro("DadosPessoaisController", "Edit - ModelState", "");
                return View(dadosPessoais);
                #endregion
            }

            #region Mensagem
            ViewBag.MensagemVoltarTexto = "Seu cadastro foi alterado com sucesso!";
            ViewBag.MensagemVoltarLink = Url.Action("Index", "Home");
            #endregion

            await SetViewBags(dadosPessoais);

            return View(dadosPessoais);
        }

        #endregion

        #region API

        public async Task<IList<InstituicaoFinanceiraModel>> InstituicaoFinanceiraObterApi(int idAtivo)
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
            LogErro("DadosPessoaisController", "InstituicaoFinanceiraObterApi", strRet);
            return null;
        }
        public async Task<IList<PaisModel>> PaisObterApi()
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
            LogErro("DadosPessoaisController", "PaisObterApi", strRet);
            return null;
        }
        public async Task<IList<IdiomaModel>> IdiomaObterApi()
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
            LogErro("DadosPessoaisController", "IdiomaObterApi", strRet);
            return null;
        }
        public async Task<IList<UsuarioSexoModel>> UsuarioSexoObterApi()
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
            LogErro("DadosPessoaisController", "UsuarioSexoObterApi", strRet);
            return null;
        }
        public async Task<int> UsuarioSalvarApi(UsuarioModel model)
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
            LogErro("DadosPessoaisController", "UsuarioSalvarApi", strRet);
            return ret;
        }
        public async Task<UsuarioDadosModel> UsuarioDadosObterApi(int idUsuarioDados)
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
            LogErro("DadosPessoaisController", "UsuarioDadosObterApi", strRet);
            return null;
        }
        public async Task<int> UsuarioDadosGravarApi(UsuarioDadosModel model)
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
            LogErro("DadosPessoaisController", "UsuarioDadosGravarApi", strRet);
            return ret;
        }

        #endregion

        #region ViewBag

        public async Task<bool> SetViewBags(DadosPessoais model)
        {
            ViewBag.UrlImage = User.Identity.Avatar();

            IList<InstituicaoFinanceiraModel> instituicoes = await InstituicaoFinanceiraObterApi(1); //== 1 ativo
            ViewBag.idInstituicaoFinanceira = new SelectList(instituicoes.ToList(), "id", "descricao", model.idInstituicaoFinanceira);

            IList<PaisModel> paises = await PaisObterApi();
            ViewBag.idPais = new SelectList(paises.ToList(), "id", "nome", model.idPais);

            IList<IdiomaModel> idiomas = await IdiomaObterApi();
            ViewBag.idIdioma = new SelectList(idiomas.ToList(), "id", "nome", model.idIdioma);

            IList<UsuarioSexoModel> usuarioSexo = await UsuarioSexoObterApi();
            ViewBag.idSexo = new SelectList(usuarioSexo.ToList(), "id", "nome", model.idSexo);

            return true;
        }

        #endregion
    }
}
