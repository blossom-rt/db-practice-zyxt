namespace ZyxtApi.Models
{
    public class MaterialCost
    {
        public string 单据号 { get; set; } = string.Empty;
        public string 物码 { get; set; } = string.Empty;
        public int 消耗数量 { get; set; }
        public decimal 单价 { get; set; }
    }
}