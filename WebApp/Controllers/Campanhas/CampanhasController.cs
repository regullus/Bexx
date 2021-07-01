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
using Microsoft.AspNetCore.Mvc.Rendering;
using Utils;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Globalization;

#endregion

namespace WebApp.Controllers.Publicidade
{
    [Authorize(Roles = "Master, Administrador, Publicitario")]
    public class CampanhasController : appController
    {
        #region Base
        int[] gRedeSocial;
        public CampanhasController()
        {
        }
        const string nomeController = "CampanhasController";

        #endregion

        #region Actions

        public async Task<IActionResult> Index()
        {
            IList<AnuncioSaldoModel> anuncios = null;
            try
            {
                await Check();

                #region Obtem lista de anuncios

                ViewBag.Erro = null;
                ViewBag.Mensagem = null;

                anuncios = await AnuncioApi(idUsuario);
                if (anuncios != null)
                {
                    List<AnuncioAuxiliarModel> listaRedeSocial = new List<AnuncioAuxiliarModel>();
                    AnuncioAuxiliarModel anuncioAuxiliar = new AnuncioAuxiliarModel();
                    AnuncioTipoModel anuncioTipo = new AnuncioTipoModel();
                    foreach (AnuncioSaldoModel item in anuncios)
                    {
                        anuncioTipo = await AnuncioTipoByIdApi(item.idAnuncioTipo);
                        anuncioAuxiliar = new AnuncioAuxiliarModel
                        {
                            id = item.id,
                            redeSocial = await RedesSociaisSelecionadas(item.id),
                            tipoNome = anuncioTipo.nome,
                            tipoIcone = anuncioTipo.icone
                        };
                        listaRedeSocial.Add(anuncioAuxiliar);
                    }
                    ViewBag.ListaRedeSocial = listaRedeSocial;
                }

                #endregion

                #region Moeda

                FinanceiroMoedaModel moeda = await ObterMoedaApi();
                string mascara = moeda.simbolo + " " + moeda.mascara;
                ViewBag.Moeda = moeda;

                #endregion

                #region Saldo Total Publicitario

                ViewBag.saldoPublicitario = null;
                int idFinanceiroConta = User.Identity.idFinanceiroConta();
                FinanceiroSaldoModel saldo = await FinanceiroSaldoApi(idFinanceiroConta, null);
                if (saldo != null && saldo.valor > 0)
                {
                    ViewBag.saldoPublicitario = saldo.valor.ToString(mascara); 
                }
                else
                {
                    int valZero = 0;
                    ViewBag.saldoPublicitario = valZero.ToString(mascara);
                }

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

        // GET: Chamadas/Create
        public async Task<ActionResult> Create(int? id)
        {
            await Check();
            int idTipoAnuncio = id ?? (int)Utils.AnuncioTipo.Carrocel; //Default é 2 - Carrossel
            if (idTipoAnuncio == 0)
            {
                idTipoAnuncio = 2;
            }
            try
            {
                await SetViewBags(null, idTipoAnuncio);
            }
            catch (Exception ex)
            {
                string ticket = LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), ex.Message);
                return RedirectToAction("Erro", "Home", new { Titulo = "Erro em Anúncios", Mensagem = "Foi aberto um ticket de n. " + ticket + " para esse incidente." });
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AnuncioSaldoModel anuncio,
                                               string valorRealString,
                                               string horasDisponibilizadasString,
                                               int[] redesSociais,
                                               IFormFile imagem)
        {
            decimal decValor = 0m;
            int intValor = 0;

            #region Moeda

            FinanceiroMoedaModel moeda = await ObterMoedaApi();
            string mascara = moeda.simbolo + " " + moeda.mascara;
            ViewBag.Moeda = moeda;

            #endregion

            #region valida valores
            ViewBag.valorRealString = valorRealString;
            ViewBag.horasDisponibilizadasString = horasDisponibilizadasString;

            if (String.IsNullOrEmpty(valorRealString))
            {
                await SetViewBags(anuncio, anuncio.idAnuncioTipo);
                ViewBag.Erro = "Um valor (" + moeda.simbolo + ") deve ser informado!";
                return View(anuncio);
            }
            else
            {
                string valor = valorRealString.Replace(moeda.simbolo, "").Replace("_", "").Replace(".", "").Replace(",", ".").Trim();
                try
                {
                    decValor = Decimal.Parse(valor, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
                    ViewBag.valorRealString = decValor;
                }
                catch (Exception ex)
                {
                    LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), ex.Message);
                    await SetViewBags(anuncio, anuncio.idAnuncioTipo);
                    ViewBag.Erro = "O valor (" + moeda.simbolo + ") informado não é valido!";
                    return View(anuncio);
                }
                if (decValor <= 0)
                {
                    await SetViewBags(anuncio, anuncio.idAnuncioTipo);
                    ViewBag.Erro = "O valor (" + moeda.simbolo + ") informado não é valido!";
                    return View(anuncio);
                }
                else
                {
                    anuncio.valor = decValor;
                }
                //Verifica se há saldo para uso na campanha
                decimal saldo = await Saldo();
                if (saldo < decValor)
                {
                    await SetViewBags(anuncio, anuncio.idAnuncioTipo);
                    ViewBag.Erro = "Não há saldo suficiente para o valor informado!";
                    return View(anuncio);
                }
            }

            if (String.IsNullOrEmpty(horasDisponibilizadasString))
            {
                await SetViewBags(anuncio, anuncio.idAnuncioTipo);
                ViewBag.Erro = "A informação em 'Horas disponibilizadas' não esta correta!";
                return View(anuncio);
            }
            else
            {
                string valor = horasDisponibilizadasString.Replace("_", "").Replace(".", "").Trim();
                try
                {
                    intValor = int.Parse(valor, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), ex.Message);
                    await SetViewBags(anuncio, anuncio.idAnuncioTipo);
                    ViewBag.Erro = "A informação em 'Horas disponibilizadas' não esta correta!";
                    return View(anuncio);
                }
                if (intValor <= 0)
                {
                    await SetViewBags(anuncio, anuncio.idAnuncioTipo);
                    ViewBag.Erro = "A informação em 'Horas disponibilizadas' não esta correta!";
                    return View(anuncio);
                }
                else
                {
                    anuncio.horasDisponibilizadas = intValor;
                }
            }

            if (redesSociais == null)
            {
                await SetViewBags(anuncio, anuncio.idAnuncioTipo);
                ViewBag.Erro = "Por favor, informe as redes sociais!";
                return View(anuncio);
            }
            else
            {
                bool validRedeSocial = false;
                foreach (int item in redesSociais)
                {
                    validRedeSocial = true;
                    break;
                }
                if (!validRedeSocial)
                {
                    await SetViewBags(anuncio, anuncio.idAnuncioTipo);
                    ViewBag.Erro = "Por favor, informe as redes sociais!";
                    return View(anuncio);
                }
            }

            #endregion

            try
            {
                int retorno = 0;
                int retornoRedeSocial = 0;
                string extencao = "jpg";
                string strArquivo = "";
                gRedeSocial = redesSociais;

                if (ModelState.IsValid)
                {
                    #region Validacao

                    if (!(imagem != null && imagem.Length > 0))
                    {
                        //imagem nao carregou
                        //AlertErro
                        await SetViewBags(anuncio, anuncio.idAnuncioTipo);
                        ViewBag.Erro = "É necessario escolher uma imagem";
                        return View(anuncio);
                    }
                    else
                    {
                        //verifica se é png, jpg ou jpeg no nome do arquivo
                        string strExtencao = imagem.FileName.Substring(imagem.FileName.IndexOf(".") + 1).ToLower();
                        if (!(strExtencao == "jpg" || strExtencao == "png" || strExtencao == "jpeg" || strExtencao == "gif"))
                        {
                            //Não é uma imagem valida
                            //AlertErro
                            await SetViewBags(anuncio, anuncio.idAnuncioTipo);
                            ViewBag.Erro = "A imagem deve ser do tipo jpg, png ou gif";
                            return View(anuncio);
                        }
                        //verifica se é png, jpg ou jpeg no conteudo do arquivo
                        string[] contentType = imagem.ContentType.Split('/');
                        if (contentType.Length > 1 && (contentType[1] == "png" || contentType[1] == "jpg" || contentType[1] == "jpeg" || contentType[1] == "gif"))
                        {
                            extencao = contentType[1];
                            if (extencao == "jpeg")
                            {
                                extencao = "jpg";
                            }
                        }
                        if (!(extencao == "png" || extencao == "jpg" || extencao == "gif"))
                        {
                            //Não é uma imagem valida
                            //AlertErro
                            await SetViewBags(anuncio, anuncio.idAnuncioTipo);
                            ViewBag.Erro = "A imagem deve ser do tipo jpg, png ou gif";
                            return View(anuncio);
                        }
                    }

                    #endregion

                    #region Ajuste

                    anuncio.atualizacao = Helpers.DataAtualInt;
                    anuncio.idAnuncioGrupo = 1; //pt-BR
                    anuncio.idMidia = 3; //streamer
                    anuncio.idUsuario = User.Identity.Id();
                    anuncio.termino = null;
                    anuncio.propaganda = "";
                    anuncio.criacao = Helpers.DateTimeTron;
                    //Financeiro
                    anuncio.idFinanceiroConta = User.Identity.idFinanceiroConta();
                    anuncio.financeiroData = anuncio.criacao;
                    anuncio.financeiroObservacao = "";
                    anuncio.totalTempoExibido = 0;
                    IList<AnuncioTempoModel> anuncioTempo = await AnuncioTempoApi();
                    anuncio.tempoExibicao = anuncioTempo.FirstOrDefault(x => x.id == anuncio.tempoExibicao).valor;
                    if (anuncio.idAnuncioTipo == (int)Utils.AnuncioTipo.Carrocel)
                    {
                        anuncio.valorUnitario = (anuncio.valor ?? 0) / (anuncio.horasDisponibilizadas ?? 1);
                        double totalSegundosNaHora = 3600;
                        anuncio.totalExibicao = ((anuncio.horasDisponibilizadas ?? 1) * totalSegundosNaHora) / (anuncio.tempoExibicao);
                    }
                    else
                    {
                        //Form devolve o id. aqui pega o valor da quantidade dado esse id
                        IList<AnuncioQuantidadeModel> anuncioQuantidade = await AnuncioQuantidadeApi();
                        anuncio.totalExibicao = anuncioQuantidade.FirstOrDefault(x => x.id == anuncio.totalExibicao).valor;
                    }

                    #endregion

                    #region Salvar

                    try
                    {
                        retorno = await AnuncioCreate(anuncio);
                        if (retorno < 1)
                        {
                            #region Erro
                            //AlertErro
                            await SetViewBags(anuncio, anuncio.idAnuncioTipo);
                            ViewBag.Erro = "Ops! um erro ocorreu. Não foi possível gravar a campanha! (Erro. 03)";
                            LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName() + " - 03", "Não foi psssível gravar a campanha!");
                            return View(anuncio);
                            #endregion
                        }

                        //Grava Associativa Anuncio x RedeSocial
                        if (redesSociais != null)
                        {
                            foreach (int item in redesSociais)
                            {
                                //Retorno contem o id do anuncio criado
                                retornoRedeSocial = await AnuncioRedeSocialCreate(retorno, item);
                            }
                        }
                        else
                        {
                            //Se não selecianoado grava como todas
                            retornoRedeSocial = await AnuncioRedeSocialCreate(retorno, 2); //2 - e todos na tabela de rede_social
                        }
                    }
                    catch (Exception ex)
                    {
                        #region Erro
                        //AlertErro
                        await SetViewBags(anuncio, anuncio.idAnuncioTipo);
                        ViewBag.Erro = "Ops! um erro ocorreu. Não foi possível gravar a campanha! (Erro. 01)";
                        LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), ex.Message);
                        return View(anuncio);
                        #endregion
                    }

                    #endregion

                    #region SalvarImagem

                    try
                    {
                        strArquivo = retorno.ToString("D6") + "_Anuncio." + extencao;
                        var filePath = Local.Configuracao("PATH_CDN") + "\\propaganda\\" + strArquivo;
                        using (var fileSteam = new FileStream(filePath, FileMode.Create))
                        {
                            await imagem.CopyToAsync(fileSteam);
                        }
                        anuncio.propaganda = strArquivo;
                        anuncio.id = retorno;
                        anuncio.token = Helpers.Morpho(anuncio.id + "|" + anuncio.idUsuario + "|" + anuncio.propaganda + "|" + anuncio.tempoExibicao + "|" + anuncio.totalExibicao, TipoCriptografia.Criptografa);
                        anuncio.financeiroToken = Helpers.Morpho(anuncio.id + "|" + anuncio.idFinanceiroConta + "|" + anuncio.idAnuncio + "|" + anuncio.valor, TipoCriptografia.Criptografa);
                    }
                    catch (Exception ex)
                    {
                        #region Erro
                        //AlertErro
                        await SetViewBags(anuncio, anuncio.idAnuncioTipo);
                        ViewBag.Erro = "Ops! um erro ocorreu. Não foi possível salvar a imagem!";
                        LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName() + " - Salvar Imagem", ex.Message);
                        return View(anuncio);
                        #endregion
                    }
                    try
                    {
                        retorno = await AnuncioUpdate(anuncio);
                    }
                    catch (Exception ex)
                    {
                        #region Erro
                        //AlertErro
                        await SetViewBags(anuncio, anuncio.idAnuncioTipo);
                        ViewBag.Erro = "Ops! um erro ocorreu. Não foi possível gravar a campanha! (Erro. 02)";
                        LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), ex.Message);
                        return View(anuncio);
                        #endregion
                    }
                    #endregion
                }
                else
                {
                    #region Erro inesperado
                    //AlertErro
                    await SetViewBags(anuncio, anuncio.idAnuncioTipo);
                    ViewBag.Erro = "Ops! um erro inesperado ocorreu. Não foi possível gravar a campanha!";
                    LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName() + " - ModelState", "");
                    return View(anuncio);
                    #endregion
                }

                #region Mensagem
                ViewBag.MensagemVoltarTexto = "O Anúncio foi criado com sucesso!";
                ViewBag.MensagemVoltarLink = Url.Action("Index", "Campanhas");
                #endregion

                await SetViewBags(anuncio, anuncio.idAnuncioTipo);
            }
            catch (Exception ex)
            {
                #region Tratamento de erro
                string ticket = LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), ex.Message);
                return RedirectToAction("Erro", "Home", new { Titulo = "Erro em Anúncios", Mensagem = "Foi aberto um ticket de n. " + ticket + " para esse incidente." });
                #endregion
            }

            return View(anuncio);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            AnuncioSaldoModel anuncio = null;
            try
            {
                #region Init

                if (id == null)
                {
                    return RedirectToAction("Index", "Campanhas");
                }
                int idLocal = id ?? 0;

                #endregion

                #region Anuncio

                anuncio = await AnuncioByIdApi(idLocal);

                #endregion

                #region ViewBag

                await SetViewBags(anuncio, anuncio.idAnuncioTipo);

                #endregion

            }
            catch (Exception ex)
            {
                #region Tratamento de erro
                string ticket = LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), ex.Message);
                return RedirectToAction("Erro", "Home", new { Titulo = "Erro em Anúncios", Mensagem = "Foi aberto um ticket de n. " + ticket + " para esse incidente." });
                #endregion
            }

            return View(anuncio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AnuncioSaldoModel anuncioEdit,
                                             string valorRealString,
                                             string horasDisponibilizadasString,
                                             int[] redesSociais,
                                             IFormFile imagem)
        {
            int retorno = 0;
            int retornoRedeSocial = 0;
            string extencao = "jpg";
            string strArquivo = "";
            decimal decValor = 0m;
            int intValor = 0;


            #region Moeda

            FinanceiroMoedaModel moeda = await ObterMoedaApi();
            string mascara = moeda.simbolo + " " + moeda.mascara;
            ViewBag.Moeda = moeda;

            #endregion

            #region valida valores

            //Obtem anuncio atual na base de dados
            AnuncioSaldoModel anuncio = await AnuncioByIdApi(anuncioEdit.id);
            anuncioEdit.propaganda = anuncio.propaganda;

            if (anuncio == null)
            {
                await SetViewBags(anuncioEdit, anuncio.idAnuncioTipo);
                ViewBag.Erro = "Não foi possível obter as informações da campanha!";
                return View(anuncioEdit);
            }
            anuncioEdit.idAnuncioTipo = anuncio.idAnuncioTipo;

            if (String.IsNullOrEmpty(valorRealString))
            {
                await SetViewBags(anuncioEdit, anuncio.idAnuncioTipo);
                ViewBag.Erro = "Um valor (" + moeda.simbolo + ") deve ser informado!";
                return View(anuncioEdit);
            }
            else
            {
                string valor = valorRealString.Replace(moeda.simbolo, "").Replace("_", "").Replace(".", "").Replace(",", ".").Trim();
                try
                {
                    decValor = Decimal.Parse(valor, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName() + " - valor", ex.Message);
                    await SetViewBags(anuncioEdit, anuncio.idAnuncioTipo);
                    ViewBag.Erro = "O valor (" + moeda.simbolo + ") informado não é valido!";
                    return View(anuncioEdit);
                }
                if (decValor <= 0)
                {
                    await SetViewBags(anuncioEdit, anuncio.idAnuncioTipo);
                    ViewBag.Erro = "O valor (" + moeda.simbolo + ") informado não é valido!";
                    return View(anuncioEdit);
                }
                else
                {
                    anuncioEdit.valor = decValor;
                }
            }

            if (String.IsNullOrEmpty(horasDisponibilizadasString))
            {
                await SetViewBags(anuncioEdit, anuncio.idAnuncioTipo);
                ViewBag.Erro = "A informação em 'Horas disponibilizadas' não esta correta!";
                return View(anuncioEdit);
            }
            else
            {
                string valor = horasDisponibilizadasString.Replace("_", "").Replace(".", "").Trim();
                try
                {
                    intValor = int.Parse(valor, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName() + " - valor", ex.Message);
                    await SetViewBags(anuncioEdit, anuncio.idAnuncioTipo);
                    ViewBag.Erro = "A informação em 'Horas disponibilizadas' não esta correta!";
                    return View(anuncioEdit);
                }
                if (intValor <= 0)
                {
                    await SetViewBags(anuncioEdit, anuncio.idAnuncioTipo);
                    ViewBag.Erro = "A informação em 'Horas disponibilizadas' não esta correta!";
                    return View(anuncioEdit);
                }
                else
                {
                    anuncioEdit.horasDisponibilizadas = intValor;
                }
            }

            if (redesSociais == null)
            {
                await SetViewBags(anuncioEdit, anuncio.idAnuncioTipo);
                ViewBag.Erro = "Por favor, informe as redes sociais!";
                return View(anuncioEdit);
            }
            else
            {
                bool validRedeSocial = false;
                foreach (int item in redesSociais)
                {
                    validRedeSocial = true;
                    break;
                }
                if (!validRedeSocial)
                {
                    await SetViewBags(anuncioEdit, anuncio.idAnuncioTipo);
                    ViewBag.Erro = "Por favor, informe as redes sociais!";
                    return View(anuncioEdit);
                }
            }

            #endregion

            if (ModelState.IsValid)
            {
                #region Validacao

                //Caso não seja enviada uma imagem, não efetua alteração na mesma
                if ((imagem != null && imagem.Length > 0))
                {
                    //verifica se é png, jpg ou jpeg no nome do arquivo
                    string strExtencao = imagem.FileName.Substring(imagem.FileName.IndexOf(".") + 1).ToLower();
                    if (!(strExtencao == "jpg" || strExtencao == "png" || strExtencao == "jpeg" || strExtencao == "gif"))
                    {
                        //Não é uma imagem valida
                        //AlertErro
                        await SetViewBags(anuncioEdit, anuncio.idAnuncioTipo);
                        ViewBag.Erro = "A imagem deve ser do tipo jpg, png ou gif!";
                        return View(anuncioEdit);
                    }
                    //verifica se é png, jpg ou jpeg no conteudo do arquivo
                    string[] contentType = imagem.ContentType.Split('/');
                    if (contentType.Length > 1 && (contentType[1] == "png" || contentType[1] == "jpg" || contentType[1] == "jpeg" || contentType[1] == "gif"))
                    {
                        extencao = contentType[1];
                        if (extencao == "jpeg")
                        {
                            extencao = "jpg";
                        }
                    }
                    if (!(extencao == "png" || extencao == "jpg" || extencao == "gif"))
                    {
                        //Não é uma imagem valida
                        //AlertErro
                        await SetViewBags(anuncioEdit, anuncio.idAnuncioTipo);
                        ViewBag.Erro = "A imagem deve ser do tipo jpg, png ou gif";
                        return View(anuncioEdit);
                    }
                }

                #endregion

                #region Ajuste
                //Verifica se há saldo para uso na campanha
                decimal saldo = await Saldo();
                //valor para credito caso diminua o valor
                decimal? diferencaSaldo = 0;
               
                //Verifica diferença entre valores
                if (anuncio.valor > anuncioEdit.valor)
                {
                    //diferença
                    diferencaSaldo = anuncioEdit.valor - anuncio.valor;
                }
                else if (anuncio.valor < anuncioEdit.valor)
                {
                    //diferença para credito
                    diferencaSaldo = anuncioEdit.valor - anuncio.valor;
                    if (saldo < diferencaSaldo)
                    {
                        await SetViewBags(anuncioEdit, anuncio.idAnuncioTipo);
                        ViewBag.Erro = "Não há saldo suficiente para o valor informado!";
                        return View(anuncioEdit);
                    }
                }

                anuncio.idAnuncio = anuncioEdit.id;
                //Alteração comum em anuncio atual
                anuncio.atualizacao = Helpers.DataAtualInt;
                anuncio.idUsuario = User.Identity.Id();
                anuncio.idAnuncioSituacao = anuncioEdit.idAnuncioSituacao;

                //Altera o anuncio atual para as alterações enviadas no anuncioEdit via form
                anuncio.nome = anuncioEdit.nome;
                anuncio.valor = anuncioEdit.valor;
                anuncio.observacao = anuncioEdit.observacao;
                //Form devolve o id. aqui pega o valor do tempo dado esse id

                IList<AnuncioTempoModel> anuncioTempo = await AnuncioTempoApi();
                anuncio.tempoExibicao = anuncioTempo.FirstOrDefault(x => x.id == anuncioEdit.tempoExibicao).valor;
                anuncio.horasDisponibilizadas = anuncioEdit.horasDisponibilizadas;

                if (anuncio.idAnuncioTipo == (int)Utils.AnuncioTipo.Carrocel)
                {
                    anuncio.valorUnitario = (anuncioEdit.valor ?? 0) / (anuncioEdit.horasDisponibilizadas ?? 1);
                    double totalSegundosNaHora = 3600;
                    anuncio.totalExibicao = ((anuncioEdit.horasDisponibilizadas ?? 1) * totalSegundosNaHora) / (anuncio.tempoExibicao);
                }
                else
                {
                    //Form devolve o id. aqui pega o valor da quantidade dado esse id
                    IList<AnuncioQuantidadeModel> anuncioQuantidade = await AnuncioQuantidadeApi();
                    anuncio.totalExibicao = anuncioQuantidade.FirstOrDefault(x => x.id == anuncio.totalExibicao).valor;
                }

                //Financeiro
                anuncio.idFinanceiroConta = User.Identity.idFinanceiroConta();
                anuncio.financeiroData = Helpers.DateTimeTron;
                anuncio.financeiroObservacao = "";

                //para exibir a imagem, caso ela não seja alterada
                anuncioEdit.propaganda = anuncio.propaganda;
                
                #endregion

                #region SalvarImagem
                //Caso não seja enviada uma imagem, não efetua alteração na mesma
                if ((imagem != null && imagem.Length > 0))
                {
                    string filePath = "";
                    try
                    {
                        strArquivo = anuncio.id.ToString("D6") + "_Anuncio." + extencao;
                        filePath = Local.Configuracao("PATH_CDN") + @"\\propaganda\\" + strArquivo;
                        using (var fileSteam = new FileStream(filePath, FileMode.Create))
                        {
                            await imagem.CopyToAsync(fileSteam);
                        }
                        anuncioEdit.propaganda = strArquivo;
                        anuncio.propaganda = anuncioEdit.propaganda;
                    }
                    catch (Exception ex)
                    {
                        #region Erro
                        //AlertErro
                        await SetViewBags(anuncio, anuncio.idAnuncioTipo);
                        ViewBag.Erro = "Ops! um erro ocorreu. Não foi possível salvar a imagem!";
                        LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName() + " - Gravar Imagem", ex.Message);
                        return View(anuncio);
                        #endregion
                    }
                }
                #endregion

                #region Salvar

                try
                {
                    retorno = await AnuncioUpdate(anuncio);
                    if (retorno < 1)
                    {
                        //AlertErro
                        await SetViewBags(anuncio, anuncio.idAnuncioTipo);
                        ViewBag.Erro = "Ops! um erro ocorreu. Não foi possível gravar a campanha! (Erro. 05)";
                        return View(anuncio);
                    }
                    //Grava Associativa Anuncio x RedeSocial
                    if (redesSociais != null)
                    {
                        //Remove os existentes
                        retornoRedeSocial = await AnuncioRedeSocialRemove(anuncio.id);

                        foreach (int item in redesSociais)
                        {
                            //Retorno contem o id do anuncio criado
                            retornoRedeSocial = await AnuncioRedeSocialCreate(anuncio.id, item);
                        }
                    }
                    else
                    {
                        //Se não selecianoado grava como todas
                        retornoRedeSocial = await AnuncioRedeSocialCreate(retorno, 1); //1 - e todos na tabela de rede_social
                    }
                }
                catch (Exception ex)
                {
                    #region Erro
                    LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName() + " - AnuncioUpdate - Catch", ex.Message);
                    //AlertErro
                    await SetViewBags(anuncio, anuncio.idAnuncioTipo);
                    ViewBag.Erro = "Ops! um erro ocorreu. Não foi possível gravar a campanha! (Erro. 01)";
                    return View(anuncio);
                    #endregion
                }

                #endregion

            }
            else
            {
                #region Erro inesperado
                //var errors = ModelState.Select(x => x.Value.Errors)
                //        .Where(y => y.Count > 0)
                //        .ToList();

                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                //AlertErro
                await SetViewBags(anuncioEdit, anuncio.idAnuncioTipo);
                ViewBag.Erro = "Ops! um erro inesperado ocorreu. Não foi possível salvar a campanha!";
                LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName() + " - ModelState", message);
                return View(anuncioEdit);
                #endregion
            }

            #region Mensagem
            ViewBag.MensagemVoltarTexto = "O Anúncio foi alterado com sucesso!";
            ViewBag.MensagemVoltarLink = Url.Action("Index", "Campanhas");
            #endregion
          
            await SetViewBags(anuncioEdit, anuncio.idAnuncioTipo);
            return View(anuncioEdit);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int? id)
        {
            int idLocal = id ?? 0;
            string strRet = "ok";
            int retorno;
            if (id != null)
            {
                try
                {
                    retorno = await AnuncioDelete(idLocal);
                }
                catch (Exception ex)
                {
                    #region Erro
                    LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName() + " - AnuncioDelete", ex.Message);
                    strRet = "Ops! um erro ocorreu. Não foi possível excluir a camanha! (Erro. 01)";
                    #endregion
                }
            }
            else
            {
                #region Erro inesperado
                LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName() + " - AnuncioDelete", "Id não informado");
                //AlertErro
                strRet = "Ops! uma camanha deve ser informada para exclusão!";
                #endregion
            }
            return Json(strRet);
        }

        #endregion

        #region API

        public async Task<IList<AnuncioSaldoModel>> AnuncioApi(int idUsuario)
        {
            string strRet;
            IList<AnuncioSaldoModel> lista = new List<AnuncioSaldoModel>();
            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/anuncio/getanunciobyidusuario", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { idUsuario = idUsuario });
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    lista = JsonConvert.DeserializeObject<IList<AnuncioSaldoModel>>(response.Content);
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

        public async Task<AnuncioSaldoModel> AnuncioByIdApi(int id)
        {
            string strRet;
            AnuncioSaldoModel anuncio = new AnuncioSaldoModel();
            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/anuncio/getbyidanuncio", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { id = id });
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    anuncio = JsonConvert.DeserializeObject<AnuncioSaldoModel>(response.Content);
                    return anuncio;
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

        public async Task<IList<AnuncioMidiaModel>> AnuncioMidiaApi()
        {
            string strRet;
            IList<AnuncioMidiaModel> lista = new List<AnuncioMidiaModel>();
            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/anuncio/getanunciomidia", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    lista = JsonConvert.DeserializeObject<IList<AnuncioMidiaModel>>(response.Content);
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

        public async Task<IList<AnuncioSituacaoModel>> AnuncioSituacaoApi()
        {
            string strRet;
            IList<AnuncioSituacaoModel> lista = new List<AnuncioSituacaoModel>();
            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/anuncio/getanunciosituacao", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    lista = JsonConvert.DeserializeObject<IList<AnuncioSituacaoModel>>(response.Content);
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
                var request = new RestRequest("/api/anuncio/getanunciotipo", Method.POST);
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

        public async Task<IList<AnuncioGrupoModel>> AnuncioGrupoApi()
        {
            string strRet;
            IList<AnuncioGrupoModel> lista = new List<AnuncioGrupoModel>();
            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/anuncio/getanunciogrupo", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    lista = JsonConvert.DeserializeObject<IList<AnuncioGrupoModel>>(response.Content);
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

        public async Task<AnuncioTipoModel> AnuncioTipoByIdApi(int id)
        {
            string strRet;
            AnuncioTipoModel ret = new AnuncioTipoModel();
            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/anuncio/getanunciotipoById", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { id = id });
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ret = JsonConvert.DeserializeObject<AnuncioTipoModel>(response.Content);
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
            LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), strRet);
            return null;
        }

        public async Task<IList<AnuncioTempoModel>> AnuncioTempoApi()
        {
            string strRet;
            IList<AnuncioTempoModel> lista = new List<AnuncioTempoModel>();
            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/anuncio/getanunciotempo", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    lista = JsonConvert.DeserializeObject<IList<AnuncioTempoModel>>(response.Content);
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

        public async Task<IList<AnuncioQuantidadeModel>> AnuncioQuantidadeApi()
        {
            string strRet;
            IList<AnuncioQuantidadeModel> lista = new List<AnuncioQuantidadeModel>();
            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/anuncio/getanuncioquantidade", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    lista = JsonConvert.DeserializeObject<IList<AnuncioQuantidadeModel>>(response.Content);
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

        public async Task<int> AnuncioCreate(AnuncioSaldoModel model)
        {
            int retorno = 0;
            string strRet = "";
            try
            {
                var request = new RestRequest("/api/anuncio/createanuncio", Method.POST);
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

        public async Task<int> AnuncioUpdate(AnuncioSaldoModel model)
        {
            int retorno = 0;
            string strRet = "";
            try
            {
                var request = new RestRequest("/api/anuncio/updateanuncio", Method.POST);
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

        public async Task<int> AnuncioDelete(int id)
        {
            int retorno = 0;
            string strRet = "";
            try
            {
                var request = new RestRequest("/api/anuncio/deletelogicanuncio", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { id = id });
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

        public async Task<int> AnuncioRedeSocialCreate(int idAnuncio, int idRedeSocial)
        {
            int retorno = 0;
            string strRet = "";
            AnuncioRedeSocialModel model = new AnuncioRedeSocialModel
            {
                idAnuncio = idAnuncio,
                idRedeSocial = idRedeSocial,
                idAtivo = 1,
                atualizacao = Helpers.DataAtualInt,
                criacao = Helpers.DataAtualInt
            };

            try
            {
                var request = new RestRequest("/api/anuncio/createAnuncioRedeSocial", Method.POST);
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

        public async Task<int> AnuncioRedeSocialRemove(int idAnuncio)
        {
            int retorno = 0;
            string strRet = "";

            try
            {
                var request = new RestRequest("/api/redesocial/deleteRedeSocialSelecionado", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { idAnuncio = idAnuncio });
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

        public async Task<IList<RedeSocialSelecionadoModel>> RedeSocialSelecionadoApi(int idAnuncio)
        {
            string strRet;
            IList<RedeSocialSelecionadoModel> lista = new List<RedeSocialSelecionadoModel>();
            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/RedeSocial/getRedeSocialSelecionado", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { idAnuncio = idAnuncio });
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    lista = JsonConvert.DeserializeObject<IList<RedeSocialSelecionadoModel>>(response.Content);
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

        public async Task<FinanceiroSaldoModel> FinanceiroSaldoApi(int idFinanceiroConta, int? idAnuncio)
        {
            string strRet;
            FinanceiroSaldoModel financeiroSaldoModel = new FinanceiroSaldoModel();
            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/Financeiro/getSaldoByIdAnuncioConta", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { idFinanceiroConta = idFinanceiroConta, idAnuncio = idAnuncio });
                var response = _clientAPI.Execute(request);

                //Converte resposta com sucesso.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    financeiroSaldoModel = JsonConvert.DeserializeObject<FinanceiroSaldoModel>(response.Content);
                    return financeiroSaldoModel;
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

        #endregion

        #region Function

        public async Task<string> RedesSociaisSelecionadas(int idAnuncio)
        {
            string strRet = "";
            try
            {
                //Para Edit
                IList<RedeSocialSelecionadoModel> redeSocial = await RedeSocialSelecionadoApi(idAnuncio);
                foreach (RedeSocialSelecionadoModel item in redeSocial)
                {
                    if (item.selecionado)
                    {
                        if (item.nome == "Todas")
                        {
                            strRet += "<span class='navi-icon mr-2'><i class='flaticon2-grids todas'></i></span>";
                        }
                        else
                        {
                            strRet += "<span class='navi-icon mr-2'><i class='socicon-" + item.nome.ToLower() + " " + item.nome.ToLower() + "'></i></span>";
                        }
                    }
                }
                return strRet;
            }
            catch (Exception ex)
            {
                strRet = ex.Message;
            }
            LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), strRet);
            return "";
        }

        public async Task<decimal> Saldo()
        {
            #region Saldo

            int idFinanceiroConta = User.Identity.idFinanceiroConta();
            FinanceiroSaldoModel saldo = await FinanceiroSaldoApi(idFinanceiroConta, null);
            return saldo.valor; 

            #endregion
        }

        private async Task<FinanceiroMoedaModel> ObterMoedaApi()
        {
            FinanceiroContaModel conta = await ObterContaByIdUsuarioApi(idUsuario);
            
            string strRet;
            FinanceiroMoedaModel moeda = new FinanceiroMoedaModel();

            try
            {
                //Cria a transação.
                var request = new RestRequest("/api/Financeiro/GetFinanceiroMoedaById", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", await TokenToApi());
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { id = conta.idMoeda });

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
            LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), strRet);
            return moeda;
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
            LogErro(nomeController, Utils.Funcoes.GetCurrentMethodName(), strRet);
            return conta;
        }

        #endregion

        #region ViewBag

        public async Task<bool> SetViewBags(AnuncioSaldoModel model, int? idTipoAnuncio)
        {
            if (model != null && model.propaganda != null)
            {
                ViewBag.UrlImage = Local.Configuracao("URL_CDN") + "propaganda/" + model.propaganda;
            }

            #region anuncioTipo

            IList<AnuncioTipoModel> anuncioTipo = await AnuncioTipoApi();
            ViewBag.AnuncioTipo = anuncioTipo.Where(x => x.id != 1).ToList();
            //ViewBag.idAnuncioTipo = null;
            if (anuncioTipo != null)
            {
                //remove todos id = 1 da lista, não pode ser usado no streamers é para tronagem comum
                anuncioTipo = anuncioTipo.Where(x => x.id != 1).ToList();
                string nomeAnuncioTipo = "";
                string iconeAnuncioTipo = "";
                AnuncioTipoModel rowAnuncioTipo;
                int intIdTipoAnuncio = idTipoAnuncio ?? (int)Utils.AnuncioTipo.Carrocel; //Default é 2 - Carrossel
                rowAnuncioTipo = anuncioTipo.SingleOrDefault(x => x.id == idTipoAnuncio);
                nomeAnuncioTipo = rowAnuncioTipo != null ? rowAnuncioTipo.nome : String.Empty;
                iconeAnuncioTipo = rowAnuncioTipo != null ? rowAnuncioTipo.icone : String.Empty;
                ViewBag.AnuncioTipoNome = nomeAnuncioTipo;
                ViewBag.AnuncioTipoIcon = iconeAnuncioTipo;
                ViewBag.AnuncioTipoId = intIdTipoAnuncio;
            }

            #endregion

            #region anuncioSituacao

            IList<AnuncioSituacaoModel> anuncioSituacao = await AnuncioSituacaoApi();
            ViewBag.idAnuncioSituacao = null;
            if (anuncioSituacao != null)
            {
                if (model == null)
                {
                    ViewBag.idAnuncioSituacao = new SelectList(anuncioSituacao, "id", "nome");
                }
                else
                {
                    ViewBag.idAnuncioSituacao = new SelectList(anuncioSituacao, "id", "nome", model.idAnuncioSituacao);
                    foreach (var item in ViewBag.idAnuncioSituacao)
                    {
                        if (item.Value == model.idAnuncioSituacao.ToString())
                        {
                            item.Selected = true;
                            break;
                        }
                    }
                }
            }

            #endregion

            #region anuncioMidia

            IList<AnuncioMidiaModel> anuncioMidia = await AnuncioMidiaApi();
            ViewBag.idMidia = null;
            if (anuncioMidia != null)
            {
                if (model == null)
                {
                    ViewBag.idMidia = new SelectList(anuncioMidia, "id", "nome");
                }
                else
                {
                    ViewBag.idMidia = new SelectList(anuncioMidia, "id", "nome", model.idMidia);
                    foreach (var item in ViewBag.idMidia)
                    {
                        if (item.Value == model.idMidia.ToString())
                        {
                            item.Selected = true;
                            break;
                        }
                    }
                }
            }

            #endregion

            #region anuncioTempo

            IList<AnuncioTempoModel> anuncioTempo = await AnuncioTempoApi();
            ViewBag.tempoExibicao = null;
            if (anuncioTempo != null)
            {
                if (model == null)
                {
                    ViewBag.tempoExibicao = new SelectList(anuncioTempo, "id", "valor");
                }
                else
                {
                    ViewBag.tempoExibicao = new SelectList(anuncioTempo, "id", "valor");
                    foreach (var item in ViewBag.tempoExibicao)
                    {
                        if (model.tempoExibicao < 15) //Se menor que 15 é busca por id
                        {
                            if (item.Value == model.tempoExibicao.ToString())
                            {
                                item.Selected = true;
                                break;
                            }
                        }
                        else
                        {
                            if (item.Text == model.tempoExibicao.ToString())
                            {
                                item.Selected = true;
                                break;
                            }

                        }
                    }
                }
            }

            #endregion 

            #region anuncioQuantidade

            IList<AnuncioQuantidadeModel> anuncioQuantidade = await AnuncioQuantidadeApi();
            ViewBag.totalExibicao = null;
            if (anuncioQuantidade != null)
            {
                if (model == null)
                {
                    ViewBag.totalExibicao = new SelectList(anuncioQuantidade, "id", "valor");
                }
                else
                {
                    ViewBag.totalExibicao = new SelectList(anuncioQuantidade, "id", "valor");
                    foreach (var item in ViewBag.totalExibicao)
                    {
                        if (model.totalExibicao < 50) //Se menor que 50 é busca por id
                        {
                            if (item.Value == model.totalExibicao.ToString())
                            {
                                item.Selected = true;
                                break;
                            }
                        }
                        else
                        {
                            if (item.Text == model.totalExibicao.ToString())
                            {
                                item.Selected = true;
                                break;
                            }
                        }
                    }
                }
            }

            #endregion

            #region RedeSocial

            IList<RedeSocialSelecionadoModel> redeSocial;
            if (model == null)
            {
                redeSocial = await RedeSocialSelecionadoApi(0); //para create 
                if (gRedeSocial == null)
                {
                    gRedeSocial = new int[] { 2 }; //Seta Twith como selecionado
                }
                foreach (int item in gRedeSocial)
                {
                    foreach (RedeSocialSelecionadoModel rede in redeSocial)
                    {
                        if (item == rede.id)
                        {
                            rede.selecionado = true;
                        }
                    }
                }
            }
            else
            {
                if (model.id == 0)
                {
                    redeSocial = await RedeSocialSelecionadoApi(0); //para create 
                    //Deu algo errado no create, deve devolver a tela com os dados digitados para nova tentativa
                    if (gRedeSocial == null) //gedit deve estar carregado aqui, mas por precaução...
                    {
                        gRedeSocial = new int[] { 1 }; //Seta Todas como selecionado
                    }
                    foreach (int item in gRedeSocial)
                    {
                        foreach (RedeSocialSelecionadoModel rede in redeSocial)
                        {
                            if (item == rede.id)
                            {
                                rede.selecionado = true;
                            }
                        }
                    }
                }
                else
                {
                    redeSocial = await RedeSocialSelecionadoApi(model.id); //Edit normal
                }

            }

            ViewBag.redeSocial = redeSocial;

            #endregion

            #region anuncioGrupo

            IList<AnuncioGrupoModel> anuncioGrupo = await AnuncioGrupoApi();
            ViewBag.idAnuncioGrupo = null;

            if (anuncioGrupo != null)
            {
                if (model == null)
                {
                    ViewBag.idAnuncioGrupo = new SelectList(anuncioGrupo, "id", "nome");
                }
                else
                {
                    ViewBag.idAnuncioGrupo = new SelectList(anuncioGrupo, "id", "nome", model.idAnuncioGrupo);
                }
            }

            #endregion

            #region Moeda

            FinanceiroMoedaModel moeda = await ObterMoedaApi();
            string mascara = moeda.simbolo + " " + moeda.mascara;
            ViewBag.Moeda = moeda;

            #endregion

            #region Saldo

            ViewBag.saldoPublicitario = null;
            decimal saldo = await Saldo();
            ViewBag.saldoPublicitario = saldo.ToString(mascara); 

            #endregion

            return true;
        }

        #endregion
    }
}
