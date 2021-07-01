#region using

using System;
using System.Collections.Generic;
using System.Text;
using Models;
using System.Threading.Tasks;

#endregion

namespace Interfaces
{
    public interface IImagemRepository : IGenericRepository<ImagemModel>
    {
        Task<IReadOnlyList<ImagemTipoModel>> GetImagemTipoAsync();
    }
}
