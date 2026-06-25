using Microsoft.Data.SqlClient;
using System.Data;

namespace ZyxtApi.DAL
{
    public class OilWellDAL
    {
        public DataTable GetAll()
        {
            return SqlHelper.QueryTable("SELECT * FROM 油水井表");
        }

        public DataTable GetByWellId(string wellId)
        {
            return SqlHelper.QueryTable("SELECT * FROM 油水井表 WHERE 井号=@wellId",
                new SqlParameter("@wellId", wellId));
        }

        public int Insert(string wellId, string unitCode, string wellType)
        {
            return SqlHelper.ExecuteNonQuery(@"
INSERT INTO 油水井表(井号,单位代码,井别) VALUES(@wellId,@unitCode,@wellType)",
                new SqlParameter("@wellId", wellId),
                new SqlParameter("@unitCode", unitCode),
                new SqlParameter("@wellType", wellType));
        }

        public int Update(string wellId, string unitCode, string wellType)
        {
            return SqlHelper.ExecuteNonQuery(
                "UPDATE 油水井表 SET 单位代码=@unitCode, 井别=@wellType WHERE 井号=@wellId",
                new SqlParameter("@wellId", wellId),
                new SqlParameter("@unitCode", unitCode),
                new SqlParameter("@wellType", wellType));
        }

        public int Delete(string wellId)
        {
            return SqlHelper.ExecuteNonQuery("DELETE FROM 油水井表 WHERE 井号=@wellId",
                new SqlParameter("@wellId", wellId));
        }

        public bool CheckUnitExist(string unitCode)
        {
            DataTable dt = SqlHelper.QueryTable("SELECT COUNT(1) FROM 单位代码表 WHERE 单位代码=@code",
                new SqlParameter("@code", unitCode));
            return Convert.ToInt64(dt.Rows[0][0]) > 0;
        }

        public (long total, DataTable data) GetPage(int pageIndex, int pageSize)
        {
            long total = Convert.ToInt64(SqlHelper.QueryTable("SELECT COUNT(1) FROM 油水井表").Rows[0][0]);
            DataTable dt = SqlHelper.QueryTable(@"
SELECT * FROM 油水井表 ORDER BY 井号
OFFSET @skip ROWS FETCH NEXT @size ROWS ONLY",
                new SqlParameter("@skip", (pageIndex - 1) * pageSize),
                new SqlParameter("@size", pageSize));
            return (total, dt);
        }
    }
}
