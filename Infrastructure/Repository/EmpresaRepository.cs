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
    class EmpresaRepository : IEmpresaRepository
    {
        #region Base

        private readonly IConfiguration configuration;
        public EmpresaRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        #endregion

        #region Crud

        public async Task<int> AddAsync(EmpresaModel entity)
        {
            //entity.AddedOn = DateTime.Now;
            StringBuilder sql = new StringBuilder();
            sql.Append(" INSERT INTO empresa(");
            sql.Append("    atualizacao,");
            sql.Append("    id_cidade,");
            sql.Append("    id_administrador,");
            sql.Append("    id_idioma,");
            sql.Append("    id_ativo,");
            sql.Append("    nome,");
            sql.Append("    nome_fantasia,");
            sql.Append("    telefone,");
            sql.Append("    celular,");
            sql.Append("    logradouro,");
            sql.Append("    identificacao,");
            sql.Append("    responsavel,");
            sql.Append("    email,");
            sql.Append("    data_validade,");
            sql.Append("    multi_language,");
            sql.Append("    observacao)");
            sql.Append(" VALUES (");
            sql.Append("    @atualizacao,");
            sql.Append("    @idCidade,");
            sql.Append("    @idAdministrador,");
            sql.Append("    @idIdioma,");
            sql.Append("    @idAtivo,");
            sql.Append("    @nome,");
            sql.Append("    @nomeFantasia,");
            sql.Append("    @telefone,");
            sql.Append("    @celular,");
            sql.Append("    @logradouro,");
            sql.Append("    @identificacao,");
            sql.Append("    @responsavel,");
            sql.Append("    @email,");
            sql.Append("    @dataValidade,");
            sql.Append("    @multiLanguage,");
            sql.Append("    @observacao");
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
            var sql = "Delete From empresa Where id = @id";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { id = id });
                return result;
            }
        }

        public async Task<IReadOnlyList<EmpresaModel>> GetAllAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    atualizacao,");
            sql.Append("    id_cidade as idCidade,");
            sql.Append("    id_administrador as idAdmistrador,");
            sql.Append("    id_idioma as idIdioma,");
            sql.Append("    id_ativo as idAtivo,");
            sql.Append("    nome,");
            sql.Append("    nome_fantasia as nomeFantasia,");
            sql.Append("    telefone,");
            sql.Append("    celular,");
            sql.Append("    logradouro,");
            sql.Append("    identificacao,");
            sql.Append("    responsavel,");
            sql.Append("    email,");
            sql.Append("    data_validade as dataValidade,");
            sql.Append("    multi_language as multiLanguage,");
            sql.Append("    observacao");
            sql.Append(" FROM");
            sql.Append("    empresa");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<EmpresaModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        public async Task<EmpresaModel> GetByIdAsync(int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    atualizacao,");
            sql.Append("    id_cidade as idCidade,");
            sql.Append("    id_administrador as idAdmistrador,");
            sql.Append("    id_idioma as idIdioma,");
            sql.Append("    id_ativo as idAtivo,");
            sql.Append("    nome,");
            sql.Append("    nome_fantasia as nomeFantasia,");
            sql.Append("    telefone,");
            sql.Append("    celular,");
            sql.Append("    logradouro,");
            sql.Append("    identificacao,");
            sql.Append("    responsavel,");
            sql.Append("    email,");
            sql.Append("    data_validade as dataValidade,");
            sql.Append("    multi_language as multiLanguage,");
            sql.Append("    observacao");
            sql.Append(" FROM");
            sql.Append("    empresa");
            sql.Append(" WHERE");
            sql.Append("    id = @id");
         
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<EmpresaModel>(Helpers.QBuild(sql), new { id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(EmpresaModel entity)
        {
            entity.atualizacao = Helpers.DataAtualInt;
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE");
            sql.Append("    empresa");
            sql.Append(" SET");
            sql.Append("     atualizacao=@atualizacao,");
            sql.Append("     id_cidade=@idCidade,");
            sql.Append("     id_administrador=@idAdministrador,");
            sql.Append("     id_idioma=@idIdioma,");
            sql.Append("     id_ativo=@idAtivo,");
            sql.Append("     nome=@nome,");
            sql.Append("     nome_fantasia=@nomeFantasia,");
            sql.Append("     telefone=@telefone,");
            sql.Append("     celular=@celular,");
            sql.Append("     logradouro=@logradouro,");
            sql.Append("     identificacao=@identificacao,");
            sql.Append("     responsavel=@responsavel,");
            sql.Append("     email=@email,");
            sql.Append("     data_validade=@dataValidade,");
            sql.Append("     multi_language=@multiLanguage,");
            sql.Append("     observacao=@observacao");
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

        #endregion

    }
}
