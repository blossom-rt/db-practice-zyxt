using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace ZyxtApi.DAL
{
    public class JobProjectDAL
    {
        #region 基表查询（管理员使用）

        public DataTable GetAll()
            => SqlHelper.QueryTable("SELECT * FROM 作业项目表");

        public DataTable GetByBillId(string billId)
            => SqlHelper.QueryTable("SELECT * FROM 作业项目表 WHERE 单据号=@billId",
                new SqlParameter("@billId", billId));

        public (long total, DataTable data) GetPage(int pageIndex, int pageSize)
        {
            long total = Convert.ToInt64(SqlHelper.QueryTable("SELECT COUNT(1) FROM 作业项目表").Rows[0][0]);
            DataTable dt = SqlHelper.QueryTable(@"
SELECT * FROM 作业项目表 ORDER BY 单据号
OFFSET @skip ROWS FETCH NEXT @size ROWS ONLY",
                new SqlParameter("@skip", (pageIndex - 1) * pageSize),
                new SqlParameter("@size", pageSize));
            return (total, dt);
        }

        #endregion

        #region 视图查询（操作员使用，行级数据隔离）

        public DataTable GetAllFromView()
            => SqlHelper.QueryTable("SELECT * FROM v_作业项目视图");

        public DataTable GetByBillIdFromView(string billId)
            => SqlHelper.QueryTable("SELECT * FROM v_作业项目视图 WHERE 单据号=@billId",
                new SqlParameter("@billId", billId));

        public (long total, DataTable data) GetPageFromView(int pageIndex, int pageSize)
        {
            long total = Convert.ToInt64(SqlHelper.QueryTable("SELECT COUNT(1) FROM v_作业项目视图").Rows[0][0]);
            DataTable dt = SqlHelper.QueryTable(@"
SELECT * FROM v_作业项目视图 ORDER BY 单据号
OFFSET @skip ROWS FETCH NEXT @size ROWS ONLY",
                new SqlParameter("@skip", (pageIndex - 1) * pageSize),
                new SqlParameter("@size", pageSize));
            return (total, dt);
        }

        /// <summary>操作员多条件组合查询（基于视图）</summary>
        public (long total, DataTable data) CombinedQueryFromView(
            string? unitCode, string? wellId, string? teamName,
            DateTime? startDate, DateTime? endDate,
            decimal? minSettlement, decimal? maxSettlement,
            int pageIndex, int pageSize)
        {
            return CombinedQueryInternal("v_作业项目视图", unitCode, wellId, teamName, startDate, endDate, minSettlement, maxSettlement, pageIndex, pageSize);
        }

        #endregion

        #region 增删改（仅管理员）

        public int Insert(
            string 单据号, string 预算单位, string 井号, decimal 预算金额, string 预算人, DateTime 预算日期,
            DateTime 开工日期, DateTime 完工日期, string 施工单位, string 施工内容,
            decimal 材料费, decimal 人工费, decimal 设备费, decimal 其它费用, decimal 结算金额,
            string 结算人, DateTime 结算日期, decimal 入账金额, string 入账人, DateTime 入账日期)
        {
            return SqlHelper.ExecuteNonQuery(@"
INSERT INTO 作业项目表(单据号,预算单位,井号,预算金额,预算人,预算日期,开工日期,完工日期,施工单位,施工内容,材料费,人工费,设备费,其它费用,结算金额,结算人,结算日期,入账金额,入账人,入账日期)
VALUES(@单据号,@预算单位,@井号,@预算金额,@预算人,@预算日期,@开工日期,@完工日期,@施工单位,@施工内容,@材料费,@人工费,@设备费,@其它费用,@结算金额,@结算人,@结算日期,@入账金额,@入账人,@入账日期)",
                new SqlParameter("@单据号", 单据号),
                new SqlParameter("@预算单位", 预算单位),
                new SqlParameter("@井号", 井号),
                new SqlParameter("@预算金额", 预算金额),
                new SqlParameter("@预算人", 预算人),
                new SqlParameter("@预算日期", 预算日期),
                new SqlParameter("@开工日期", 开工日期),
                new SqlParameter("@完工日期", 完工日期),
                new SqlParameter("@施工单位", 施工单位),
                new SqlParameter("@施工内容", 施工内容),
                new SqlParameter("@材料费", 材料费),
                new SqlParameter("@人工费", 人工费),
                new SqlParameter("@设备费", 设备费),
                new SqlParameter("@其它费用", 其它费用),
                new SqlParameter("@结算金额", 结算金额),
                new SqlParameter("@结算人", 结算人),
                new SqlParameter("@结算日期", 结算日期),
                new SqlParameter("@入账金额", 入账金额),
                new SqlParameter("@入账人", 入账人),
                new SqlParameter("@入账日期", 入账日期));
        }

        public int Insert(SqlConnection conn, SqlTransaction trans,
            string 单据号, string 预算单位, string 井号, decimal 预算金额, string 预算人, DateTime 预算日期,
            DateTime 开工日期, DateTime 完工日期, string 施工单位, string 施工内容,
            decimal 材料费, decimal 人工费, decimal 设备费, decimal 其它费用, decimal 结算金额,
            string 结算人, DateTime 结算日期, decimal 入账金额, string 入账人, DateTime 入账日期)
        {
            return SqlHelper.ExecuteNonQuery(conn, trans, @"
INSERT INTO 作业项目表(单据号,预算单位,井号,预算金额,预算人,预算日期,开工日期,完工日期,施工单位,施工内容,材料费,人工费,设备费,其它费用,结算金额,结算人,结算日期,入账金额,入账人,入账日期)
VALUES(@单据号,@预算单位,@井号,@预算金额,@预算人,@预算日期,@开工日期,@完工日期,@施工单位,@施工内容,@材料费,@人工费,@设备费,@其它费用,@结算金额,@结算人,@结算日期,@入账金额,@入账人,@入账日期)",
                new SqlParameter("@单据号", 单据号),
                new SqlParameter("@预算单位", 预算单位),
                new SqlParameter("@井号", 井号),
                new SqlParameter("@预算金额", 预算金额),
                new SqlParameter("@预算人", 预算人),
                new SqlParameter("@预算日期", 预算日期),
                new SqlParameter("@开工日期", 开工日期),
                new SqlParameter("@完工日期", 完工日期),
                new SqlParameter("@施工单位", 施工单位),
                new SqlParameter("@施工内容", 施工内容),
                new SqlParameter("@材料费", 材料费),
                new SqlParameter("@人工费", 人工费),
                new SqlParameter("@设备费", 设备费),
                new SqlParameter("@其它费用", 其它费用),
                new SqlParameter("@结算金额", 结算金额),
                new SqlParameter("@结算人", 结算人),
                new SqlParameter("@结算日期", 结算日期),
                new SqlParameter("@入账金额", 入账金额),
                new SqlParameter("@入账人", 入账人),
                new SqlParameter("@入账日期", 入账日期));
        }

        public int Update(
            string 单据号, string 预算单位, string 井号, string 施工单位,
            decimal 预算金额, decimal 材料费, decimal 人工费, decimal 设备费, decimal 其它费用, decimal 结算金额,
            string 施工内容, DateTime 预算日期, DateTime 开工日期, DateTime 完工日期)
        {
            return SqlHelper.ExecuteNonQuery(@"
UPDATE 作业项目表 SET 预算单位=@预算单位,井号=@井号,施工单位=@施工单位,
预算金额=@预算金额,材料费=@材料费,人工费=@人工费,设备费=@设备费,其它费用=@其它费用,结算金额=@结算金额,
施工内容=@施工内容,预算日期=@预算日期,开工日期=@开工日期,完工日期=@完工日期
WHERE 单据号=@单据号",
                new SqlParameter("@单据号", 单据号),
                new SqlParameter("@预算单位", 预算单位),
                new SqlParameter("@井号", 井号),
                new SqlParameter("@施工单位", 施工单位),
                new SqlParameter("@预算金额", 预算金额),
                new SqlParameter("@材料费", 材料费),
                new SqlParameter("@人工费", 人工费),
                new SqlParameter("@设备费", 设备费),
                new SqlParameter("@其它费用", 其它费用),
                new SqlParameter("@结算金额", 结算金额),
                new SqlParameter("@施工内容", 施工内容),
                new SqlParameter("@预算日期", 预算日期),
                new SqlParameter("@开工日期", 开工日期),
                new SqlParameter("@完工日期", 完工日期));
        }

        public int Delete(string billId)
            => SqlHelper.ExecuteNonQuery("DELETE FROM 作业项目表 WHERE 单据号=@billId",
                new SqlParameter("@billId", billId));

        #endregion

        #region 联查 & 存储过程（所有角色可访问）

        public DataTable GetViewMaterialAll()
        {
            return SqlHelper.QueryTable(@"
SELECT j.单据号, j.预算单位, j.井号, j.施工单位, j.施工内容, j.开工日期,
       mc.物码, mc.消耗数量, mc.单价,
       m.名称规格, m.计量单位
FROM 作业项目表 j
LEFT JOIN 材料费表 mc ON j.单据号 = mc.单据号
LEFT JOIN 物码表 m ON mc.物码 = m.物码");
        }

        public DataTable ExecProcCostStat()
            => SqlHelper.ExecProc("Proc_CalcProjectCost");

        #endregion

        #region 多条件组合查询（管理员基表版）

        public (long total, DataTable data) CombinedQuery(
            string? unitCode, string? wellId, string? teamName,
            DateTime? startDate, DateTime? endDate,
            decimal? minSettlement, decimal? maxSettlement,
            int pageIndex, int pageSize)
        {
            return CombinedQueryInternal("作业项目表", unitCode, wellId, teamName, startDate, endDate, minSettlement, maxSettlement, pageIndex, pageSize);
        }

        #endregion

        #region 外键校验

        public bool CheckUnitExist(string unitCode) =>
            Convert.ToInt64(SqlHelper.QueryTable("SELECT COUNT(1) FROM 单位代码表 WHERE 单位代码=@code",
                new SqlParameter("@code", unitCode)).Rows[0][0]) > 0;

        public bool CheckWellExist(string wellId) =>
            Convert.ToInt64(SqlHelper.QueryTable("SELECT COUNT(1) FROM 油水井表 WHERE 井号=@well",
                new SqlParameter("@well", wellId)).Rows[0][0]) > 0;

        public bool CheckTeamExist(string teamName) =>
            Convert.ToInt64(SqlHelper.QueryTable("SELECT COUNT(1) FROM 施工单位表 WHERE 施工单位名称=@team",
                new SqlParameter("@team", teamName)).Rows[0][0]) > 0;

        #endregion

        #region 通用动态SQL拼接

        private static (long total, DataTable data) CombinedQueryInternal(
            string tableName,
            string? unitCode, string? wellId, string? teamName,
            DateTime? startDate, DateTime? endDate,
            decimal? minSettlement, decimal? maxSettlement,
            int pageIndex, int pageSize)
        {
            var where = new StringBuilder("WHERE 1=1 ");
            var paras = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(unitCode))
            { where.Append("AND 预算单位=@unitCode "); paras.Add(new SqlParameter("@unitCode", unitCode)); }
            if (!string.IsNullOrWhiteSpace(wellId))
            { where.Append("AND 井号 LIKE @wellId "); paras.Add(new SqlParameter("@wellId", $"%{wellId}%")); }
            if (!string.IsNullOrWhiteSpace(teamName))
            { where.Append("AND 施工单位 LIKE @teamName "); paras.Add(new SqlParameter("@teamName", $"%{teamName}%")); }
            if (startDate.HasValue)
            { where.Append("AND 预算日期 >= @startDate "); paras.Add(new SqlParameter("@startDate", startDate.Value)); }
            if (endDate.HasValue)
            { where.Append("AND 预算日期 <= @endDate "); paras.Add(new SqlParameter("@endDate", endDate.Value)); }
            if (minSettlement.HasValue)
            { where.Append("AND 结算金额 >= @minSettlement "); paras.Add(new SqlParameter("@minSettlement", minSettlement.Value)); }
            if (maxSettlement.HasValue)
            { where.Append("AND 结算金额 <= @maxSettlement "); paras.Add(new SqlParameter("@maxSettlement", maxSettlement.Value)); }

            string countSql = $"SELECT COUNT(1) FROM {tableName} {where}";
            long total = Convert.ToInt64(SqlHelper.QueryTable(countSql, paras.ToArray()).Rows[0][0]);

            // 创建新的参数副本用于分页查询，避免重用SqlParameter报错
            var pageParas = paras.Select(p => new SqlParameter(p.ParameterName, p.Value)).ToList();
            pageParas.Add(new SqlParameter("@skip", (pageIndex - 1) * pageSize));
            pageParas.Add(new SqlParameter("@size", pageSize));

            string pageSql = $@"
SELECT * FROM {tableName} {where} ORDER BY 单据号
OFFSET @skip ROWS FETCH NEXT @size ROWS ONLY";
            DataTable dt = SqlHelper.QueryTable(pageSql, pageParas.ToArray());
            return (total, dt);
        }

        #endregion
    }
}
