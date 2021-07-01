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
//using WebApp.Helper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Utils;
using System.IO;
using Microsoft.AspNetCore.Http;

#endregion

namespace WebApp.Controllers
{
    [Authorize(Roles = "Master, Administrador, Streamer")]
    public class AnunciosUsuarioController : appController
    {
        #region Base

        public AnunciosUsuarioController()
        {

        }

        const string nomeController = "AnunciosUsuarioController";

        #endregion

        #region Actions

        public async Task<IActionResult> Index(int? id, int? rede, string erro)
        {
            IList<AnuncioSelecionadoModel> anuncios = null;
            ViewBag.id = id;
            ViewBag.rede = rede;

            try
            {
                await Check();

                #region Init
                int idAnuncioTipo = id ?? 2; //Inicia no Carrossel
                int idRedeSocial = rede ?? 2; //Set Twitch (2) como Default
                if (idAnuncioTipo <= 0) //Evitar hack de usuario
                {
                    idAnuncioTipo = 2;
                }
                if (rede <= 0) //Evitar hack de usuario
                {
                    idRedeSocial = 2;
                }

                await SetViewBags(idAnuncioTipo, idRedeSocial);

                #endregion

                #region Obtem lista de anuncios

                anuncios = await AnuncioSelecionadoApi(idUsuario, idAnuncioTipo, idRedeSocial);

                #endregion
            }
            catch (Exception ex)
            {
                #region Tratamento de erro
                string ticket = LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), ex.Message);
                return RedirectToAction("Erro", "Home", new { Titulo = "Erro em Anúncios", Mensagem = "Foi aberto um ticket de n. " + ticket + " para esse incidente." });
                #endregion
            }

            return View(anuncios);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int redeSocialAtivo, int anuncioTipoAtivo, string username, int[] selecionados)
        {
            //Toda action deve conter essa validacao
            await Check();

            int retorno = 0;
            string linkTronado = "";

            #region Validacao

            //Rerifica se anuncioTipo e redeSocial estão dentro do range, evita hack de usuario
            int maxAnuncioTipo = (int)Enum.GetValues(typeof(Utils.AnuncioTipo)).Cast<Utils.AnuncioTipo>().Max();
            int minAnuncioTipo = (int)Enum.GetValues(typeof(Utils.AnuncioTipo)).Cast<Utils.AnuncioTipo>().Min();
            int maxRedeSocial = (int)Enum.GetValues(typeof(Utils.RedeSocial)).Cast<Utils.RedeSocial>().Max();
            int minRedeSocial = (int)Enum.GetValues(typeof(Utils.RedeSocial)).Cast<Utils.RedeSocial>().Min();

            if (anuncioTipoAtivo > maxAnuncioTipo || anuncioTipoAtivo <= minAnuncioTipo) //1 - Normal, não é usado aqui
            {
                TempData["erro"] = "Parâmetro inválido para o tipo de anuncio.";
                return RedirectToAction("index", "AnunciosUsuario", new { id = anuncioTipoAtivo, rede = redeSocialAtivo });
            }
            if (redeSocialAtivo > maxRedeSocial || redeSocialAtivo <= minRedeSocial) //1 - Todas, não é usado aqui
            {
                TempData["erro"] = "Parâmetro inválido para Rede Social.";
                return RedirectToAction("index", "AnunciosUsuario", new { id = anuncioTipoAtivo, rede = redeSocialAtivo });
            }
            //Verifica se o usuario da rede social foi informado
            if (String.IsNullOrEmpty(username))
            {
                TempData["erro"] = "O 'UserName' utilizado na rede social selecionada deve ser informado.";
                return RedirectToAction("index", "AnunciosUsuario", new { id = anuncioTipoAtivo, rede = redeSocialAtivo });
            }

            #endregion

            #region Salvar

            try
            {
                //Obtem nome no enum da redeSocial selecionada
                string nomeRedeSocial = Enum.GetName(typeof(Utils.RedeSocial), redeSocialAtivo);

                //Verifica se linkTronado já existe
                //Obtem linktonado
                TronarModel tronar = await LinkTronadoApi(anuncioTipoAtivo, nomeRedeSocial);

                if (tronar == null)
                {
                    //Cria registro na tabela tronar
                    retorno = await TronarCreateApi(anuncioTipoAtivo, nomeRedeSocial, username);
                    if (retorno > 0)
                    {
                        //Obtem linktronado
                        tronar = await LinkTronadoApi(anuncioTipoAtivo, nomeRedeSocial);
                        linkTronado = tronar.linkTronado;
                    }
                    else
                    {
                        TempData["erro"] = "Não foi possível criar o link.";
                        return RedirectToAction("index", "AnunciosUsuario", new { id = anuncioTipoAtivo, rede = redeSocialAtivo });
                    }
                }
                else
                {
                    linkTronado = tronar.linkTronado;
                    //update no link tronado (username pode ser alterado)
                    //altera registro na tabela tronar
                    retorno = await TronarUpdateApi(linkTronado, username);
                }

                //Remove os existentes
                retorno = await AnuncioUsuarioRemoveApi(anuncioTipoAtivo, redeSocialAtivo);

                //retorno = AnuncioUsuarioUpdate(anuncio);
                //Grava Associativa Anuncio x Usuario
                if (selecionados != null)
                {
                    foreach (int item in selecionados)
                    {
                        //Retorno contem o id do anuncio criado
                        retorno = await AnuncioUsuarioCreateApi(anuncioTipoAtivo, redeSocialAtivo, item);
                    }
                }
            }
            catch (Exception ex)
            {
                #region Erro
                LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), ex.Message);
                //AlertErro
                ViewBag.Erro = "Ops! um erro ocorreu. Não foi possível gravar a campanha! (Erro. 01)";
                return RedirectToAction("index", "AnunciosUsuario", new { id = anuncioTipoAtivo, rede = redeSocialAtivo });
                #endregion
            }

            #endregion

            #region Mensagem
            TempData["mensagem"] = "A Seleção de anúncios foi alterada com sucesso!";
            #endregion

            return RedirectToAction("index", "AnunciosUsuario", new { id = anuncioTipoAtivo, rede = redeSocialAtivo });
        }

        #endregion

        #region API

        public async Task<IList<RedeSocialModel>> RedeSocialApi()
        {
            string strRet;
            IList<RedeSocialModel> lista = new List<RedeSocialModel>();
            try
            {
                int idRedeSocialTipo = 2;  //2 são as redes sociais que aceitam anuncios
                //Cria a transação.
                var request = new RestRequest("/api/RedeSocial/getByIdTipoAsync", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { id = idRedeSocialTipo });
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    lista = JsonConvert.DeserializeObject<IList<RedeSocialModel>>(response.Content);
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
            LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), strRet);
            return null;
        }

        public async Task<IList<AnuncioTipoModel>> AnuncioTipoApi()
        {
            string strRet;
            IList<AnuncioTipoModel> lista = new List<AnuncioTipoModel>();
            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/anuncio/getForStreamerAnuncioTipo", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    lista = JsonConvert.DeserializeObject<IList<AnuncioTipoModel>>(response.Content);
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
            LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), strRet);
            return null;
        }

        public async Task<TronarModel> LinkTronadoApi(int idAnuncioTipo, string redeSocial)
        {
            string strRet = "";
            try
            {
                //int idUsuario = 5;// teste
                //Cria a transação.
                var request = new RestRequest("/api/tronar/getByIdAnuncioTipoRedeSocial", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { idUsuario = idUsuario, idAnuncioTipo = idAnuncioTipo, redeSocial = redeSocial });
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    TronarModel model = JsonConvert.DeserializeObject<TronarModel>(response.Content);
                    return model;
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
            LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName() + " 1", strRet);
            return null;
        }

        public async Task<string> getUsuarioRedeSocialApi(string redeSocial)
        {
            string strRet = "";
            try
            {
                //int idUsuario = 5;// teste
                //Cria a transação.
                var request = new RestRequest("/api/tronar/getUsuarioRedeSocial", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { idUsuario = idUsuario, redeSocial = redeSocial });
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    TronarUsuarioRedeSocialModel model = JsonConvert.DeserializeObject<TronarUsuarioRedeSocialModel>(response.Content);
                    if (model != null)
                    {
                        return model.usuarioRedeSocial;
                    }
                    else
                    {
                        return null;
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
            LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), strRet);
            return null;
        }

        public async Task<int> UsuarioRedeSocialUpdateApi(string redeSocial, string usuarioRedeSocial)
        {
            int retorno = 0;
            string strRet = "";
            try
            {
                var request = new RestRequest("/api/tronar/updateUsuarioRedeSocial", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { idUsuario = idUsuario, redeSocial = redeSocial, usuarioRedeSocial = usuarioRedeSocial });
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    retorno = JsonConvert.DeserializeObject<int>(response.Content);
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
            LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), strRet);
            return retorno;
        }

        public async Task<TronarModel> LinkTronadoApi(string linkTronado)
        {
            string strRet = "";
            try
            {
                //int idUsuario = 5;// teste
                //Cria a transação.
                var request = new RestRequest("/api/tronar/getByLinkTronado", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { linkTronado = linkTronado });
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    TronarModel model = JsonConvert.DeserializeObject<TronarModel>(response.Content);
                    return model;
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
            LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName() + " 2", strRet);
            return null;
        }

        public async Task<int> TronarCreateApi(int idAnuncioTipo, string redeSocial, string usuarioRedeSocial)
        {
            int retorno = 0;
            string strRet = "";

            TronarModel model = new TronarModel();
            model.id = 0;
            model.idUsuario = User.Identity.Id();
            model.idAnuncio = null;
            model.idTronarSituacao = 1; //Ativo
            model.idAnuncioTipo = idAnuncioTipo;
            model.linkOriginal = null;
            model.linkTronado = "";
            model.tituloPaginaOriginal = null;
            model.descricaoPaginaOriginal = null;
            model.imagemPaginaOriginal = null;
            model.redeSocial = redeSocial;
            model.usuarioRedeSocial = usuarioRedeSocial;
            model.atualizacao = Helpers.DataAtualInt;
            model.criacao = Helpers.DataAtualInt;
            model.acessos = 0;

            try
            {
                var request = new RestRequest("/api/tronar/createTronar", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                var json = JsonConvert.SerializeObject(model);
                request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    retorno = JsonConvert.DeserializeObject<int>(response.Content);
                    //Update no nome do usuario da rede social
                    int ret = await UsuarioRedeSocialUpdateApi(redeSocial, usuarioRedeSocial);
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
            LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), strRet);
            return retorno;
        }

        public async Task<int> TronarUpdateApi(string linkTronado, string usuarioRedeSocial)
        {
            int retorno = 0;
            string strRet = "";

            TronarModel model = new TronarModel();
            model = await LinkTronadoApi(linkTronado);

            if (model != null)
            {
                model.usuarioRedeSocial = usuarioRedeSocial;
                model.atualizacao = Helpers.DataAtualInt;

                try
                {
                    var request = new RestRequest("/api/tronar/updateTronar", Method.POST);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("Authorization", await TokenToApi());
                    request.RequestFormat = DataFormat.Json;
                    var json = JsonConvert.SerializeObject(model);
                    request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
                    var response = _clientAPI.Execute(request);

                    //Converte resposta com sucesso.
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        retorno = JsonConvert.DeserializeObject<int>(response.Content);
                        //Update no nome do usuario da rede social
                        int ret = await UsuarioRedeSocialUpdateApi(model.redeSocial, usuarioRedeSocial);
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
            }
            else
            {
                strRet = "Busca por linkTronado não teve retorno";
            }
            LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), strRet);
            return retorno;
        }

        public async Task<IList<AnuncioSelecionadoModel>> AnuncioSelecionadoApi(int idUsuario, int idAnuncioTipo, int idRedeSocial)
        {
            string strRet;
            IList<AnuncioSelecionadoModel> lista = new List<AnuncioSelecionadoModel>();
            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/anuncio/getAnuncioUsuario", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { idUsuario = idUsuario, idAnuncioTipo = idAnuncioTipo, idRedeSocial = idRedeSocial });
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    lista = JsonConvert.DeserializeObject<IList<AnuncioSelecionadoModel>>(response.Content);
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
            LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), strRet);
            return null;
        }

        public async Task<int> AnuncioUsuarioRemoveApi(int idAnuncioTipo, int idRedeSocial)
        {
            int intRet = 0;
            string strRet = "";
            try
            {
                var request = new RestRequest("/api/anuncio/delAnuncioUsuario", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { idUsuario = idUsuario, idAnuncioTipo = idAnuncioTipo, idRedeSocial = idRedeSocial });
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    intRet = JsonConvert.DeserializeObject<int>(response.Content);
                    return intRet;
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
            LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), strRet);
            return intRet;
        }

        public async Task<int> AnuncioUsuarioCreateApi(int idAnuncioTipo, int idRedeSocial, int idAnuncio)
        {
            int intRet = 0;
            string strRet = "";
            AnuncioUsuarioModel model = new AnuncioUsuarioModel();
            model.idAnuncio = idAnuncio;
            model.idUsuario = idUsuario;
            model.idAnuncioTipo = idAnuncioTipo;
            model.idRedeSocial = idRedeSocial;
            model.idAtivo = 1; //Ativo
            model.criacao = Helpers.DataAtualInt;

            try
            {
                var request = new RestRequest("/api/anuncio/addAnuncioUsuario", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                request.RequestFormat = DataFormat.Json;
                var json = JsonConvert.SerializeObject(model);
                request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    intRet = JsonConvert.DeserializeObject<int>(response.Content);
                    return intRet;
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
            LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), strRet);
            return intRet;
        }

        #endregion

        #region ViewBag

        public async Task<bool> SetViewBags(int idAnuncioTipo, int idRedeSocial)
        {

            #region Anuncio Tipo (id)

            IList<AnuncioTipoModel> anuncioTipo = await AnuncioTipoApi();
            string nomeAnunciotipo = anuncioTipo.Single(x => x.id == idAnuncioTipo).nome;
            if (String.IsNullOrEmpty(nomeAnunciotipo))
            {
                //Se null o idAnuncioTipo é inválido, seta o default (evitar hack de usuario)
                idAnuncioTipo = 1; //Set Carrocell (2) como Default
                                   //Obtem nome novamente com id correto
                nomeAnunciotipo = anuncioTipo.Single(x => x.id == idAnuncioTipo).nome;
            }
            ViewBag.anuncioTipo = anuncioTipo;
            ViewBag.anuncioTipoAtivo = idAnuncioTipo;
            ViewBag.anuncioTipoNome = nomeAnunciotipo;

            #endregion

            #region RedeSocial (rede)

            IList<RedeSocialModel> redeSocial = await RedeSocialApi();

            //Remove 'Todas' id = 1 da lista
            redeSocial = redeSocial.Where(x => x.id != 1).ToList();

            //Verifica se idRedeSocial informado esta na lista de redes sociais disponibilizda 
            string nomeRedeSocial = redeSocial.Single(x => x.id == idRedeSocial).nome;
            if (String.IsNullOrEmpty(nomeRedeSocial))
            {
                //Se null o idRedeSocial é inválido, seta o default (evitar hack de usuario)
                idRedeSocial = 2; //Set Twitch (2) como Default
                                  //Obtem nome novamente com id correto
                nomeRedeSocial = redeSocial.Single(x => x.id == idRedeSocial).nome;
            }
            ViewBag.redeSocial = redeSocial;
            ViewBag.redeSocialAtivo = idRedeSocial;
            ViewBag.nomeRedeSocial = nomeRedeSocial;

            #endregion

            #region LinkTronado

            //Busca link tronado, caso não exita cria um
            TronarModel tronar = await LinkTronadoApi(idAnuncioTipo, nomeRedeSocial);
            if (tronar != null)
            {
                ViewBag.linkTronado = Local.Configuracao("URL_PUB") + tronar.linkTronado;
                ViewBag.usuarioRedeSocial = tronar.usuarioRedeSocial;
            }
            else
            {
                ViewBag.linkTronado = null;
                string usuarioRedeSocial = await getUsuarioRedeSocialApi(nomeRedeSocial);
                ViewBag.usuarioRedeSocial = usuarioRedeSocial;
            }

            #endregion

            return true;
        }

        #endregion

    }
}
