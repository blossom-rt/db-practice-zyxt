using System.Data;
using ZyxtApi.DAL;
using ZyxtApi.Models;

namespace ZyxtApi.BLL
{
    public class ConstructUnitBLL
    {
        private readonly ConstructUnitDAL _dal = new();

        private static string GetSafeString(DataRow row, string col)
            => row[col] == DBNull.Value ? "" : row[col].ToString()!.Trim();

        public ApiResult<List<ConstructUnit>> GetAll()
        {
            DataTable dt = _dal.GetAll();
            var list = new List<ConstructUnit>();
            foreach (DataRow row in dt.Rows)
                list.Add(new ConstructUnit { 施工单位名称 = GetSafeString(row, "施工单位名称") });
            return ApiResult<List<ConstructUnit>>.Success(list);
        }

        public ApiResult<ConstructUnit> GetByName(string name)
        {
            DataTable dt = _dal.GetByName(name);
            if (dt.Rows.Count == 0) return ApiResult<ConstructUnit>.Fail("该施工单位不存在", 404);
            return ApiResult<ConstructUnit>.Success(new ConstructUnit { 施工单位名称 = GetSafeString(dt.Rows[0], "施工单位名称") });
        }

        public ApiResult<string> Create(ConstructUnit model)
        {
            if (string.IsNullOrWhiteSpace(model.施工单位名称)) return ApiResult<string>.Fail("施工单位名称不能为空");
            if (_dal.GetByName(model.施工单位名称).Rows.Count > 0) return ApiResult<string>.Fail("该施工单位已存在");
            int rows = _dal.Insert(model.施工单位名称);
            return rows > 0 ? ApiResult<string>.Success("", "新增成功") : ApiResult<string>.Fail("新增失败");
        }

        public ApiResult<string> Update(string oldName, ConstructUnit model)
        {
            if (string.IsNullOrWhiteSpace(oldName)) return ApiResult<string>.Fail("原名称不能为空");
            if (string.IsNullOrWhiteSpace(model.施工单位名称)) return ApiResult<string>.Fail("施工单位名称不能为空");
            if (_dal.GetByName(oldName).Rows.Count == 0) return ApiResult<string>.Fail("该施工单位不存在", 404);
            if (oldName != model.施工单位名称 && _dal.GetByName(model.施工单位名称).Rows.Count > 0)
                return ApiResult<string>.Fail("该施工单位名称已存在");
            int rows = _dal.Update(oldName, model.施工单位名称);
            return rows > 0 ? ApiResult<string>.Success("", "修改成功") : ApiResult<string>.Fail("修改失败");
        }

        public ApiResult<string> Delete(string name)
        {
            if (_dal.GetByName(name).Rows.Count == 0) return ApiResult<string>.Fail("该施工单位不存在", 404);
            _dal.Delete(name);
            return ApiResult<string>.Success("", "删除成功");
        }

        public ApiResult<object> GetPage(int pageIndex, int pageSize)
        {
            var (total, dt) = _dal.GetPage(pageIndex, pageSize);
            var list = new List<ConstructUnit>();
            foreach (DataRow row in dt.Rows)
                list.Add(new ConstructUnit { 施工单位名称 = GetSafeString(row, "施工单位名称") });
            return ApiResult<object>.Success(new { pageIndex, pageSize, total, totalPage = (total + pageSize - 1) / pageSize, data = list });
        }
    }
}
