/****** Script for SelectTopNRows command from SSMS  ******/

--INSERT INTO [Chat_Module].[dbo].[Chat_ChatObject] 
--      ([Id],[Name],[Code],[CreationTime] )
--SELECT [Id],[Name],[Code],[CreationTime]  from [Organization_Module].[dbo].[Organization_Employee] 



SELECT * FROM [Chat_Module].[dbo].[Chat_Session]
SELECT * FROM [Chat_Module].[dbo].[Chat_SessionUnit]
SELECT * FROM [Chat_Module].[dbo].[Chat_Message]

SELECT * FROM [Chat_Module].[dbo].[Chat_SessionTag]

SELECT * FROM [Chat_Module].[dbo].[Chat_SessionRole]

SELECT * FROM [Chat_Module].[dbo].[Chat_Room]


SELECT *  FROM [Chat_Module].[dbo].[Chat_Session] where channel='PrivateChannel'

update [Chat_Module].[dbo].[Chat_Message] set IsRemindAll = 1 where autoid=561

update [Chat_Module].[dbo].[Chat_SessionUnit] set [ReadedMessageAutoId]=564 where [OwnerId]= '5B6A6100-CA52-8040-09F2-3A07D4A367ED'

update [Chat_Module].[dbo].[Chat_Message] set channel='PrivateChannel'
update [Chat_Module].[dbo].[Chat_Message] set SessionId=null
--delete FROM [Chat_Module].[dbo].[Chat_Session]
--delete from [Chat_Module].[dbo].[Chat_SessionUnit]

update  [Chat_Module].[dbo].[Chat_SessionUnit] set [ReadedMessageAutoId]=300


select * from [dbo].[Chat_Room]


