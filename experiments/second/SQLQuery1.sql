-- 实验三 SQL练习2
-- 数据库：zyxt

USE zyxt;
GO

-- 1. 索引操作

-- ⑴ 创建索引
CREATE INDEX IX_作业项目表_预算日期 ON 作业项目表(预算日期);
CREATE INDEX IX_作业项目表_结算日期 ON 作业项目表(结算日期);
CREATE INDEX IX_作业项目表_入账日期 ON 作业项目表(入账日期);
GO

-- ⑵ 删除索引
--DROP INDEX IX_作业项目表_预算日期 ON 作业项目表;
--DROP INDEX IX_作业项目表_结算日期 ON 作业项目表;
--DROP INDEX IX_作业项目表_入账日期 ON 作业项目表;
--GO

-- 2. 综合查询

-- ⑴ 采油一矿二队预算项目明细
SELECT 
    a.单据号, a.井号, a.预算金额, b.井别, c.单位名称, a.预算日期
FROM 作业项目表 a
JOIN 油水井表 b ON a.井号 = b.井号
JOIN 单位代码表 c ON b.单位代码 = c.单位代码
WHERE c.单位名称 = '采油一矿二队'
AND a.预算日期 BETWEEN '2018-05-01' AND '2018-05-28';
GO

-- ⑵ 采油一矿二队结算项目明细
SELECT 
    单据号, 施工内容, 材料费, 人工费, 结算金额, 结算日期
FROM 作业项目表
WHERE 预算单位 LIKE '112201002%'
AND 结算日期 BETWEEN '2018-05-01' AND '2018-05-28'
AND 结算金额 IS NOT NULL;
GO

-- ⑶ 采油一矿二队材料费明细
SELECT 
    b.单据号, a.名称规格, a.计量单位, b.消耗数量, b.单价, 
    (b.消耗数量 * b.单价) AS 材料总金额
FROM 材料费表 b
JOIN 物码表 a ON b.物码 = a.物码
WHERE b.单据号 IN (
    SELECT 单据号 FROM 作业项目表 
    WHERE 预算单位 LIKE '112201002%'
    AND 预算日期 BETWEEN '2018-05-01' AND '2018-05-28'
);
GO

-- ⑷ 采油一矿二队入账项目明细
SELECT 
    单据号, 入账金额, 入账人, 入账日期
FROM 作业项目表
WHERE 预算单位 LIKE '112201002%'
AND 入账日期 BETWEEN '2018-05-01' AND '2018-05-28'
AND 入账金额 IS NOT NULL;
GO

-- ⑸ 采油一矿二队总预算金额
SELECT 
    SUM(预算金额) AS 采油一矿二队总预算金额
FROM 作业项目表
WHERE 预算单位 LIKE '112201002%'
AND 预算日期 BETWEEN '2018-05-01' AND '2018-05-28';
GO

-- ⑹ 采油一矿二队总结算金额
SELECT 
    SUM(结算金额) AS 采油一矿二队总结算金额
FROM 作业项目表
WHERE 预算单位 LIKE '112201002%'
AND 结算日期 BETWEEN '2018-05-01' AND '2018-05-28';
GO

-- ⑺ 采油一矿二队总入账金额
SELECT 
    SUM(入账金额) AS 采油一矿二队总入账金额
FROM 作业项目表
WHERE 预算单位 LIKE '112201002%'
AND 入账日期 BETWEEN '2018-05-01' AND '2018-05-28';
GO

-- ⑻ 采油一矿总入账金额
SELECT 
    SUM(入账金额) AS 采油一矿总入账金额
FROM 作业项目表
WHERE 预算单位 LIKE '112201%'
AND 入账日期 BETWEEN '2018-05-01' AND '2018-05-28';
GO

-- ⑼ 参与入账人员
SELECT DISTINCT 入账人
FROM 作业项目表
WHERE 入账人 IS NOT NULL;
GO

-- ⑽ 已结算未入账项目
SELECT 
    单据号, 结算金额, 结算日期
FROM 作业项目表
WHERE 结算日期 BETWEEN '2018-05-01' AND '2018-05-28'
AND 结算金额 IS NOT NULL
AND 入账金额 IS NULL;
GO

-- ⑾ 采油一矿二队项目按入账金额降序
SELECT *
FROM 作业项目表
WHERE 预算单位 = '112201002'
ORDER BY 入账金额 DESC;
GO

-- ⑿ 各施工单位结算金额总和
SELECT 
    施工单位, SUM(结算金额) AS 结算总额
FROM 作业项目表
GROUP BY 施工单位;
GO

-- ⒀ 消耗材料三 >2000 元（子查询）
SELECT *
FROM 材料费表
WHERE 物码 = 'wm003'
AND 消耗数量 * 单价 > 2000;
GO

-- ⒁ 作业公司二队参与项目
SELECT *
FROM 作业项目表
WHERE 施工单位 = '作业公司作业二队';
GO

-- ⒂ 一队、二队项目（UNION）
SELECT 单据号,施工单位,施工内容,结算金额
FROM 作业项目表
WHERE 施工单位 = '作业公司作业一队'
UNION
SELECT 单据号,施工单位,施工内容,结算金额
FROM 作业项目表
WHERE 施工单位 = '作业公司作业二队';
GO

-- ⒃ 采油一矿油井施工单位
SELECT DISTINCT 施工单位
FROM 作业项目表 a
JOIN 油水井表 b ON a.井号 = b.井号
WHERE a.预算单位 LIKE '112201%'
AND b.井别 = '油井';
GO

-- 3. 高级增删改 + 事务回滚

-- ⑴ 建立数据表：施工单位、年月、结算金额
CREATE TABLE 施工单位月结算(
    施工单位 NVARCHAR(50),
    年月 VARCHAR(10),
    结算金额 MONEY
);
GO

-- 查看新建的空表
SELECT * FROM 施工单位月结算;
GO

-- ⑵ 插入各施工单位每月结算总和（子查询）
INSERT INTO 施工单位月结算(施工单位,年月,结算金额)
SELECT 
    施工单位,
    CAST(YEAR(结算日期) AS VARCHAR)+'-'+CAST(MONTH(结算日期) AS VARCHAR),
    SUM(结算金额)
FROM 作业项目表
GROUP BY 施工单位,YEAR(结算日期),MONTH(结算日期);
GO

-- 查看插入后的数据
SELECT * FROM 施工单位月结算;
GO

-- ⑶ 修改：采油一矿油井项目结算人改为“李兵”
-- ⑷ 删除：采油一矿油井作业项目
-- ⑸ 回滚撤销所有操作

BEGIN TRANSACTION;

-- 修改
UPDATE 作业项目表
SET 结算人 = '李兵'
WHERE 井号 IN (
    SELECT 井号 FROM 油水井表
    WHERE 井别 = '油井' AND 单位代码 LIKE '112201%'
);

-- 查看修改后结果
SELECT 单据号,结算人 FROM 作业项目表;
GO

-- 删除
DELETE FROM 作业项目表
WHERE 井号 IN (
    SELECT 井号 FROM 油水井表
    WHERE 井别 = '油井' AND 单位代码 LIKE '112201%'
);

-- 查看删除后结果
SELECT * FROM 作业项目表;
GO

-- 回滚（撤销修改和删除）
ROLLBACK TRANSACTION;
GO

-- 查看回滚后恢复的数据
SELECT * FROM 作业项目表;
GO