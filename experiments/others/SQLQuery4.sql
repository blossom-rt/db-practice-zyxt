-- 1. 删除旧触发器（如果已存在综合触发器则先清理）
DROP TRIGGER IF EXISTS [dbo].[Tri_Job_Insert];
DROP TRIGGER IF EXISTS [dbo].[Tri_Job_Update];
DROP TRIGGER IF EXISTS [dbo].[Tri_Job_Delete];
GO

-- 2. 单位代码表
CREATE OR ALTER TRIGGER [dbo].[trg_单位代码_日志] ON [dbo].[单位代码表]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @usr NVARCHAR(100);
    SET @usr = CAST(SESSION_CONTEXT(N'Username') AS NVARCHAR(100));
    IF @usr IS NULL SET @usr = SYSTEM_USER;

    IF EXISTS(SELECT 1 FROM INSERTED) AND EXISTS(SELECT 1 FROM DELETED)
    BEGIN
        INSERT INTO 操作日志表(操作类型,操作表,操作人,操作内容)
        SELECT 'UPDATE', '单位代码表', @usr,
            N'修改单位代码 ' + i.单位代码 + N'：' + d.单位名称 + N' → ' + i.单位名称
        FROM INSERTED i JOIN DELETED d ON i.单位代码 = d.单位代码;
    END
    ELSE IF EXISTS(SELECT 1 FROM INSERTED)
    BEGIN
        INSERT INTO 操作日志表(操作类型,操作表,操作人,操作内容)
        SELECT 'INSERT', '单位代码表', @usr, N'新增单位代码 ' + 单位代码 + N' - ' + 单位名称
        FROM INSERTED;
    END
    ELSE
    BEGIN
        INSERT INTO 操作日志表(操作类型,操作表,操作人,操作内容)
        SELECT 'DELETE', '单位代码表', @usr, N'删除单位代码 ' + 单位代码
        FROM DELETED;
    END
END;
GO

-- 3. 油水井表
CREATE OR ALTER TRIGGER [dbo].[trg_油水井_日志] ON [dbo].[油水井表]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @usr NVARCHAR(100);
    SET @usr = CAST(SESSION_CONTEXT(N'Username') AS NVARCHAR(100));
    IF @usr IS NULL SET @usr = SYSTEM_USER;

    IF EXISTS(SELECT 1 FROM INSERTED) AND EXISTS(SELECT 1 FROM DELETED)
    BEGIN
        INSERT INTO 操作日志表(操作类型,操作表,操作人,操作内容)
        SELECT 'UPDATE', '油水井表', @usr,
            N'修改油水井 ' + i.井号 + N' 井别：' + ISNULL(d.井别, '') + N' → ' + ISNULL(i.井别, '')
        FROM INSERTED i JOIN DELETED d ON i.井号 = d.井号;
    END
    ELSE IF EXISTS(SELECT 1 FROM INSERTED)
    BEGIN
        INSERT INTO 操作日志表(操作类型,操作表,操作人,操作内容)
        SELECT 'INSERT', '油水井表', @usr, N'新增油水井 ' + 井号 + ISNULL(N' 井别：' + 井别, '')
        FROM INSERTED;
    END
    ELSE
    BEGIN
        INSERT INTO 操作日志表(操作类型,操作表,操作人,操作内容)
        SELECT 'DELETE', '油水井表', @usr, N'删除油水井 ' + 井号
        FROM DELETED;
    END
END;
GO

-- 4. 施工单位表
CREATE OR ALTER TRIGGER [dbo].[trg_施工单位_日志] ON [dbo].[施工单位表]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @usr NVARCHAR(100);
    SET @usr = CAST(SESSION_CONTEXT(N'Username') AS NVARCHAR(100));
    IF @usr IS NULL SET @usr = SYSTEM_USER;

    IF EXISTS(SELECT 1 FROM INSERTED) AND EXISTS(SELECT 1 FROM DELETED)
    BEGIN
        INSERT INTO 操作日志表(操作类型,操作表,操作人,操作内容)
        SELECT 'UPDATE', '施工单位表', @usr,
            N'修改施工单位 ' + i.施工单位名称
        FROM INSERTED i JOIN DELETED d ON i.施工单位名称 = d.施工单位名称;
    END
    ELSE IF EXISTS(SELECT 1 FROM INSERTED)
    BEGIN
        INSERT INTO 操作日志表(操作类型,操作表,操作人,操作内容)
        SELECT 'INSERT', '施工单位表', @usr, N'新增施工单位 ' + 施工单位名称
        FROM INSERTED;
    END
    ELSE
    BEGIN
        INSERT INTO 操作日志表(操作类型,操作表,操作人,操作内容)
        SELECT 'DELETE', '施工单位表', @usr, N'删除施工单位 ' + 施工单位名称
        FROM DELETED;
    END
END;
GO

-- 5. 物码表
CREATE OR ALTER TRIGGER [dbo].[trg_物码_日志] ON [dbo].[物码表]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @usr NVARCHAR(100);
    SET @usr = CAST(SESSION_CONTEXT(N'Username') AS NVARCHAR(100));
    IF @usr IS NULL SET @usr = SYSTEM_USER;

    IF EXISTS(SELECT 1 FROM INSERTED) AND EXISTS(SELECT 1 FROM DELETED)
    BEGIN
        INSERT INTO 操作日志表(操作类型,操作表,操作人,操作内容)
        SELECT 'UPDATE', '物码表', @usr,
            N'修改物料 ' + i.物码 + ISNULL(N' ' + d.名称规格 + N'→' + i.名称规格, '')
        FROM INSERTED i JOIN DELETED d ON i.物码 = d.物码;
    END
    ELSE IF EXISTS(SELECT 1 FROM INSERTED)
    BEGIN
        INSERT INTO 操作日志表(操作类型,操作表,操作人,操作内容)
        SELECT 'INSERT', '物码表', @usr,
            N'新增物料 ' + 物码 + ISNULL(N' ' + 名称规格, '')
        FROM INSERTED;
    END
    ELSE
    BEGIN
        INSERT INTO 操作日志表(操作类型,操作表,操作人,操作内容)
        SELECT 'DELETE', '物码表', @usr, N'删除物料 ' + 物码
        FROM DELETED;
    END
END;
GO

-- 6. 材料费表
CREATE OR ALTER TRIGGER [dbo].[trg_材料费_日志] ON [dbo].[材料费表]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @usr NVARCHAR(100);
    SET @usr = CAST(SESSION_CONTEXT(N'Username') AS NVARCHAR(100));
    IF @usr IS NULL SET @usr = SYSTEM_USER;

    IF EXISTS(SELECT 1 FROM INSERTED) AND EXISTS(SELECT 1 FROM DELETED)
    BEGIN
        INSERT INTO 操作日志表(操作类型,操作表,操作人,操作内容)
        SELECT 'UPDATE', '材料费表', @usr,
            N'修改材料消耗 单据号：' + i.单据号 + N' 物码：' + i.物码
        FROM INSERTED i JOIN DELETED d ON i.单据号 = d.单据号 AND i.物码 = d.物码;
    END
    ELSE IF EXISTS(SELECT 1 FROM INSERTED)
    BEGIN
        INSERT INTO 操作日志表(操作类型,操作表,操作人,操作内容)
        SELECT 'INSERT', '材料费表', @usr,
            N'新增材料消耗 单据号：' + 单据号 + N' 物码：' + 物码 + N' 数量：' + CAST(消耗数量 AS NVARCHAR)
        FROM INSERTED;
    END
    ELSE
    BEGIN
        INSERT INTO 操作日志表(操作类型,操作表,操作人,操作内容)
        SELECT 'DELETE', '材料费表', @usr,
            N'删除材料消耗 单据号：' + 单据号 + N' 物码：' + 物码
        FROM DELETED;
    END
END;
GO

-- 7. 作业项目表（综合触发器，替代老的 Tri_Job_*）
CREATE OR ALTER TRIGGER [dbo].[trg_作业项目_日志] ON [dbo].[作业项目表]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @usr NVARCHAR(100);
    SET @usr = CAST(SESSION_CONTEXT(N'Username') AS NVARCHAR(100));
    IF @usr IS NULL SET @usr = SYSTEM_USER;

    IF EXISTS(SELECT 1 FROM INSERTED) AND EXISTS(SELECT 1 FROM DELETED)
    BEGIN
        INSERT INTO 操作日志表(操作类型,操作表,操作人,操作内容)
        SELECT 'UPDATE', '作业项目表', @usr,
            N'修改作业项目 单据号：' + i.单据号 + N' 井号：' + i.井号
        FROM INSERTED i JOIN DELETED d ON i.单据号 = d.单据号;
    END
    ELSE IF EXISTS(SELECT 1 FROM INSERTED)
    BEGIN
        INSERT INTO 操作日志表(操作类型,操作表,操作人,操作内容)
        SELECT 'INSERT', '作业项目表', @usr,
            N'新增作业项目 单据号：' + 单据号 + N' 井号：' + 井号 + ISNULL(N' 施工内容：' + 施工内容, '')
        FROM INSERTED;
    END
    ELSE
    BEGIN
        INSERT INTO 操作日志表(操作类型,操作表,操作人,操作内容)
        SELECT 'DELETE', '作业项目表', @usr, N'删除作业项目 单据号：' + 单据号
        FROM DELETED;
    END
END;
GO
