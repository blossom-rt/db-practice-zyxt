USE zyxt;
GO

-- 1 事务处理：批量插入数据，异常回滚，正常提交
BEGIN TRY
    BEGIN TRANSACTION; -- 开启事务

    -- 五条插入语句
    insert into 作业项目表 values('zy2018006','112202002','y005',
    10000,'张三', '07-01-2018' ,'07-04-2018','07-25-2018',
    '作业公司作业一队','堵漏',7000,2500,1000,1400,11900,
    '李四','07-26-2018',11900,'王五','07-28-2018');

    insert into 材料费表 values('zy2018006','wm001',200,10);
    insert into 材料费表 values('zy2018006','wm002',200,10);
    insert into 材料费表 values('zy2018006','wm003',200,10);
    insert into 材料费表 values('zy2018006','wm004',100,10);

    COMMIT TRANSACTION; -- 全部成功，提交事务
    PRINT '事务执行成功：所有数据已正常插入';
END TRY
BEGIN CATCH
    -- 出现异常，回滚事务
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    -- 输出错误信息
    PRINT '事务执行失败，事务已回滚';
    PRINT '错误编号：' + CAST(ERROR_NUMBER() AS VARCHAR(20));
    PRINT '错误描述：' + ERROR_MESSAGE();
    PRINT '错误行号：' + CAST(ERROR_LINE() AS VARCHAR(20));
END CATCH;
GO