USE [TestDB]
GO
/****** Object:  Table [dbo].[Ts_ProductData]    Script Date: 2022/10/9 18:21:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ts_ProductData](
	[F_Id] [varchar](50) NOT NULL,
	[c_ProductStationID] [varchar](50) NULL,
	[F_CreatorUserId] [varchar](50) NULL,
	[F_CreatorTime] [datetime] NULL,
	[F_LastModifyUserId] [varchar](50) NULL,
	[F_LastModifyTime] [datetime] NULL,
	[F_DeleteTime] [datetime] NULL,
	[F_DeleteUserId] [varchar](50) NULL,
	[c_column0] [decimal](12, 3) NULL,
	[c_column1] [decimal](12, 3) NULL,
	[c_column2] [decimal](12, 3) NULL,
	[c_column3] [decimal](12, 3) NULL,
	[c_column4] [decimal](12, 3) NULL,
	[c_column5] [decimal](12, 3) NULL,
	[c_column6] [decimal](12, 3) NULL,
	[c_column7] [decimal](12, 3) NULL,
	[c_column8] [decimal](12, 3) NULL,
	[c_column9] [decimal](12, 3) NULL,
	[c_column10] [decimal](12, 3) NULL,
	[c_column11] [decimal](12, 3) NULL,
	[c_column12] [decimal](12, 3) NULL,
	[c_column13] [decimal](12, 3) NULL,
	[c_column14] [decimal](12, 3) NULL,
	[c_column15] [decimal](12, 3) NULL,
	[c_column16] [decimal](12, 3) NULL,
	[c_column17] [decimal](12, 3) NULL,
	[c_column18] [decimal](12, 3) NULL,
	[c_column19] [decimal](12, 3) NULL,
	[c_column20] [decimal](12, 3) NULL,
	[c_column21] [decimal](12, 3) NULL,
 CONSTRAINT [PK_Ts_ProductData_F_Id] PRIMARY KEY CLUSTERED 
(
	[F_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ts_ProductStation]    Script Date: 2022/10/9 18:21:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ts_ProductStation](
	[F_Id] [varchar](50) NOT NULL,
	[c_ProductRecordID] [varchar](50) NULL,
	[c_ClassID] [varchar](50) NULL,
	[c_LineID] [varchar](50) NULL,
	[c_BaseStationID] [varchar](50) NULL,
	[c_ChildNumber] [int] NULL,
	[c_TrayProcessCode] [varchar](50) NULL,
	[c_ResultCode] [int] NULL,
	[c_Result] [bit] NULL,
	[c_GetDataType] [int] NULL,
	[c_RecipeRecordID] [varchar](50) NULL,
	[F_CreatorUserId] [varchar](50) NULL,
	[F_CreatorTime] [datetime] NULL,
	[F_LastModifyTime] [datetime] NULL,
	[F_LastModifyUserId] [varchar](50) NULL,
	[F_DeleteTime] [datetime] NULL,
	[F_DeleteUserId] [varchar](50) NULL,
 CONSTRAINT [PK_Ts_Prodion_F_Id0954] PRIMARY KEY CLUSTERED 
(
	[F_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[Ts_ProductData] ([F_Id], [c_ProductStationID], [F_CreatorUserId], [F_CreatorTime], [F_LastModifyUserId], [F_LastModifyTime], [F_DeleteTime], [F_DeleteUserId], [c_column0], [c_column1], [c_column2], [c_column3], [c_column4], [c_column5], [c_column6], [c_column7], [c_column8], [c_column9], [c_column10], [c_column11], [c_column12], [c_column13], [c_column14], [c_column15], [c_column16], [c_column17], [c_column18], [c_column19], [c_column20], [c_column21]) VALUES (N'adc7b7a3-c16a-4bcd-8422-a41a5e2afce0', N'd0678f52-b6f9-41f8-a49a-be850fe49498', NULL, CAST(N'2022-10-09T15:28:57.567' AS DateTime), NULL, NULL, NULL, NULL, CAST(0.980 AS Decimal(12, 3)), CAST(9.485 AS Decimal(12, 3)), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Ts_ProductStation] ([F_Id], [c_ProductRecordID], [c_ClassID], [c_LineID], [c_BaseStationID], [c_ChildNumber], [c_TrayProcessCode], [c_ResultCode], [c_Result], [c_GetDataType], [c_RecipeRecordID], [F_CreatorUserId], [F_CreatorTime], [F_LastModifyTime], [F_LastModifyUserId], [F_DeleteTime], [F_DeleteUserId]) VALUES (N'd0678f52-b6f9-41f8-a49a-be850fe49498', N'261b4365-b145-4327-b1e2-07cfa1e6bb02', N'b4836db0-cb72-4048-833d-10887ba7eafc', N'4beb0f02-a455-49ba-bd38-5e0ad76441a1', N'd3e4770e-d00f-4553-b291-68a0c09c2c0f', 1, N'WEB2022100911253845403000000000A5', 1, 1, 0, N'6898e104-6711-4fa5-bbfa-79f265fe72fa', NULL, CAST(N'2022-10-09T15:28:57.547' AS DateTime), NULL, NULL, NULL, NULL)
GO
ALTER TABLE [dbo].[Ts_ProductData]  WITH CHECK ADD  CONSTRAINT [FK_Ts_Prodata_c_ProdunID6B2E] FOREIGN KEY([c_ProductStationID])
REFERENCES [dbo].[Ts_ProductStation] ([F_Id])
GO
ALTER TABLE [dbo].[Ts_ProductData] CHECK CONSTRAINT [FK_Ts_Prodata_c_ProdunID6B2E]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ts_ProductData', @level2type=N'COLUMN',@level2name=N'F_Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'记录工作站ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ts_ProductData', @level2type=N'COLUMN',@level2name=N'c_ProductStationID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ts_ProductData', @level2type=N'COLUMN',@level2name=N'F_CreatorUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ts_ProductData', @level2type=N'COLUMN',@level2name=N'F_CreatorTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ts_ProductData', @level2type=N'COLUMN',@level2name=N'F_LastModifyUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最后修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ts_ProductData', @level2type=N'COLUMN',@level2name=N'F_LastModifyTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'删除时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ts_ProductData', @level2type=N'COLUMN',@level2name=N'F_DeleteTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'删除人员ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ts_ProductData', @level2type=N'COLUMN',@level2name=N'F_DeleteUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品测试详细信息' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ts_ProductData'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ts_ProductStation', @level2type=N'COLUMN',@level2name=N'F_Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'记录ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ts_ProductStation', @level2type=N'COLUMN',@level2name=N'c_ProductRecordID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'班次信息' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ts_ProductStation', @level2type=N'COLUMN',@level2name=N'c_ClassID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产线ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ts_ProductStation', @level2type=N'COLUMN',@level2name=N'c_LineID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'基本工作站ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ts_ProductStation', @level2type=N'COLUMN',@level2name=N'c_BaseStationID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'子工作站顺序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ts_ProductStation', @level2type=N'COLUMN',@level2name=N'c_ChildNumber'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'托盘过程码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ts_ProductStation', @level2type=N'COLUMN',@level2name=N'c_TrayProcessCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'结果代码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ts_ProductStation', @level2type=N'COLUMN',@level2name=N'c_ResultCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'结果判定' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ts_ProductStation', @level2type=N'COLUMN',@level2name=N'c_Result'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'获取数据方式' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ts_ProductStation', @level2type=N'COLUMN',@level2name=N'c_GetDataType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'当前配方记录ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ts_ProductStation', @level2type=N'COLUMN',@level2name=N'c_RecipeRecordID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ts_ProductStation', @level2type=N'COLUMN',@level2name=N'F_CreatorUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ts_ProductStation', @level2type=N'COLUMN',@level2name=N'F_CreatorTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最后修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ts_ProductStation', @level2type=N'COLUMN',@level2name=N'F_LastModifyTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ts_ProductStation', @level2type=N'COLUMN',@level2name=N'F_LastModifyUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'删除时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ts_ProductStation', @level2type=N'COLUMN',@level2name=N'F_DeleteTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'删除人员ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ts_ProductStation', @level2type=N'COLUMN',@level2name=N'F_DeleteUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品信息 产品工作站记录情况' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ts_ProductStation'
GO
