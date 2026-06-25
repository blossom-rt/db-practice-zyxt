using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace ZyxtApi.DAL
{
    public class OperationLogDAL
    {
        public (long total, DataTable data) GetPage(
            int pageIndex, int pageSize,
            string? opType, string? opTable, string? opUser,
            DateTime? dateFrom, DateTime? dateTo)
        {
            var sb = new StringBuilder("WHERE 1=1");
            var paras = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(opType))
            {
                sb.Append(" AND 操作类型 LIKE @opType");
                paras.Add(new SqlParameter("@opType", $"%{opType}%"));
            }
            if (!string.IsNullOrWhiteSpace(opTable))
            {
                sb.Append(" AND 操作表 LIKE @opTable");
                paras.Add(new SqlParameter("@opTable", $"%{opTable}%"));
            }
            if (!string.IsNullOrWhiteSpace(opUser))
            {
                sb.Append(" AND 操作人 LIKE @opUser");
                paras.Add(new SqlParameter("@opUser", $"%{opUser}%"));
            }
            if (dateFrom.HasValue)
            {
                sb.Append(" AND 操作时间 >= @dateFrom");
                paras.Add(new SqlParameter("@dateFrom", dateFrom.Value));
            }
            if (dateTo.HasValue)
            {
                sb.Append(" AND 操作时间 <= @dateTo");
                paras.Add(new SqlParameter("@dateTo", dateTo.Value));
            }

            string where = sb.ToString();

            // COUNT 查询 — 用独立参数数组
            long total = Convert.ToInt64(SqlHelper.QueryTable(
                $"SELECT COUNT(1) FROM 操作日志表 {where}", BuildParams(paras).ToArray()).Rows[0][0]);

            // 分页查询 — 重新创建参数实例，避免重复添加到不同 SqlCommand
            var dataParas = BuildParams(paras);
            dataParas.Add(new SqlParameter("@skip", (pageIndex - 1) * pageSize));
            dataParas.Add(new SqlParameter("@size", pageSize));

            DataTable dt = SqlHelper.QueryTable($@"
SELECT * FROM 操作日志表 {where} ORDER BY 操作时间 DESC
OFFSET @skip ROWS FETCH NEXT @size ROWS ONLY", dataParas.ToArray());

            return (total, dt);
        }

        /// <summary>
        /// 从原型列表重新创建 SqlParameter 实例，避免重复添加异常
        /// </summary>
        private static List<SqlParameter> BuildParams(List<SqlParameter> source)
        {
            var result = new List<SqlParameter>(source.Count);
            foreach (var p in source)
            {
                result.Add(new SqlParameter(p.ParameterName, p.Value));
            }
            return result;
        }
    }
}
