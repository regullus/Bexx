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
    class TronarRepository : ITronarRepository
    {
        #region Base

        private readonly IConfiguration configuration;
        public TronarRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        #endregion

        #region Crud

        public async Task<int> AddAsync(TronarModel entity)
        {
            //entity.AddedOn = DateTime.Now;
            StringBuilder sql = new StringBuilder();
            sql.Append(" INSERT INTO tronar (");
            sql.Append("    id_usuario,");
            sql.Append("    id_anuncio,");
            sql.Append("    id_tronar_situacao,");
            sql.Append("    id_anuncio_tipo,");
            sql.Append("    link_original,");
            sql.Append("    link_tronado,");
            sql.Append("    titulo_pagina_original,");
            sql.Append("    descricao_pagina_original,");
            sql.Append("    imagem_pagina_original,");
            sql.Append("    rede_social,");
            sql.Append("    usuario_rede_social,");
            sql.Append("    atualizacao,");
            sql.Append("    criacao,");
            sql.Append("    acessos)");
            sql.Append(" VALUES (");
            sql.Append("    @idUsuario,");
            sql.Append("    @idAnuncio,");
            sql.Append("    @idTronarSituacao,");
            sql.Append("    @idAnuncioTipo,");
            sql.Append("    @linkOriginal,");
            sql.Append("    @linkTronado,");
            sql.Append("    @tituloPaginaOriginal,");
            sql.Append("    @descricaoPaginaOriginal,");
            sql.Append("    @imagemPaginaOriginal,");
            sql.Append("    @redeSocial,");
            sql.Append("    @usuarioRedeSocial,");
            sql.Append("    @atualizacao,");
            sql.Append("    @criacao,");
            sql.Append("    @acessos");
            sql.Append("    )");
            sql.Append(" RETURNING id;");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                int result = await connection.ExecuteScalarAsync<int>(Helpers.QBuild(sql), entity);
                //Apos criar o registro deve-se gerar e fazer o update no link tronado
                entity.id = result;
                entity.linkTronado = Utils.Helpers.Base64Encode(result);
                int ret = await UpdateAsync(entity);

                return result;
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            var sql = "DELETE FROM tronar WHERE id = @id";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { id = id });
                return result;
            }
        }

        public async Task<IReadOnlyList<TronarModel>> GetAllAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    id_usuario as idUsuario,");
            sql.Append("    id_anuncio as idAnuncio,");
            sql.Append("    id_tronar_situacao as idTronarSituacao,");
            sql.Append("    id_anuncio_tipo as idAnuncioTipo,");
            sql.Append("    link_original as linkOriginal,");
            sql.Append("    link_tronado as linkTronado,");
            sql.Append("    titulo_pagina_original as tituloPaginaOriginal,");
            sql.Append("    descricao_pagina_original as descricaoPaginaOriginal,");
            sql.Append("    imagem_pagina_original as imagemPaginaOriginal,");
            sql.Append("    rede_social as redeSocial,");
            sql.Append("    usuario_rede_social as usuarioRedeSocial,");
            sql.Append("    atualizacao,");
            sql.Append("    criacao,");
            sql.Append("    acessos");
            sql.Append(" FROM");
            sql.Append("    tronar");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<TronarModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        public async Task<TronarModel> GetByIdAsync(int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    id_usuario as idUsuario,");
            sql.Append("    id_anuncio as idAnuncio,");
            sql.Append("    id_tronar_situacao as idTronarSituacao,");
            sql.Append("    id_anuncio_tipo as idAnuncioTipo,");
            sql.Append("    link_original as linkOriginal,");
            sql.Append("    link_tronado as linkTronado,");
            sql.Append("    titulo_pagina_original as tituloPaginaOriginal,");
            sql.Append("    descricao_pagina_original as descricaoPaginaOriginal,");
            sql.Append("    imagem_pagina_original as imagemPaginaOriginal,");
            sql.Append("    rede_social as redeSocial,");
            sql.Append("    usuario_rede_social as usuarioRedeSocial,");
            sql.Append("    atualizacao,");
            sql.Append("    criacao,");
            sql.Append("    acessos");
            sql.Append(" FROM");
            sql.Append("    tronar");
            sql.Append(" WHERE");
            sql.Append("    id = @id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<TronarModel>(Helpers.QBuild(sql), new { id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(TronarModel entity)
        {
            entity.atualizacao = Helpers.DataAtualInt;
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE");
            sql.Append("    tronar");
            sql.Append(" SET");
            sql.Append("     id_usuario=@idUsuario,");
            sql.Append("     id_anuncio=@idAnuncio,");
            sql.Append("     id_tronar_situacao=@idTronarSituacao,");
            sql.Append("     id_anuncio_tipo=@idAnuncioTipo,");
            sql.Append("     acessos=@acessos,");
            sql.Append("     link_original=@linkOriginal,");
            sql.Append("     link_tronado=@linkTronado,");
            sql.Append("     titulo_pagina_original=@tituloPaginaOriginal,");
            sql.Append("     descricao_pagina_original=@descricaoPaginaOriginal,");
            sql.Append("     imagem_pagina_original=@imagemPaginaOriginal,");
            sql.Append("     rede_social=@redeSocial,");
            sql.Append("     usuario_rede_social=@usuarioRedeSocial,");
            sql.Append("     atualizacao=@atualizacao,");
            sql.Append("     criacao=@criacao");
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
            TronarModel entity = await GetByIdAsync(id);

            entity.atualizacao = Helpers.DataAtualInt;
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE");
            sql.Append("    tronar");
            sql.Append(" SET");
            sql.Append("    id_tronar_situacao=3,"); //3 é exclusão
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

        #endregion

        #region Custom

        public async Task<IReadOnlyList<TronarSituacaoModel>> GetTronarSituacaoAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    criacao,");
            sql.Append("    nome,");
            sql.Append("    nome_ing as nomeIng,");
            sql.Append("    nome_esp as nomeEsp");
            sql.Append(" FROM");
            sql.Append("    tronar_situacao");
            sql.Append(" ORDER BY id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<TronarSituacaoModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        public async Task<TronarModel> GetByIdAnuncioTipoAsync(int idUsuario, int idAnuncioTipo)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    id_usuario as idUsuario,");
            sql.Append("    id_anuncio as idAnuncio,");
            sql.Append("    id_tronar_situacao as idTronarSituacao,");
            sql.Append("    id_anuncio_tipo as idAnuncioTipo,");
            sql.Append("    link_original as linkOriginal,");
            sql.Append("    link_tronado as linkTronado,");
            sql.Append("    titulo_pagina_original as tituloPaginaOriginal,");
            sql.Append("    descricao_pagina_original as descricaoPaginaOriginal,");
            sql.Append("    imagem_pagina_original as imagemPaginaOriginal,");
            sql.Append("    rede_social as redeSocial,");
            sql.Append("    usuario_rede_social as usuarioRedeSocial,");
            sql.Append("    atualizacao,");
            sql.Append("    criacao,");
            sql.Append("    acessos");
            sql.Append(" FROM");
            sql.Append("    tronar");
            sql.Append(" WHERE");
            sql.Append("    id_usuario = @idUsuario and");
            sql.Append("    id_anuncio_tipo = @idAnuncioTipo");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<TronarModel>(Helpers.QBuild(sql), new { idUsuario = idUsuario, idAnuncioTipo = idAnuncioTipo });
                return result;
            }
        }

        public async Task<TronarUsuarioRedeSocialModel> GetUsuarioRedeSocialAsync(int idUsuario, string redeSocial)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT DISTINCT");
            sql.Append("    usuario_rede_social as usuarioRedeSocial");
            sql.Append(" FROM");
            sql.Append("    tronar");
            sql.Append(" WHERE");
            sql.Append("    id_usuario = @idUsuario and");
            sql.Append("    rede_social = @redeSocial");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<TronarUsuarioRedeSocialModel>(Helpers.QBuild(sql), new { idUsuario = idUsuario, redeSocial = redeSocial });
                return result;
            }
        }

        public async Task<int> UpdateUsuarioRedeSocialAsync(int idUsuario, string redeSocial, string usuarioRedeSocial)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE");
            sql.Append("    tronar");
            sql.Append(" SET");
            sql.Append("    usuario_rede_social = @usuarioRedeSocial");
            sql.Append(" WHERE");
            sql.Append("    id_usuario = @idUsuario and");
            sql.Append("    rede_social = @redeSocial");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(Helpers.QBuild(sql), new { idUsuario= idUsuario, redeSocial = redeSocial, usuarioRedeSocial = usuarioRedeSocial });
                return result;
            }
        }

        public async Task<TronarModel> GetByIdAnuncioTipoRedeSocialAsync(int idUsuario, int idAnuncioTipo, string redeSocial)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    id_usuario as idUsuario,");
            sql.Append("    id_anuncio as idAnuncio,");
            sql.Append("    id_tronar_situacao as idTronarSituacao,");
            sql.Append("    id_anuncio_tipo as idAnuncioTipo,");
            sql.Append("    link_original as linkOriginal,");
            sql.Append("    link_tronado as linkTronado,");
            sql.Append("    titulo_pagina_original as tituloPaginaOriginal,");
            sql.Append("    descricao_pagina_original as descricaoPaginaOriginal,");
            sql.Append("    imagem_pagina_original as imagemPaginaOriginal,");
            sql.Append("    rede_social as redeSocial,");
            sql.Append("    usuario_rede_social as usuarioRedeSocial,");
            sql.Append("    atualizacao,");
            sql.Append("    criacao,");
            sql.Append("    acessos");
            sql.Append(" FROM");
            sql.Append("    tronar");
            sql.Append(" WHERE");
            sql.Append("    id_usuario = @idUsuario and");
            sql.Append("    id_anuncio_tipo = @idAnuncioTipo and");
            sql.Append("    rede_social = @redeSocial");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<TronarModel>(Helpers.QBuild(sql), new { idUsuario = idUsuario, idAnuncioTipo = idAnuncioTipo, redeSocial = redeSocial });
                return result;
            }
        }

        public async Task<TronarModel> GetByLinkTronadoAsync(string linkTronado)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    id_usuario as idUsuario,");
            sql.Append("    id_anuncio as idAnuncio,");
            sql.Append("    id_tronar_situacao as idTronarSituacao,");
            sql.Append("    id_anuncio_tipo as idAnuncioTipo,");
            sql.Append("    link_original as linkOriginal,");
            sql.Append("    link_tronado as linkTronado,");
            sql.Append("    titulo_pagina_original as tituloPaginaOriginal,");
            sql.Append("    descricao_pagina_original as descricaoPaginaOriginal,");
            sql.Append("    imagem_pagina_original as imagemPaginaOriginal,");
            sql.Append("    rede_social as redeSocial,");
            sql.Append("    usuario_rede_social as usuarioRedeSocial,");
            sql.Append("    atualizacao,");
            sql.Append("    criacao,");
            sql.Append("    acessos");
            sql.Append(" FROM");
            sql.Append("    tronar");
            sql.Append(" WHERE");
            sql.Append("    link_tronado = @linkTronado");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<TronarModel>(Helpers.QBuild(sql), new { linkTronado = linkTronado });
                return result;
            }
        }

        #endregion
    }
}
