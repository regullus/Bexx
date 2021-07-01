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
    class MensagemRepository : IMensagemRepository
    {
        #region Base

        private readonly IConfiguration configuration;
        public MensagemRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        #endregion

        #region Crud

        public async Task<int> AddAsync(MensagemModel entity)
        {
            //entity.AddedOn = DateTime.Now;
            StringBuilder sql = new StringBuilder();
            sql.Append(" INSERT INTO mensagem(");
            sql.Append("    id_destino,");
            sql.Append("    id_usuario_origem,");
            sql.Append("    id_usuario_destino,");
            sql.Append("    id_ativo,");
            sql.Append("    id_empresa,");
            sql.Append("    id_mensagem_tipo,");
            sql.Append("    texto,");
            sql.Append("    texto_ing,");
            sql.Append("    texto_esp,");
            sql.Append("    data,");
            sql.Append("    validade,");
            sql.Append("    urgente");
            sql.Append(" VALUES (");
            sql.Append("    @idDestino,");
            sql.Append("    @idUsuarioOrigem,");
            sql.Append("    @idUsuarioDestino,");
            sql.Append("    @idAtivo,");
            sql.Append("    @idEmpresa,");
            sql.Append("    @idMensagemTipo,");
            sql.Append("    @texto,");
            sql.Append("    @textoIng,");
            sql.Append("    @textoEsp,");
            sql.Append("    @data,");
            sql.Append("    @validade,");
            sql.Append("    @urgente");
            sql.Append("    )");
            sql.Append(" RETURNING id;");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                int result = await connection.ExecuteScalarAsync<int>(Helpers.QBuild(sql), entity);
                return result;
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            var sql = "DELETE FROM mensagem Where id = @id";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { id = id });
                return result;
            }
        }

        public async Task<IReadOnlyList<MensagemModel>> GetAllAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("     id,");
            sql.Append("    id_destino as idDestino,");
            sql.Append("    id_usuario_origem as idUsuarioOrigem,");
            sql.Append("    id_usuario_destino as idUsuarioDestino,");
            sql.Append("    id_ativo as idAtivo,");
            sql.Append("    id_empresa as idEmpresa,");
            sql.Append("    id_mensagem_tipo as idMensagemTipo,");
            sql.Append("    texto,");
            sql.Append("    texto_ing as textoIng,");
            sql.Append("    texto_esp as textoEsp,");
            sql.Append("    data,");
            sql.Append("    validade,");
            sql.Append("    urgente");
            sql.Append(" FROM");
            sql.Append("    mensagem ");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<MensagemModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        public async Task<MensagemModel> GetByIdAsync(int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("     id,");
            sql.Append("    id_destino as idDestino,");
            sql.Append("    id_usuario_origem as idUsuarioOrigem,");
            sql.Append("    id_usuario_destino as idUsuarioDestino,");
            sql.Append("    id_ativo as idAtivo,");
            sql.Append("    id_empresa as idEmpresa,");
            sql.Append("    id_mensagem_tipo as idMensagemTipo,");
            sql.Append("    texto,");
            sql.Append("    texto_ing as textoIng,");
            sql.Append("    texto_esp as textoEsp,");
            sql.Append("    data,");
            sql.Append("    validade,");
            sql.Append("    urgente");
            sql.Append(" FROM");
            sql.Append("    mensagem ");
            sql.Append(" WHERE");
            sql.Append("    id = @id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<MensagemModel>(Helpers.QBuild(sql), new { id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(MensagemModel entity)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE");
            sql.Append("     mensagem");
            sql.Append(" SET");
            sql.Append("     id_destino=@idDestino,");
            sql.Append("     id_usuario_origem=@idUsuarioOrigem,");
            sql.Append("     id_usuario_destino=@idUsuarioDestino,");
            sql.Append("     id_ativo=@idAtivo,");
            sql.Append("     id_empresa=@idEmpresa,");
            sql.Append("     id_mensagem_tipo=@idMensagemTipo,");
            sql.Append("     texto=@texto,");
            sql.Append("     texto_ing=@textoIng,");
            sql.Append("     texto_esp=@textoEsp,");
            sql.Append("     data=@data,");
            sql.Append("     validade=@validade,");
            sql.Append("     urgente=@urgente");
            sql.Append(" WHERE");
            sql.Append("    id = @id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(Helpers.QBuild(sql), entity);
                return result;
            }
        }

        #endregion

        #region Custom
        public async Task<IReadOnlyList<MensagemTipoModel>> GetMensagemTipoAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    atualizacao,");
            sql.Append("    nome,");
            sql.Append("    nome_ing as nomeIng,");
            sql.Append("    nome_esp as nomeEsp");
            sql.Append(" FROM");
            sql.Append("    mensagem_tipo");
            sql.Append(" ORDER BY id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<MensagemTipoModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        #endregion

    }
}
