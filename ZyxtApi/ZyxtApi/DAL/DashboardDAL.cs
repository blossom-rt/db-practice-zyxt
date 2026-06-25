using Microsoft.Data.SqlClient;
using System.Data;

namespace ZyxtApi.DAL
{
    public class DashboardDAL
    {
        public long GetProjectCount()
        {
            var dt = SqlHelper.QueryTable("SELECT COUNT(1) FROM 作业项目表");
            return Convert.ToInt64(dt.Rows[0][0]);
        }

        public decimal GetTotalCost()
        {
            var dt = SqlHelper.QueryTable("SELECT ISNULL(SUM(结算金额),0) FROM 作业项目表");
            return Convert.ToDecimal(dt.Rows[0][0]);
        }

        public long GetMonthlyProjectCount()
        {
            var dt = SqlHelper.QueryTable(
                "SELECT COUNT(1) FROM 作业项目表 WHERE 预算日期 >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)");
            return Convert.ToInt64(dt.Rows[0][0]);
        }

        public long GetUnitCount()
        {
            var dt = SqlHelper.QueryTable("SELECT COUNT(1) FROM 单位代码表");
            return Convert.ToInt64(dt.Rows[0][0]);
        }

        public long GetWellCount()
        {
            var dt = SqlHelper.QueryTable("SELECT COUNT(1) FROM 油水井表");
            return Convert.ToInt64(dt.Rows[0][0]);
        }

        public long GetTeamCount()
        {
            var dt = SqlHelper.QueryTable("SELECT COUNT(1) FROM 施工单位表");
            return Convert.ToInt64(dt.Rows[0][0]);
        }

        public long GetMaterialCount()
        {
            var dt = SqlHelper.QueryTable("SELECT COUNT(1) FROM 物码表");
            return Convert.ToInt64(dt.Rows[0][0]);
        }
    }
}
