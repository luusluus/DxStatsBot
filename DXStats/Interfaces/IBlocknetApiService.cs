using DXStats.Domain.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DXStats.Interfaces
{
    public interface IBlocknetApiService
    {
        Task<List<OpenOrder>> DxGetOrders();

        Task<List<string>> DxGetTokens();
    }
}
