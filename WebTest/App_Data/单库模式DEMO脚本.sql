USE [master]
GO
/****** Object:  Database [SqlSugarTest]    Script Date: 2016/7/9 0:34:26 ******/
CREATE DATABASE [SqlSugarTest]  
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [SqlSugarTest].[dbo].[sp_fulltext_database] @action = 'enable'
end
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
/****** Object:  Table [dbo].[GuidTest]    Script Date: 2016/7/9 0:34:26 ******/
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
/****** Object:  Table [dbo].[InsertTest]    Script Date: 2016/7/9 0:34:26 ******/
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
/****** Object:  Table [dbo].[School]    Script Date: 2016/7/9 0:34:26 ******/
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
/****** Object:  Table [dbo].[Student]    Script Date: 2016/7/9 0:34:26 ******/
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
/****** Object:  Table [dbo].[Subject]    Script Date: 2016/7/9 0:34:26 ******/
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
/****** Object:  Table [dbo].[TableDataTypeTest]    Script Date: 2016/7/9 0:34:26 ******/
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
/****** Object:  Table [dbo].[TestOfNull]    Script Date: 2016/7/9 0:34:26 ******/
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
/****** Object:  Table [dbo].[TestUpdateColumns]    Script Date: 2016/7/9 0:34:26 ******/
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
INSERT [dbo].[GuidTest] ([GUID], [Name]) VALUES (N'2cdbb14c-0cb7-4455-8aac-18d52974a6e3', N'test')
INSERT [dbo].[GuidTest] ([GUID], [Name]) VALUES (N'8a166e32-e1c1-4aa7-84bc-18ec9596ebba', N'test')
INSERT [dbo].[GuidTest] ([GUID], [Name]) VALUES (N'504e8170-1423-43be-8bbb-3ced8dfcfad7', N'test')
INSERT [dbo].[GuidTest] ([GUID], [Name]) VALUES (N'9e3d3283-f045-431f-b975-498f79accfeb', N'test')
INSERT [dbo].[GuidTest] ([GUID], [Name]) VALUES (N'9868ec9f-de19-4754-86e9-ecd2772e0fb6', N'test')
SET IDENTITY_INSERT [dbo].[School] ON 

INSERT [dbo].[School] ([id], [name]) VALUES (1, N'À¶Ïè1')
INSERT [dbo].[School] ([id], [name]) VALUES (2, N'À¶Ïè2')
INSERT [dbo].[School] ([id], [name]) VALUES (3, N'À¶Ïè3')
INSERT [dbo].[School] ([id], [name]) VALUES (4, N'À¶Ïè4')
INSERT [dbo].[School] ([id], [name]) VALUES (5, N'À¶Ïè')
INSERT [dbo].[School] ([id], [name]) VALUES (6, N'À¶Ïè')
INSERT [dbo].[School] ([id], [name]) VALUES (7, N'À¶Ïè')
INSERT [dbo].[School] ([id], [name]) VALUES (8, N'À¶Ïè')
INSERT [dbo].[School] ([id], [name]) VALUES (9, N'À¶Ïè')
INSERT [dbo].[School] ([id], [name]) VALUES (11, N'À¶Ïè')
INSERT [dbo].[School] ([id], [name]) VALUES (12, N'À¶Ïè2')
SET IDENTITY_INSERT [dbo].[School] OFF
SET IDENTITY_INSERT [dbo].[Student] ON 

INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1, N'ÕÅÈý', 1, N'1         ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2, N'ÀîËÄ', 2, N'0         ', 1)
SET IDENTITY_INSERT [dbo].[Student] OFF
INSERT [dbo].[TableDataTypeTest] ([Id], [name]) VALUES (1, N'h         ')
/****** Object:  StoredProcedure [dbo].[sp_school]    Script Date: 2016/7/9 0:34:26 ******/
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

Create Table LanguageTest(
 Id int primary key identity(1,1),
 LanguageId int ,
 Name varchar(100)
)
go

create view V_LanguageTest
as
select * from LanguageTest where LanguageId=1

go