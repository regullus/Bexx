#region using

using System;
using System.Collections.Generic;
using System.Text;
using Models;
using System.Threading.Tasks;

#endregion

namespace Interfaces
{
    public interface ITicketsRepository : IGenericRepository<TicketsModel>
    {
    }
}
