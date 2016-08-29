USE [master]
GO
/****** Object:  Database [SqlSugarTest]    Script Date: 2016/8/29 16:39:23 ******/
CREATE DATABASE [SqlSugarTest] 
GO
ALTER DATABASE [SqlSugarTest] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [SqlSugarTest] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [SqlSugarTest] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [SqlSugarTest] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [SqlSugarTest] SET ARITHABORT OFF 
GO
ALTER DATABASE [SqlSugarTest] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [SqlSugarTest] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [SqlSugarTest] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [SqlSugarTest] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [SqlSugarTest] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [SqlSugarTest] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [SqlSugarTest] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [SqlSugarTest] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [SqlSugarTest] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [SqlSugarTest] SET  ENABLE_BROKER 
GO
ALTER DATABASE [SqlSugarTest] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [SqlSugarTest] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [SqlSugarTest] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [SqlSugarTest] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [SqlSugarTest] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [SqlSugarTest] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [SqlSugarTest] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [SqlSugarTest] SET RECOVERY FULL 
GO
ALTER DATABASE [SqlSugarTest] SET  MULTI_USER 
GO
ALTER DATABASE [SqlSugarTest] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [SqlSugarTest] SET DB_CHAINING OFF 
GO
EXEC sys.sp_db_vardecimal_storage_format N'SqlSugarTest', N'ON'
GO
USE [SqlSugarTest]
GO
/****** Object:  Table [dbo].[Area]    Script Date: 2016/8/29 16:39:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Area](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](50) NULL,
 CONSTRAINT [PK_Area] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GuidTest]    Script Date: 2016/8/29 16:39:23 ******/
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
/****** Object:  Table [dbo].[InsertTest]    Script Date: 2016/8/29 16:39:23 ******/
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
/****** Object:  Table [dbo].[LanguageTest]    Script Date: 2016/8/29 16:39:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[LanguageTest](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LanguageId] [int] NULL,
	[Name] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[School]    Script Date: 2016/8/29 16:39:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[School](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](100) NULL,
	[AreaId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Student]    Script Date: 2016/8/29 16:39:23 ******/
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
/****** Object:  Table [dbo].[Subject]    Script Date: 2016/8/29 16:39:23 ******/
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
/****** Object:  Table [dbo].[TableDataTypeTest]    Script Date: 2016/8/29 16:39:23 ******/
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
/****** Object:  Table [dbo].[TestOfNull]    Script Date: 2016/8/29 16:39:23 ******/
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
/****** Object:  Table [dbo].[TestUpdateColumns]    Script Date: 2016/8/29 16:39:23 ******/
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
/****** Object:  View [dbo].[V_LanguageTest]    Script Date: 2016/8/29 16:39:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [dbo].[V_LanguageTest]
as
select * from LanguageTest where LanguageId=1
GO
/****** Object:  View [dbo].[V_LanguageTest_$_en]    Script Date: 2016/8/29 16:39:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [dbo].[V_LanguageTest_$_en]
as
select * from LanguageTest where LanguageId = 2
GO
SET IDENTITY_INSERT [dbo].[Area] ON 

INSERT [dbo].[Area] ([Id], [name]) VALUES (1, N'上海')
INSERT [dbo].[Area] ([Id], [name]) VALUES (2, N'北京')
INSERT [dbo].[Area] ([Id], [name]) VALUES (3, N'南通')
SET IDENTITY_INSERT [dbo].[Area] OFF
INSERT [dbo].[GuidTest] ([GUID], [Name]) VALUES (N'2cdbb14c-0cb7-4455-8aac-18d52974a6e3', N'test')
INSERT [dbo].[GuidTest] ([GUID], [Name]) VALUES (N'8a166e32-e1c1-4aa7-84bc-18ec9596ebba', N'test')
INSERT [dbo].[GuidTest] ([GUID], [Name]) VALUES (N'504e8170-1423-43be-8bbb-3ced8dfcfad7', N'test')
INSERT [dbo].[GuidTest] ([GUID], [Name]) VALUES (N'9e3d3283-f045-431f-b975-498f79accfeb', N'test')
INSERT [dbo].[GuidTest] ([GUID], [Name]) VALUES (N'9868ec9f-de19-4754-86e9-ecd2772e0fb6', N'test')
SET IDENTITY_INSERT [dbo].[School] ON 

INSERT [dbo].[School] ([id], [name], [AreaId]) VALUES (1, N'北大青鸟', 1)
INSERT [dbo].[School] ([id], [name], [AreaId]) VALUES (2, N'IT清华', 2)
INSERT [dbo].[School] ([id], [name], [AreaId]) VALUES (3, N'哈哈哈', 3)
INSERT [dbo].[School] ([id], [name], [AreaId]) VALUES (4, N'全智', 1)
INSERT [dbo].[School] ([id], [name], [AreaId]) VALUES (5, N'蓝翔1', 2)
INSERT [dbo].[School] ([id], [name], [AreaId]) VALUES (6, N'蓝翔2', 3)
INSERT [dbo].[School] ([id], [name], [AreaId]) VALUES (7, N'蓝翔3', 1)
INSERT [dbo].[School] ([id], [name], [AreaId]) VALUES (8, N'蓝翔4', 1)
INSERT [dbo].[School] ([id], [name], [AreaId]) VALUES (9, N'蓝翔技校', 2)
INSERT [dbo].[School] ([id], [name], [AreaId]) VALUES (11, N'蓝翔', 2)
INSERT [dbo].[School] ([id], [name], [AreaId]) VALUES (12, N'蓝翔2', 1)
INSERT [dbo].[School] ([id], [name], [AreaId]) VALUES (13, N'58c11-2016-07-25', 3)
INSERT [dbo].[School] ([id], [name], [AreaId]) VALUES (14, N'7c11-2016-07-25', NULL)
INSERT [dbo].[School] ([id], [name], [AreaId]) VALUES (15, N'ch-102016-07-25', 3)
INSERT [dbo].[School] ([id], [name], [AreaId]) VALUES (16, N'ch-422016-07-25', NULL)
INSERT [dbo].[School] ([id], [name], [AreaId]) VALUES (1001, NULL, 1)
SET IDENTITY_INSERT [dbo].[School] OFF
SET IDENTITY_INSERT [dbo].[Student] ON 

INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1, N'小红', 1, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2, N'李四', 2, N'0         ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (3, N'小明', 1, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (4, N'7上u25-2016-07-25', 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (5, N'stud-2016-07-25', 3, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (6, N'stud-2016-07-25', 4, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1001, NULL, 1, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1002, NULL, 1, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1003, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1004, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1005, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1006, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1007, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1008, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1009, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1010, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1011, N'张804437276', 0, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1012, N'张1036321559', 0, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1013, N'张1036321559', 0, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1014, N'张1532870660', 0, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1015, N'张1172627556', 0, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1016, N'张566199765', 0, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1017, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1018, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1019, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1020, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1021, N'张809520923', NULL, NULL, NULL)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1022, N'张1804842201', NULL, NULL, NULL)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1023, N'张1453470310', NULL, NULL, NULL)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1024, N'张1956336091', 0, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1025, N'张40736727', 0, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1026, N'张40736727', 0, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1027, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1028, N'张三', 1, N'男        ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1029, N'sun', 1, N'女        ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1030, N'mama', 1, N'gril      ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1031, N'张三', 1, N'男        ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1032, N'sun', 1, N'女        ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1033, N'mama', 1, N'gril      ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1034, N'张三', 1, N'男        ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1035, N'sun', 1, N'女        ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1036, N'mama', 1, N'gril      ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1037, N'张三', 1, N'男        ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1038, N'sun', 1, N'女        ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1039, N'mama', 1, N'gril      ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1040, N'张三', 1, N'男        ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1041, N'sun', 1, N'女        ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1042, N'mama', 1, N'gril      ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1043, N'张三', 1, N'男        ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1044, N'sun', 1, N'女        ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1045, N'mama', 1, N'gril      ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1046, N'张三', 1, N'男        ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1047, N'sun', 1, N'女        ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1048, N'mama', 1, N'gril      ', 1)
SET IDENTITY_INSERT [dbo].[Student] OFF
INSERT [dbo].[TableDataTypeTest] ([Id], [name]) VALUES (1, N'h         ')
/****** Object:  StoredProcedure [dbo].[sp_school]    Script Date: 2016/8/29 16:39:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[sp_school]
@p1 int,
@p2 int
as
select * from  School

GO
USE [master]
GO
ALTER DATABASE [SqlSugarTest] SET  READ_WRITE 
GO
