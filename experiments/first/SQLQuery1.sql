-- 实验二 SQL练习1
-- 功能：采油厂油水井作业成本管理系统

-- 1. 创建数据库 zyxt
IF EXISTS (SELECT * FROM sys.databases WHERE name='zyxt')
    DROP DATABASE zyxt;
CREATE DATABASE zyxt;
GO

USE zyxt;
GO

-- 步骤2：创建6个基础表，指定数据类型+主键约束（实体完整性）
-- 2.1 单位代码表：单位代码(主键)、单位名称
CREATE TABLE 单位代码表(
    单位代码 CHAR(10) PRIMARY KEY, -- 字符型，主键唯一非空
    单位名称 NVARCHAR(50) NOT NULL -- 中文用NVARCHAR，避免乱码
);
GO

-- 2.2 油水井表：井号(主键)、井别(油井/水井)、单位代码(关联单位代码表)
CREATE TABLE 油水井表(
    井号 CHAR(10) PRIMARY KEY,
    井别 CHAR(4) NOT NULL,
    单位代码 CHAR(10) NOT NULL
);
GO

-- 2.3 施工单位表：施工单位名称(主键，无重复)
CREATE TABLE 施工单位表(
    施工单位名称 NVARCHAR(50) PRIMARY KEY
);
GO

-- 2.4 物码表：物码(主键)、名称规格、计量单位
CREATE TABLE 物码表(
    物码 CHAR(10) PRIMARY KEY,
    名称规格 NVARCHAR(50) NOT NULL,
    计量单位 CHAR(10) NOT NULL
);
GO

-- 2.5 作业项目表：主表，单据号(主键)，包含预算/结算/入账全字段
CREATE TABLE 作业项目表(
    单据号 CHAR(20) PRIMARY KEY,
    预算单位 CHAR(10) NOT NULL,
    井号 CHAR(10) NOT NULL,
    预算金额 MONEY NOT NULL, -- 货币类型，适配金额计算
    预算人 NVARCHAR(20) NOT NULL,
    预算日期 DATE NOT NULL, -- 日期类型
    开工日期 DATE NOT NULL,
    完工日期 DATE NOT NULL,
    施工单位 NVARCHAR(50) NOT NULL,
    施工内容 NVARCHAR(100) NOT NULL,
    材料费 MONEY NOT NULL,
    人工费 MONEY NOT NULL,
    设备费 MONEY NOT NULL,
    其它费用 MONEY NOT NULL,
    结算金额 MONEY NOT NULL,
    结算人 NVARCHAR(20) NOT NULL,
    结算日期 DATE NOT NULL,
    入账金额 MONEY,
    入账人 NVARCHAR(20),
    入账日期 DATE
);
GO

-- 2.6 材料费表：子表，单据号+物码联合主键（一个单据对应多个物码）
CREATE TABLE 材料费表(
    单据号 CHAR(20) NOT NULL,
    物码 CHAR(10) NOT NULL,
    消耗数量 INT NOT NULL, -- 数值型，消耗数量
    单价 MONEY NOT NULL,
    PRIMARY KEY(单据号,物码) -- 联合主键，避免重复明细
);
GO

-- 参照完整性（外键约束，先判断是否已存在）

ALTER TABLE 油水井表
ADD CONSTRAINT FK_油水井_单位代码
FOREIGN KEY (单位代码) REFERENCES 单位代码表(单位代码);
GO

ALTER TABLE 作业项目表
ADD CONSTRAINT FK_作业项目_井号
FOREIGN KEY (井号) REFERENCES 油水井表(井号);
GO

ALTER TABLE 材料费表
ADD CONSTRAINT FK_材料费_单据号
FOREIGN KEY (单据号) REFERENCES 作业项目表(单据号)
ON DELETE CASCADE;
GO

ALTER TABLE 材料费表
ADD CONSTRAINT FK_材料费_物码
FOREIGN KEY (物码) REFERENCES 物码表(物码);
GO

ALTER TABLE 作业项目表
ADD CONSTRAINT FK_作业项目_预算单位
FOREIGN KEY (预算单位) REFERENCES 单位代码表(单位代码);
GO

ALTER TABLE 作业项目表
ADD CONSTRAINT FK_作业项目_施工单位
FOREIGN KEY (施工单位) REFERENCES 施工单位表(施工单位名称);
GO

-- 步骤3：插入基础数据（完全按实验要求）

-- 3.1 单位代码表
INSERT INTO 单位代码表
VALUES
('1122','采油厂'),
('112201','采油一矿'),
('112202','采油二矿'),
('112201001','采油一矿一队'),
('112201002','采油一矿二队'),
('112201003','采油一矿三队'),
('112202001','采油二矿一队'),
('112202002','采油二矿二队');
GO

-- 3.2 油水井表
INSERT INTO 油水井表
VALUES
('y001','油井','112201001'),
('y002','油井','112201001'),
('y003','油井','112201002'),
('s001','水井','112201002'),
('y004','油井','112201003'),
('s002','水井','112202001'),
('s003','水井','112202001'),
('y005','油井','112202002');
GO

-- 3.3 施工单位表
INSERT INTO 施工单位表
VALUES
('作业公司作业一队'),
('作业公司作业二队'),
('作业公司作业三队');
GO

-- 3.4 物码表
INSERT INTO 物码表
VALUES
('wm001','材料一','吨'),
('wm002','材料二','米'),
('wm003','材料三','桶'),
('wm004','材料四','袋');
GO

-- 步骤4：插入业务数据 zy2018001 ~ zy2018005

INSERT INTO 作业项目表
VALUES
-- zy2018001
('zy2018001','112201001','y001',10000,'张三','2018-05-01',
'2018-05-04','2018-05-25','作业公司作业一队','堵漏',7000,2500,1000,1400,11900,'李四','2018-05-26',11900,'王五','2018-05-28'),

-- zy2018002
('zy2018002','112201002','y003',11000,'张三','2018-05-01',
'2018-05-04','2018-05-23','作业公司作业二队','检泵',6000,1500,1000,2400,10900,'李四','2018-05-26',10900,'王五','2018-05-28'),

-- zy2018003
('zy2018003','112201002','s001',10500,'张三','2018-05-01',
'2018-05-06','2018-05-23','作业公司作业二队','调剖',6500,2000,500,1400,10400,'李四','2018-05-26',10400,'王五','2018-05-28'),

-- zy2018004
('zy2018004','112202001','s002',12000,'张三','2018-05-01',
'2018-05-04','2018-05-24','作业公司作业三队','解堵',6000,2000,1000,1600,10600,'李四','2018-05-26',10600,'赵六','2018-05-28'),

-- zy2018005
('zy2018005','112202002','y005',12000,'张三','2018-05-01',
'2018-05-04','2018-05-28','作业公司作业三队','防砂',7000,1000,2000,1300,11300,'李四','2018-06-01',11300,'赵六','2018-05-28');
GO

-- 材料费明细
INSERT INTO 材料费表
VALUES
('zy2018001','wm001',200,10),
('zy2018001','wm002',200,10),
('zy2018001','wm003',200,10),
('zy2018001','wm004',100,10),

('zy2018002','wm001',200,10),
('zy2018002','wm002',200,10),
('zy2018002','wm003',200,10),

('zy2018003','wm001',200,10),
('zy2018003','wm002',200,10),
('zy2018003','wm003',250,10),

('zy2018004','wm001',200,10),
('zy2018004','wm002',200,10),
('zy2018004','wm004',200,10),

('zy2018005','wm001',200,10),
('zy2018005','wm002',200,10),
('zy2018005','wm004',300,10);
GO

-- 步骤5：实验要求操作 —— 修改 + 删除 + 撤销

BEGIN TRANSACTION;
GO

-- ⑴ 修改 zy2018004：人工费+200，结算金额+200
UPDATE 作业项目表
SET 人工费 = 人工费 + 200, 结算金额 = 结算金额 + 200
WHERE 单据号 = 'zy2018004';

SELECT 单据号, 人工费, 结算金额 FROM 作业项目表 WHERE 单据号 = 'zy2018004';
GO

-- ⑵ 删除已结算但未入账的数据
DELETE FROM 作业项目表
WHERE 结算金额 > 0 AND 入账金额 IS NULL;

SELECT * FROM 作业项目表;
GO

-- ⑶ 撤销所有操作
ROLLBACK TRANSACTION;
GO

-- 步骤6：查询所有表，验证结果

SELECT * FROM 单位代码表;
SELECT * FROM 油水井表;
SELECT * FROM 施工单位表;
SELECT * FROM 物码表;
SELECT * FROM 作业项目表;
SELECT * FROM 材料费表;
GO