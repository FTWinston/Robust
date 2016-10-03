-- creating the schemas seems to have to run as separate commands
create schema [model] GO
create schema [data] GO


SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
GO



CREATE TABLE [model].EntityTypes
	(
	ID int NOT NULL IDENTITY (1, 1),
	Name nvarchar(50) NOT NULL
	) 
GO
ALTER TABLE [model].EntityTypes ADD CONSTRAINT
	PK_EntityTypes PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO
ALTER TABLE [model].EntityTypes ADD CONSTRAINT
	IX_EntityTypes_Name UNIQUE NONCLUSTERED 
	(
	Name
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO
ALTER TABLE [model].EntityTypes SET (LOCK_ESCALATION = TABLE)
GO



CREATE TABLE [model].FieldTypes
	(
	ID int NOT NULL IDENTITY (1, 1),
	Name nvarchar(50) NOT NULL,
	SortOrder int NOT NULL,
	ParentFieldTypeID int NULL,
	DisplayFormat nvarchar(50) NULL
	) 
GO
ALTER TABLE [model].FieldTypes ADD CONSTRAINT
	PK_FieldTypes PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO
ALTER TABLE [model].FieldTypes ADD CONSTRAINT
	IX_FieldTypes_Name UNIQUE NONCLUSTERED 
	(
	Name
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO
CREATE UNIQUE NONCLUSTERED INDEX IX_FieldTypes_ParentFieldTypeID_SortOrder ON [model].FieldTypes
	(
	ParentFieldTypeID,
	SortOrder
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
ALTER TABLE [model].FieldTypes ADD CONSTRAINT
	FK_FieldTypes_FieldTypes FOREIGN KEY
	(
	ParentFieldTypeID
	) REFERENCES [model].FieldTypes
	(
	ID
	) ON UPDATE NO ACTION
	 ON DELETE NO ACTION
	
GO
ALTER TABLE [model].FieldTypes SET (LOCK_ESCALATION = TABLE)
GO



ALTER TABLE [model].FieldTypes SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE [model].EntityTypes SET (LOCK_ESCALATION = TABLE)
GO
CREATE TABLE [model].Fields
	(
	ID int NOT NULL IDENTITY (1, 1),
	EntityTypeID int NOT NULL,
	FieldTypeID int NOT NULL,
	Name nvarchar(50) NOT NULL,
	SortOrder int NOT NULL,
	Mandatory bit NOT NULL,
	MinNumber int NOT NULL,
	MaxNumber int NULL
	) 
GO
ALTER TABLE [model].Fields ADD CONSTRAINT
	CK_Fields_Max CHECK (MaxNumber is null or MaxNumber >= MinNumber)
GO
ALTER TABLE [model].Fields ADD CONSTRAINT
	CK_Fields_Min CHECK (MinNumber >= 0)
GO
ALTER TABLE [model].Fields ADD CONSTRAINT
	DF_Fields_Mandatory DEFAULT 0 FOR Mandatory
GO
ALTER TABLE [model].Fields ADD CONSTRAINT
	PK_Fields PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO
CREATE UNIQUE NONCLUSTERED INDEX IX_Fields_EntityTypeID_SortOrder ON [model].Fields
	(
	EntityTypeID,
	SortOrder
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
CREATE UNIQUE NONCLUSTERED INDEX IX_Fields_EntityTypeID_Name ON [model].Fields
	(
	EntityTypeID,
	Name
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
CREATE NONCLUSTERED INDEX IX_Fields_FieldTypeID ON [model].Fields
	(
	FieldTypeID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
ALTER TABLE [model].Fields ADD CONSTRAINT
	FK_Fields_EntityTypes FOREIGN KEY
	(
	EntityTypeID
	) REFERENCES [model].EntityTypes
	(
	ID
	) ON UPDATE NO ACTION
	 ON DELETE NO ACTION
	
GO
ALTER TABLE [model].Fields ADD CONSTRAINT
	FK_Fields_FieldTypes FOREIGN KEY
	(
	FieldTypeID
	) REFERENCES [model].FieldTypes
	(
	ID
	) ON UPDATE NO ACTION
	 ON DELETE NO ACTION
	
GO
ALTER TABLE [model].Fields SET (LOCK_ESCALATION = TABLE)
GO



ALTER TABLE model.EntityTypes SET (LOCK_ESCALATION = TABLE)
GO
CREATE TABLE [data].Entities
	(
	ID int NOT NULL IDENTITY (1, 1),
	EntityTypeID int NOT NULL,
	CreatedOn datetimeoffset(7) NOT NULL,
	DeletedOn datetimeoffset(7) NULL
	) 
GO
ALTER TABLE [data].Entities ADD CONSTRAINT
	DF_Entities_CreatedOn DEFAULT current_timestamp FOR CreatedOn
GO
ALTER TABLE [data].Entities ADD CONSTRAINT
	PK_Entities PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO
CREATE NONCLUSTERED INDEX IX_Entities_EntityTypeID_DeletedOn ON [data].Entities
	(
	EntityTypeID,
	DeletedOn DESC
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
CREATE NONCLUSTERED INDEX IX_Entities_EntityTypeID_CreatedOn ON [data].Entities
	(
	EntityTypeID,
	CreatedOn
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
ALTER TABLE [data].Entities ADD CONSTRAINT
	FK_Entities_EntityTypes FOREIGN KEY
	(
	EntityTypeID
	) REFERENCES model.EntityTypes
	(
	ID
	) ON UPDATE NO ACTION
	 ON DELETE NO ACTION
	
GO
ALTER TABLE [data].Entities SET (LOCK_ESCALATION = TABLE)
GO



ALTER TABLE model.Fields SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE [data].Entities SET (LOCK_ESCALATION = TABLE)
GO
CREATE TABLE [data].FieldValues
	(
	ID int NOT NULL IDENTITY (1, 1),
	EntityID int NULL,
	FieldID int NOT NULL,
	ValueNumber int NOT NULL,
	CreatedOn datetimeoffset(7) NOT NULL,
	Deleted bit NOT NULL
	) 
GO
ALTER TABLE [data].FieldValues ADD CONSTRAINT
	DF_FieldValues_ValueNumber DEFAULT 1 FOR ValueNumber
GO
ALTER TABLE [data].FieldValues ADD CONSTRAINT
	DF_FieldValues_CreatedOn DEFAULT current_timestamp FOR CreatedOn
GO
ALTER TABLE [data].FieldValues ADD CONSTRAINT
	DF_FieldValues_Deleted DEFAULT 0 FOR Deleted
GO
ALTER TABLE [data].FieldValues ADD CONSTRAINT
	PK_FieldValues PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO
CREATE UNIQUE NONCLUSTERED INDEX IX_FieldValues_EntityID_FieldID_ValueNumber ON [data].FieldValues
	(
	EntityID,
	FieldID,
	ValueNumber
	)
	WHERE Deleted = 0
	WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
CREATE NONCLUSTERED INDEX IX_FieldValues_EntityID_FieldID_Deleted_CreatedOn_ValueNumber ON [data].FieldValues
	(
	ID,
	EntityID,
	FieldID,
	Deleted DESC,
	CreatedOn,
	ValueNumber
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
CREATE NONCLUSTERED INDEX IX_FieldValues_EntityID_Deleted_CreatedOn ON [data].FieldValues
	(
	EntityID,
	Deleted DESC,
	CreatedOn
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
ALTER TABLE [data].FieldValues ADD CONSTRAINT
	FK_FieldValues_Entities FOREIGN KEY
	(
	EntityID
	) REFERENCES [data].Entities
	(
	ID
	) ON UPDATE NO ACTION
	 ON DELETE NO ACTION
	
GO
ALTER TABLE [data].FieldValues ADD CONSTRAINT
	FK_FieldValues_Fields FOREIGN KEY
	(
	FieldID
	) REFERENCES model.Fields
	(
	ID
	) ON UPDATE NO ACTION
	 ON DELETE NO ACTION
GO
ALTER TABLE [data].FieldValues ADD CONSTRAINT
	CK_FieldValues_ValueNumber CHECK (ValueNumber > 0)
GO
ALTER TABLE [data].FieldValues SET (LOCK_ESCALATION = TABLE)
GO



CREATE TABLE [data].FieldValues_Bit
	(
	FieldValueID int NOT NULL,
	Value bit NOT NULL
	) 
GO
ALTER TABLE [data].FieldValues_Bit ADD CONSTRAINT
	PK_FieldValues_Bit PRIMARY KEY CLUSTERED 
	(
	FieldValueID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO
CREATE NONCLUSTERED INDEX IX_FieldValues_Bit_Value ON [data].FieldValues_Bit
	(
	Value
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
ALTER TABLE [data].FieldValues_Bit ADD CONSTRAINT
	FK_FieldValues_Bit_FieldValues FOREIGN KEY
	(
	FieldValueID
	) REFERENCES [data].FieldValues
	(
	ID
	) ON UPDATE NO ACTION 
	 ON DELETE NO ACTION
GO
ALTER TABLE [data].FieldValues_Bit SET (LOCK_ESCALATION = TABLE)
GO



CREATE TABLE [data].FieldValues_Date
	(
	FieldValueID int NOT NULL,
	Value datetimeoffset(7) NOT NULL
	) 
GO
ALTER TABLE [data].FieldValues_Date ADD CONSTRAINT
	PK_FieldValues_Date PRIMARY KEY CLUSTERED 
	(
	FieldValueID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO
CREATE NONCLUSTERED INDEX IX_FieldValues_Date_Value ON [data].FieldValues_Date
	(
	Value
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
ALTER TABLE [data].FieldValues_Date ADD CONSTRAINT
	FK_FieldValues_Date_FieldValues FOREIGN KEY
	(
	FieldValueID
	) REFERENCES [data].FieldValues
	(
	ID
	) ON UPDATE NO ACTION 
	 ON DELETE NO ACTION
GO
ALTER TABLE [data].FieldValues_Date SET (LOCK_ESCALATION = TABLE)
GO




CREATE TABLE [data].FieldValues_Decimal
	(
	FieldValueID int NOT NULL,
	Value decimal(18,6) NOT NULL
	) 
GO
ALTER TABLE [data].FieldValues_Decimal ADD CONSTRAINT
	PK_FieldValues_Decimal PRIMARY KEY CLUSTERED 
	(
	FieldValueID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO
CREATE NONCLUSTERED INDEX IX_FieldValues_Decimal_Value ON [data].FieldValues_Decimal
	(
	Value
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
ALTER TABLE [data].FieldValues_Decimal ADD CONSTRAINT
	FK_FieldValues_Decimal_FieldValues FOREIGN KEY
	(
	FieldValueID
	) REFERENCES [data].FieldValues
	(
	ID
	) ON UPDATE NO ACTION 
	 ON DELETE NO ACTION
GO
ALTER TABLE [data].FieldValues_Decimal SET (LOCK_ESCALATION = TABLE)
GO



CREATE TABLE [data].FieldValues_FreeText
	(
	FieldValueID int NOT NULL,
	Value nvarchar(MAX) NOT NULL
	) 
GO
ALTER TABLE [data].FieldValues_FreeText ADD CONSTRAINT
	PK_FieldValues_FreeText PRIMARY KEY CLUSTERED 
	(
	FieldValueID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO
ALTER TABLE [data].FieldValues_FreeText ADD CONSTRAINT
	FK_FieldValues_FreeText_FieldValues FOREIGN KEY
	(
	FieldValueID
	) REFERENCES [data].FieldValues
	(
	ID
	) ON UPDATE NO ACTION 
	 ON DELETE NO ACTION
GO
ALTER TABLE [data].FieldValues_FreeText SET (LOCK_ESCALATION = TABLE)
GO



CREATE TABLE [data].FieldValues_Int
	(
	FieldValueID int NOT NULL,
	Value int NOT NULL
	) 
GO
ALTER TABLE [data].FieldValues_Int ADD CONSTRAINT
	PK_FieldValues_Int PRIMARY KEY CLUSTERED 
	(
	FieldValueID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO
CREATE NONCLUSTERED INDEX IX_FieldValues_Int_Value ON [data].FieldValues_Int
	(
	Value
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
ALTER TABLE [data].FieldValues_Int ADD CONSTRAINT
	FK_FieldValues_Int_FieldValues FOREIGN KEY
	(
	FieldValueID
	) REFERENCES [data].FieldValues
	(
	ID
	) ON UPDATE NO ACTION 
	 ON DELETE NO ACTION
GO
ALTER TABLE [data].FieldValues_Int SET (LOCK_ESCALATION = TABLE)
GO



CREATE TABLE [data].FieldValues_Text
	(
	FieldValueID int NOT NULL,
	Value nvarchar(255) NOT NULL
	) 
GO
ALTER TABLE [data].FieldValues_Text ADD CONSTRAINT
	PK_FieldValues_Text PRIMARY KEY CLUSTERED 
	(
	FieldValueID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO
CREATE NONCLUSTERED INDEX IX_FieldValues_Text_Value ON [data].FieldValues_Text
	(
	Value
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
ALTER TABLE [data].FieldValues_Text ADD CONSTRAINT
	FK_FieldValues_Text_FieldValues FOREIGN KEY
	(
	FieldValueID
	) REFERENCES [data].FieldValues
	(
	ID
	) ON UPDATE NO ACTION 
	 ON DELETE NO ACTION
GO
ALTER TABLE [data].FieldValues_Text SET (LOCK_ESCALATION = TABLE)
GO



ALTER TABLE [data].Entities SET (LOCK_ESCALATION = TABLE)
GO
CREATE TABLE [data].FieldValues_ForeignKey
	(
	FieldValueID int NOT NULL,
	Value int NOT NULL,
	IsChild bit NOT NULL
	)
GO
ALTER TABLE [data].FieldValues_ForeignKey ADD CONSTRAINT
	DF_FieldValues_ForeignKey_IsChild DEFAULT 0 FOR IsChild
GO
ALTER TABLE [data].FieldValues_ForeignKey ADD CONSTRAINT
	PK_FieldValues_ForeignKey PRIMARY KEY CLUSTERED 
	(
	FieldValueID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO
CREATE NONCLUSTERED INDEX IX_FieldValues_ForeignKey_Value ON [data].FieldValues_ForeignKey
	(
	Value
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
ALTER TABLE [data].FieldValues_ForeignKey ADD CONSTRAINT
	FK_FieldValues_ForeignKey_FieldValues FOREIGN KEY
	(
	FieldValueID
	) REFERENCES [data].FieldValues
	(
	ID
	) ON UPDATE NO ACTION
	 ON DELETE NO ACTION
	
GO
ALTER TABLE [data].FieldValues_ForeignKey ADD CONSTRAINT
	FK_FieldValues_ForeignKey_Entities FOREIGN KEY
	(
	Value
	) REFERENCES [data].Entities
	(
	ID
	) ON UPDATE NO ACTION
	 ON DELETE NO ACTION
	
GO
ALTER TABLE [data].FieldValues_ForeignKey SET (LOCK_ESCALATION = TABLE)
GO



CREATE VIEW [data].[CurrentFieldValues]
AS
SELECT ID, EntityID, FieldID, ValueNumber
FROM (SELECT ID, EntityID, FieldID, ValueNumber, Deleted,
    RANK() OVER (PARTITION BY EntityID, FieldID, ValueNumber ORDER BY CreatedOn DESC) num
    FROM data.FieldValues
) fv WHERE num = 1 and Deleted = 0
GO



CREATE VIEW [data].CurrentEntities
AS
select ID, EntityTypeID, CreatedOn from data.Entities where DeletedOn is null
GO



insert into model.FieldTypes select 'Boolean', 1, null, null
insert into model.FieldTypes select 'Date', 4, null, null
insert into model.FieldTypes select 'Decimal', 3, null, null
insert into model.FieldTypes select 'Foreign Key', 7, null, null
insert into model.FieldTypes select 'Free Text', 6, null, null
insert into model.FieldTypes select 'Integer', 2, null, null
insert into model.FieldTypes select 'Text', 5, null, null
