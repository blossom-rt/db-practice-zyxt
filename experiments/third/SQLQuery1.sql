-- 实验四 SQL练习3
-- 数据库：zyxt
-- 表：单位代码表、油水井表、施工单位表、物码表、作业项目表、材料费表

USE zyxt;
GO

-- 1. 表结构修改（增加列、主键、删除表/数据）

-- 先重建表避免报错
IF EXISTS (SELECT * FROM sys.tables WHERE name='施工单位月结算')
DROP TABLE 施工单位月结算;
GO

CREATE TABLE 施工单位月结算(
施工单位 NVARCHAR(50) NOT NULL,
年月 VARCHAR(10) NOT NULL,
结算金额 MONEY
);
GO

-- 1⑴ 给 施工单位月结算表 增加【备注】列（字符型）
ALTER TABLE 施工单位月结算 ADD 备注 VARCHAR(50);
GO
-- 查看新增列
SELECT * FROM 施工单位月结算;
GO

-- 1⑵ 增加主键约束
ALTER TABLE 施工单位月结算
ADD CONSTRAINT PK_施工单位月结算 PRIMARY KEY (施工单位, 年月);
GO
-- 再次执行实验三3.2插入（验证主键）
INSERT INTO 施工单位月结算(施工单位,年月,结算金额)
SELECT 施工单位,CAST(YEAR(结算日期)AS VARCHAR)+'-'+CAST(MONTH(结算日期)AS VARCHAR),SUM(结算金额)
FROM 作业项目表 GROUP BY 施工单位,YEAR(结算日期),MONTH(结算日期);
GO

-- 1⑶ 删除数据、删除表
DELETE FROM 施工单位月结算;
GO
SELECT * FROM 施工单位月结算;
GO

DROP TABLE 施工单位月结算;
GO

-- 2. 三类完整性约束（实体/参照/用户定义）

-- 2⑴ 实体完整性：给6张表加主键（先判断是否已存在）
IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE name='PK_单位代码表')
ALTER TABLE 单位代码表
ADD CONSTRAINT PK_单位代码表 PRIMARY KEY (单位代码);
GO

IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE name='PK_油水井表')
ALTER TABLE 油水井表
ADD CONSTRAINT PK_油水井表 PRIMARY KEY (井号);
GO

IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE name='PK_施工单位表')
ALTER TABLE 施工单位表
ADD CONSTRAINT PK_施工单位表 PRIMARY KEY (施工单位名称);
GO

IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE name='PK_物码表')
ALTER TABLE 物码表
ADD CONSTRAINT PK_物码表 PRIMARY KEY (物码);
GO

IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE name='PK_作业项目表')
ALTER TABLE 作业项目表
ADD CONSTRAINT PK_作业项目表 PRIMARY KEY (单据号);
GO

IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE name='PK_材料费表')
ALTER TABLE 材料费表
ADD CONSTRAINT PK_材料费表 PRIMARY KEY (单据号, 物码);
GO

-- 测试实体完整性违约
-- 1 重复主键（报错）
INSERT INTO 材料费表 VALUES('zy2018001','wm004',100,10);
GO

-- 2 主键为空（报错）
INSERT INTO 材料费表 VALUES('zy2018002',NULL,200,10);
GO

-- 2⑵ 参照完整性（外键约束，先判断是否已存在）
IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE name='FK_油水井_单位代码')
ALTER TABLE 油水井表
ADD CONSTRAINT FK_油水井_单位代码
FOREIGN KEY (单位代码) REFERENCES 单位代码表(单位代码);
GO

IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE name='FK_作业项目_井号')
ALTER TABLE 作业项目表
ADD CONSTRAINT FK_作业项目_井号
FOREIGN KEY (井号) REFERENCES 油水井表(井号);
GO

IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE name='FK_材料费_单据号')
ALTER TABLE 材料费表
ADD CONSTRAINT FK_材料费_单据号
FOREIGN KEY (单据号) REFERENCES 作业项目表(单据号)
ON DELETE CASCADE;
GO

IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE name='FK_材料费_物码')
ALTER TABLE 材料费表
ADD CONSTRAINT FK_材料费_物码
FOREIGN KEY (物码) REFERENCES 物码表(物码);
GO

IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE name='FK_作业项目_预算单位')
ALTER TABLE 作业项目表
ADD CONSTRAINT FK_作业项目_预算单位
FOREIGN KEY (预算单位) REFERENCES 单位代码表(单位代码);
GO

IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE name='FK_作业项目_施工单位')
ALTER TABLE 作业项目表
ADD CONSTRAINT FK_作业项目_施工单位
FOREIGN KEY (施工单位) REFERENCES 施工单位表(施工单位名称);
GO

-- 测试参照完整性违约
BEGIN TRANSACTION;
GO

-- （1）插入不存在的单位代码（报错）
INSERT INTO 油水井表 VALUES('y007','油井','112203002');
GO

-- （2）插入不存在的单据号/物码（报错）
INSERT INTO 材料费表 VALUES('zy2018007','wm006',100,10);
GO

-- （3）修改为不存在的施工单位（报错）
UPDATE 作业项目表
SET 施工单位='作业公司作业五队'
WHERE 单据号='zy2018001';
GO

-- （4）删除被引用的单位代码（报错）
DELETE FROM 单位代码表 WHERE 单位代码='112202002';
GO

-- （5）修改被引用的物码（报错）
UPDATE 物码表 SET 物码='wm04' WHERE 物码='wm004';
GO

-- 撤销成功操作
ROLLBACK TRANSACTION;
GO

-- 2⑶ 用户定义完整性（先删除已存在的约束和索引）
IF EXISTS (SELECT * FROM sys.check_constraints WHERE name='UQ_单位名称')
ALTER TABLE 单位代码表 DROP CONSTRAINT UQ_单位名称;
GO

IF EXISTS (SELECT * FROM sys.check_constraints WHERE name='CK_井别')
ALTER TABLE 油水井表 DROP CONSTRAINT CK_井别;
GO

IF EXISTS (SELECT * FROM sys.check_constraints WHERE name='UQ_名称规格')
ALTER TABLE 物码表 DROP CONSTRAINT UQ_名称规格;
GO

IF EXISTS (SELECT * FROM sys.check_constraints WHERE name='CK_金额非负')
ALTER TABLE 作业项目表 DROP CONSTRAINT CK_金额非负;
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE name='IX_作业项目表_入账日期' AND object_id=OBJECT_ID('作业项目表'))
DROP INDEX IX_作业项目表_入账日期 ON 作业项目表;
GO

-- 添加非空约束
ALTER TABLE 单位代码表 ALTER COLUMN 单位名称 NVARCHAR(50) NOT NULL;
ALTER TABLE 油水井表 ALTER COLUMN 单位代码 CHAR(10) NOT NULL;
ALTER TABLE 物码表 ALTER COLUMN 名称规格 NVARCHAR(50) NOT NULL;
ALTER TABLE 物码表 ALTER COLUMN 计量单位 CHAR(10) NOT NULL;
ALTER TABLE 材料费表 ALTER COLUMN 消耗数量 INT NOT NULL;
ALTER TABLE 材料费表 ALTER COLUMN 单价 MONEY NOT NULL;
ALTER TABLE 作业项目表 ALTER COLUMN 预算单位 CHAR(10) NOT NULL;
ALTER TABLE 作业项目表 ALTER COLUMN 井号 CHAR(10) NOT NULL;
ALTER TABLE 作业项目表 ALTER COLUMN 预算人 NVARCHAR(20) NOT NULL;
ALTER TABLE 作业项目表 ALTER COLUMN 结算人 NVARCHAR(20) NOT NULL;
ALTER TABLE 作业项目表 ALTER COLUMN 材料费 MONEY NOT NULL;
ALTER TABLE 作业项目表 ALTER COLUMN 人工费 MONEY NOT NULL;
ALTER TABLE 作业项目表 ALTER COLUMN 设备费 MONEY NOT NULL;
ALTER TABLE 作业项目表 ALTER COLUMN 其它费用 MONEY NOT NULL;
ALTER TABLE 作业项目表 ALTER COLUMN 开工日期 DATE NOT NULL;
ALTER TABLE 作业项目表 ALTER COLUMN 完工日期 DATE NOT NULL;
ALTER TABLE 作业项目表 ALTER COLUMN 施工单位 NVARCHAR(50) NOT NULL;
ALTER TABLE 作业项目表 ALTER COLUMN 施工内容 NVARCHAR(50) NOT NULL;
ALTER TABLE 作业项目表 ALTER COLUMN 结算日期 DATE NOT NULL;
ALTER TABLE 作业项目表 ALTER COLUMN 入账金额 MONEY NOT NULL;
ALTER TABLE 作业项目表 ALTER COLUMN 入账人 NVARCHAR(20) NOT NULL;
ALTER TABLE 作业项目表 ALTER COLUMN 入账日期 DATE NOT NULL;
GO

-- 添加唯一和CHECK约束
ALTER TABLE 单位代码表 ADD CONSTRAINT UQ_单位名称 UNIQUE (单位名称);
GO

ALTER TABLE 油水井表 ADD CONSTRAINT CK_井别 CHECK (井别 IN ('油井','水井'));
GO

ALTER TABLE 物码表 ADD CONSTRAINT UQ_名称规格 UNIQUE (名称规格);
GO

ALTER TABLE 作业项目表 ADD CONSTRAINT CK_金额非负 CHECK (预算金额>=0 AND 结算金额>=0 AND 入账金额>=0);
GO

-- 3. 视图操作（创建、查询、更新、撤销）

-- 3⑴ 创建视图：作业项目表+材料费表 全部列
IF EXISTS (SELECT * FROM sys.views WHERE name='V_项目材料费')
DROP VIEW V_项目材料费;
GO

CREATE VIEW V_项目材料费
AS
SELECT a.*,b.物码,b.消耗数量,b.单价
FROM 作业项目表 a
JOIN 材料费表 b ON a.单据号=b.单据号;
GO

-- 3⑵ 视图查询2个
-- 查询1：采油一矿二队项目
SELECT * FROM V_项目材料费 WHERE 预算单位='112201002';
GO
-- 查询2：材料费>6000
SELECT * FROM V_项目材料费 WHERE 材料费>6000;
GO

-- 3⑶ 预算视图 + 插入
IF EXISTS (SELECT * FROM sys.views WHERE name='V_项目预算')
DROP VIEW V_项目预算;
GO

CREATE VIEW V_项目预算
AS
SELECT 单据号,预算单位,井号,预算金额,预算人,预算日期,开工日期,完工日期,施工单位,施工内容,材料费,人工费,设备费,其它费用,结算金额,结算人,结算日期,入账金额,入账人,入账日期
FROM 作业项目表;
GO

-- 插入数据（补全所有非空字段，避免报错）
BEGIN TRANSACTION;
GO

INSERT INTO V_项目预算 VALUES(
'zy2018008','112202002','y005',10000,'张三','2018-07-02',
'2018-07-02','2018-07-03','作业公司作业三队','防砂',
7000,1000,2000,1300,11300,'李四','2018-07-03',11300,'赵六','2018-07-04'
);
GO

-- 查看基表变化
SELECT * FROM 作业项目表 WHERE 单据号='zy2018008';
GO

-- 3⑷ 撤销更新
ROLLBACK TRANSACTION;
GO