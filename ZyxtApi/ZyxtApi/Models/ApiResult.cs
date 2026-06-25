namespace ZyxtApi.Models
{
    public class ApiResult<T>
    {
        public int Code { get; set; }
        public string Msg { get; set; } = string.Empty;
        public T? Data { get; set; }

        /// <summary>
        /// 成功返回
        /// </summary>
        public static ApiResult<T> Success(T? data, string msg = "操作成功")
        {
            return new ApiResult<T> { Code = 200, Msg = msg, Data = data };
        }

        /// <summary>
        /// 业务失败返回
        /// </summary>
        public static ApiResult<T> Fail(string msg = "操作失败", int code = 400)
        {
            return new ApiResult<T> { Code = code, Msg = msg };
        }
    }
}