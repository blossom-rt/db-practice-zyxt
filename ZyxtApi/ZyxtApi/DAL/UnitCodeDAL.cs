using Microsoft.Data.SqlClient;
using System.Data;

namespace ZyxtApi.DAL
{
    public class UnitCodeDAL
    {
        public DataTable GetAll()
        {
            return SqlHelper.QueryTable("SELECT * FROM 单位代码表");
        }

        public DataTable GetByCode(string code)
        {
            return SqlHelper.QueryTable("SELECT * FROM 单位代码表 WHERE 单位代码=@code",
                new SqlParameter("@code", code));
        }

        public int Insert(string code, string name)
        {
            return SqlHelper.ExecuteNonQuery(@"
INSERT INTO 单位代码表(单位代码,单位名称) VALUES(@code,@name)",
                new SqlParameter("@code", code),
                new SqlParameter("@name", name));
        }

        public int Update(string code, string name)
        {
            return SqlHelper.ExecuteNonQuery(
                "UPDATE 单位代码表 SET 单位名称=@name WHERE 单位代码=@code",
                new SqlParameter("@code", code),
                new SqlParameter("@name", name));
        }

        public int Delete(string code)
        {
            return SqlHelper.ExecuteNonQuery("DELETE FROM 单位代码表 WHERE 单位代码=@code",
                new SqlParameter("@code", code));
        }

        public (long total, DataTable data) GetPage(int pageIndex, int pageSize)
        {
            long total = Convert.ToInt64(SqlHelper.QueryTable("SELECT COUNT(1) FROM 单位代码表").Rows[0][0]);
            DataTable dt = SqlHelper.QueryTable(@"
SELECT * FROM 单位代码表 ORDER BY 单位代码
OFFSET @skip ROWS FETCH NEXT @size ROWS ONLY",
                new SqlParameter("@skip", (pageIndex - 1) * pageSize),
                new SqlParameter("@size", pageSize));
            return (total, dt);
        }
    }
}
