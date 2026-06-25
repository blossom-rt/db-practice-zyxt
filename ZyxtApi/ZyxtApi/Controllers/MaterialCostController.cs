using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZyxtApi.BLL;
using ZyxtApi.Models;

namespace ZyxtApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialCostController : ControllerBase
    {
        private readonly MaterialCostBLL _bll = new();

        [Authorize]
        [HttpGet("{billId}")]
        public ActionResult<ApiResult<List<MaterialCost>>> GetByBill(string billId) => Ok(_bll.GetByBill(billId));

        [Authorize]
        [HttpGet("{billId}/{matCode}")]
        public ActionResult<ApiResult<MaterialCost>> GetSingle(string billId, string matCode) => Ok(_bll.GetSingle(billId, matCode));

        [Authorize(Roles = "管理员")]
        [HttpPost]
        public ActionResult<ApiResult<string>> Create(MaterialCost model) => Ok(_bll.Create(model));

        [Authorize(Roles = "管理员")]
        [HttpPut]
        public ActionResult<ApiResult<string>> Update(MaterialCost model) => Ok(_bll.Update(model));

        [Authorize(Roles = "管理员")]
        [HttpDelete("{billId}/{matCode}")]
        public ActionResult<ApiResult<string>> Delete(string billId, string matCode) => Ok(_bll.Delete(billId, matCode));

        [Authorize]
        [HttpGet("page")]
        public ActionResult<ApiResult<object>> GetPage(int pageIndex = 1, int pageSize = 10)
            => Ok(_bll.GetPage(pageIndex, pageSize));
    }
}
