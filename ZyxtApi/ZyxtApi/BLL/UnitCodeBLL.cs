using System.Data;
using ZyxtApi.DAL;
using ZyxtApi.Models;

namespace ZyxtApi.BLL
{
    public class UnitCodeBLL
    {
        private readonly UnitCodeDAL _dal = new();

        private static string GetSafeString(DataRow row, string col)
            => row[col] == DBNull.Value ? "" : row[col].ToString()!.Trim();

        public ApiResult<List<UnitCode>> GetAll()
        {
            DataTable dt = _dal.GetAll();
            var list = new List<UnitCode>();
            foreach (DataRow row in dt.Rows)
                list.Add(new UnitCode { 单位代码 = GetSafeString(row, "单位代码"), 单位名称 = GetSafeString(row, "单位名称") });
            return ApiResult<List<UnitCode>>.Success(list);
        }

        public ApiResult<UnitCode> GetByCode(string code)
        {
            DataTable dt = _dal.GetByCode(code);
            if (dt.Rows.Count == 0)
                return ApiResult<UnitCode>.Fail("该单位不存在", 404);
            var row = dt.Rows[0];
            return ApiResult<UnitCode>.Success(new UnitCode { 单位代码 = GetSafeString(row, "单位代码"), 单位名称 = GetSafeString(row, "单位名称") });
        }

        public ApiResult<string> Create(UnitCode model)
        {
            if (string.IsNullOrWhiteSpace(model.单位代码))
                return ApiResult<string>.Fail("单位代码不能为空");
            if (string.IsNullOrWhiteSpace(model.单位名称))
                return ApiResult<string>.Fail("单位名称不能为空");
            if (_dal.GetByCode(model.单位代码).Rows.Count > 0)
                return ApiResult<string>.Fail("该单位代码已存在，请勿重复新增");
            int rows = _dal.Insert(model.单位代码, model.单位名称);
            return rows > 0 ? ApiResult<string>.Success("", "新增成功") : ApiResult<string>.Fail("新增失败，无数据写入");
        }

        public ApiResult<string> Update(UnitCode model)
        {
            if (string.IsNullOrWhiteSpace(model.单位代码))
                return ApiResult<string>.Fail("单位代码不能为空");
            if (string.IsNullOrWhiteSpace(model.单位名称))
                return ApiResult<string>.Fail("单位名称不能为空");
            if (_dal.GetByCode(model.单位代码).Rows.Count == 0)
                return ApiResult<string>.Fail("该单位不存在", 404);
            int rows = _dal.Update(model.单位代码, model.单位名称);
            return rows > 0 ? ApiResult<string>.Success("", "修改成功") : ApiResult<string>.Fail("修改失败");
        }

        public ApiResult<string> Delete(string code)
        {
            if (_dal.GetByCode(code).Rows.Count == 0)
                return ApiResult<string>.Fail("该单位不存在", 404);
            _dal.Delete(code);
            return ApiResult<string>.Success("", "删除成功");
        }

        public ApiResult<object> GetPage(int pageIndex, int pageSize)
        {
            var (total, dt) = _dal.GetPage(pageIndex, pageSize);
            var list = new List<UnitCode>();
            foreach (DataRow row in dt.Rows)
                list.Add(new UnitCode { 单位代码 = GetSafeString(row, "单位代码"), 单位名称 = GetSafeString(row, "单位名称") });
            return ApiResult<object>.Success(new { pageIndex, pageSize, total, totalPage = (total + pageSize - 1) / pageSize, data = list });
        }
    }
}
