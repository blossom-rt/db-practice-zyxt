using Microsoft.Data.SqlClient;
using System.Data;

namespace ZyxtApi.DAL
{
    public class UserDAL
    {
        public DataTable GetByUsername(string username)
        {
            return SqlHelper.QueryTable("SELECT * FROM 用户表 WHERE 用户名=@user",
                new SqlParameter("@user", username));
        }
    }
}
