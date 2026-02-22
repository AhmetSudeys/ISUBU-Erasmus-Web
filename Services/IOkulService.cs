using deneme.Models;

namespace deneme.Services
{
    public interface IOkulService
    {
        Task<List<Okul>> GetAllOkullarAsync();
        Task<Okul?> GetOkulByIdAsync(int id);
        Task<Okul> OkulEkleAsync(string okulAd, int ulkeId, string internetSitesi, double? latitude, double? longitude);
        Task<bool> OkulExistsAsync(int id);
    }
}

