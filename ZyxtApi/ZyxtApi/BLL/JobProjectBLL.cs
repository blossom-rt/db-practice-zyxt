using Microsoft.Data.SqlClient;
using System.Data;
using ZyxtApi.DAL;
using ZyxtApi.Models;

namespace ZyxtApi.BLL
{
    public class JobProjectBLL
    {
        private readonly JobProjectDAL _dal = new();
        private readonly MaterialCostDAL _costDal = new();

        private static string GetSafeString(DataRow row, string col)
            => row[col] == DBNull.Value ? "" : row[col].ToString()!.Trim();

        private static JobProject MapRow(DataRow row) => new()
        {
            单据号 = GetSafeString(row, "单据号"),
            预算单位 = GetSafeString(row, "预算单位"),
            井号 = GetSafeString(row, "井号"),
            施工单位 = GetSafeString(row, "施工单位"),
            预算金额 = Convert.ToDecimal(row["预算金额"]),
            材料费 = Convert.ToDecimal(row["材料费"]),
            人工费 = Convert.ToDecimal(row["人工费"]),
            设备费 = Convert.ToDecimal(row["设备费"]),
            其它费用 = Convert.ToDecimal(row["其它费用"]),
            结算金额 = Convert.ToDecimal(row["结算金额"]),
            施工内容 = GetSafeString(row, "施工内容"),
            预算日期 = Convert.ToDateTime(row["预算日期"]),
            开工日期 = Convert.ToDateTime(row["开工日期"]),
            完工日期 = Convert.ToDateTime(row["完工日期"])
        };

        #region 管理员-基表查询

        public ApiResult<List<JobProject>> GetAll()
        {
            DataTable dt = _dal.GetAll();
            var list = new List<JobProject>();
            foreach (DataRow row in dt.Rows) list.Add(MapRow(row));
            return ApiResult<List<JobProject>>.Success(list);
        }

        public ApiResult<JobProject> GetByBillId(string billId)
        {
            DataTable dt = _dal.GetByBillId(billId);
            if (dt.Rows.Count == 0) return ApiResult<JobProject>.Fail("该单据不存在", 404);
            return ApiResult<JobProject>.Success(MapRow(dt.Rows[0]));
        }

        public ApiResult<object> GetPage(int pageIndex, int pageSize)
        {
            var (total, dt) = _dal.GetPage(pageIndex, pageSize);
            var list = new List<JobProject>();
            foreach (DataRow row in dt.Rows) list.Add(MapRow(row));
            return ApiResult<object>.Success(new { pageIndex, pageSize, total, totalPage = (total + pageSize - 1) / pageSize, data = list });
        }

        public ApiResult<object> CombinedQuery(
            string? unitCode, string? wellId, string? teamName,
            DateTime? startDate, DateTime? endDate,
            decimal? minSettlement, decimal? maxSettlement,
            int pageIndex, int pageSize)
        {
            var (total, dt) = _dal.CombinedQuery(unitCode, wellId, teamName, startDate, endDate, minSettlement, maxSettlement, pageIndex, pageSize);
            var list = new List<JobProject>();
            foreach (DataRow row in dt.Rows) list.Add(MapRow(row));
            return ApiResult<object>.Success(new { pageIndex, pageSize, total, totalPage = (total + pageSize - 1) / pageSize, data = list });
        }

        #endregion

        #region 操作员-视图查询（行级数据隔离）

        public ApiResult<List<JobProject>> GetAllFromView()
        {
            DataTable dt = _dal.GetAllFromView();
            var list = new List<JobProject>();
            foreach (DataRow row in dt.Rows) list.Add(MapRow(row));
            return ApiResult<List<JobProject>>.Success(list);
        }

        public ApiResult<JobProject> GetByBillIdFromView(string billId)
        {
            DataTable dt = _dal.GetByBillIdFromView(billId);
            if (dt.Rows.Count == 0) return ApiResult<JobProject>.Fail("该单据不存在", 404);
            return ApiResult<JobProject>.Success(MapRow(dt.Rows[0]));
        }

        public ApiResult<object> GetPageFromView(int pageIndex, int pageSize)
        {
            var (total, dt) = _dal.GetPageFromView(pageIndex, pageSize);
            var list = new List<JobProject>();
            foreach (DataRow row in dt.Rows) list.Add(MapRow(row));
            return ApiResult<object>.Success(new { pageIndex, pageSize, total, totalPage = (total + pageSize - 1) / pageSize, data = list });
        }

        public ApiResult<object> CombinedQueryFromView(
            string? unitCode, string? wellId, string? teamName,
            DateTime? startDate, DateTime? endDate,
            decimal? minSettlement, decimal? maxSettlement,
            int pageIndex, int pageSize)
        {
            var (total, dt) = _dal.CombinedQueryFromView(unitCode, wellId, teamName, startDate, endDate, minSettlement, maxSettlement, pageIndex, pageSize);
            var list = new List<JobProject>();
            foreach (DataRow row in dt.Rows) list.Add(MapRow(row));
            return ApiResult<object>.Success(new { pageIndex, pageSize, total, totalPage = (total + pageSize - 1) / pageSize, data = list });
        }

        #endregion

        #region 增删改（管理员专用）

        public ApiResult<string> Create(JobProject model)
        {
            var v = ValidateCreateBasic(model);
            if (v != null) return v;
            int rows = _dal.Insert(
                model.单据号, model.预算单位, model.井号, model.预算金额, model.预算人, model.预算日期,
                model.开工日期, model.完工日期, model.施工单位, model.施工内容,
                model.材料费, model.人工费, model.设备费, model.其它费用, model.结算金额,
                model.结算人, model.结算日期, model.入账金额, model.入账人, model.入账日期);
            return rows > 0 ? ApiResult<string>.Success("", "新增成功") : ApiResult<string>.Fail("新增失败，无数据写入");
        }

        public ApiResult<string> Update(JobProject model)
        {
            if (string.IsNullOrWhiteSpace(model.单据号)) return ApiResult<string>.Fail("单据号不能为空");
            if (!_dal.CheckUnitExist(model.预算单位)) return ApiResult<string>.Fail("预算单位不存在，请核对编码");
            if (!_dal.CheckWellExist(model.井号)) return ApiResult<string>.Fail("井号不存在，请核对井号");
            if (!_dal.CheckTeamExist(model.施工单位)) return ApiResult<string>.Fail("施工单位不存在，请核对名称");
            if (model.预算金额 < 0 || model.材料费 < 0 || model.人工费 < 0 || model.设备费 < 0 || model.其它费用 < 0 || model.结算金额 < 0)
                return ApiResult<string>.Fail("所有金额不能为负数");
            int rows = _dal.Update(model.单据号, model.预算单位, model.井号, model.施工单位,
                model.预算金额, model.材料费, model.人工费, model.设备费, model.其它费用, model.结算金额,
                model.施工内容, model.预算日期, model.开工日期, model.完工日期);
            return rows > 0 ? ApiResult<string>.Success("", "修改成功") : ApiResult<string>.Fail("单据不存在或无数据变更", 404);
        }

        public ApiResult<string> Delete(string billId)
        {
            int rows = _dal.Delete(billId);
            return rows > 0 ? ApiResult<string>.Success("", "删除成功，对应材料明细已自动清理") : ApiResult<string>.Fail("单据不存在", 404);
        }

        #endregion

        #region 事务批量新增

        public ApiResult<string> CreateWithMaterials(JobProject project, List<MaterialCost> materials)
        {
            var validation = ValidateCreateBasic(project);
            if (validation != null) return validation;
            if (materials == null || materials.Count == 0)
                return ApiResult<string>.Fail("材料明细不能为空，请至少新增一条材料消耗");
            foreach (var mat in materials)
            {
                if (string.IsNullOrWhiteSpace(mat.物码)) return ApiResult<string>.Fail("物码不能为空");
                if (mat.消耗数量 <= 0) return ApiResult<string>.Fail("消耗数量必须大于0");
                if (mat.单价 < 0) return ApiResult<string>.Fail("单价不能为负数");
                if (!_costDal.CheckMatExist(mat.物码)) return ApiResult<string>.Fail($"物码 {mat.物码} 不存在，请核对");
            }

            using SqlConnection conn = SqlHelper.CreateConnection();
            conn.Open();
            SqlHelper.ApplySessionContext(conn);
            using SqlTransaction trans = conn.BeginTransaction();
            try
            {
                SqlHelper.ExecuteNonQuery(conn, trans, @"
INSERT INTO 作业项目表(单据号,预算单位,井号,预算金额,预算人,预算日期,开工日期,完工日期,施工单位,施工内容,材料费,人工费,设备费,其它费用,结算金额,结算人,结算日期,入账金额,入账人,入账日期)
VALUES(@单据号,@预算单位,@井号,@预算金额,@预算人,@预算日期,@开工日期,@完工日期,@施工单位,@施工内容,@材料费,@人工费,@设备费,@其它费用,@结算金额,@结算人,@结算日期,@入账金额,@入账人,@入账日期)",
                    new SqlParameter("@单据号", project.单据号),
                    new SqlParameter("@预算单位", project.预算单位),
                    new SqlParameter("@井号", project.井号),
                    new SqlParameter("@预算金额", project.预算金额),
                    new SqlParameter("@预算人", project.预算人),
                    new SqlParameter("@预算日期", project.预算日期),
                    new SqlParameter("@开工日期", project.开工日期),
                    new SqlParameter("@完工日期", project.完工日期),
                    new SqlParameter("@施工单位", project.施工单位),
                    new SqlParameter("@施工内容", project.施工内容),
                    new SqlParameter("@材料费", project.材料费),
                    new SqlParameter("@人工费", project.人工费),
                    new SqlParameter("@设备费", project.设备费),
                    new SqlParameter("@其它费用", project.其它费用),
                    new SqlParameter("@结算金额", project.结算金额),
                    new SqlParameter("@结算人", project.结算人),
                    new SqlParameter("@结算日期", project.结算日期),
                    new SqlParameter("@入账金额", project.入账金额),
                    new SqlParameter("@入账人", project.入账人),
                    new SqlParameter("@入账日期", project.入账日期));

                foreach (var mat in materials)
                {
                    SqlHelper.ExecuteNonQuery(conn, trans,
                        "INSERT INTO 材料费表(单据号,物码,消耗数量,单价) VALUES(@bill,@mat,@qty,@price)",
                        new SqlParameter("@bill", project.单据号),
                        new SqlParameter("@mat", mat.物码),
                        new SqlParameter("@qty", mat.消耗数量),
                        new SqlParameter("@price", mat.单价));
                }

                trans.Commit();
                return ApiResult<string>.Success("", $"新增成功，已录入 {materials.Count} 条材料明细");
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return ApiResult<string>.Fail($"事务回滚：{ex.Message}", 500);
            }
        }

        #endregion

        #region 校验

        private ApiResult<string>? ValidateCreateBasic(JobProject model)
        {
            if (string.IsNullOrWhiteSpace(model.预算人)) return ApiResult<string>.Fail("预算人不能为空");
            if (string.IsNullOrWhiteSpace(model.结算人)) return ApiResult<string>.Fail("结算人不能为空");
            if (string.IsNullOrWhiteSpace(model.入账人)) return ApiResult<string>.Fail("入账人不能为空");
            if (string.IsNullOrWhiteSpace(model.单据号)) return ApiResult<string>.Fail("单据号不能为空");
            if (string.IsNullOrWhiteSpace(model.预算单位)) return ApiResult<string>.Fail("预算单位不能为空");
            if (string.IsNullOrWhiteSpace(model.井号)) return ApiResult<string>.Fail("井号不能为空");
            if (string.IsNullOrWhiteSpace(model.施工单位)) return ApiResult<string>.Fail("施工单位不能为空");
            if (!_dal.CheckUnitExist(model.预算单位)) return ApiResult<string>.Fail("预算单位不存在，请核对编码");
            if (!_dal.CheckWellExist(model.井号)) return ApiResult<string>.Fail("井号不存在，请核对井号");
            if (!_dal.CheckTeamExist(model.施工单位)) return ApiResult<string>.Fail("施工单位不存在，请核对名称");
            if (model.预算金额 < 0 || model.材料费 < 0 || model.人工费 < 0 || model.设备费 < 0 || model.其它费用 < 0 || model.结算金额 < 0)
                return ApiResult<string>.Fail("所有金额不能为负数");
            return null;
        }

        #endregion

        #region 视图联查 & 存储过程统计

        public ApiResult<List<ViewMaterialModel>> GetViewMaterialAll()
        {
            DataTable dt = _dal.GetViewMaterialAll();
            var list = new List<ViewMaterialModel>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ViewMaterialModel
                {
                    单据号 = GetSafeString(row, "单据号"),
                    预算单位 = GetSafeString(row, "预算单位"),
                    井号 = GetSafeString(row, "井号"),
                    施工单位 = GetSafeString(row, "施工单位"),
                    施工内容 = GetSafeString(row, "施工内容"),
                    开工日期 = Convert.ToDateTime(row["开工日期"]),                    物码 = row["物码"] == DBNull.Value ? null : GetSafeString(row, "物码"),
                    名称规格 = row["名称规格"] == DBNull.Value ? null : GetSafeString(row, "名称规格"),
                    计量单位 = row["计量单位"] == DBNull.Value ? null : GetSafeString(row, "计量单位"),
                    消耗数量 = row["消耗数量"] == DBNull.Value ? null : Convert.ToInt32(row["消耗数量"]),
                    单价 = row["单价"] == DBNull.Value ? null : Convert.ToDecimal(row["单价"])
                });
            }
            return ApiResult<List<ViewMaterialModel>>.Success(list);
        }

        public ApiResult<List<ViewMaterialModel>> GetCostStatistics()
        {
            DataTable dt = _dal.ExecProcCostStat();
            var list = new List<ViewMaterialModel>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ViewMaterialModel
                {
                    单据号 = GetSafeString(row, "单据号"),
                    预算单位 = GetSafeString(row, "预算单位"),
                    井号 = GetSafeString(row, "井号"),
                    施工单位 = GetSafeString(row, "施工单位"),
                    物码 = row["物码"] == DBNull.Value ? null : GetSafeString(row, "物码"),
                    名称规格 = row["名称规格"] == DBNull.Value ? null : GetSafeString(row, "名称规格"),
                    计量单位 = row["计量单位"] == DBNull.Value ? null : GetSafeString(row, "计量单位"),
                    消耗数量 = row["消耗数量"] == DBNull.Value ? null : Convert.ToInt32(row["消耗数量"]),
                    单价 = row["单价"] == DBNull.Value ? null : Convert.ToDecimal(row["单价"]),
                    材料总成本 = row["材料总成本"] == DBNull.Value ? null : Convert.ToDecimal(row["材料总成本"])
                });
            }
            return ApiResult<List<ViewMaterialModel>>.Success(list);
        }

        #endregion
    }
}
