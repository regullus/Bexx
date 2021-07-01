#region using

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WebApp.Model;

#endregion

namespace WebApp.Controllers.Usuario
{
    public class ExtratoUsuariosController : appController
    {
        const string nomeController = "ExtratoUsuariosController";

        #region Actions
        public async Task<IActionResult> Index()
        {
            try
            {
                //Toda action deve conter essa validacao
                await Check();
                ViewBag.IdUsuario = User.Identity.Id();
                ViewBag.NomeUsuario = User.Identity.Nome();
                ViewBag.QtdeDias = 15;

                //Preenche as ViewBag
                await SetViewBags(User.Identity.Id(), 15);
            }
            catch (Exception ex)
            {
                string ticket = LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), ex.Message);
                return RedirectToAction("Erro", "Home", new { Titulo = "Erro em Dados Pessoais", Mensagem = "Foi aberto um ticket de n. " + ticket + " para esse incidente." });
            }
            ViewBag.idSelected = 0;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(int idQtdeDias)
        {
            //Toda action deve conter essa validacao
            await Check();

            ViewBag.IdUsuario = User.Identity.Id();
            ViewBag.NomeUsuario = User.Identity.Nome();
            ViewBag.QtdeDias = idQtdeDias;  

            try
            {
                await SetViewBags(idUsuario, idQtdeDias);
            }
            catch (Exception ex)
            {
                #region Erro
                //LogErro
                LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName() + " - Catch", ex.Message);
                //AlertErro            
                ViewBag.Erro = "Ops! um erro ocorreu. Não foi possível gerar o extrato! (Erro. 01)";
                return View();
                #endregion
            }

            #region Mensagem 
            if (ViewBag.Saldos == null || ViewBag.Saldos.Count == 0)
            {
                ViewBag.MensagemVoltarTexto = "O usuário não possui movimentação!";
                ViewBag.MensagemVoltarLink = Url.Action("Index", "Home");
            }
            #endregion

            return View();
        }

        #endregion

        #region API

        private async Task<IList<AnuncioModel>> ObterAnunciosUsuarioApi(int idUsuario)
        {
            string strRet;
            IList<AnuncioModel> lista = new List<AnuncioModel>();
            IList<AnuncioModel> reslista = new List<AnuncioModel>();

            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/anuncio/GetAnuncioByIdUsuario", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { idUsuario = idUsuario });
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {

                    reslista.Add(new AnuncioModel
                    {
                        id = 0,
                        nome = "Nenhum"
                    });

                    lista = JsonConvert.DeserializeObject<IList<AnuncioModel>>(response.Content);
                    if (lista != null)
                    {
                        foreach (var e in lista)
                        {
                            reslista.Add(e);
                        }
                    }

                    return reslista;
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
        private async Task<FinanceiroContaModel> ObterContaByIdUsuarioApi(int idUsuario)
        {
            string strRet;
            FinanceiroContaModel conta = new FinanceiroContaModel();

            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/financeiro/getContaByIdUsuario", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { id = idUsuario });
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {

                    conta = JsonConvert.DeserializeObject<FinanceiroContaModel>(response.Content);
                    return conta;
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
            return conta;
        }
        private async Task<FinanceiroMoedaModel> ObterMoedaApi(int id)
        {
            string strRet;
            FinanceiroMoedaModel moeda = new FinanceiroMoedaModel();

            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/Financeiro/GetFinanceiroMoedaById", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { id = id });
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {

                    moeda = JsonConvert.DeserializeObject<FinanceiroMoedaModel>(response.Content);
                    return moeda;
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
            return moeda;
        }
        private async Task<IList<ExtratoItem>> ObterSadosApi(int idConta, 
                                                             IList<AnuncioModel> anuncios, 
                                                             FinanceiroMoedaModel moeda)
        {
            string strRet = string.Empty;
            IList<ExtratoItem> extrato = new List<ExtratoItem>();
            IList<FinanceiroSaldoModel> saldos = new List<FinanceiroSaldoModel>();

            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/financeiro/GetSaldoByIdConta", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { id = idConta }); // quantidade de dias
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    saldos = JsonConvert.DeserializeObject<IList<FinanceiroSaldoModel>>(response.Content);
                    if (saldos != null)
                    {
                        foreach (var item in saldos)
                        {
                            extrato.Add(new ExtratoItem
                            {                               
                                idAnuncio = item.idAnuncio ?? 0,
                                nomeAnuncio = (item.idAnuncio == null || item.idAnuncio == 0 ? "Principal" : anuncios.Where(x => x.id == item.idAnuncio).FirstOrDefault().nome),
                                data = item.data,
                                descricao = "",
                                valor = (item.valor < 0 ? "- " : "") + moeda.simbolo + " " + string.Format(moeda.mascaraOut, Math.Abs(item.valor)),
                                tipo = (item.valor < 0 ? "D" : "C")
                            });
                        }
                    }
                }
                else
                {
                    strRet = TrataErro(response);
                    //LogErro
                    LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName() + " - Saldos 01", strRet);
                    return null;
                }
            }
            catch (Exception ex)
            {
                strRet = ex.Message;
                //LogErro
                LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName() + " - Saldos 02", strRet);
                return null;
            }

            var ret = extrato.OrderBy(x => x.idAnuncio).ThenByDescending(x => x.data);
            return ret.ToList();
        }
        private async Task<IList<ExtratoItem>> ObterLancamentosApi(int idConta,
                                                                   FinanceiroMoedaModel moeda,
                                                                   int qtdeDias)
        {
            string strRet = string.Empty;
            IList<ExtratoItem> extrato = new List<ExtratoItem>();
            IList<FinanceiroLancamentoModel> lancamentos = new List<FinanceiroLancamentoModel>();

            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/financeiro/GetLancamentoByIdContaAsync", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { id = idConta, valor = qtdeDias });
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    lancamentos = JsonConvert.DeserializeObject<IList<FinanceiroLancamentoModel>>(response.Content);
                    if (lancamentos != null)
                    {
                        foreach (var item in lancamentos)
                        {
                            extrato.Add(new ExtratoItem
                            {         
                                idAnuncio = item.idAnuncio,
                                nomeAnuncio = "",
                                data = item.data,
                                descricao = item.descricao,
                                valor = (item.valor < 0 ? "- ": "") + moeda.simbolo + " " + string.Format(moeda.mascaraOut, Math.Abs(item.valor)),
                                tipo = (item.valor < 0 ? "D" : "C")
                            });
                        }
                    }
                }
                else
                {
                    strRet = TrataErro(response);
                    //LogErro
                    LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName() + " - Lancamentos 01", strRet);
                    return null;
                }
            }
            catch (Exception ex)
            {
                strRet = ex.Message;
                //LogErro
                LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName() + " - Lancamentos 02", strRet);
                return null;
            }

            var ret = extrato.OrderBy(x => x.idAnuncio).ThenByDescending(x => x.data);
            return ret.ToList();
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

        #endregion

        #region ViewBag

        public async Task<bool> SetViewBags(int? idUsuario, int qtdeDias)
        {
            //Obtem a lista de quantidade de dias para obter lançamentos
            IList<valorStrModel> ltQtdeDias = new List<valorStrModel>();
            ltQtdeDias.Add(new valorStrModel { id = 0, valor = "Todos" });
            ltQtdeDias.Add(new valorStrModel { id = 15, valor = "15 dias"} );
            ltQtdeDias.Add(new valorStrModel { id = 30, valor = "30 dias" });
            ltQtdeDias.Add(new valorStrModel { id = 60, valor = "60 dias" });
            ltQtdeDias.Add(new valorStrModel { id = 90, valor = "90 dias" });
            ltQtdeDias.Add(new valorStrModel { id = 120, valor = "120 dias" });
            ViewBag.idQtdeDias = new SelectList(ltQtdeDias.ToList(), "id", "valor", qtdeDias);

            if (idUsuario != null || idUsuario > 0)
            {                       
                #region Anuncios 
                IList<AnuncioModel> anuncios = new List<AnuncioModel>();
                anuncios = await ObterAnunciosUsuarioApi((int)idUsuario);
                if (anuncios == null)
                {
                    #region Erro inesperado
                    //LogErro
                    LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), "Problemas em obter os anuncios do usuario ( " + idUsuario + " ).");

                    #endregion
                }
                #endregion

                #region Conta 
                FinanceiroContaModel conta = new FinanceiroContaModel();
                conta = await ObterContaByIdUsuarioApi((int)idUsuario);
                if (conta == null || conta.id == 0)
                {
                    conta = new FinanceiroContaModel
                    {
                        idUsuario = (int)idUsuario,
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
                        #region Erro inesperado
                        //LogErro
                        LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), "Problemas  em obter a conta do usuario ( " + idUsuario + " ).");

                        #endregion
                    }
                }
                #endregion

                #region Moeda 
                FinanceiroMoedaModel moeda = new FinanceiroMoedaModel();
                moeda = await ObterMoedaApi(conta.idMoeda);
                if (moeda == null)
                {
                    #region Erro inesperado
                    //LogErro
                    LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), "Problemas em obter os anuncios do usuario ( " + idUsuario + " ).");

                    #endregion
                }
                #endregion

                ViewBag.Saldos = await ObterSadosApi(conta.id, anuncios, moeda);

                ViewBag.Lancamentos = await ObterLancamentosApi(conta.id, moeda, qtdeDias);
            }

            return true;
        }

        #endregion
    }
}