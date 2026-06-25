using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZyxtApi.BLL;
using ZyxtApi.Models;

namespace ZyxtApi.Controllers
{
    /// <summary>
    /// 作业项目控制器 — 管理员查基表，操作员强制走视图（行级数据隔离）
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class JobProjectController : ControllerBase
    {
        private readonly JobProjectBLL _bll = new();

        private bool IsAdmin => User.IsInRole("管理员");

        #region 查询（管理员→基表，操作员→视图）

        [Authorize]
        [HttpGet]
        public ActionResult<ApiResult<List<JobProject>>> GetAll()
            => Ok(IsAdmin ? _bll.GetAll() : _bll.GetAllFromView());

        [Authorize]
        [HttpGet("{billId}")]
        public ActionResult<ApiResult<JobProject>> GetSingle(string billId)
            => Ok(IsAdmin ? _bll.GetByBillId(billId) : _bll.GetByBillIdFromView(billId));

        [Authorize]
        [HttpGet("page")]
        public ActionResult<ApiResult<object>> GetPage(int pageIndex = 1, int pageSize = 10)
            => Ok(IsAdmin ? _bll.GetPage(pageIndex, pageSize) : _bll.GetPageFromView(pageIndex, pageSize));

        [Authorize]
        [HttpGet("query")]
        public ActionResult<ApiResult<object>> CombinedQuery(
            string? unitCode, string? wellId, string? teamName,
            DateTime? startDate, DateTime? endDate,
            decimal? minSettlement, decimal? maxSettlement,
            int pageIndex = 1, int pageSize = 10)
            => Ok(IsAdmin
                ? _bll.CombinedQuery(unitCode, wellId, teamName, startDate, endDate, minSettlement, maxSettlement, pageIndex, pageSize)
                : _bll.CombinedQueryFromView(unitCode, wellId, teamName, startDate, endDate, minSettlement, maxSettlement, pageIndex, pageSize));

        #endregion

        #region 增删改（仅管理员）

        [Authorize(Roles = "管理员")]
        [HttpPost]
        public ActionResult<ApiResult<string>> Create(JobProject model) => Ok(_bll.Create(model));

        [Authorize(Roles = "管理员")]
        [HttpPost("with-materials")]
        public ActionResult<ApiResult<string>> CreateWithMaterials([FromBody] CreateProjectWithMaterialsRequest req)
            => Ok(_bll.CreateWithMaterials(req.Project, req.Materials));

        [Authorize(Roles = "管理员")]
        [HttpPut]
        public ActionResult<ApiResult<string>> Update(JobProject model) => Ok(_bll.Update(model));

        [Authorize(Roles = "管理员")]
        [HttpDelete("{billId}")]
        public ActionResult<ApiResult<string>> Delete(string billId) => Ok(_bll.Delete(billId));

        #endregion

        #region 联查 & 统计（所有认证用户可访问）

        [Authorize]
        [HttpGet("view/material")]
        public ActionResult<ApiResult<List<ViewMaterialModel>>> ViewMaterialAll() => Ok(_bll.GetViewMaterialAll());

        [Authorize]
        [HttpGet("proc/cost")]
        public ActionResult<ApiResult<List<ViewMaterialModel>>> ProcCostStat() => Ok(_bll.GetCostStatistics());

        #endregion
    }

    public class CreateProjectWithMaterialsRequest
    {
        public JobProject Project { get; set; } = new();
        public List<MaterialCost> Materials { get; set; } = new();
    }
}
