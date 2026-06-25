using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZyxtApi.BLL;
using ZyxtApi.Extensions;
using ZyxtApi.Models;

namespace ZyxtApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitCodeController : ControllerBase
    {
        private readonly UnitCodeBLL _bll = new();

        [Authorize]
        [HttpGet]
        public ActionResult<ApiResult<List<UnitCode>>> GetAll() => Ok(_bll.GetAll());

        [Authorize]
        [HttpGet("{code}")]
        public ActionResult<ApiResult<UnitCode>> GetSingle(string code) => Ok(_bll.GetByCode(code));

        [Authorize(Roles = "管理员")]
        [HttpPost]
        public ActionResult<ApiResult<string>> Create(UnitCode model) => Ok(_bll.Create(model));

        [Authorize(Roles = "管理员")]
        [HttpPut]
        public ActionResult<ApiResult<string>> Update(UnitCode model) => Ok(_bll.Update(model));

        [Authorize(Roles = "管理员")]
        [HttpDelete("{code}")]
        public ActionResult<ApiResult<string>> Delete(string code) => Ok(_bll.Delete(code));

        [Authorize]
        [HttpGet("page")]
        public ActionResult<ApiResult<object>> GetPage(int pageIndex = 1, int pageSize = 10)
            => Ok(_bll.GetPage(pageIndex, pageSize));
    }
}
