#region using

using System;
using System.Collections.Generic;
using System.Text;
using Models;
using System.Threading.Tasks;

#endregion

namespace Interfaces
{
    public interface IRedeSocialRepository : IGenericRepository<RedeSocialModel>
    {
        Task<IReadOnlyList<RedeSocialModel>> GetRedeSocialByIdTipoAsync(int idRedeSocialTipo);

        Task<IReadOnlyList<RedeSocialTipoModel>> GetRedeSocialTipoAsync();

        Task<IReadOnlyList<RedeSocialSelecionadoModel>> GetRedeSocialSelecionadoAsync(int idAnuncio);

        Task<int> DeleteRedeSocialSelecionadoAsync(int idAnuncio);

    }
}
