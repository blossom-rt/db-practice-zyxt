namespace ZyxtApi.Models
{
    public class OperationLog
    {
        public int Id { get; set; }
        public string 操作类型 { get; set; } = string.Empty; // INSERT/UPDATE/DELETE
        public string 操作表 { get; set; } = string.Empty;
        public string 操作人 { get; set; } = string.Empty;
        public DateTime 操作时间 { get; set; }
        public string 操作内容 { get; set; } = string.Empty;
    }
}
