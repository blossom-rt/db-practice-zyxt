USE master;
GO
-- 1.创建登录名
IF SUSER_ID(N'login_user11') IS NULL
CREATE LOGIN login_user11
WITH PASSWORD = 'User11_Str0ng_2026!',
CHECK_POLICY = ON,
CHECK_EXPIRATION = OFF;
GO

IF SUSER_ID(N'login_user12') IS NULL
CREATE LOGIN login_user12
WITH PASSWORD = 'User12_Str0ng_2026!', CHECK_POLICY = ON;
GO

USE zyxt;
GO
-- 2.创建数据库用户
IF USER_ID(N'user11') IS NULL
CREATE USER user11 FOR LOGIN login_user11
WITH DEFAULT_SCHEMA = dbo;
GO

IF USER_ID(N'user12') IS NULL
CREATE USER user12 FOR LOGIN login_user12;
GO

IF USER_ID(N'user_schema_owner') IS NULL
CREATE USER user_schema_owner FOR LOGIN login_user11;
GO

-- 3.架构操作
CREATE SCHEMA scourse AUTHORIZATION user_schema_owner;
GO
CREATE TABLE scourse.TEST01
(
    SCID CHAR(8) NOT NULL,
    SCNAME NVARCHAR(30) NOT NULL,
    SP CHAR(4) NULL
);
GO

-- 4.视图实现数据安全隔离
CREATE OR ALTER VIEW dbo.v_采油一矿作业项目
AS
SELECT 单据号 AS 作业项目编号,预算单位 AS 单位编号,施工内容 AS 项目名称,开工日期,完工日期
FROM dbo.作业项目表
WHERE 预算单位 = N'112201002';
GO
-- 分配视图权限，禁止访问基表
GRANT SELECT ON OBJECT::dbo.v_采油一矿作业项目 TO user11;
DENY SELECT ON OBJECT::dbo.作业项目表 TO user11;
GO
-- 视图权限验证
EXECUTE AS USER = 'user11';
SELECT * FROM dbo.v_采油一矿作业项目;
-- 下面这条会报错，无基表权限
SELECT * FROM dbo.作业项目表;
REVERT;
GO

-- 5.存储过程作为受控操作入口
CREATE OR ALTER PROCEDURE dbo.p_单位成本运行情况
    @单位编号 NVARCHAR(50),
    @开始日期 DATE,
    @结束日期 DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 预算单位,单据号 AS 作业项目编号,施工内容 AS 项目名称,材料费,人工费,材料费+人工费+设备费+其它费用 AS 总成本
    FROM dbo.作业项目表
    WHERE 预算单位 = @单位编号
      AND 开工日期 >= @开始日期
      AND 开工日期 < DATEADD(DAY, 1, @结束日期)
    ORDER BY 开工日期,单据号;
END;
GO
-- 授权前执行（报错）
EXECUTE AS USER = 'user12';
EXEC dbo.p_单位成本运行情况
    @单位编号 = N'112201002',
    @开始日期 = '2026-01-01',
    @结束日期 = '2026-12-31';
REVERT;
GO
-- 授予执行权限
GRANT EXECUTE ON OBJECT::dbo.p_单位成本运行情况 TO user12;
GO
-- 授权后正常执行
EXECUTE AS USER = 'user12';
EXEC dbo.p_单位成本运行情况
    @单位编号 = N'112201002',
    @开始日期 = '2026-01-01',
    @结束日期 = '2026-12-31';
REVERT;
GO

-- 6.角色批量管理权限
IF DATABASE_PRINCIPAL_ID(N'role_采油一矿查询员') IS NULL
CREATE ROLE role_采油一矿查询员;
GO
ALTER ROLE role_采油一矿查询员 ADD MEMBER user11;
ALTER ROLE role_采油一矿查询员 ADD MEMBER user12;
GO
GRANT SELECT ON OBJECT::dbo.v_采油一矿作业项目 TO role_采油一矿查询员;
DENY SELECT ON OBJECT::dbo.作业项目表 TO role_采油一矿查询员;
GO
-- 撤销权限
REVOKE SELECT ON OBJECT::dbo.v_采油一矿作业项目 FROM role_采油一矿查询员;
GO

-- 7.安全触发器：限制更新时间
CREATE OR ALTER TRIGGER dbo.tr_作业项目_限制更新时间
ON dbo.作业项目表
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @当前时间 TIME(0) = CONVERT(TIME(0), SYSDATETIME());
    IF @当前时间 < '08:00' OR @当前时间 > '18:00'
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50001, N'作业项目表只能在工作时间 08:00—18:00 内更新。', 1;
    END;
END;
GO

-- 8.清理脚本（实验结束执行）
DROP TRIGGER IF EXISTS dbo.tr_作业项目_限制更新时间;
DROP PROCEDURE IF EXISTS dbo.p_单位成本运行情况;
DROP VIEW IF EXISTS dbo.v_采油一矿作业项目;
DROP TABLE IF EXISTS scourse.TEST01;
DROP SCHEMA IF EXISTS scourse;
GO
IF DATABASE_PRINCIPAL_ID(N'role_采油一矿查询员') IS NOT NULL
DROP ROLE role_采油一矿查询员;
GO
DROP USER IF EXISTS user11;
DROP USER IF EXISTS user12;
DROP USER IF EXISTS user_schema_owner;
GO
USE master;
GO
DROP LOGIN IF EXISTS login_user11;
DROP LOGIN IF EXISTS login_user12;
GO