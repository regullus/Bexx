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
using Models;
using Utils;
using RestSharp;
using Newtonsoft.Json;
using System.Net;

#endregion

namespace WebApp
{
    #region Identity

    public static class IdentityExtension
    {
        private const string classe = "IdentityExtension";
        public static string Avatar(this IIdentity identity)
        {
            string retorno;
            try
            {
                ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
                Claim claim = claimsIdentity.FindFirst(type: "Avatar");
                if (claim != null)
                    retorno = Local.Configuracao("URL_CDN") + @"avatar/" + claim.Value         + "?id=" + Utils.Helpers.Chave();
                else
                    retorno = Local.Configuracao("URL_CDN") + @"avatar/" + "000000_Avatar.png" + "?id=" + Utils.Helpers.Chave();
            }
            catch (Exception ex)
            {
                LogErro(classe, "Avatar", ex.Message, identity.Id());
                retorno = "";
            }
            return retorno;
        }      
        public static string AutenticadorGoogleChaveSecreta(this IIdentity identity)
        {
            string retorno;
            try
            {
                ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
                Claim claim = claimsIdentity.FindFirst(type: "AutenticadorGoogleChaveSecreta");
                retorno = Utils.Helpers.Morpho(claim.Value, Utils.TipoCriptografia.Descriptografa);
            }
            catch (Exception ex)
            {
                LogErro(classe, "AutenticadorGoogleChaveSecreta", ex.Message, identity.Id());
                retorno = "";
            }
            return retorno;
        }       
        public static bool DoisFatoresHabilitado(this IIdentity identity)
        {
            bool retorno;
            try
            {
                ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
                Claim claim = claimsIdentity.FindFirst(type: "DoisFatoresHabilitado");
                retorno = claim.Value.ToUpper() == "TRUE" ? true : false;
            }
            catch (Exception ex)
            {
                LogErro(classe, "DoisFatoresHabilitado", ex.Message, identity.Id());
                retorno = false;
            }
            return retorno;
        }
        public static string Email(this IIdentity identity)
        {
            string retorno;
            try
            {
                ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
                Claim claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                retorno = claim.Value;
            }
            catch (Exception ex)
            {
                LogErro(classe, "Email", ex.Message, identity.Id());
                retorno = "";
            }
            return retorno;
        }
        public static int Id(this IIdentity identity)
        {
            int retorno;
            try
            {
                ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
                Claim claim = claimsIdentity.FindFirst(type: "Id");
                retorno = Convert.ToInt32(claim.Value);
            }
            catch (Exception ex)
            {
                LogErro(classe, "Id", ex.Message, 1);
                retorno = 0;
            }
            return retorno;
        }
        public static int idAtivo(this IIdentity identity)
        {
            int retorno;
            try
            {
                ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
                Claim claim = claimsIdentity.FindFirst(type: "idAtivo");
                retorno = Convert.ToInt32(claim.Value);
            }
            catch (Exception ex)
            {
                LogErro(classe, "idAtivo", ex.Message, identity.Id());
                retorno = 0;
            }
            return retorno;
        }
        public static string Lembrar(this IIdentity identity)
        {
            string retorno;
            try
            {
                ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
                Claim claim = claimsIdentity.FindFirst(type: "Lembrar");
                retorno = claim.Value;
            }
            catch (Exception ex)
            {
                LogErro(classe, "Lembrar", ex.Message, identity.Id());
                retorno = "";
            }
            return retorno;
        }
        public static string Login(this IIdentity identity)
        {
            string retorno;
            try
            {
                ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
                Claim claim = claimsIdentity.FindFirst(ClaimTypes.Name);
                retorno = claim.Value;
            }
            catch (Exception ex)
            {
                LogErro(classe, "Login", ex.Message, identity.Id());
                retorno = "";
            }
            return retorno;
        }
        public static string Nome(this IIdentity identity)
        {
            string retorno;
            try
            {
                ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
                Claim claim = claimsIdentity.FindFirst(type: "Nome");
                retorno = claim.Value;
            }
            catch (Exception ex)
            {
                LogErro(classe, "Nome", ex.Message, identity.Id());
                retorno = "";
            }
            return retorno;
        }
        public static string Regra(this IIdentity identity)
        {
            string retorno;
            try
            {
                ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
                Claim claim = claimsIdentity.FindFirst(ClaimTypes.Role);
                string strRegra = claim.Value.Replace("Publicitario", "Publicitário");
                retorno = strRegra.Replace("Usuario", "Usuário");
            }
            catch (Exception ex)
            {
                LogErro(classe, "Regra", ex.Message, identity.Id());
                retorno = "";
            }
            return retorno;
        }
        public static string Role(this IIdentity identity)
        {
            string retorno;
            try
            {//Devolve a regra para uso em código
                ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
                Claim claim = claimsIdentity.FindFirst(ClaimTypes.Role);
                retorno = claim.Value;
            }
            catch (Exception ex)
            {
                LogErro(classe, "Role", ex.Message, identity.Id());
                retorno = "";
            }
            return retorno;
        }
        public static string Senha(this IIdentity identity)
        {
            string retorno;
            try
            {
                ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
                Claim claim = claimsIdentity.FindFirst(type: "Senha");
                retorno = claim.Value;
            }
            catch (Exception ex)
            {
                LogErro(classe, "Senha", ex.Message, identity.Id());
                retorno = "";
            }
            return retorno;
        }
        public static string Token(this IIdentity identity)
        {
            string retorno;
            try
            {
                ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
                Claim claim = claimsIdentity.FindFirst(type: "Token");
                retorno = claim.Value;
            }
            catch (Exception ex)
            {
                LogErro(classe, "Token", ex.Message, identity.Id());
                retorno = "";
            }
            return retorno;
        }
        public static string RefreshToken(this IIdentity identity)
        {
            string retorno;
            try
            {
                ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
                Claim claim = claimsIdentity.FindFirst(type: "RefreshToken");
                retorno = claim.Value;
            }
            catch (Exception ex)
            {
                LogErro(classe, "RefreshToken", ex.Message, identity.Id());
                retorno = "";
            }
            return retorno;
        }
        public static int idFinanceiroConta(this IIdentity identity)
        {
            int retorno;
            try
            {
                ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
                Claim claim = claimsIdentity.FindFirst(type: "idFinanceiroConta");
                retorno = Convert.ToInt32(claim.Value);
            }
            catch (Exception ex)
            {
                LogErro(classe, "idFinanceiroConta", ex.Message, identity.Id());
                retorno = 0;
            }
            return retorno;
        }
        public static string DataExpiracao(this IIdentity identity)
        {
            string retorno;
            try
            {
                ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
                Claim claim = claimsIdentity.FindFirst(type: "Expiracao");
                retorno = claim.Value;
            }
            catch (Exception ex)
            {
                LogErro(classe, "DataExpiracao", ex.Message, identity.Id());
                retorno = "";
            }
            return retorno;
        }
        public static bool TokenUpdate(this IIdentity identity, string valor)
        {
            try
            {
                ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
                Claim claim = claimsIdentity.FindFirst(type: "Token");
                claimsIdentity.RemoveClaim(claim);
                claimsIdentity.AddClaim(new Claim(type: "Token", value: valor));
            }
            catch (Exception ex)
            {
                LogErro(classe, "TokenUpdate", ex.Message, identity.Id());
                return false;
            }
            return true;
        }
        public static bool AvatarUpdate(this IIdentity identity, string valor)
        {
            try
            {
                ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
                Claim claim = claimsIdentity.FindFirst(type: "Avatar");
                claimsIdentity.RemoveClaim(claim);
                claimsIdentity.AddClaim(new Claim(type: "Avatar", value: valor));
            }
            catch (Exception ex)
            {
                LogErro(classe, "AvatarUpdate", ex.Message, identity.Id());
                return false;
            }
            return true;
        }
        public static string Idioma(this IIdentity identity)
        {
            string retorno;
            try
            {
                ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
                Claim claim = claimsIdentity.FindFirst(type: "Idioma");
                retorno = claim.Value;
            }
            catch (Exception ex)
            {
                LogErro(classe, "Idioma", ex.Message, identity.Id());
                retorno = "";
            }
            return retorno;
        }
        public static int idPais(this IIdentity identity)
        {
            int retorno;
            try
            {
                ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
                Claim claim = claimsIdentity.FindFirst(type: "Pais");
                retorno = Convert.ToInt32(claim.Value);
            }
            catch (Exception ex)
            {
                LogErro(classe, "Pais", ex.Message, identity.Id());
                retorno = 31; //Default Brasil
            }
            return retorno;
        }

        #region Apoio - Somente para uso local

        private static string LogErro(string Origem, string strBreveDescricao, string strErroMensagem, int idUsuario)
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
                    strErro = ex.Message;
                }
            }
            return strTicket;
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

        private static void LogFile(string strFileName, string strMensagem)
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
    }

    #endregion
}