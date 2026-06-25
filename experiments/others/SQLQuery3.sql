-- 1. 删除冗余触发器（每表只保留一个）
DROP TRIGGER IF EXISTS [dbo].[Tri_Job_Insert];
DROP TRIGGER IF EXISTS [dbo].[Tri_Job_Update];
DROP TRIGGER IF EXISTS [dbo].[Tri_Job_Delete];
