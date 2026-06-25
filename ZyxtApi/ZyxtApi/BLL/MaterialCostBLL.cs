using System.Data;
using ZyxtApi.DAL;
using ZyxtApi.Models;

namespace ZyxtApi.BLL
{
    public class MaterialCostBLL
    {
        private readonly MaterialCostDAL _dal = new();

        private static string GetSafeString(DataRow row, string col)
            => row[col] == DBNull.Value ? "" : row[col].ToString()!.Trim();

        public ApiResult<List<MaterialCost>> GetByBill(string billId)
        {
            DataTable dt = _dal.GetByBill(billId);
            var list = new List<MaterialCost>();
            foreach (DataRow row in dt.Rows) list.Add(MapRow(row));
            return ApiResult<List<MaterialCost>>.Success(list);
        }

        public ApiResult<MaterialCost> GetSingle(string billId, string matCode)
        {
            DataTable dt = _dal.GetByBillAndMat(billId, matCode);
            if (dt.Rows.Count == 0) return ApiResult<MaterialCost>.Fail("该材料明细不存在", 404);
            return ApiResult<MaterialCost>.Success(MapRow(dt.Rows[0]));
        }

        public ApiResult<string> Create(MaterialCost model)
        {
            if (string.IsNullOrWhiteSpace(model.单据号)) return ApiResult<string>.Fail("单据号不能为空");
            if (string.IsNullOrWhiteSpace(model.物码)) return ApiResult<string>.Fail("物码不能为空");
            if (model.消耗数量 <= 0) return ApiResult<string>.Fail("消耗数量必须大于0");
            if (model.单价 < 0) return ApiResult<string>.Fail("单价不能为负数");
            if (_dal.GetByBillAndMat(model.单据号, model.物码).Rows.Count > 0)
                return ApiResult<string>.Fail("该单据下已存在此物料明细，不可重复新增");
            if (!_dal.CheckBillExist(model.单据号)) return ApiResult<string>.Fail("单据号不存在，请核对");
            if (!_dal.CheckMatExist(model.物码)) return ApiResult<string>.Fail("物码不存在，请核对");
            int rows = _dal.Insert(model.单据号, model.物码, model.消耗数量, model.单价);
            return rows > 0 ? ApiResult<string>.Success("", "新增成功") : ApiResult<string>.Fail("新增失败");
        }

        public ApiResult<string> Update(MaterialCost model)
        {
            if (string.IsNullOrWhiteSpace(model.单据号)) return ApiResult<string>.Fail("单据号不能为空");
            if (string.IsNullOrWhiteSpace(model.物码)) return ApiResult<string>.Fail("物码不能为空");
            if (model.消耗数量 <= 0) return ApiResult<string>.Fail("消耗数量必须大于0");
            if (model.单价 < 0) return ApiResult<string>.Fail("单价不能为负数");
            if (_dal.GetByBillAndMat(model.单据号, model.物码).Rows.Count == 0)
                return ApiResult<string>.Fail("该材料明细不存在", 404);
            if (!_dal.CheckBillExist(model.单据号)) return ApiResult<string>.Fail("单据号不存在，请核对");
            if (!_dal.CheckMatExist(model.物码)) return ApiResult<string>.Fail("物码不存在，请核对");
            int rows = _dal.Update(model.单据号, model.物码, model.消耗数量, model.单价);
            return rows > 0 ? ApiResult<string>.Success("", "修改成功") : ApiResult<string>.Fail("修改失败");
        }

        public ApiResult<string> Delete(string billId, string matCode)
        {
            if (_dal.GetByBillAndMat(billId, matCode).Rows.Count == 0)
                return ApiResult<string>.Fail("该材料明细不存在", 404);
            _dal.Delete(billId, matCode);
            return ApiResult<string>.Success("", "删除成功");
        }

        public ApiResult<object> GetPage(int pageIndex, int pageSize)
        {
            var (total, dt) = _dal.GetPage(pageIndex, pageSize);
            var list = new List<MaterialCost>();
            foreach (DataRow row in dt.Rows) list.Add(MapRow(row));
            return ApiResult<object>.Success(new { pageIndex, pageSize, total, totalPage = (total + pageSize - 1) / pageSize, data = list });
        }

        private static MaterialCost MapRow(DataRow row) => new()
        {
            单据号 = GetSafeString(row, "单据号"),
            物码 = GetSafeString(row, "物码"),
            消耗数量 = Convert.ToInt32(row["消耗数量"]),
            单价 = Convert.ToDecimal(row["单价"])
        };
    }
}
