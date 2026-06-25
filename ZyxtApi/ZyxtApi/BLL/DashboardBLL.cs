using ZyxtApi.DAL;
using ZyxtApi.Models;

namespace ZyxtApi.BLL
{
    public class DashboardBLL
    {
        private readonly DashboardDAL _dal = new();

        public ApiResult<object> GetStats()
        {
            var stats = new
            {
                projectCount = _dal.GetProjectCount(),
                totalCost = _dal.GetTotalCost(),
                monthlyProjectCount = _dal.GetMonthlyProjectCount(),
                unitCount = _dal.GetUnitCount(),
                wellCount = _dal.GetWellCount(),
                teamCount = _dal.GetTeamCount(),
                materialCount = _dal.GetMaterialCount()
            };
            return ApiResult<object>.Success(stats);
        }
    }
}
