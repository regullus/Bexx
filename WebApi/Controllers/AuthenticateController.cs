#region using

using WebApi.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Utils;
using Models;
using WebApi.Helper;
using WebApi.Model;
using WebApi.Services;
using Interfaces;
using System.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;


#endregion

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration configuration;
        private readonly IMailService mailService;
        private readonly IUnitOfWork unitOfWork;
        private readonly TokenValidationParameters tokenValidationParams;
        private const string cLocal = "AuthenticateController";

        public AuthenticateController(UserManager<ApplicationUser> userManager,
                                      IConfiguration configuration,
                                      IMailService mailService,
                                      IUnitOfWork unitOfWork,
                                      TokenValidationParameters tokenValidationParams)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.mailService = mailService;
            this.unitOfWork = unitOfWork;
            this.tokenValidationParams = tokenValidationParams;
        }

        #region Login

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var user = await userManager.FindByNameAsync(model.userName);

                if (user != null)
                {
                    //Checa Senha
                    //int idUsuario = user.Id;
                    //UsuarioModel ret = await unitOfWork.Usuario.GetByIdAsync(idUsuario);
                    //string strPassord = Helpers.Morpho(ret.senha, TipoCriptografia.Descriptografa);

                    if (await userManager.CheckPasswordAsync(user, model.password))
                    {
                        UsuarioClaimModel usuarioClain = await GenerateJwtToken(user);
                        return Ok(usuarioClain);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }

            return Unauthorized();
        }

        [HttpPost]
        [Route("loginExiste")]
        public async Task<IActionResult> LoginExiste([FromBody] UserNameModel model)
        {
            string existe = "no";
            var userExists = await userManager.FindByNameAsync(model.UserName);
            if (userExists != null)
            {
                existe = "ok";
            }
            return Ok(new Response { Status = "Success", Message = existe });
        }

        #endregion

        #region Api

        private async Task<int> FinanceiroIdContaApi(int idUsuario)
        {
            int idConta = 0;
            try
            {
                FinanceiroContaModel ret = await unitOfWork.FinanceiroConta.GetByIdUsuarioAsync(idUsuario);
                if (ret != null)
                {
                    idConta = ret.id;
                }
                return idConta;
            }
            catch (Exception ex)
            {
                Geral.LogErro(cLocal, "FinanceiroIdContaApi", ex.Message, idUsuario);
                return 0;
            }
        }

        private async Task<int> addTokenRefresh(TokenRefreshModel refreshToken)
        {
            int ret = 0;
            try
            {
                ret = await unitOfWork.TokenRefresh.AddAsync(refreshToken);
                return ret;
            }
            catch (Exception ex)
            {
                Geral.LogErro(cLocal, "addTokenRefresh", ex.Message, refreshToken.idUsuario);
                return 0;
            }
        }

        private async Task<int> updateTokenRefresh(TokenRefreshModel refreshToken)
        {
            int ret = 0;
            try
            {
                ret = await unitOfWork.TokenRefresh.UpdateAsync(refreshToken);
                return ret;
            }
            catch (Exception ex)
            {
                Geral.LogErro(cLocal, "updateTokenRefresh", ex.Message, refreshToken.idUsuario);
                return 0;
            }
        }

        private async Task<TokenRefreshModel> getTokenRefresh(int idUsuario)
        {
            TokenRefreshModel ret;
            try
            {
                ret = await unitOfWork.TokenRefresh.GetByIdUsusarioAsync(idUsuario);
                return ret;
            }
            catch (Exception ex)
            {
                Geral.LogErro(cLocal, "updateTokenRefresh", ex.Message, idUsuario);
                return null;
            }
        }

        #endregion

        #region Cadastro 

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExists = await userManager.FindByNameAsync(model.userName);
            if (userExists != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseId { Id = 0 , Status = "Error", Message = "Usuário já existe!!" });
            }

            ApplicationUser user = new ApplicationUser()
            {
                Email = model.email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.userName,
                id_usuario_dados = model.idUsuarioDados,
                id_imagem = model.idImagem,
                id_empresa = model.idEmpresa,
                id_filial = model.idFilial,
                id_user_situacao = model.idUserSituacao,
                id_plano = model.idPlano,
                id_pais = model.idPais,
                id_sexo = model.idSexo,
                atualizacao = Utils.Helpers.DataAtualInt,
                criacao = Utils.Helpers.DataAtualInt,
                data_vencimento_plano = model.dataVencimentoPlano,
                data_expiracao_senha = model.dataExpiracaoSenha,
                termo_uso_em = DateTime.Now,
                nome = model.nome,
                senha = Helpers.Morpho(model.password, TipoCriptografia.Criptografa),
                id_idioma = model.idIdioma,
                avatar = model.avatar ?? "000000_Avatar.png",
                google_authenticator_secretkey = "",
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
                        UserNameModel userEmail = new UserNameModel();
                        userEmail.UserName = currentUser.UserName;
                        string retEnvioEmail = await EmailConfirmacao(userEmail);

                        if (retEnvioEmail != "ok")
                        {
                            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseId { Id = currentUser.Id , Status = "Error", Message = "Não foi possível enviar o email de confirmação!" });
                        }

                        return Ok(new ResponseId { Id = currentUser.Id, Status = "Success", Message = "Usuário criado com sucesso!" });
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
                        return StatusCode(StatusCodes.Status500InternalServerError, new ResponseId { Id = 0 , Status = "Error", Message = erros });
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

                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseId { Status = "Error", Message = erros });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseId { Id = 0 , Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("confirmarCadastro")]
        /// <summary>a entrada do userneme aqui é criptografado</summary>
        public async Task<IActionResult> ConfirmarCadastro([FromBody] UserNameTokenModel model)
        {
            if (String.IsNullOrEmpty(model.UserNameToken))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "token não informado!" });
            }
            string dado = Helpers.Morpho(model.UserNameToken, TipoCriptografia.Descriptografa);
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
                chave = dados[1].ToString(); //ToDo - Salvar em banco e confirmar se chave é a mesma (SEGURANCA)
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
        public async Task<String> EmailConfirmacao([FromBody] UserNameModel model)
        {
            try
            {
                var user = await userManager.FindByNameAsync(model.UserName);
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
                Geral.LogErro(cLocal, "EmailConfirmacao", ex.Message, 1);
                return ex.Message;
            }
        }
        #endregion

        #region EsqueceuSenha

        [HttpPost]
        [Route("esqueceuSenha")]
        public async Task<IActionResult> EsqueceuSenha([FromBody] UserNameModel model)
        {
            //Aqui ira enviar um email para o usuario, com o link para uma tela no WebApp
            //para que ele troque a senha

            //Tenta achar usuario por login
            var userExists = await userManager.FindByNameAsync(model.UserName);
            if (userExists == null)
            {
                //Tenta achar usuario por email
                userExists = await userManager.FindByEmailAsync(model.UserName);
            }

            if (userExists == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Email não existe!!" });
            }

            //Envia email confirmacao
            EsqueceuSenhaRequest request = new EsqueceuSenhaRequest()
            {
                ToEmail = userExists.Email,
                UserName = userExists.nome,
                link = Geral.Configuracao("URL_ESQUECEU_SENHA") + "?user=" + Helpers.Morpho("ES|" + Helpers.Chave() + "|" + userExists.UserName + "|" + Helpers.DataAtualIntAddDias(Helper.Geral.ConfiguracaoInt("VALIDADE_ESQUECI_SENHA_EM_DIAS")), TipoCriptografia.Criptografa),
            };
            try
            {
                await mailService.SendEsqueceuSenhaEmailAsync(request);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Não foi possível enviar o email de instruções para recuperação de senha!" });
            }
            return Ok(new Response { Status = "Success", Message = "Email de recuperação de senha enviado!" });
        }

        [HttpPost]
        [Route("novaSenha")]
        ///<summary>a entrada do userneme aqui é criptografado</summary>
        public async Task<IActionResult> NovaSenha([FromBody] LoginModel model)
        {
            if (String.IsNullOrEmpty(model.userName))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Usuário não foi informado!" });
            }
            if (String.IsNullOrEmpty(model.password))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Senha não foi informado!" });
            }
            try
            {
                var user = await userManager.FindByNameAsync(model.userName);
                if (user == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Usuário não encontrado!" });
                }
                //Troca a senha do usuario
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var result = await userManager.ResetPasswordAsync(user, token, model.password);

                if (result.Succeeded)
                {
                    string afetados = await UpdatePassword(user.Id, model.password);

                    if (!afetados.All(char.IsDigit)) //Caso não seja numero (total de registros afetados) é erro
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = afetados });
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
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Não foi possível trocar a senha!!" });
            }

            return Ok(new Response { Status = "Success", Message = "Sua senha foi alterada com sucesso!" });

        }

        #endregion

        #region Update

        private async Task<string> UpdatePassword(int id, string password)
        {
            try
            {
                UsuarioModel usuario = await unitOfWork.Usuario.GetByIdAsync(id);
                string oldPassword = Helpers.Morpho(usuario.senha, TipoCriptografia.Descriptografa);
                usuario.senha = Helpers.Morpho(password, TipoCriptografia.Criptografa);
                usuario.atualizacao = Helpers.DataAtualInt;
                int usuarioUpdate = await unitOfWork.Usuario.UpdateAsync(usuario);

                return usuarioUpdate.ToString();
            }
            catch (Exception ex)
            {
                Geral.LogErro(cLocal, "UpdatePassword", ex.Message, id);
                return ex.Message;
            }
        }

        #endregion

        #region Token

        [HttpPost]
        [Route("refreshtoken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest entity)
        {
            string ret = "ok";
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            if (ModelState.IsValid)
            {
                try
                {
                    var token = jwtTokenHandler.ReadJwtToken(entity.Token);

                    if (token is JwtSecurityToken jwtSecurityToken)
                    {
                        var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                        if (result == false)
                        {
                            ret = "O Token não é valido!";
                        }
                    }
                    var claims = token.Claims; //get the roles/claims in the token

                    var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

                    TokenRefreshModel tokenRefresh = await getTokenRefresh(entity.idUsuario);

                    if (tokenRefresh == null)
                    {
                        ret = "O TokenRefresh não existe!";
                    }
                    if (tokenRefresh == null || tokenRefresh.idAtivo != 1) //Ativo
                    {
                        ret = "O TokenRefresh não esta ativo!";
                    }
                    if (tokenRefresh != null && tokenRefresh.revogado)
                    {
                        ret = "O TokenRefresh foi revogado!";
                    }
                    // Validation 7 - validate the id
                    //var jti = token.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                    //if (tokenRefresh.idJWT != jti)
                    //{
                    //    ret = "O Token não confere!";
                    //}
                    //update current token 
                    if (ret == "ok")
                    {
                        var user = await userManager.FindByIdAsync(entity.idUsuario.ToString());
                        UsuarioClaimModel usuarioClain = await GenerateJwtToken(user);
                        return Ok(usuarioClain);
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = ret });
                    }
                }
                catch (Exception ex)
                {
                    Geral.LogErro(cLocal, "RefreshToken", ex.Message, entity.idUsuario);
                    throw;
                }
            }

            return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = ret });
        }

        private async Task<UsuarioClaimModel> GenerateJwtToken(ApplicationUser user)
        {
            var userRoles = await userManager.GetRolesAsync(user);

            DateTime dataExpiracao = DateTime.Now.AddSeconds(30); //Teste
            //DateTime dataExpiracao = DateTime.Now.AddHours(4);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            string roles = "";
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                roles += userRole + ",";
            }

            if (!String.IsNullOrEmpty(roles))
            {
                roles = roles.Remove(roles.Length - 1);
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: configuration["JWT:ValidIssuer"],
                audience: configuration["JWT:ValidAudience"],
                expires: dataExpiracao,
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            var retToken = new JwtSecurityTokenHandler().WriteToken(token);

            //getTokenRefresh
            TokenRefreshModel tokenRefresh = await getTokenRefresh(user.Id);

            if (tokenRefresh == null)
            {
                tokenRefresh = new TokenRefreshModel()
                {
                    idAtivo = 1,
                    idUsuario = user.Id,
                    idJWT = token.Id,
                    token = Helpers.GenerateRefreshToken,
                    revogado = false,
                    dataAdicao = DateTime.Now,
                    dataExpiracao = DateTime.Now.AddDays(1)
                };
                //Adicona registro na tabela tokenRefresh
                int ret = await addTokenRefresh(tokenRefresh);
            }
            else
            {
                tokenRefresh.idJWT = token.Id;
                tokenRefresh.token = Helpers.GenerateRefreshToken;
                tokenRefresh.dataAdicao = DateTime.UtcNow;
                tokenRefresh.dataExpiracao = DateTime.Now.AddDays(7);
                //update registro na tabela tokenRefresh
                int ret = await updateTokenRefresh(tokenRefresh);
            }

            string strIdioma = "pt-BR"; //Default
            IdiomaModel idioma = await unitOfWork.Fixo.GetIdiomaByIdAsync(user.id_idioma);

            if (!String.IsNullOrEmpty(idioma.sigla))
            {
                strIdioma = idioma.sigla;
            }

            //id da Conta finaceira do usuario, não existindo id = 0
            int idFinanceiroConta = await FinanceiroIdContaApi(user.Id);

            UsuarioClaimModel usuarioClain = new UsuarioClaimModel()
            {
                Id = user.Id,
                idAtivo = user.id_user_situacao,
                Nome = user.nome,
                Email = user.Email,
                Senha = user.senha,
                Roles = roles,
                Token = retToken,
                RefreshToken = tokenRefresh.token,
                Idioma = strIdioma,
                Expiracao = dataExpiracao,
                Avatar = user.avatar,
                DoisFatoresHabilitado = user.TwoFactorEnabled,
                idFinanceiroConta = idFinanceiroConta,
                AutenticadorGoogleChaveSecreta = user.google_authenticator_secretkey,
                idPais = user.id_pais,
            };

            return usuarioClain;
        }

        #endregion
    }
}
