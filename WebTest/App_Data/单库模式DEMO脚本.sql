USE [master]
GO
/****** Object:  Database [SqlSugarTest]    Script Date: 2016/6/16 11:18:40 ******/
CREATE DATABASE [SqlSugarTest]  
GO
USE [SqlSugarTest]
GO
/****** Object:  Table [dbo].[GuidTest]    Script Date: 2016/6/16 11:18:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GuidTest](
	[GUID] [uniqueidentifier] NOT NULL,
	[Name] [varchar](50) NULL,
 CONSTRAINT [PK_GuidTest] PRIMARY KEY CLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[InsertTest]    Script Date: 2016/6/16 11:18:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[InsertTest](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[v1] [varchar](50) NULL,
	[v2] [varchar](50) NULL,
	[v3] [varchar](50) NULL,
	[int1] [int] NULL,
	[d1] [datetime] NULL,
	[txt] [text] NULL,
 CONSTRAINT [PK_InsertTest] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[School]    Script Date: 2016/6/16 11:18:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[School](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Student]    Script Date: 2016/6/16 11:18:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Student](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](100) NULL,
	[sch_id] [int] NULL,
	[sex] [char](10) NULL,
	[isOk] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Subject]    Script Date: 2016/6/16 11:18:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Subject](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[sid] [int] NULL,
	[name] [varchar](150) NULL,
 CONSTRAINT [PK_Subject] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TableDataTypeTest]    Script Date: 2016/6/16 11:18:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TableDataTypeTest](
	[Id] [tinyint] NOT NULL,
	[name] [nchar](10) NULL,
 CONSTRAINT [PK_TableDataTypeTest] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TestOfNull]    Script Date: 2016/6/16 11:18:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TestOfNull](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[createDate] [datetime] NULL,
	[bytes] [binary](250) NULL,
 CONSTRAINT [PK__TestOfNu__3213E83F07773285] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
INSERT [dbo].[GuidTest] ([GUID], [Name]) VALUES (N'2cdbb14c-0cb7-4455-8aac-18d52974a6e3', N'test')
INSERT [dbo].[GuidTest] ([GUID], [Name]) VALUES (N'8a166e32-e1c1-4aa7-84bc-18ec9596ebba', N'test')
INSERT [dbo].[GuidTest] ([GUID], [Name]) VALUES (N'504e8170-1423-43be-8bbb-3ced8dfcfad7', N'test')
INSERT [dbo].[GuidTest] ([GUID], [Name]) VALUES (N'9e3d3283-f045-431f-b975-498f79accfeb', N'test')
INSERT [dbo].[GuidTest] ([GUID], [Name]) VALUES (N'9868ec9f-de19-4754-86e9-ecd2772e0fb6', N'test')
SET IDENTITY_INSERT [dbo].[School] ON 

INSERT [dbo].[School] ([id], [name]) VALUES (1, N'À¶Ïè2')
INSERT [dbo].[School] ([id], [name]) VALUES (2, N'À¶Ïè2')
INSERT [dbo].[School] ([id], [name]) VALUES (3, N'À¶Ïè2')
INSERT [dbo].[School] ([id], [name]) VALUES (4, N'À¶Ïè')
INSERT [dbo].[School] ([id], [name]) VALUES (5, N'À¶Ïè')
INSERT [dbo].[School] ([id], [name]) VALUES (6, N'À¶Ïè')
INSERT [dbo].[School] ([id], [name]) VALUES (7, N'À¶Ïè')
INSERT [dbo].[School] ([id], [name]) VALUES (8, N'À¶Ïè')
INSERT [dbo].[School] ([id], [name]) VALUES (9, N'À¶Ïè')
INSERT [dbo].[School] ([id], [name]) VALUES (11, N'À¶Ïè')
INSERT [dbo].[School] ([id], [name]) VALUES (12, N'À¶Ïè2')
SET IDENTITY_INSERT [dbo].[School] OFF
SET IDENTITY_INSERT [dbo].[Student] ON 

INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (3, N'1', 1, N'1         ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (4, N'1', 1, N'1         ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (5, NULL, 1, NULL, NULL)
SET IDENTITY_INSERT [dbo].[Student] OFF
INSERT [dbo].[TableDataTypeTest] ([Id], [name]) VALUES (1, N'h         ')
USE [master]
GO
ALTER DATABASE [SqlSugarTest] SET  READ_WRITE 
GO

USE [SqlSugarTest]
GO

/****** Object:  Table [dbo].[TestUpdateColumns]    Script Date: 2016/6/23 11:29:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[TestUpdateColumns](
	[VGUID] [uniqueidentifier] NOT NULL,
	[IdentityField] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NULL,
	[Name2] [varchar](50) NULL,
	[CreateTime] [datetime] NULL,
 CONSTRAINT [PK_TestUpdateColumns] PRIMARY KEY CLUSTERED 
(
	[VGUID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


