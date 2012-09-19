
alter table syncEvents
alter column EventType nvarchar(50) null

Go

/****** Object:  Index [IX_SessionContext_Id]    Script Date: 09/19/2012 12:17:52 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SyncEvents]') AND name = N'IX_SessionContext_Id')
DROP INDEX [IX_SessionContext_Id] ON [dbo].[SyncEvents] WITH ( ONLINE = OFF )
GO


/****** Object:  Index [IX_SessionContext_Id]    Script Date: 09/19/2012 12:17:52 ******/
CREATE NONCLUSTERED INDEX [IX_SessionContext_Id] ON [dbo].[SyncEvents] 
(
	[SessionContext_Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO



/****** Object:  Index [IX_SessionContextId_EventType]    Script Date: 09/19/2012 13:57:16 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SyncEvents]') AND name = N'IX_SessionContextId_EventType')
DROP INDEX [IX_SessionContextId_EventType] ON [dbo].[SyncEvents] WITH ( ONLINE = OFF )
GO


/****** Object:  Index [IX_SessionContextId_EventType]    Script Date: 09/19/2012 13:57:16 ******/
CREATE NONCLUSTERED INDEX [IX_SessionContextId_EventType] ON [dbo].[SyncEvents] 
(
	[SessionContext_Id] ASC,
	[EventType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO



/****** Object:  Index [IX_eventtype_syncevents]    Script Date: 09/19/2012 14:18:47 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SyncEvents]') AND name = N'IX_eventtype_syncevents')
DROP INDEX [IX_eventtype_syncevents] ON [dbo].[SyncEvents] WITH ( ONLINE = OFF )
GO



/****** Object:  Index [IX_eventtype_syncevents]    Script Date: 09/19/2012 14:18:47 ******/
CREATE NONCLUSTERED INDEX [IX_eventtype_syncevents] ON [dbo].[SyncEvents] 
(
	[EventType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO





