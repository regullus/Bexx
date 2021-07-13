#region using

using Interfaces;
using Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Dapper;
using Utils;

#endregion region

namespace Infrastructure.Repository
{
    class UsuarioRepository : IUsuarioRepository
    {
        #region Base

        private readonly IConfiguration configuration;
        public UsuarioRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        #endregion

        #region Crud
        //Não há criação de usuario aqui, quem cria é o identity em WebApi
        //As tabelas não se chamam usuario e sim user por padrao do identity
        //aqui usa em porqugres 'usuario' para manter o padrao no sistema

        public async Task<int> DeleteAsync(int id)
        {
            var sql = "DELETE FROM users WHERE id = @id";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { id = id });
                return result;
            }
        }

        public async Task<IReadOnlyList<UsuarioModel>> GetAllAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    id_usuario_dados as idUsuarioDados,");
            sql.Append("    id_imagem as idImagem,");
            sql.Append("    id_empresa as idEmpresa,");
            sql.Append("    id_filial as idGrupo,");
            sql.Append("    id_user_situacao as idUsuarioSituacao,");
            sql.Append("    id_plano as idArea,");
            sql.Append("    id_pais as idPais,");
            sql.Append("    id_sexo as idSexo,");
            sql.Append("    atualizacao,");
            sql.Append("    nome,");
            sql.Append("    email,");
            sql.Append("    login,");
            sql.Append("    senha,");
            sql.Append("    two_factor_enabled as doisFatoresHabilitados,");            
            sql.Append("    data_expiracao_senha as dataExpiracaoSenha,");
            sql.Append("    id_idioma as idIdioma,");
            sql.Append("    avatar as avatar,");
            sql.Append("    celular as celular,");
            sql.Append("    celular_confirmado as celularConfirmado");
            sql.Append(" FROM");
            sql.Append("    users");
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<UsuarioModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        public async Task<UsuarioModel> GetByIdAsync(int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    id_usuario_dados as idUsuarioDados,");
            sql.Append("    id_imagem as idImagem,");
            sql.Append("    id_empresa as idEmpresa,");
            sql.Append("    id_filial as idGrupo,");
            sql.Append("    id_user_situacao as idUsuarioSituacao,");
            sql.Append("    id_plano as idArea,");
            sql.Append("    id_pais as idPais,");
            sql.Append("    id_sexo as idSexo,");
            sql.Append("    atualizacao,");
            sql.Append("    nome,");
            sql.Append("    email,");
            sql.Append("    login,");
            sql.Append("    senha,");
            sql.Append("    two_factor_enabled as doisFatoresHabilitados,");
            sql.Append("    data_expiracao_senha as dataExpiracaoSenha,");
            sql.Append("    id_idioma as idIdioma,");
            sql.Append("    avatar,");
            sql.Append("    celular,");
            sql.Append("    celular_confirmado as celularConfirmado");
            sql.Append(" FROM");
            sql.Append("    users");
            sql.Append(" WHERE");
            sql.Append("    id = @id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<UsuarioModel>(Helpers.QBuild(sql), new { id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(UsuarioModel entity)
        {
            entity.atualizacao = Helpers.DataAtualInt;
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE");
            sql.Append("    users");
            sql.Append(" SET");
            sql.Append("    id_usuario_dados=@idUsuarioDados,");
            sql.Append("    id_imagem=@idImagem,");
            sql.Append("    id_empresa=@idEmpresa,");
            sql.Append("    id_filial=@idGrupo,");
            sql.Append("    id_user_situacao=@idUsuarioSituacao,");
            sql.Append("    id_plano=@idArea,");
            sql.Append("    id_pais=@idPais,");
            sql.Append("    id_sexo=@idSexo,");
            sql.Append("    atualizacao=@atualizacao,");
            sql.Append("    nome=@nome,");
            sql.Append("    email=@email,");
            sql.Append("    senha=@senha,");
            sql.Append("    two_factor_enabled=@doisFatoresHabilitados,");
            sql.Append("    data_expiracao_senha=@dataExpiracaoSenha,");
            sql.Append("    id_idioma=@idIdioma,");
            sql.Append("    avatar=@avatar,");
            sql.Append("    celular=@celular,");
            sql.Append("    celular_confirmado=@celularConfirmado");
            sql.Append(" WHERE");
            sql.Append("    id = @id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {

                connection.Open();
                var result = await connection.ExecuteAsync(Helpers.QBuild(sql), entity);
                return result;
            }
        }

        public async Task<int> DeleteLogicAsync(int id)
        {
            UsuarioModel entity = await GetByIdAsync(id);

            entity.atualizacao = Helpers.DataAtualInt;
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE");
            sql.Append("    users");
            sql.Append(" SET");
            sql.Append("    id_anuncio_situacao=2,"); //2 é Desativado
            sql.Append("    atualizacao=@atualizacao");
            sql.Append(" WHERE");
            sql.Append("    id=@id;");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(Helpers.QBuild(sql), entity);
                return result;
            }
        }

        public async Task<int> UpdateDoisFatoresAsync(DoisFatoresModel entity)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE");
            sql.Append("    users");
            sql.Append(" SET");
            sql.Append("    two_factor_enabled=@doisFatoresHabilitado,");
            sql.Append("    google_authenticator_secretkey=@autenticadorGoogleChaveSecreta");
            sql.Append(" WHERE");
            sql.Append("    id = @id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(Helpers.QBuild(sql), entity);
                return result;
            }
        }

        public async Task<IReadOnlyList<UsuarioListaCrudModel>> GetListaUsuarioCrudAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    u.id,");       
            sql.Append("    u.nome,");         
            sql.Append("    u.login");           
            sql.Append(" FROM");
            sql.Append("    users     AS u,");
            sql.Append("    user_role AS r");
            sql.Append(" WHERE");
            sql.Append("    u.id = r.user_id AND");
            sql.Append("    r.role_id != " + (int)Utils.Perfil.Master );

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<UsuarioListaCrudModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        public async Task<IReadOnlyList<UsuarioListaCrudModel>> GetListaUsuarioCrudByPerfilAsync(string perfil)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    u.id,");
            sql.Append("    u.nome,");
            sql.Append("    u.login");
            sql.Append(" FROM");
            sql.Append("    users     AS u,");
            sql.Append("    user_role AS r");
            sql.Append(" WHERE");
            sql.Append("    u.id = r.user_id and");
            sql.Append("    r.role_id in (" + perfil + ");");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<UsuarioListaCrudModel>(Helpers.QBuild(sql), new { perfil = perfil });
                return result.ToList();
            }
        }

        #endregion

        #region Custom

        #region EmailConfim

        public async Task<int> EmailConfimAsync(int id)
        {
            //ToDo Verificar se já não esta confirmado, se não confima
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    count(id)");
            sql.Append(" FROM");
            sql.Append("    users");
            sql.Append(" WHERE ");
            sql.Append("    id = @id and");
            sql.Append("    email_confirmed = true");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                var result1 = await connection.QueryFirstAsync<int>(Helpers.QBuild(sql), new { id = id });
                if (result1 < 1)
                {
                    sql.Clear();
                    sql.Append(" UPDATE");
                    sql.Append("    users");
                    sql.Append(" SET");
                    sql.Append("    atualizacao = @atualizacao,");
                    sql.Append("    id_user_situacao = 1,");
                    sql.Append("    email_confirmed = true");
                    sql.Append(" WHERE");
                    sql.Append("    id = @id");

                    var result2 = await connection.ExecuteAsync(Helpers.QBuild(sql), new { id = id, atualizacao = Helpers.DataAtualInt });
                    return result2;
                }
                else
                {
                    return 0;
                }
            }
        }

        #endregion

        #region UsuarioSexo

        public async Task<IReadOnlyList<UsuarioSexoModel>> GetUsuarioSexoAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    nome,");
            sql.Append("    nome_ing as nomeIng,");
            sql.Append("    nome_esp as nomeEsp");
            sql.Append(" FROM");
            sql.Append("    user_sexo");
            sql.Append(" ORDER BY id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<UsuarioSexoModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        #endregion

        #region UsuarioPais

        public async Task<UsuarioPaisModel> GetUsuarioPaisByIdUsuarioAsync(int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    pais.id,");
            sql.Append("    pais.nome,");
            sql.Append("    pais.nome_ing as nomeIng,");
            sql.Append("    pais.nome_esp as nomeEsp,");
            sql.Append("    pais.nome_internacional as nomeInternacional,");
            sql.Append("    pais.culture as cultura");
            sql.Append(" FROM");
            sql.Append("    user usu,");
            sql.Append("    base_pais pais");
            sql.Append(" WHERE");
            sql.Append("    usu.id = @id and");
            sql.Append("    usu.id_pais = pais.id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<UsuarioPaisModel>(Helpers.QBuild(sql), new { id = id });
                return result;
            }
        }

        #endregion

        #region UsuarioSituacao

        public async Task<IReadOnlyList<UsuarioSituacaoModel>> GetUsuarioSituacaoAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    criacao,");
            sql.Append("    nome,");
            sql.Append("    nome_ing as nomeIng,");
            sql.Append("    nome_esp as nomeEsp");
            sql.Append(" FROM");
            sql.Append("    user_situacao");
            sql.Append(" ORDER BY id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<UsuarioSituacaoModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        #endregion

        #region UsuarioPerfil

        public async Task<int> GetUsuarioPerfilAsync(int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    role_id");      
            sql.Append(" FROM");
            sql.Append("    user_role");
            sql.Append(" WHERE");
            sql.Append("    user_id=@id;");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<int>(Helpers.QBuild(sql), new { id = id });

                return result.FirstOrDefault();
            }
        }

        #endregion

        #region UsuarioDados

        public async Task<int> AddUsuarioDadosAsync(UsuarioDadosModel entity)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("INSERT INTO user_data (");
            sql.Append("    atualizacao,");
            sql.Append("    data_ultimo_acesso,");
            sql.Append("    exibe_boas_vindas,");
            sql.Append("    social_twitch,");
            sql.Append("    social_facebook,");
            sql.Append("    social_twitter,");
            sql.Append("    social_youtube,");
            sql.Append("    social_instagram,");
            sql.Append("    cpf,");
            sql.Append("    id_instituicao_financeira,");
            sql.Append("    agencia,");
            sql.Append("    agencia_digito,");
            sql.Append("    conta,");
            sql.Append("    conta_digito,");
            sql.Append("    chave_pix");
            sql.Append("    )");
            sql.Append("VALUES (");
            sql.Append("    @Atualizacao,");
            sql.Append("    @DataUltimoAcesso,");
            sql.Append("    @ExibeBoasVindas,");
            sql.Append("    @SocialTwitch,");
            sql.Append("    @SocialFacebook,");
            sql.Append("    @SocialTwitter,");
            sql.Append("    @SocialYouTube,");
            sql.Append("    @SocialInstagram,");
            sql.Append("    @cpf,");
            sql.Append("    @idInstituicaoFinanceira,");
            sql.Append("    @agencia,");
            sql.Append("    @agenciaDigito,");
            sql.Append("    @conta,");
            sql.Append("    @contaDigito,");
            sql.Append("    @chavePix");
            sql.Append("    )");
            sql.Append(" RETURNING id;");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                int result = await connection.ExecuteScalarAsync<int>(Helpers.QBuild(sql), entity);
                return result;
            }
        }

        public async Task<int> UpdateUsuarioDadosAsync(UsuarioDadosModel entity)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE");
            sql.Append("    user_data");
            sql.Append(" SET");
            sql.Append("    atualizacao=@Atualizacao,");
            sql.Append("    data_ultimo_acesso=@DataUltimoAcesso,");
            sql.Append("    exibe_boas_vindas=@ExibeBoasVindas,");
            sql.Append("    social_twitch=@SocialTwitch,");
            sql.Append("    social_facebook=@SocialFacebook,");
            sql.Append("    social_twitter=@SocialTwitter,");
            sql.Append("    social_youtube=@SocialYouTube,");
            sql.Append("    social_instagram=@SocialInstagram,");
            sql.Append("    cpf=@cpf,");
            sql.Append("    id_instituicao_financeira=@idInstituicaoFinanceira,");
            sql.Append("    agencia=@agencia,");
            sql.Append("    agencia_digito=@agenciaDigito,");
            sql.Append("    conta=@conta,");
            sql.Append("    conta_digito=@contaDigito,");
            sql.Append("    chave_pix=@chavePix");
            sql.Append(" WHERE");
            sql.Append("    id = @id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(Helpers.QBuild(sql), entity);
                return result;
            }
        }

        public async Task<UsuarioDadosModel> GetUsuarioDadosAsync(int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    atualizacao,");
            sql.Append("    data_ultimo_acesso as DataUltimoAcesso,");
            sql.Append("    exibe_boas_vindas as ExibeBoasVindas,");
            sql.Append("    social_twitch as SocialTwitch,");
            sql.Append("    social_facebook as SocialFacebook,");
            sql.Append("    social_twitter as SocialTwitter,");
            sql.Append("    social_youtube as SocialYouTube,");
            sql.Append("    social_instagram as SocialInstagram,");
            sql.Append("    cpf,");
            sql.Append("    id_instituicao_financeira as idInstituicaoFinanceira,");
            sql.Append("    agencia,");
            sql.Append("    agencia_digito as agenciaDigito,");
            sql.Append("    conta,");
            sql.Append("    conta_digito as contaDigito,");
            sql.Append("    chave_pix as chavePix");
            sql.Append(" FROM");
            sql.Append("    user_data");
            sql.Append(" WHERE");
            sql.Append("    id = @id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<UsuarioDadosModel>(Helpers.QBuild(sql), new { id = id });
                return result;
            }
        }

        #endregion

        #endregion

    }
}
