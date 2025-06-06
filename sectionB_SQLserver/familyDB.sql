USE [family]
GO
/****** Object:  Table [dbo].[FamilyRelations]    Script Date: 08/04/2025 20:34:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FamilyRelations](
	[Person_Id] [int] NOT NULL,
	[Relative_Id] [int] NOT NULL,
	[Relationship_Type_Id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Person_Id] ASC,
	[Relative_Id] ASC,
	[Relationship_Type_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Personal_Info]    Script Date: 08/04/2025 20:34:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Personal_Info](
	[Person_Id] [int] NOT NULL,
	[Personal_Name] [nvarchar](100) NULL,
	[Family_Name] [nvarchar](100) NULL,
	[Gender] [nvarchar](10) NULL,
	[Father_Id] [int] NULL,
	[Mother_Id] [int] NULL,
	[Spouse_Id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Person_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RelationshipTypes]    Script Date: 08/04/2025 20:34:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RelationshipTypes](
	[Relationship_Type_Id] [int] IDENTITY(1,1) NOT NULL,
	[Relationship_Type_Name] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Relationship_Type_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FamilyRelations]  WITH CHECK ADD FOREIGN KEY([Person_Id])
REFERENCES [dbo].[Personal_Info] ([Person_Id])
GO
ALTER TABLE [dbo].[FamilyRelations]  WITH CHECK ADD FOREIGN KEY([Relative_Id])
REFERENCES [dbo].[Personal_Info] ([Person_Id])
GO
ALTER TABLE [dbo].[FamilyRelations]  WITH CHECK ADD FOREIGN KEY([Relationship_Type_Id])
REFERENCES [dbo].[RelationshipTypes] ([Relationship_Type_Id])
GO
/****** Object:  StoredProcedure [dbo].[AddFamilyRelationSimple]    Script Date: 08/04/2025 20:34:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AddFamilyRelationSimple]
    @PersonID INT,
    @RelativeID INT,
    @RelationTypeID INT
AS
BEGIN
    INSERT INTO FamilyRelations (Person_ID, Relative_Id,Relationship_Type_Id)
    VALUES (@PersonID, @RelativeID, @RelationTypeID)
END
GO
