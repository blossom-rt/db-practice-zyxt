using Microsoft.Data.SqlClient;
using System.Data;

namespace ZyxtApi.DAL
{
    public class MaterialCostDAL
    {
        public DataTable GetByBill(string billId)
        {
            return SqlHelper.QueryTable("SELECT * FROM 材料费表 WHERE 单据号=@billId",
                new SqlParameter("@billId", billId));
        }

        public DataTable GetByBillAndMat(string billId, string matCode)
        {
            return SqlHelper.QueryTable(
                "SELECT * FROM 材料费表 WHERE 单据号=@bill AND 物码=@mat",
                new SqlParameter("@bill", billId),
                new SqlParameter("@mat", matCode));
        }

        public int Insert(string billId, string matCode, int qty, decimal price)
        {
            return SqlHelper.ExecuteNonQuery(@"
INSERT INTO 材料费表(单据号,物码,消耗数量,单价) VALUES(@billId,@matCode,@qty,@price)",
                new SqlParameter("@billId", billId),
                new SqlParameter("@matCode", matCode),
                new SqlParameter("@qty", qty),
                new SqlParameter("@price", price));
        }

        /// <summary>事务版新增（供BLL批量新增使用）</summary>
        public int Insert(SqlConnection conn, SqlTransaction trans, string billId, string matCode, int qty, decimal price)
        {
            return SqlHelper.ExecuteNonQuery(conn, trans, @"
INSERT INTO 材料费表(单据号,物码,消耗数量,单价) VALUES(@billId,@matCode,@qty,@price)",
                new SqlParameter("@billId", billId),
                new SqlParameter("@matCode", matCode),
                new SqlParameter("@qty", qty),
                new SqlParameter("@price", price));
        }

        public int Update(string billId, string matCode, int qty, decimal price)
        {
            return SqlHelper.ExecuteNonQuery(
                "UPDATE 材料费表 SET 消耗数量=@qty, 单价=@price WHERE 单据号=@billId AND 物码=@matCode",
                new SqlParameter("@billId", billId),
                new SqlParameter("@matCode", matCode),
                new SqlParameter("@qty", qty),
                new SqlParameter("@price", price));
        }

        public int Delete(string billId, string matCode)
        {
            return SqlHelper.ExecuteNonQuery(
                "DELETE FROM 材料费表 WHERE 单据号=@billId AND 物码=@matCode",
                new SqlParameter("@billId", billId),
                new SqlParameter("@matCode", matCode));
        }

        public (long total, DataTable data) GetPage(int pageIndex, int pageSize)
        {
            long total = Convert.ToInt64(SqlHelper.QueryTable("SELECT COUNT(1) FROM 材料费表").Rows[0][0]);
            DataTable dt = SqlHelper.QueryTable(@"
SELECT * FROM 材料费表 ORDER BY 单据号,物码
OFFSET @skip ROWS FETCH NEXT @size ROWS ONLY",
                new SqlParameter("@skip", (pageIndex - 1) * pageSize),
                new SqlParameter("@size", pageSize));
            return (total, dt);
        }

        public bool CheckBillExist(string billId)
        {
            DataTable dt = SqlHelper.QueryTable("SELECT COUNT(1) FROM 作业项目表 WHERE 单据号=@bill",
                new SqlParameter("@bill", billId));
            return Convert.ToInt64(dt.Rows[0][0]) > 0;
        }

        public bool CheckMatExist(string matCode)
        {
            DataTable dt = SqlHelper.QueryTable("SELECT COUNT(1) FROM 物码表 WHERE 物码=@code",
                new SqlParameter("@code", matCode));
            return Convert.ToInt64(dt.Rows[0][0]) > 0;
        }
    }
}
