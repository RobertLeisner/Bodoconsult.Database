***********************************************
Instructions for testing
***********************************************


To run the test there is required a database with (atleast) one table dbo.Settings. 
Here is the script to create the table on LocalDb, SqlServer Express or SqlServer:

**** Script start ****

USE [MediaDB]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Settings](
	[S_ID] [uniqueidentifier] NOT NULL,
	[sKey] [varchar](255) NOT NULL,
	[Value] [varchar](max) NULL,
	[Description] [varchar](max) NULL,
 CONSTRAINT [PK_Settings] PRIMARY KEY NONCLUSTERED 
(
	[S_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Settings] ADD  CONSTRAINT [DF_Settings_S_ID]  DEFAULT (newid()) FOR [S_ID]
GO

GO
insert into [dbo].[Settings]([S_ID],[sKey],[Value],[Description])
select 'c3f41d97-26bf-4bf2-b428-756f97a17079', 'Company', 'Testfirma', 'Company''s name'
where not exists (select * from [dbo].[Settings] where [S_ID]='c3f41d97-26bf-4bf2-b428-756f97a17079')
GO
GO
insert into [dbo].[Settings]([S_ID],[sKey],[Value],[Description])
select '981751e1-62da-4141-bdb1-2f8c3133f8f7', 'CompanyAddress', 'Ostring 45A
85591 Vaterstetten', 'Company''s address'
where not exists (select * from [dbo].[Settings] where [S_ID]='981751e1-62da-4141-bdb1-2f8c3133f8f7')

**** Script end ****
