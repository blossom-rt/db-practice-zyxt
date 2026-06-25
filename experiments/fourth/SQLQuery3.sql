USE zyxt;
GO

-- 3 创建存储过程：按单位+时间段统计成本运行情况
-- 输入参数：@单位代码  @起始日期  @结束日期
CREATE PROCEDURE Proc_Unit_Cost
@单位代码 CHAR(10),
@起始日期 DATE,
@结束日期 DATE
AS
BEGIN
    SET NOCOUNT ON;
    -- 定义统计变量
    DECLARE @总预算 MONEY,@总结算 MONEY,@总入账 MONEY;
    DECLARE @未结算 MONEY,@未入账 MONEY;
    DECLARE @单位名称 NVARCHAR(50);

    -- 获取单位名称（关联单位代码表）
    SELECT @单位名称 = 单位名称 FROM 单位代码表 WHERE 单位代码 = @单位代码;

    -- 聚合统计指定时间段内金额
    SELECT 
    @总预算 = SUM(预算金额),
    @总结算 = SUM(结算金额),
    @总入账 = SUM(入账金额)
    FROM 作业项目表
    WHERE 预算单位 = @单位代码 
    AND 预算日期 BETWEEN @起始日期 AND @结束日期;

    -- 计算衍生金额
    SET @未结算 = @总预算 - @总结算;
    SET @未入账 = @总结算 - @总入账;

    -- 按指定格式输出
    PRINT '***'+@单位名称+'时间'+CAST(@起始日期 AS VARCHAR)+'---'+CAST(@结束日期 AS VARCHAR)+'成本运行情况';
    PRINT '预算金额        结算金额        入账金额        未结算金额      未入账金额';
    PRINT CAST(@总预算 AS VARCHAR)+'    '+CAST(@总结算 AS VARCHAR)+'    '+CAST(@总入账 AS VARCHAR)+'    '+CAST(@未结算 AS VARCHAR)+'    '+CAST(@未入账 AS VARCHAR);
END;
GO

-- 调用测试（分三类单位执行）
-- 测试1：采油厂
EXEC Proc_Unit_Cost @单位代码='112201001',@起始日期='2018-01-01',@结束日期='2018-12-31';
-- 测试2：采油矿
EXEC Proc_Unit_Cost @单位代码='112201002',@起始日期='2018-01-01',@结束日期='2018-12-31';
-- 测试3：采油队
EXEC Proc_Unit_Cost @单位代码='112202001',@起始日期='2018-01-01',@结束日期='2018-12-31';
GO