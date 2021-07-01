#region using

using System;
using System.Collections.Generic;
using System.Text;
using Models;
using System.Threading.Tasks;

#endregion

namespace Interfaces
{
    public interface IBaseRepository
    {
        Task<IReadOnlyList<InstituicaoFinanceiraModel>> GetInstituicaoFinanceiraAsync(int IdAtivo);
        Task<IReadOnlyList<PaisModel>> GetPaisAsync();
    }
}
