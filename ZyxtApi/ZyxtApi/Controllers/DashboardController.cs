using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZyxtApi.BLL;
using ZyxtApi.Models;

namespace ZyxtApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardBLL _bll = new();

        [Authorize]
        [HttpGet("stats")]
        public ActionResult<ApiResult<object>> GetStats() => Ok(_bll.GetStats());
    }
}
