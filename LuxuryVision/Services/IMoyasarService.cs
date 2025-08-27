using LuxuryVision.Models;
using System.Threading.Tasks;

namespace LuxuryVision.Services
{
    public interface IMoyasarService
    {
        Task<string> CreatePaymentAsync(Order order);
    }
}