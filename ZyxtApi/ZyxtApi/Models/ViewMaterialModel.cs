namespace ZyxtApi.Models
{
    public class ViewMaterialModel
    {
        // 项目基础字段
        public string 单据号 { get; set; } = string.Empty;
        public string 预算单位 { get; set; } = string.Empty;
        public string 井号 { get; set; } = string.Empty;
        public string 施工单位 { get; set; } = string.Empty;
        public string 施工内容 { get; set; } = string.Empty;
        public DateTime 开工日期 { get; set; }

        // 材料相关 全部可空
        public string? 物码 { get; set; }
        public int? 消耗数量 { get; set; }
        public decimal? 单价 { get; set; }
        public string? 名称规格 { get; set; }
        public string? 计量单位 { get; set; }

        // 存储过程汇总字段
        public decimal? 材料总成本 { get; set; }
    }
}