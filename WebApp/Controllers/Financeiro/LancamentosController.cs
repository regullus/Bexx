#region using

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Newtonsoft.Json;
using OtpSharp;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WebApp.Model;
using Wiry.Base32;

#endregion

namespace WebApp.Controllers.Usuario
{
    public class LancamentosController : appController
    {
        const string nomeController = "LancamentosController";

        #region Actions
        public async Task<IActionResult> Index()
        {
            FinanceiroLancamentoModel lancamento = new FinanceiroLancamentoModel();
            try
            {
                //Toda action deve conter essa validacao
                await Check();

                ViewBag.NomeUsuario = "Selecione o Usuário";

                lancamento.data = DateTime.Now;
                //Preenche as ViewBag
                await SetViewBags(0, lancamento);
            }
            catch (Exception ex)
            {
                string ticket = LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), ex.Message);
                return RedirectToAction("Erro", "Home", new { Titulo = "Erro em Dados Pessoais", Mensagem = "Foi aberto um ticket de n. " + ticket + " para esse incidente." });
            }
            ViewBag.idSelected = 0;
            return View(lancamento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(FinanceiroLancamentoModel lancamento,
                                              string acao,
                                              int? idUsuario,
                                              string nomeUsuario,
                                              string valorRealString,
                                              string sinal,
                                              string token)
        {
            //Toda action deve conter essa validacao
            await Check();

            ViewBag.IdUsuario = idUsuario;
            ViewBag.NomeUsuario = nomeUsuario;

            if (acao == "SALVAR")
            {
                if (ModelState.IsValid)
                {
                    #region Validacao

                    string strMsgErro = string.Empty;

                    ViewBag.valorRealString = valorRealString;
                    string valor = valorRealString.Replace("R$", "").Replace("_", "").Replace(".", "").Replace(",", ".").Trim();
                    try
                    {
                        lancamento.valor = Decimal.Parse(valor, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
                    }
                    catch (Exception ex)
                    {
                        LogErro("CampanhasController", "Index - valor", ex.Message);
                        LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName() + " - parse de valor", ex.Message);
                        await SetViewBags(idUsuario, lancamento);
                        ViewBag.Erro = "O valor (R$) informado não é valido!";
                        return View(lancamento);
                    }

                    if (string.IsNullOrEmpty(lancamento.data.ToString()))
                        strMsgErro = "O Nome deve ser informado.";
                    else if (lancamento.valor == 0)
                        strMsgErro = "O valor deve ser informado.";
                    else if (string.IsNullOrEmpty(lancamento.descricao))
                        strMsgErro = "A descrição deve ser informada.";
                    else if (string.IsNullOrEmpty(token))
                        strMsgErro = "O token deve ser informado.";
                    else
                    {
                        string chaveSecreta = User.Identity.AutenticadorGoogleChaveSecreta();
                        if (!string.IsNullOrEmpty(chaveSecreta))
                        {
                            byte[] secretKey = Base32Encoding.ZBase32.ToBytes(chaveSecreta);
                            long timeStepMatched = 0;
                            var otp = new Totp(secretKey);
                            if (!otp.VerifyTotp(token, out timeStepMatched, new VerificationWindow(10, 10)))
                            {
                                strMsgErro = "O Token informado está invalido, tente novamente!";
                            }
                        }
                        else
                        {
                            strMsgErro = "A opção de verificação em duas etapas não está habilitada para seu usuário!";
                        }
                    }

                    if (!string.IsNullOrEmpty(strMsgErro))
                    {
                        //AlertErro
                        await SetViewBags(idUsuario, lancamento);
                        ViewBag.Erro = strMsgErro;
                        return View(lancamento);
                    }

                    #endregion

                    #region Cria Lancamento

                    try
                    {
                        if (sinal == "D")
                            lancamento.valor = lancamento.valor * -1;

                        if (lancamento.idAnuncio == 0)
                            lancamento.idAnuncio = null;

                        lancamento.idUsuario = User.Identity.Id();

                        var ret = await CriarLancamentoApi(lancamento);
                        if (ret <= 0)
                        {
                            LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName() + " - Salvar.Usuario", "CriarLancamentoApi");
                            //AlertErro
                            await SetViewBags(idUsuario, lancamento);
                            ViewBag.Erro = "Ops! um erro ocorreu. Não foi possível salvar seus dados! (Erro. 00)";
                            return View(lancamento);
                        }
                    }
                    catch (Exception ex)
                    {
                        #region Erro
                        //LogErro
                        LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName() + " - Salvar.Usuario.Catch", ex.Message);
                        //AlertErro
                        await SetViewBags(idUsuario, lancamento);
                        ViewBag.Erro = "Ops! um erro ocorreu. Não foi possível salvar seus dados! (Erro. 01)";
                        return View(lancamento);
                        #endregion
                    }

                    #endregion
                }
                else
                {
                    #region Erro inesperado
                    //AlertErro
                    await SetViewBags(idUsuario, lancamento);
                    ViewBag.Erro = "Ops! um erro inesperado ocorreu. Não foi possível salvar seus dados!";
                    //LogErro
                    LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName() + " - Edit.ModelState", "");
                    return View(lancamento);
                    #endregion
                }

                #region Mensagem 
                ViewBag.MensagemVoltarTexto = "O lançamento foi criado com sucesso!";
                ViewBag.MensagemVoltarLink = Url.Action("Index", "Home");
                #endregion

                lancamento = new FinanceiroLancamentoModel();
            }

            await SetViewBags(idUsuario, lancamento);

            return View(lancamento);
        }

        #endregion

        #region API

        private async Task<IList<UsuarioListaCrudModel>> ObterListaUsuarioCrudByPerfilApi(string perfis)
        {
            string strRet;
            IList<UsuarioListaCrudModel> lista = new List<UsuarioListaCrudModel>();
            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/Usuario/getListaUsuarioCrudByPerfilAsync", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { valor = perfis });
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    lista = JsonConvert.DeserializeObject<IList<UsuarioListaCrudModel>>(response.Content);
                    return lista.OrderBy(x => x.nome).ToList();
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
                        nome = "Principal"
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

        private async Task<IList<FinanceiroLancamentoTipoModel>> ObteridFinanceiroLancamentoTiposApi()
        {
            string strRet;
            IList<FinanceiroLancamentoTipoModel> lista = new List<FinanceiroLancamentoTipoModel>();

            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/Financeiro/getFinanceiroLancamentoTipo", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    lista = JsonConvert.DeserializeObject<IList<FinanceiroLancamentoTipoModel>>(response.Content);
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

        private async Task<int> CriarLancamentoApi(FinanceiroLancamentoModel model)
        {
            string strRet;
            int ret = 0;
            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/Financeiro/addWithSaldoAsync", Method.POST);
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
            LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), strRet);
            return ret;
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

        public async Task<bool> SetViewBags(int? idUsuario, FinanceiroLancamentoModel model)
        {
            //Obtem a lista de usuarios existentes
            string strPerfil = (int)Utils.Perfil.Publicitario + "," + (int)Utils.Perfil.Streamer;
            var ltUsuarios = await ObterListaUsuarioCrudByPerfilApi(strPerfil);
            ViewBag.Usuarios = ltUsuarios;

            if (idUsuario == 0)
            {
                idUsuario = ltUsuarios.First<UsuarioListaCrudModel>().id;
                ViewBag.IdUsuario = idUsuario;
                ViewBag.NomeUsuario = ltUsuarios.First<UsuarioListaCrudModel>().nome;
            }

            IList<AnuncioModel> idAnuncios = await ObterAnunciosUsuarioApi((int)idUsuario);
            ViewBag.idAnuncio = new SelectList(idAnuncios.ToList(), "id", "nome", model.idAnuncio != 0 ? model.idAnuncio : 1);

            IList<FinanceiroLancamentoTipoModel> idFinanceiroLancamentoTipos = await ObteridFinanceiroLancamentoTiposApi();
            ViewBag.idFinanceiroLancamentoTipo = new SelectList(idFinanceiroLancamentoTipos.ToList(), "id", "nome", model.idFinanceiroLancamentoTipo != 0 ? model.idFinanceiroLancamentoTipo : 1);

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

            ViewBag.idFinanceiroConta = conta.id;
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
            else
            {
                ViewBag.Mascara = moeda == null ? "R$ #.###.##0,00" : moeda.simbolo + " " + moeda.mascara;
                ViewBag.MascaraInput = moeda == null ? "R$ 9.999.999,99" : moeda.simbolo + " " + moeda.mascaraInput;
            }
            #endregion

            ViewBag.Saldos = await ObterSadosApi(conta.id, anuncios, moeda);

            ViewBag.Lancamentos = await ObterLancamentosApi(conta.id, moeda, 30);

            return true;
        }

        #endregion
    }
}
