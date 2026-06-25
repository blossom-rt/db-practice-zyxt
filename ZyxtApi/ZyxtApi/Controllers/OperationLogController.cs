using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZyxtApi.BLL;
using ZyxtApi.Models;

namespace ZyxtApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "管理员")]
    public class OperationLogController : ControllerBase
    {
        private readonly OperationLogBLL _bll = new();

        /// <summary>分页查询操作日志（仅管理员可查）</summary>
        [HttpGet("page")]
        public ActionResult<ApiResult<object>> GetPage(
            int pageIndex = 1, int pageSize = 10,
            string? opType = null, string? opTable = null, string? opUser = null,
            DateTime? dateFrom = null, DateTime? dateTo = null)
            => Ok(_bll.GetPage(pageIndex, pageSize, opType, opTable, opUser, dateFrom, dateTo));
    }
}
