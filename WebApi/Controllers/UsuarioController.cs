#region using

using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;
using WebApi.Authentication;
using WebApi.Helper;
using WebApi.Model;
using WebApi.Services;


#endregion

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class Usuario : ControllerBase
    {

        #region base

        private readonly IUnitOfWork unitOfWork;

        private readonly UserManager<ApplicationUser> userManager;
        // private readonly IConfiguration configuration;
        private readonly IMailService mailService;
        //  private readonly TokenValidationParameters tokenValidationParams;

        public Usuario(UserManager<ApplicationUser> userManager,
                      // IConfiguration configuration,
                       IMailService mailService,
                       IUnitOfWork unitOfWork
                      // TokenValidationParameters tokenValidationParams
            )
        {
            this.unitOfWork = unitOfWork;

            this.userManager = userManager;
           // this.configuration = configuration;
            this.mailService = mailService;
          //  this.tokenValidationParams = tokenValidationParams;
        }

        #endregion

        #region Custom

        #region Usuario

        #region Cadastro 

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExists = await userManager.FindByNameAsync(model.usuario);
            if (userExists != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Usuário já existe!!" });
            }

            ApplicationUser user = new ApplicationUser()
            {
                Email = model.email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.usuario,
                idUsuarioDados = model.idUsuarioDados,
                idEmpresa = model.idEmpresa,
                idGrupo = model.idGrupo,
                idArea = model.idArea,
                idUsuarioSituacao = model.idUsuarioSituacao,
                idPais = model.idPais,
                idSexo = model.idSexo,
                atualizacao = Utils.Helpers.DataAtualInt,
                criacao = Utils.Helpers.DataAtualInt,
                dataExpiracaoSenha = model.dataExpiracaoSenha,
                termoUsoEm = DateTime.Now,
                nome = model.nome,
                senha = Helpers.Morpho(model.password, TipoCriptografia.Criptografa),
                idIdioma = model.idIdioma,
                avatar = model.avatar ?? "000000_Avatar.png",
                googleAuthenticatorSecretKey = "",
            };

            try
            {
                var result = await userManager.CreateAsync(user, model.password);
                if (result.Succeeded)
                {
                    var currentUser = await userManager.FindByNameAsync(user.UserName);
                    var roleresult = await userManager.AddToRoleAsync(currentUser, model.perfil);
                    if (roleresult.Succeeded)
                    {
                        UsuarioNomeModel userEmail = new UsuarioNomeModel();
                        userEmail.usuarioNome = currentUser.UserName;
                        string retEnvioEmail = await EmailConfirmacao(userEmail);

                        if (retEnvioEmail != "ok")
                        {
                            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Não foi possível enviar o email de confirmação!" });
                        }
                    }
                    else
                    {
                        string erros = "";
                        foreach (var error in result.Errors)
                        {
                            erros += error.Description + "<br/>";
                        }
                        //Traduz
                        //erros = erros.Replace("", "");
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = erros });
                    }
                }
                else
                {
                    string erros = "";
                    foreach (var error in result.Errors)
                    {
                        erros += error.Description + "<br/>";
                    }
                    //Traduz
                    erros = erros.Replace("Passwords must be at least 6 characters", "A Senha deve ter pelo menos 6 caracteres");
                    erros = erros.Replace("Passwords must have at least one non alphanumeric character", "A senha deve ter pelo menos um caractere não alfanumérico");
                    erros = erros.Replace("Passwords must have at least one digit", "A senha deve ter pelo menos um dígito");
                    erros = erros.Replace("Passwords must have at least one uppercase", "A senha deve ter pelo menos um caracter maiúsculo");
                    erros = erros.Replace("Passwords must have at least one lowercase", "A senha deve ter pelo menos um caracter minúsculo");

                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = erros });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }

            return Ok(new Response { Status = "Success", Message = "Usuário criado com sucesso!" });
        }

        [HttpPost]
        [Route("confirmarCadastro")]
        /// <summary>a entrada do userneme aqui é criptografado</summary>
        public async Task<IActionResult> ConfirmarCadastro([FromBody] UsuarioNomeTokenModel model)
        {
            if (String.IsNullOrEmpty(model.usuarioNomeToken))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "token não informado!" });
            }
            string dado = Helpers.Morpho(model.usuarioNomeToken, TipoCriptografia.Descriptografa);
            if (String.IsNullOrEmpty(dado))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "token não é válido!" });
            }
            string funcao = "";
            string chave = "";
            string usuario = "";
            int validade = 0;

            string[] dados = dado.Split('|');
            try
            {
                funcao = dados[0].ToString();
                chave = dados[1].ToString(); //ToDo - Salver em vanco e confirmar se chave é a mesma (SEGURANCA)
                usuario = dados[2].ToString();
                validade = Convert.ToInt32(dados[3].ToString());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "token não é válido!!" });
            }

            if (validade < Helpers.DataAtualInt) //Configuracao tem o total em dias
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "O email enviado perdeu a validade, caso não tenha se logado no MULTIPLIX, por favor faça um novo cadastro." });
            }

            var user = await userManager.FindByNameAsync(usuario);
            if (user == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Usuário não encontrado!" });
            }
            try
            {
                //Atualiza id_Ativo para 1 e email_confirmed para true
                var data = await unitOfWork.Usuario.EmailConfimAsync(user.Id);

                switch (data)
                {
                    case 0:
                        return Ok(new Response { Status = "Success", Message = "Seu cadastro já havia sido confirmado..." });
                    case < -1:
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "A criação do usuário falhou! Por favor verifique as informações enviadas e tente novamente." });
                    default:
                        return Ok(new Response { Status = "Success", Message = "Seu cadastro foi confirmado com sucesso..." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }

        }

        [HttpPost]
        [Route("emailConfirmacao")]
        public async Task<String> EmailConfirmacao([FromBody] UsuarioNomeModel model)
        {
            try
            {
                var user = await userManager.FindByNameAsync(model.usuarioNome);
                //Envia email confirmacao
                WelcomeRequest request = new WelcomeRequest()
                {
                    ToEmail = user.Email,
                    UserName = user.nome,
                    link = Geral.Configuracao("URL_CONFIRMAR_USUARIO") + "?user=" + Helpers.Morpho("CC|" + Helpers.Chave() + "|" + user.UserName + "|" + Helpers.DataAtualIntAddDias(Helper.Geral.ConfiguracaoInt("VALIDADE_CONFIRMAR_USUARIO_EM_DIAS")), TipoCriptografia.Criptografa),
                };
                await mailService.SendWelcomeEmailAsync(request);
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        [HttpPost]
        [Route("getById")]
        public async Task<IActionResult> GetbyId([FromBody] IdModel model)
        {
            try
            {
                UsuarioModel ret = await unitOfWork.Usuario.GetByIdAsync(model.id);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }
        
        [HttpPost]
        [Route("getListaUsuarioCrudAsync")]
        public async Task<IActionResult> GetListaUsuarioCrudAsync()
        {
            try
            {
                IReadOnlyList<UsuarioListaCrudModel> ret = await unitOfWork.Usuario.GetListaUsuarioCrudAsync();
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("getListaUsuarioCrudByPerfilAsync")]
        public async Task<IActionResult> GetListaUsuarioCrudByPerfilAsync([FromBody] stringModel perfil)
        {
            try
            {
                IReadOnlyList<UsuarioListaCrudModel> ret = await unitOfWork.Usuario.GetListaUsuarioCrudByPerfilAsync(perfil.valor);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("updateDoisFatoresAsync")]
        public async Task<IActionResult> UpdateDoisFatoresAsync([FromBody] DoisFatoresModel model)
        {
            try
            {
                model.autenticadorGoogleChaveSecreta = Utils.Helpers.Morpho(model.autenticadorGoogleChaveSecreta, Utils.TipoCriptografia.Criptografa);
                int ret = await unitOfWork.Usuario.UpdateDoisFatoresAsync(model);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }


        #endregion

        #region UsuarioSituacao

        [HttpPost]
        [Route("getUsuarioSituacao")]
        public async Task<IActionResult> GetUsuarioSituacao()
        {
            try
            {
                IReadOnlyList<UsuarioSituacaoModel> ret = await unitOfWork.Usuario.GetUsuarioSituacaoAsync();
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        #endregion
                
        #region UsuarioPerfil

        [HttpPost]
        [Route("getUsuarioPerfilAsync")]
        public async Task<IActionResult> GetUsuarioPerfilAsync([FromBody] IdModel model)
        {
            try
            {
                int ret = await unitOfWork.Usuario.GetUsuarioPerfilAsync(model.id);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        #endregion

        #region UsuarioSexo

        [HttpPost]
        [Route("getUsuarioSexo")]
        public async Task<IActionResult> GetUsuarioSexo()
        {
            try
            {
                IReadOnlyList<UsuarioSexoModel> ret = await unitOfWork.Usuario.GetUsuarioSexoAsync();
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        #endregion

        #region UsuarioPais

        [HttpPost]
        [Route("getUsuarioPaisbyIdUsuario")]
        public async Task<IActionResult> GetUsuarioPaisbyIdUsuario(int id)
        {
            try
            {
                UsuarioPaisModel ret = await unitOfWork.Usuario.GetUsuarioPaisByIdUsuarioAsync(id);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        #endregion

        #region UsuarioDados

        [HttpPost]
        [Route("createUsuarioDados")]
        public async Task<IActionResult> CreateUsuarioDados([FromBody] UsuarioDadosModel model)
        {
            try
            {
                int ret = await unitOfWork.Usuario.AddUsuarioDadosAsync(model);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("updateUsuarioDados")]
        public async Task<IActionResult> UpdateUsuarioDados([FromBody] UsuarioDadosModel model)
        {
            try
            {
                int ret = await unitOfWork.Usuario.UpdateUsuarioDadosAsync(model);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("getUsuarioDados")]
        public async Task<IActionResult> GetUsuarioDados([FromBody] IdModel model)
        {
            try
            {
                UsuarioDadosModel ret = await unitOfWork.Usuario.GetUsuarioDadosAsync(model.id);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        #endregion

        #endregion

        #region InsUpdDel

        //Não há create aqui, quem cria o usuario é o identity veja AuthenticateController

        [HttpPost]
        [Route("updateUsuario")]
        public async Task<IActionResult> UpdateUsuario([FromBody] UsuarioModel model)
        {
            try
            {
                int ret = await unitOfWork.Usuario.UpdateAsync(model);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("deleteLogicUsuario")]
        public async Task<IActionResult> DeleteLogicUsuario([FromBody] IdModel model)
        {
            try
            {
                int ret = await unitOfWork.Usuario.DeleteLogicAsync(model.id);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }

        #endregion

    }
}
