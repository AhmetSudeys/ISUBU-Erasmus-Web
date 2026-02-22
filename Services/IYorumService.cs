using deneme.Models;

namespace deneme.Services
{
    public interface IYorumService
    {
        Task<Yorum> YorumEkleAsync(int erasmusId, string kullaniciAdi, string email, string yorumYazisi);
        Task<List<Yorum>> GetYorumlarByProgramIdAsync(int erasmusId);
        Task<int> GetYorumSayisiAsync(int erasmusId);
    }
}

