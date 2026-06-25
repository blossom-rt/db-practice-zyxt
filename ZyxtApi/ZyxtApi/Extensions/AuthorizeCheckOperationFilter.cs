using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ZyxtApi.Extensions
{
    /// <summary>
    /// 为所有接口挂载 JWT Bearer 鉴权锁，使 Swagger 正确附加 Authorization 请求头
    /// </summary>
    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        public void Apply(Microsoft.OpenApi.OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Security ??= new List<Microsoft.OpenApi.OpenApiSecurityRequirement>();

            operation.Security.Add(new Microsoft.OpenApi.OpenApiSecurityRequirement
            {
                {
                    new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer"),
                    new List<string>()
                }
            });
        }
    }
}
