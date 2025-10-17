using Sub_Pal_API.Models.DTOs;

namespace Sub_Pal_API.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetDashboardSummaryAsync(int userId);
    }
}
