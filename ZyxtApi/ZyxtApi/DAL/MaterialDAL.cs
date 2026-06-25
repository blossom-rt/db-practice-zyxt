using Microsoft.Data.SqlClient;
using System.Data;

namespace ZyxtApi.DAL
{
    public class MaterialDAL
    {
        public DataTable GetAll()
        {
            return SqlHelper.QueryTable("SELECT * FROM 物码表");
        }

        public DataTable GetByCode(string code)
        {
            return SqlHelper.QueryTable("SELECT * FROM 物码表 WHERE 物码=@code",
                new SqlParameter("@code", code));
        }

        public int Insert(string code, string spec, string unit)
        {
            return SqlHelper.ExecuteNonQuery(
                "INSERT INTO 物码表(物码,名称规格,计量单位) VALUES(@code,@spec,@unit)",
                new SqlParameter("@code", code),
                new SqlParameter("@spec", spec),
                new SqlParameter("@unit", unit));
        }

        public int Update(string code, string spec, string unit)
        {
            return SqlHelper.ExecuteNonQuery(
                "UPDATE 物码表 SET 名称规格=@spec, 计量单位=@unit WHERE 物码=@code",
                new SqlParameter("@code", code),
                new SqlParameter("@spec", spec),
                new SqlParameter("@unit", unit));
        }

        public int Delete(string code)
        {
            return SqlHelper.ExecuteNonQuery("DELETE FROM 物码表 WHERE 物码=@code",
                new SqlParameter("@code", code));
        }

        public (long total, DataTable data) GetPage(int pageIndex, int pageSize)
        {
            long total = Convert.ToInt64(SqlHelper.QueryTable("SELECT COUNT(1) FROM 物码表").Rows[0][0]);
            DataTable dt = SqlHelper.QueryTable(@"
SELECT * FROM 物码表 ORDER BY 物码
OFFSET @skip ROWS FETCH NEXT @size ROWS ONLY",
                new SqlParameter("@skip", (pageIndex - 1) * pageSize),
                new SqlParameter("@size", pageSize));
            return (total, dt);
        }
    }
}
