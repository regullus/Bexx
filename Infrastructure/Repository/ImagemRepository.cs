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
    class ImagemRepository : IImagemRepository
    {
        #region Base

        private readonly IConfiguration configuration;
        public ImagemRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        #endregion

        #region Crud

        public async Task<int> AddAsync(ImagemModel entity)
        {
            //entity.AddedOn = DateTime.Now;
            StringBuilder sql = new StringBuilder();
            sql.Append(" INSERT INTO imagem(");
            sql.Append("    atualizacao,");
            sql.Append("    id_empresa,");
            sql.Append("    id_imagem_tipo,");
            sql.Append("    nome,");
            sql.Append("    tamanho,");
            sql.Append("    x,");
            sql.Append("    y,");
            sql.Append("    nome_ing,");
            sql.Append("    nome_esp,");
            sql.Append("    extensao,");
            sql.Append(" 	VALUES (?,");
            sql.Append("    @atualizacao,");
            sql.Append("    @idEmpresa,");
            sql.Append("    @idImagemTipo,");
            sql.Append("    @nome,");
            sql.Append("    @tamanho,");
            sql.Append("    @x,");
            sql.Append("    @y,");
            sql.Append("    @nomeIng,");
            sql.Append("    @nomeEsp,");
            sql.Append("    @extensao");
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
            var sql = "DELETE FROM imagem WHERE id = @id";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { id = id });
                return result;
            }
        }

        public async Task<IReadOnlyList<ImagemModel>> GetAllAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    atualizacao,");
            sql.Append("    id_empresa as idEmpresa,");
            sql.Append("    id_imagem_tipo as idImagemTipo,");
            sql.Append("    nome,");
            sql.Append("    tamanho,");
            sql.Append("    x,");
            sql.Append("    y,");
            sql.Append("    nome_ing as nomeIng,");
            sql.Append("    nome_esp as nomeEsp,");
            sql.Append("    extensao,");
            sql.Append(" FROM");
            sql.Append("    imagem");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<ImagemModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        public async Task<ImagemModel> GetByIdAsync(int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    atualizacao,");
            sql.Append("    id_empresa as idEmpresa,");
            sql.Append("    id_imagem_tipo as idImagemTipo,");
            sql.Append("    nome,");
            sql.Append("    tamanho,");
            sql.Append("    x,");
            sql.Append("    y,");
            sql.Append("    nome_ing as nomeIng,");
            sql.Append("    nome_esp as nomeEsp,");
            sql.Append("    extensao,");
            sql.Append(" FROM");
            sql.Append("    imagem");
            sql.Append(" WHERE");
            sql.Append("    id = @id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<ImagemModel>(Helpers.QBuild(sql), new { id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(ImagemModel entity)
        {
            entity.atualizacao = Helpers.DataAtualInt;
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE"); 
            sql.Append("     imagem");
            sql.Append(" SET");
            sql.Append("     atualizacao=@atualizacao,");
            sql.Append("     id_empresa=@idEmpresa,");
            sql.Append("     id_imagem_tipo=@idImagemTipo,");
            sql.Append("     nome=@nome,");
            sql.Append("     tamanho=@tamanho,");
            sql.Append("     x=@x,");
            sql.Append("     y=@y,");
            sql.Append("     nome_ing=@nomeIng,");
            sql.Append("     nome_esp=@nomeEsp,");
            sql.Append("     extensao=@extensao,");
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

        public async Task<IReadOnlyList<ImagemTipoModel>> GetImagemTipoAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    atualizacao,");
            sql.Append("    nome,");
            sql.Append("    nome_ing as nomeIng,");
            sql.Append("    nome_esp as nomeEsp");
            sql.Append(" FROM");
            sql.Append("    imagem_tipo");
            sql.Append(" ORDER BY id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<ImagemTipoModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        #endregion

    }
}
