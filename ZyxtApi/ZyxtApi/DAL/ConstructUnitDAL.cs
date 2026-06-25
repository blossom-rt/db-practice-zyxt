using Microsoft.Data.SqlClient;
using System.Data;

namespace ZyxtApi.DAL
{
    public class ConstructUnitDAL
    {
        public DataTable GetAll()
        {
            return SqlHelper.QueryTable("SELECT * FROM 施工单位表");
        }

        public DataTable GetByName(string name)
        {
            return SqlHelper.QueryTable("SELECT * FROM 施工单位表 WHERE 施工单位名称=@name",
                new SqlParameter("@name", name));
        }

        public int Insert(string name)
        {
            return SqlHelper.ExecuteNonQuery("INSERT INTO 施工单位表(施工单位名称) VALUES(@name)",
                new SqlParameter("@name", name));
        }

        public int Update(string oldName, string newName)
        {
            return SqlHelper.ExecuteNonQuery(
                "UPDATE 施工单位表 SET 施工单位名称=@newName WHERE 施工单位名称=@oldName",
                new SqlParameter("@oldName", oldName),
                new SqlParameter("@newName", newName));
        }

        public int Delete(string name)
        {
            return SqlHelper.ExecuteNonQuery("DELETE FROM 施工单位表 WHERE 施工单位名称=@name",
                new SqlParameter("@name", name));
        }

        public (long total, DataTable data) GetPage(int pageIndex, int pageSize)
        {
            long total = Convert.ToInt64(SqlHelper.QueryTable("SELECT COUNT(1) FROM 施工单位表").Rows[0][0]);
            DataTable dt = SqlHelper.QueryTable(@"
SELECT * FROM 施工单位表 ORDER BY 施工单位名称
OFFSET @skip ROWS FETCH NEXT @size ROWS ONLY",
                new SqlParameter("@skip", (pageIndex - 1) * pageSize),
                new SqlParameter("@size", pageSize));
            return (total, dt);
        }
    }
}
