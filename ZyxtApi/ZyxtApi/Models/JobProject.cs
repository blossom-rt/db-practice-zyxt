using System;

namespace ZyxtApi.Models
{
    public class JobProject
    {
        // 1
        public string 单据号 { get; set; } = string.Empty;
        // 2
        public string 预算单位 { get; set; } = string.Empty;
        // 3
        public string 井号 { get; set; } = string.Empty;
        // 4
        public decimal 预算金额 { get; set; }
        // 5
        public string 预算人 { get; set; } = string.Empty;
        // 6
        public DateTime 预算日期 { get; set; }
        // 7
        public DateTime 开工日期 { get; set; }
        // 8
        public DateTime 完工日期 { get; set; }
        // 9
        public string 施工单位 { get; set; } = string.Empty;
        // 10
        public string 施工内容 { get; set; } = string.Empty;
        // 11
        public decimal 材料费 { get; set; }
        // 12
        public decimal 人工费 { get; set; }
        // 13
        public decimal 设备费 { get; set; }
        // 14
        public decimal 其它费用 { get; set; }
        // 15
        public decimal 结算金额 { get; set; }
        // 16
        public string 结算人 { get; set; } = string.Empty;
        // 17
        public DateTime 结算日期 { get; set; }
        // 18
        public decimal 入账金额 { get; set; }
        // 19
        public string 入账人 { get; set; } = string.Empty;
        // 20
        public DateTime 入账日期 { get; set; }
    }
}