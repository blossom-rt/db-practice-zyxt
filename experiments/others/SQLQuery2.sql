-- 1. 作业项目表
CREATE OR ALTER TRIGGER trg_作业项目_日志 ON 作业项目表
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @type NVARCHAR(20), @content NVARCHAR(MAX) = '';

    IF EXISTS(SELECT * FROM inserted) AND EXISTS(SELECT * FROM deleted)
        SET @type = 'UPDATE';
    ELSE IF EXISTS(SELECT * FROM inserted)
        SET @type = 'INSERT';
    ELSE
        SET @type = 'DELETE';

    SELECT @content = STRING_AGG(单据号, ',') FROM
        (SELECT 单据号 FROM inserted UNION SELECT 单据号 FROM deleted) t;

    INSERT INTO 操作日志表(操作类型,操作表,操作人,操作内容)
    VALUES(@type, '作业项目表', SYSTEM_USER, @content);
END;
GO

-- 2. 单位代码表
CREATE OR ALTER TRIGGER trg_单位代码_日志 ON 单位代码表
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @type NVARCHAR(20) = 'INSERT';

    IF EXISTS(SELECT * FROM inserted) AND EXISTS(SELECT * FROM deleted)
        SET @type = 'UPDATE';
    ELSE IF NOT EXISTS(SELECT * FROM inserted)
        SET @type = 'DELETE';

    INSERT INTO 操作日志表(操作类型,操作表,操作人,操作内容)
    VALUES(@type, '单位代码表', SYSTEM_USER, '');
END;
GO

-- 3. 油水井表
CREATE OR ALTER TRIGGER trg_油水井_日志 ON 油水井表
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @type NVARCHAR(20) = 'INSERT';

    IF EXISTS(SELECT * FROM inserted) AND EXISTS(SELECT * FROM deleted)
        SET @type = 'UPDATE';
    ELSE IF NOT EXISTS(SELECT * FROM inserted)
        SET @type = 'DELETE';

    INSERT INTO 操作日志表(操作类型,操作表,操作人,操作内容)
    VALUES(@type, '油水井表', SYSTEM_USER, '');
END;
GO

-- 4. 施工单位表
CREATE OR ALTER TRIGGER trg_施工单位_日志 ON 施工单位表
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @type NVARCHAR(20) = 'INSERT';

    IF EXISTS(SELECT * FROM inserted) AND EXISTS(SELECT * FROM deleted)
        SET @type = 'UPDATE';
    ELSE IF NOT EXISTS(SELECT * FROM inserted)
        SET @type = 'DELETE';

    INSERT INTO 操作日志表(操作类型,操作表,操作人,操作内容)
    VALUES(@type, '施工单位表', SYSTEM_USER, '');
END;
GO

-- 5. 物码表
CREATE OR ALTER TRIGGER trg_物码_日志 ON 物码表
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @type NVARCHAR(20) = 'INSERT';

    IF EXISTS(SELECT * FROM inserted) AND EXISTS(SELECT * FROM deleted)
        SET @type = 'UPDATE';
    ELSE IF NOT EXISTS(SELECT * FROM inserted)
        SET @type = 'DELETE';

    INSERT INTO 操作日志表(操作类型,操作表,操作人,操作内容)
    VALUES(@type, '物码表', SYSTEM_USER, '');
END;
GO

-- 6. 材料费表
CREATE OR ALTER TRIGGER trg_材料费_日志 ON 材料费表
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @type NVARCHAR(20) = 'INSERT';

    IF EXISTS(SELECT * FROM inserted) AND EXISTS(SELECT * FROM deleted)
        SET @type = 'UPDATE';
    ELSE IF NOT EXISTS(SELECT * FROM inserted)
        SET @type = 'DELETE';

    INSERT INTO 操作日志表(操作类型,操作表,操作人,操作内容)
    VALUES(@type, '材料费表', SYSTEM_USER, '');
END;
GO
