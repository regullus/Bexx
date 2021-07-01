#region using
using System;
using System.Collections.Generic;
using System.Text;
using Models;
using System.Threading.Tasks;
#endregion

namespace Interfaces
{
    public interface IUsuarioRepository : ICleanRepository<UsuarioModel>
    {
        Task<int> EmailConfimAsync(int id);
        Task<int> DeleteLogicAsync(int id);
        Task<IReadOnlyList<UsuarioSexoModel>> GetUsuarioSexoAsync();
        Task<IReadOnlyList<UsuarioSituacaoModel>> GetUsuarioSituacaoAsync();
        Task<int> UpdateDoisFatoresAsync(DoisFatoresModel entity);
        Task<IReadOnlyList<UsuarioListaCrudModel>> GetListaUsuarioCrudAsync();
        Task<IReadOnlyList<UsuarioListaCrudModel>> GetListaUsuarioCrudByPerfilAsync(string perfil);
        Task<UsuarioPaisModel> GetUsuarioPaisByIdUsuarioAsync(int id);

        #region UsuarioDados

        Task<UsuarioDadosModel> GetUsuarioDadosAsync(int id);
        Task<int> AddUsuarioDadosAsync(UsuarioDadosModel model);
        Task<int> UpdateUsuarioDadosAsync(UsuarioDadosModel model);

        #endregion

        #region UsuarioPerfil
        public Task<int> GetUsuarioPerfilAsync(int id);

        #endregion

    }
}
