USE zyxt;
GO

-- 2 游标：遍历作业项目表并打印表头与数据
-- 声明与表字段对应变量
DECLARE 
@单据号 CHAR(10),
@预算单位 CHAR(10),
@井号 CHAR(10),
@预算金额 MONEY,
@预算人 NVARCHAR(20),
@预算日期 DATE,
@开工日期 DATE,
@完工日期 DATE,
@施工单位 NVARCHAR(50),
@施工内容 NVARCHAR(50),
@材料费 MONEY,
@人工费 MONEY,
@设备费 MONEY,
@其它费用 MONEY,
@结算金额 MONEY,
@结算人 NVARCHAR(20),
@结算日期 DATE,
@入账金额 MONEY,
@入账人 NVARCHAR(20),
@入账日期 DATE;

-- 声明游标，绑定作业项目表全字段
DECLARE Job_Cursor CURSOR FOR
SELECT 单据号,预算单位,井号,预算金额,预算人,预算日期,开工日期,完工日期,
施工单位,施工内容,材料费,人工费,设备费,其它费用,结算金额,结算人,
结算日期,入账金额,入账人,入账日期
FROM 作业项目表;

-- 打开游标
OPEN Job_Cursor;

-- 打印表头
PRINT '单据号 预算单位 井号 预算金额 预算人 预算日期 开工日期 完工日期 施工单位 施工内容 材料费 人工费 设备费 其它费用 结算金额 结算人 结算日期 入账金额 入账人 入账日期';

-- 读取第一行数据
FETCH NEXT FROM Job_Cursor INTO 
@单据号,@预算单位,@井号,@预算金额,@预算人,@预算日期,@开工日期,@完工日期,
@施工单位,@施工内容,@材料费,@人工费,@设备费,@其它费用,@结算金额,@结算人,
@结算日期,@入账金额,@入账人,@入账日期;

-- 循环遍历游标所有数据
WHILE @@FETCH_STATUS = 0
BEGIN
    -- 打印单行数据
    PRINT CAST(@单据号 AS VARCHAR)+' '+CAST(@预算单位 AS VARCHAR)+' '+CAST(@井号 AS VARCHAR)+' '
    +CAST(@预算金额 AS VARCHAR)+' '+CAST(@预算人 AS VARCHAR)+' '+CAST(@预算日期 AS VARCHAR)+' '
    +CAST(@开工日期 AS VARCHAR)+' '+CAST(@完工日期 AS VARCHAR)+' '+CAST(@施工单位 AS VARCHAR)+' '
    +CAST(@施工内容 AS VARCHAR)+' '+CAST(@材料费 AS VARCHAR)+' '+CAST(@人工费 AS VARCHAR)+' '
    +CAST(@设备费 AS VARCHAR)+' '+CAST(@其它费用 AS VARCHAR)+' '+CAST(@结算金额 AS VARCHAR)+' '
    +CAST(@结算人 AS VARCHAR)+' '+CAST(@结算日期 AS VARCHAR)+' '+CAST(@入账金额 AS VARCHAR)+' '
    +CAST(@入账人 AS VARCHAR)+' '+CAST(@入账日期 AS VARCHAR);

    -- 读取下一行
    FETCH NEXT FROM Job_Cursor INTO 
    @单据号,@预算单位,@井号,@预算金额,@预算人,@预算日期,@开工日期,@完工日期,
    @施工单位,@施工内容,@材料费,@设备费,@其它费用,@结算金额,@结算人,
    @结算日期,@入账金额,@入账人,@入账日期;
END

-- 关闭、释放游标
CLOSE Job_Cursor;
DEALLOCATE Job_Cursor;
GO