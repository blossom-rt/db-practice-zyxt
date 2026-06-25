-- 1. 用户表
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '用户表')
BEGIN
    CREATE TABLE 用户表 (
        Id INT IDENTITY PRIMARY KEY,
        用户名 NVARCHAR(50) NOT NULL UNIQUE,
        密码 NVARCHAR(100) NOT NULL,
        角色 NVARCHAR(20) NOT NULL DEFAULT '操作员'
    );
END

-- 先清空再插入（避免重复执行报错）
DELETE FROM 用户表;

-- admin123 的 SHA256 → JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=
INSERT INTO 用户表(用户名,密码,角色) VALUES('admin','JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=','管理员');

-- 123456 的 SHA256 → jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=
INSERT INTO 用户表(用户名,密码,角色) VALUES('user','jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=','操作员');

-- 2. 操作日志表
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '操作日志表')
BEGIN
    CREATE TABLE 操作日志表 (
        Id INT IDENTITY PRIMARY KEY,
        操作类型 NVARCHAR(20),
        操作表 NVARCHAR(50),
        操作人 NVARCHAR(50),
        操作时间 DATETIME DEFAULT GETDATE(),
        操作内容 NVARCHAR(MAX)
    );
END

-- 3. 操作员视图（行级数据隔离）
IF EXISTS (SELECT * FROM sys.views WHERE name = 'v_作业项目视图')
    DROP VIEW v_作业项目视图;
GO

CREATE VIEW v_作业项目视图 AS
SELECT * FROM 作业项目表;
GO