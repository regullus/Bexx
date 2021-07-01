#region using

using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using Utils;
using Models;
using System.Threading.Tasks;
using Interfaces;
using RestSharp;
using Newtonsoft.Json;
using System.Net;

#endregion

namespace WebApi.Helper
{
    #region Geral

    public class Geral
    {
        private const string cLocal = "Geral";

        #region Geral

        public static string Configuracao(string chave)
        {
            string valor = "";

            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile($"appsettings.json").Build();
            valor = builder.GetSection("Configuration").GetValue<string>(chave);

            return valor;
        }

        public static int ConfiguracaoInt(string chave)
        {
            int valor = 0;

            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile($"appsettings.json").Build();
            valor = builder.GetSection("Configuration").GetValue<int>(chave);

            return valor;
        }

        public static string LogErro(string Origem, string strBreveDescricao, string strErroMensagem, int idUsuario)
        {
            string strTicket = Helpers.Chave();
            if (!String.IsNullOrEmpty(strErroMensagem))
            {
                string strErro = "";
                try
                {
                    TicketsModel model = new TicketsModel();
                    model.idUsuario = idUsuario;
                    model.origem = Origem;
                    model.descricao = strBreveDescricao;
                    model.valor = strErroMensagem;
                    model.ticket = strTicket;
                    model.atualizacao = Helpers.DateTimeTron;
                    model.criacao = Helpers.DateTimeTron;
                    model.status = 1; //Ativo
                    Ticket(model, idUsuario.ToString());

                }
                catch (Exception ex)
                {
                    LogFile("API_LogErro", idUsuario + "|" + Origem + "|" + strBreveDescricao + "|" + strErroMensagem + "|" + strTicket);
                    strErro = ex.Message;
                }
            }
            return strTicket;
        }

        #region Apoio uso Local

        private static void LogFile(string strFileName, string strMensagem)
        {
            strFileName = Helpers.DataAtual() + "_" + strFileName + ".txt";

            string strPath = Configuracao("PATH_CDN") + "\\Log";
            System.IO.StreamWriter sw;
            try
            {
                sw = new System.IO.StreamWriter(strPath + strFileName, true);
                sw.WriteLine(strMensagem);
                sw.Close();
            }
            catch (Exception ex)
            {
                Geral.LogErro(cLocal, "LogFile - " + strFileName, ex.Message, 1);
            }
            finally
            {
                sw = null;
            }
        }

        private static void Ticket(TicketsModel model, string userToken)
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
                    RestClient _clientAPI = new RestClient(Configuracao("URL_API"));
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

        private static string TrataErro(IRestResponse resp)
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
        
        #endregion

        #endregion
    }

    #endregion

}
