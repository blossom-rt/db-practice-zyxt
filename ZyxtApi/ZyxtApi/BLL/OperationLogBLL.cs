using System.Data;
using ZyxtApi.DAL;
using ZyxtApi.Models;

namespace ZyxtApi.BLL
{
    public class OperationLogBLL
    {
        private readonly OperationLogDAL _dal = new();

        private static string GetSafeString(DataRow row, string col)
            => row[col] == DBNull.Value ? "" : row[col].ToString()!.Trim();

        public ApiResult<object> GetPage(
            int pageIndex, int pageSize,
            string? opType, string? opTable, string? opUser,
            DateTime? dateFrom, DateTime? dateTo)
        {
            var (total, dt) = _dal.GetPage(pageIndex, pageSize, opType, opTable, opUser, dateFrom, dateTo);
            var list = new List<OperationLog>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new OperationLog
                {
                    Id = Convert.ToInt32(row["Id"]),
                    操作类型 = GetSafeString(row, "操作类型"),
                    操作表 = GetSafeString(row, "操作表"),
                    操作人 = GetSafeString(row, "操作人"),
                    操作时间 = Convert.ToDateTime(row["操作时间"]),
                    操作内容 = GetSafeString(row, "操作内容")
                });
            }
            return ApiResult<object>.Success(new { pageIndex, pageSize, total, totalPage = (total + pageSize - 1) / pageSize, data = list });
        }
    }
}
