using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System;
using System.Text.Json;

namespace ZyxtApi.Extensions
{
    /// <summary>
    /// 全局统一异常中间件，捕获所有接口异常，标准化返回格式
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 500;
                string msg = ex.Message;

                // 递归遍历所有内部异常，拿到SqlException
                Exception tempEx = ex;
                while (tempEx.InnerException != null)
                {
                    tempEx = tempEx.InnerException;
                    if (tempEx is SqlException sqlEx)
                    {
                        switch (sqlEx.Number)
                        {
                            case 2627:
                                msg = "操作失败：单据号重复，不能重复创建";
                                break;
                            case 515:
                                msg = "操作失败：必填字段不能为空，请补充完整数据";
                                break;
                            case 547:
                                msg = "操作失败：关联的单位/井/施工队不存在，请检查输入";
                                break;
                            case 8134:
                                msg = "操作失败：金额不能为负数";
                                break;
                            default:
                                msg = $"数据库异常：{sqlEx.Message}";
                                break;
                        }
                        break;
                    }
                }

                var result = new { code = 500, msg, data = (object?)null };
                string json = JsonSerializer.Serialize(result);
                await context.Response.WriteAsync(json);
            }
        }
    }

    public static class GlobalExceptionExt
    {
        public static IApplicationBuilder UseGlobalException(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}