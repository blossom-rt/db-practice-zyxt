using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZyxtApi.BLL;
using ZyxtApi.Models;

namespace ZyxtApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConstructUnitController : ControllerBase
    {
        private readonly ConstructUnitBLL _bll = new();

        [Authorize]
        [HttpGet]
        public ActionResult<ApiResult<List<ConstructUnit>>> GetAll() => Ok(_bll.GetAll());

        [Authorize]
        [HttpGet("{name}")]
        public ActionResult<ApiResult<ConstructUnit>> GetSingle(string name) => Ok(_bll.GetByName(name));

        [Authorize(Roles = "管理员")]
        [HttpPost]
        public ActionResult<ApiResult<string>> Create(ConstructUnit model) => Ok(_bll.Create(model));

        [Authorize(Roles = "管理员")]
        [HttpPut("{oldName}")]
        public ActionResult<ApiResult<string>> Update(string oldName, ConstructUnit model) => Ok(_bll.Update(oldName, model));

        [Authorize(Roles = "管理员")]
        [HttpDelete("{name}")]
        public ActionResult<ApiResult<string>> Delete(string name) => Ok(_bll.Delete(name));

        [Authorize]
        [HttpGet("page")]
        public ActionResult<ApiResult<object>> GetPage(int pageIndex = 1, int pageSize = 10)
            => Ok(_bll.GetPage(pageIndex, pageSize));
    }
}
