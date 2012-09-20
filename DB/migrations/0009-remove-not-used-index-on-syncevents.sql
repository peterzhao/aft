
/****** Object:  Index [IX_SessionContextId_EventType]    Script Date: 09/19/2012 13:57:16 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SyncEvents]') AND name = N'IX_SessionContextId_EventType')
DROP INDEX [IX_SessionContextId_EventType] ON [dbo].[SyncEvents] WITH ( ONLINE = OFF )
GO

/****** Object:  Index [IX_eventtype_syncevents]    Script Date: 09/19/2012 14:18:47 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SyncEvents]') AND name = N'IX_eventtype_syncevents')
DROP INDEX [IX_eventtype_syncevents] ON [dbo].[SyncEvents] WITH ( ONLINE = OFF )
GO
