using System.Data;
using ZyxtApi.DAL;
using ZyxtApi.Models;

namespace ZyxtApi.BLL
{
    public class MaterialBLL
    {
        private readonly MaterialDAL _dal = new();

        private static string GetSafeString(DataRow row, string col)
            => row[col] == DBNull.Value ? "" : row[col].ToString()!.Trim();

        public ApiResult<List<Material>> GetAll()
        {
            DataTable dt = _dal.GetAll();
            var list = new List<Material>();
            foreach (DataRow row in dt.Rows)
                list.Add(new Material { 物码 = GetSafeString(row, "物码"), 名称规格 = GetSafeString(row, "名称规格"), 计量单位 = GetSafeString(row, "计量单位") });
            return ApiResult<List<Material>>.Success(list);
        }

        public ApiResult<Material> GetByCode(string code)
        {
            DataTable dt = _dal.GetByCode(code);
            if (dt.Rows.Count == 0) return ApiResult<Material>.Fail("该物料不存在", 404);
            var row = dt.Rows[0];
            return ApiResult<Material>.Success(new Material { 物码 = GetSafeString(row, "物码"), 名称规格 = GetSafeString(row, "名称规格"), 计量单位 = GetSafeString(row, "计量单位") });
        }

        public ApiResult<string> Create(Material model)
        {
            if (string.IsNullOrWhiteSpace(model.物码)) return ApiResult<string>.Fail("物码不能为空");
            if (string.IsNullOrWhiteSpace(model.名称规格)) return ApiResult<string>.Fail("名称规格不能为空");
            if (string.IsNullOrWhiteSpace(model.计量单位)) return ApiResult<string>.Fail("计量单位不能为空");
            if (_dal.GetByCode(model.物码).Rows.Count > 0) return ApiResult<string>.Fail("该物码已存在");
            int rows = _dal.Insert(model.物码, model.名称规格, model.计量单位);
            return rows > 0 ? ApiResult<string>.Success("", "新增成功") : ApiResult<string>.Fail("新增失败");
        }

        public ApiResult<string> Update(Material model)
        {
            if (string.IsNullOrWhiteSpace(model.物码)) return ApiResult<string>.Fail("物码不能为空");
            if (string.IsNullOrWhiteSpace(model.名称规格)) return ApiResult<string>.Fail("名称规格不能为空");
            if (string.IsNullOrWhiteSpace(model.计量单位)) return ApiResult<string>.Fail("计量单位不能为空");
            if (_dal.GetByCode(model.物码).Rows.Count == 0) return ApiResult<string>.Fail("该物料不存在", 404);
            int rows = _dal.Update(model.物码, model.名称规格, model.计量单位);
            return rows > 0 ? ApiResult<string>.Success("", "修改成功") : ApiResult<string>.Fail("修改失败");
        }

        public ApiResult<string> Delete(string code)
        {
            if (_dal.GetByCode(code).Rows.Count == 0) return ApiResult<string>.Fail("该物料不存在", 404);
            _dal.Delete(code);
            return ApiResult<string>.Success("", "删除成功");
        }

        public ApiResult<object> GetPage(int pageIndex, int pageSize)
        {
            var (total, dt) = _dal.GetPage(pageIndex, pageSize);
            var list = new List<Material>();
            foreach (DataRow row in dt.Rows)
                list.Add(new Material { 物码 = GetSafeString(row, "物码"), 名称规格 = GetSafeString(row, "名称规格"), 计量单位 = GetSafeString(row, "计量单位") });
            return ApiResult<object>.Success(new { pageIndex, pageSize, total, totalPage = (total + pageSize - 1) / pageSize, data = list });
        }
    }
}
