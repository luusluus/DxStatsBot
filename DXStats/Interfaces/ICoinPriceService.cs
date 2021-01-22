using System.Collections.Generic;

namespace DXStats.Interfaces
{
    public interface ICoinPriceService
    {
        Dictionary<string, Dictionary<string, decimal>> GetCoinPrice(List<string> fromCoins, List<string> toCoins);
    }
}
