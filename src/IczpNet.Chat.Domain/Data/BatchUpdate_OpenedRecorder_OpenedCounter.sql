/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [Id]
      ,[SessionKey]
      ,[Channel]
      ,[Title]
      ,[Description]
      ,[OwnerId]
      ,[LastMessageId]
      ,[ExtraProperties]
      ,[ConcurrencyStamp]
      ,[CreationTime]
      ,[CreatorId]
      ,[LastModificationTime]
      ,[LastModifierId]
      ,[IsDeleted]
      ,[DeleterId]
      ,[DeletionTime]
      ,[TenantId]
  FROM [Chat_Module_v3].[dbo].[Chat_Session]

  where SessionKey='6019'