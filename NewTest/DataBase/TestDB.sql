USE [master]
GO
/****** Object:  Database [SqlSugarTest]    Script Date: 2016/9/7 15:29:39 ******/
CREATE DATABASE [SqlSugarTest] 
go
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
/****** Object:  Table [dbo].[Area]    Script Date: 2016/9/7 15:29:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Area](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](50) NULL,
 CONSTRAINT [PK_Area] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[InsertTest]    Script Date: 2016/9/7 15:29:40 ******/
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
/****** Object:  Table [dbo].[LanguageTest]    Script Date: 2016/9/7 15:29:40 ******/
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
/****** Object:  Table [dbo].[School]    Script Date: 2016/9/7 15:29:40 ******/
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
/****** Object:  Table [dbo].[Student]    Script Date: 2016/9/7 15:29:40 ******/
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
/****** Object:  Table [dbo].[Subject]    Script Date: 2016/9/7 15:29:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Subject](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[studentId] [int] NULL,
	[name] [varchar](150) NULL,
 CONSTRAINT [PK_Subject] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TestUpdateColumns]    Script Date: 2016/9/7 15:29:40 ******/
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
/****** Object:  View [dbo].[V_LanguageTest]    Script Date: 2016/9/7 15:29:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [dbo].[V_LanguageTest]
as
select * from LanguageTest where LanguageId=1

GO
/****** Object:  View [dbo].[V_LanguageTest_$_en]    Script Date: 2016/9/7 15:29:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [dbo].[V_LanguageTest_$_en]
as
select * from LanguageTest where LanguageId = 2

GO
SET IDENTITY_INSERT [dbo].[Area] ON 

INSERT [dbo].[Area] ([id], [name]) VALUES (1, N'上海')
INSERT [dbo].[Area] ([id], [name]) VALUES (2, N'北京')
INSERT [dbo].[Area] ([id], [name]) VALUES (3, N'南通')
SET IDENTITY_INSERT [dbo].[Area] OFF
SET IDENTITY_INSERT [dbo].[School] ON 

INSERT [dbo].[School] ([id], [name], [AreaId]) VALUES (1, N'北大青鸟', 1)
INSERT [dbo].[School] ([id], [name], [AreaId]) VALUES (2, N'ＩＴ清华', 2)
INSERT [dbo].[School] ([id], [name], [AreaId]) VALUES (3, N'全智', 3)
INSERT [dbo].[School] ([id], [name], [AreaId]) VALUES (4, N'新东方学校', 1)
INSERT [dbo].[School] ([id], [name], [AreaId]) VALUES (5, N'蓝翔技效', 2)
INSERT [dbo].[School] ([id], [name], [AreaId]) VALUES (6, N'美术学校', 3)
INSERT [dbo].[School] ([id], [name], [AreaId]) VALUES (7, N'美女学校', 1)
INSERT [dbo].[School] ([id], [name], [AreaId]) VALUES (8, N'画画学校', 1)
INSERT [dbo].[School] ([id], [name], [AreaId]) VALUES (9, N'翔蓝高中', 2)
INSERT [dbo].[School] ([id], [name], [AreaId]) VALUES (11, N'蓝翔2', NULL)
INSERT [dbo].[School] ([id], [name], [AreaId]) VALUES (12, N'蓝翔2', 1)
INSERT [dbo].[School] ([id], [name], [AreaId]) VALUES (13, N'58c11-2016-07-25', 3)
INSERT [dbo].[School] ([id], [name], [AreaId]) VALUES (14, N'7c11-2016-07-25', NULL)
INSERT [dbo].[School] ([id], [name], [AreaId]) VALUES (15, N'ch-102016-07-25', 3)
INSERT [dbo].[School] ([id], [name], [AreaId]) VALUES (16, N'ch-422016-07-25', NULL)
INSERT [dbo].[School] ([id], [name], [AreaId]) VALUES (2002, N'ch-32016-09-07', NULL)
SET IDENTITY_INSERT [dbo].[School] OFF
SET IDENTITY_INSERT [dbo].[Student] ON 

INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1, N'小杰', 3, N'1         ', 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2, N'李四', 2, N'2         ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (3, N'小明', 1, N'2         ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (4, N'小张', 4, N'1         ', 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (5, N'小丽', 5, N'1         ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (6, N'小青', 6, N'2         ', 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1001, N'小智', 1, N'2         ', 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1002, N'小菜', 1, N'2         ', 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1003, N'王八', 2, N'1         ', 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1004, NULL, 2, N'1         ', 0)
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
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1049, N'张1467040646', NULL, NULL, NULL)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1050, N'张1128188879', NULL, NULL, NULL)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1051, N'张1128188879', NULL, NULL, NULL)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1052, N'张三', 1, N'男        ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1053, N'sun', 1, N'女        ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1054, N'mama', 1, N'gril      ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1055, N'张三', 1, N'男        ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1056, N'sun', 1, N'女        ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1057, N'mama', 1, N'gril      ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1058, N'张1877314028', NULL, NULL, NULL)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1059, N'张500358309', NULL, NULL, NULL)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1060, N'张500358309', NULL, NULL, NULL)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1062, N'我是最后一个', 1, N'gril      ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1063, N'张三', 1, N'男        ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1064, N'sun', 1, N'女        ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1065, N'mama', 1, N'gril      ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1066, N'张三', 1, N'男        ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1067, N'sun', 1, N'女        ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1068, N'mama', 1, N'gril      ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1069, N'张三', 1, N'男        ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1070, N'sun', 1, N'女        ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1071, N'mama', 1, N'gril      ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1072, N'张三', NULL, NULL, NULL)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1073, N'张1594225777', 0, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1074, N'张1826110060', 0, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1075, N'张1826110060', 0, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1076, N'张三', 1, N'男        ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1077, N'sun', 1, N'女        ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1078, N'mama', 1, N'gril      ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1079, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1080, N'张1621875820', 0, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1081, N'张1390159650', 0, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1082, N'张1038787759', 0, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1083, N'张三', 1, N'男        ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1084, N'sun', 1, N'女        ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1085, N'mama', 1, N'gril      ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1086, N'stud-2016-09-06', NULL, NULL, NULL)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (1087, N'stud-2016-09-06', NULL, NULL, NULL)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2086, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2087, N'张三', NULL, NULL, NULL)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2088, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2089, N'张595133836', NULL, NULL, NULL)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2090, N'张400771149', NULL, NULL, NULL)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2091, N'张400771149', NULL, NULL, NULL)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2092, N'张462255352', NULL, NULL, NULL)
GO
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2093, N'张2003311208', NULL, NULL, NULL)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2094, N'张2003311208', NULL, NULL, NULL)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2095, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2096, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2097, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2098, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2099, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2100, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2101, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2102, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2103, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2104, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2105, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2106, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2107, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2108, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2109, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2110, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2111, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2112, N'张944476819', NULL, NULL, NULL)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2113, N'张2134160784', NULL, NULL, NULL)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2114, N'张2134160784', NULL, NULL, NULL)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2115, N'张1745435410', NULL, NULL, NULL)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2116, N'张1139007619', NULL, NULL, NULL)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2117, N'张1139007619', NULL, NULL, NULL)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2118, N'张1551072723', NULL, NULL, NULL)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2119, N'张174117004', NULL, NULL, NULL)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2120, N'张174117004', NULL, NULL, NULL)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2121, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2122, NULL, 2, NULL, 0)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2123, N'张三', NULL, NULL, NULL)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2124, N'张2029255309', NULL, NULL, NULL)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2125, N'张300927699', NULL, NULL, NULL)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2126, N'张300927699', NULL, NULL, NULL)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2127, N'张三', 1, N'男        ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2128, N'sun', 1, N'女        ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2129, N'mama', 1, N'gril      ', 1)
INSERT [dbo].[Student] ([id], [name], [sch_id], [sex], [isOk]) VALUES (2130, N'stud-2016-09-07', NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[Student] OFF
SET IDENTITY_INSERT [dbo].[Subject] ON 

INSERT [dbo].[Subject] ([id], [studentId], [name]) VALUES (1, 1, N'数学')
INSERT [dbo].[Subject] ([id], [studentId], [name]) VALUES (2, 2, N'语文')
INSERT [dbo].[Subject] ([id], [studentId], [name]) VALUES (3, 3, N'英语')
INSERT [dbo].[Subject] ([id], [studentId], [name]) VALUES (4, 4, N'物理')
INSERT [dbo].[Subject] ([id], [studentId], [name]) VALUES (5, 5, N'化学')
INSERT [dbo].[Subject] ([id], [studentId], [name]) VALUES (6, 6, N'.NET')
SET IDENTITY_INSERT [dbo].[Subject] OFF
/****** Object:  StoredProcedure [dbo].[sp_school]    Script Date: 2016/9/7 15:29:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[sp_school]
@p1 int,
@p2 int output
as
set @p2=100
select * from  School


GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'地域ID，关联area表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'School', @level2type=N'COLUMN',@level2name=N'id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'学校ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Student', @level2type=N'COLUMN',@level2name=N'sch_id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'学生ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Subject', @level2type=N'COLUMN',@level2name=N'studentId'
GO
USE [master]
GO
ALTER DATABASE [SqlSugarTest] SET  READ_WRITE 
GO
