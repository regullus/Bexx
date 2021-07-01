#region using

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace Interfaces
{
    /// <summary>
    /// Sem Add
    /// </summary>
    public interface ICleanRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<int> UpdateAsync(T entity);
        Task<int> DeleteAsync(int id);
    }
}
