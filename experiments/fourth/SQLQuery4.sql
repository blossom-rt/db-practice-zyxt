USE zyxt;
GO

-- 触发器1：插入数据时自动计算结算金额
CREATE TRIGGER Tri_Job_Insert
ON 作业项目表
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO 作业项目表 (
        单据号,预算单位,井号,预算金额,预算人,预算日期,开工日期,完工日期,
        施工单位,施工内容,材料费,人工费,设备费,其它费用,结算金额,结算人,
        结算日期,入账金额,入账人,入账日期
    )
    SELECT 
        单据号,预算单位,井号,预算金额,预算人,预算日期,开工日期,完工日期,
        施工单位,施工内容,材料费,人工费,设备费,其它费用,
        材料费 + 人工费 + 设备费 + 其它费用,
        结算人,结算日期,入账金额,入账人,入账日期
    FROM INSERTED;
END;
GO

-- 触发器2：修改数据时自动更新结算金额
CREATE TRIGGER Tri_Job_Update
ON 作业项目表
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE 作业项目表
    SET 结算金额 = i.材料费 + i.人工费 + i.设备费 + i.其它费用
    FROM 作业项目表 j
    JOIN INSERTED i ON j.单据号 = i.单据号;
END;
GO

-- 触发器3：删除数据时，同步删除材料费表明细
CREATE TRIGGER Tri_Job_Delete
ON 作业项目表
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM 材料费表
    WHERE 单据号 IN (SELECT 单据号 FROM DELETED);
END;
GO

-- 触发器验证语句
-- 验证1：插入数据，测试插入触发器
INSERT INTO 作业项目表(单据号,预算单位,井号,预算金额,预算人,预算日期,开工日期,完工日期,
施工单位,施工内容,材料费,人工费,设备费,其它费用,结算人,结算日期,入账金额,入账人,入账日期)
VALUES('zy2018009','112202002','y005',20000,'赵六','2018-08-01','2018-08-02','2018-08-20',
'作业公司作业二队','压裂',8000,3000,1500,2000,'孙七','2018-08-21',14500,'周八','2018-08-22');
SELECT 单据号,材料费,人工费,设备费,其它费用,结算金额 FROM 作业项目表 WHERE 单据号='zy2018009';
GO

-- 验证2：修改数据，测试更新触发器
UPDATE 作业项目表 SET 材料费=9000 WHERE 单据号='zy2018009';
SELECT 单据号,材料费,人工费,设备费,其它费用,结算金额 FROM 作业项目表 WHERE 单据号='zy2018009';
GO

-- 验证3：删除数据，测试删除触发器
DELETE FROM 作业项目表 WHERE 单据号='zy2018009';
SELECT * FROM 作业项目表 WHERE 单据号='zy2018009';
SELECT * FROM 材料费表 WHERE 单据号='zy2018009';
GO

-- 实验结束：删除触发器
-- 课堂实验原流程：创建触发器 → 测试 → 删除触发器
-- 本次大作业修改：注释DROP语句，保留触发器持久生效，用于系统业务自动计算与级联删除
-- DROP TRIGGER Tri_Job_Insert;
-- DROP TRIGGER Tri_Job_Update;
-- DROP TRIGGER Tri_Job_Delete;
GO