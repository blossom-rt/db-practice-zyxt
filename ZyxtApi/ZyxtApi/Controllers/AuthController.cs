using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using ZyxtApi.DAL;
using ZyxtApi.Extensions;
using ZyxtApi.Models;

namespace ZyxtApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserDAL _userDal = new();

        /// <summary>
        /// 用户登录，验证SHA256密码哈希，成功返回JWT Token
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult<ApiResult<object>> Login([FromBody] LoginRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                return Ok(ApiResult<object>.Fail("用户名和密码不能为空"));

            DataTable dt = _userDal.GetByUsername(req.Username);
            if (dt.Rows.Count == 0)
                return Ok(ApiResult<object>.Fail("用户名或密码错误"));

            string dbHash = dt.Rows[0]["密码"].ToString()!;
            string inputHash = HashPassword(req.Password);

            if (!string.Equals(dbHash, inputHash, StringComparison.Ordinal))
                return Ok(ApiResult<object>.Fail("用户名或密码错误"));

            string role = dt.Rows[0]["角色"].ToString()!;
            string token = JwtHelper.GenerateToken(req.Username, role);

            return Ok(ApiResult<object>.Success(new { token, username = req.Username, role }));
        }

        private static string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
