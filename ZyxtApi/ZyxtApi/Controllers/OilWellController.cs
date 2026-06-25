using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZyxtApi.BLL;
using ZyxtApi.Models;

namespace ZyxtApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OilWellController : ControllerBase
    {
        private readonly OilWellBLL _bll = new();

        [Authorize]
        [HttpGet]
        public ActionResult<ApiResult<List<OilWell>>> GetAll() => Ok(_bll.GetAll());

        [Authorize]
        [HttpGet("{wellId}")]
        public ActionResult<ApiResult<OilWell>> GetSingle(string wellId) => Ok(_bll.GetByWellId(wellId));

        [Authorize(Roles = "管理员")]
        [HttpPost]
        public ActionResult<ApiResult<string>> Create(OilWell model) => Ok(_bll.Create(model));

        [Authorize(Roles = "管理员")]
        [HttpPut]
        public ActionResult<ApiResult<string>> Update(OilWell model) => Ok(_bll.Update(model));

        [Authorize(Roles = "管理员")]
        [HttpDelete("{wellId}")]
        public ActionResult<ApiResult<string>> Delete(string wellId) => Ok(_bll.Delete(wellId));

        [Authorize]
        [HttpGet("page")]
        public ActionResult<ApiResult<object>> GetPage(int pageIndex = 1, int pageSize = 10)
            => Ok(_bll.GetPage(pageIndex, pageSize));
    }
}
