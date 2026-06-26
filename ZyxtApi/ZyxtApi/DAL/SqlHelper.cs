using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;

namespace ZyxtApi.DAL
{
    public static class SqlHelper
    {
        /// <summary>
        /// 当前请求的操作人（通过AsyncLocal按请求传递）
        /// </summary>
        private static readonly AsyncLocal<string?> _currentUser = new();

        /// <summary>
        /// 设置当前请求的操作人（由中间件在认证后调用）
        /// </summary>
        public static string? CurrentUser
        {
            get => _currentUser.Value;
            set => _currentUser.Value = value;
        }

        /// <summary>
        /// 在连接上设置SESSION_CONTEXT，供触发器读取操作人
        /// </summary>
        private static void SetSessionContext(SqlConnection conn)
        {
            if (_currentUser.Value != null)
            {
                using SqlCommand cmd = new SqlCommand("sp_set_session_context", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@key", "Username");
                cmd.Parameters.AddWithValue("@value", _currentUser.Value);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 注入配置对象，从appsettings读取连接字符串（标记可空消除CS8618）
        /// </summary>
        public static IConfiguration? Config { get; set; }

        /// <summary>
        /// 获取数据库连接字符串
        /// </summary>
        private static string GetConnectionString()
        {
            if (Config == null)
                throw new Exception("未注入配置文件，请在Program.cs中赋值SqlHelper.Config = builder.Configuration");
            // ?? ""兜底，消除CS8603；空串直接抛出异常
            string connStr = Config.GetConnectionString("Default") ?? "";
            if (string.IsNullOrWhiteSpace(connStr))
                throw new Exception("配置文件中未找到ConnectionStrings:Default节点");
            return connStr;
        }

        /// <summary>
        /// 在已打开的连接上设置SESSION_CONTEXT，供事务BLL手动调用
        /// </summary>
        public static void ApplySessionContext(SqlConnection conn)
        {
            SetSessionContext(conn);
        }

        /// <summary>
        /// 执行增删改SQL，返回受影响行数
        /// </summary>
        public static int ExecuteNonQuery(string sql, params SqlParameter[] paras)
        {
            try
            {
                string connStr = GetConnectionString();
                using SqlConnection conn = new SqlConnection(connStr);
                conn.Open();
                SetSessionContext(conn);
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddRange(paras);
                return cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                // 包装后抛出，交由全局异常中间件解析错误码
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 执行查询SQL，返回DataTable
        /// </summary>
        public static DataTable QueryTable(string sql, params SqlParameter[] paras)
        {
            try
            {
                string connStr = GetConnectionString();
                using SqlConnection conn = new SqlConnection(connStr);
                SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                adapter.SelectCommand.Parameters.AddRange(paras);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 执行存储过程，返回结果表
        /// </summary>
        public static DataTable ExecProc(string procName, params SqlParameter[] paras)
        {
            try
            {
                string connStr = GetConnectionString();
                using SqlConnection conn = new SqlConnection(connStr);
                SqlDataAdapter adapter = new SqlDataAdapter();
                SqlCommand cmd = new SqlCommand(procName, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddRange(paras);
                adapter.SelectCommand = cmd;
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        // 事务支持重载

        /// <summary>
        /// 事务版增删改
        /// </summary>
        public static int ExecuteNonQuery(SqlConnection conn, SqlTransaction trans, string sql, params SqlParameter[] paras)
        {
            SqlCommand cmd = new SqlCommand(sql, conn, trans);
            cmd.Parameters.AddRange(paras);
            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 事务版查询
        /// </summary>
        public static DataTable QueryTable(SqlConnection conn, SqlTransaction trans, string sql, params SqlParameter[] paras)
        {
            SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
            adapter.SelectCommand.Transaction = trans;
            adapter.SelectCommand.Parameters.AddRange(paras);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        /// <summary>
        /// 获取新连接（供BLL事务使用）
        /// </summary>
        public static SqlConnection CreateConnection()
        {
            return new SqlConnection(GetConnectionString());
        }
    }
}