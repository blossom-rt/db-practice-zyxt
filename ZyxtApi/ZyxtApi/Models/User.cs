namespace ZyxtApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string 用户名 { get; set; } = string.Empty;
        public string 密码 { get; set; } = string.Empty;
        public string 角色 { get; set; } = string.Empty; // "管理员" / "操作员"
    }
}
