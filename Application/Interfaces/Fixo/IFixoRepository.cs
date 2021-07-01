#region using

using System;
using System.Collections.Generic;
using System.Text;
using Models;
using System.Threading.Tasks;

#endregion

namespace Interfaces
{
    public interface IFixoRepository
    {
        Task<IdiomaModel> GetIdiomaByIdAsync(int id);
        Task<IReadOnlyList<IdiomaModel>> GetIdiomaAllAsync();
        Task<AtivoModel> GetAtivoByIdAsync(int id);
        Task<IReadOnlyList<AtivoModel>> GetAtivoAllAsync();
    }
}
