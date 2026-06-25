using System.Data;
using ZyxtApi.DAL;
using ZyxtApi.Models;

namespace ZyxtApi.BLL
{
    public class OilWellBLL
    {
        private readonly OilWellDAL _dal = new();

        private static string GetSafeString(DataRow row, string col)
            => row[col] == DBNull.Value ? "" : row[col].ToString()!.Trim();

        public ApiResult<List<OilWell>> GetAll()
        {
            DataTable dt = _dal.GetAll();
            var list = new List<OilWell>();
            foreach (DataRow row in dt.Rows)
                list.Add(MapRow(row));
            return ApiResult<List<OilWell>>.Success(list);
        }

        public ApiResult<OilWell> GetByWellId(string wellId)
        {
            DataTable dt = _dal.GetByWellId(wellId);
            if (dt.Rows.Count == 0) return ApiResult<OilWell>.Fail("该井号不存在", 404);
            return ApiResult<OilWell>.Success(MapRow(dt.Rows[0]));
        }

        public ApiResult<string> Create(OilWell model)
        {
            if (string.IsNullOrWhiteSpace(model.井号)) return ApiResult<string>.Fail("井号不能为空");
            if (string.IsNullOrWhiteSpace(model.井别)) return ApiResult<string>.Fail("井别不能为空");
            if (string.IsNullOrWhiteSpace(model.所属单位)) return ApiResult<string>.Fail("所属单位代码不能为空");
            if (_dal.GetByWellId(model.井号).Rows.Count > 0) return ApiResult<string>.Fail("该井号已存在");
            if (!_dal.CheckUnitExist(model.所属单位)) return ApiResult<string>.Fail("单位代码不存在，请核对");
            int rows = _dal.Insert(model.井号, model.所属单位, model.井别);
            return rows > 0 ? ApiResult<string>.Success("", "新增成功") : ApiResult<string>.Fail("新增失败");
        }

        public ApiResult<string> Update(OilWell model)
        {
            if (string.IsNullOrWhiteSpace(model.井号)) return ApiResult<string>.Fail("井号不能为空");
            if (string.IsNullOrWhiteSpace(model.井别)) return ApiResult<string>.Fail("井别不能为空");
            if (string.IsNullOrWhiteSpace(model.所属单位)) return ApiResult<string>.Fail("所属单位代码不能为空");
            if (_dal.GetByWellId(model.井号).Rows.Count == 0) return ApiResult<string>.Fail("该井号不存在", 404);
            if (!_dal.CheckUnitExist(model.所属单位)) return ApiResult<string>.Fail("单位代码不存在，请核对");
            int rows = _dal.Update(model.井号, model.所属单位, model.井别);
            return rows > 0 ? ApiResult<string>.Success("", "修改成功") : ApiResult<string>.Fail("修改失败");
        }

        public ApiResult<string> Delete(string wellId)
        {
            if (_dal.GetByWellId(wellId).Rows.Count == 0) return ApiResult<string>.Fail("该井号不存在", 404);
            _dal.Delete(wellId);
            return ApiResult<string>.Success("", "删除成功");
        }

        public ApiResult<object> GetPage(int pageIndex, int pageSize)
        {
            var (total, dt) = _dal.GetPage(pageIndex, pageSize);
            var list = new List<OilWell>();
            foreach (DataRow row in dt.Rows) list.Add(MapRow(row));
            return ApiResult<object>.Success(new { pageIndex, pageSize, total, totalPage = (total + pageSize - 1) / pageSize, data = list });
        }

        private static OilWell MapRow(DataRow row) => new()
        {
            井号 = GetSafeString(row, "井号"),
            井别 = GetSafeString(row, "井别"),
            所属单位 = GetSafeString(row, "单位代码")
        };
    }
}
