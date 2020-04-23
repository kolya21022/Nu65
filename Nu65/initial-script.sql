-- ��������/������������� �� Nu65DB [2018.11.26]


 ---- �������� ���� ������, ���� ��� ����������
 --USE [master]
 --IF EXISTS (select * from [sys].[databases] where [name]='Nu65DB')
 --	DROP DATABASE [Nu65DB]
 --GO

 ---- �������� ������ ���� ������, ���� ��� ���������� (������� �������� �� ��������� ������, ����� ������������) 
 --USE [Nu65DB]
 --GO
 --IF OBJECT_ID('Nu65DBTable', 'U') IS NOT NULL      DROP TABLE [Nu65DBTable]
 --GO
 --IF OBJECT_ID('Measures', 'U') IS NOT NULL  DROP TABLE [Measures]
 --GO
 --IF OBJECT_ID('Materials', 'U') IS NOT NULL               DROP TABLE [Materials]
 --GO
 --IF OBJECT_ID('Products', 'U') IS NOT NULL          DROP TABLE [Products]
 --GO


-- �������� ���� ������
CREATE DATABASE [Nu65DB]
GO
EXEC dbo.sp_dbcmptlevel @dbname=N'Nu65DB', @new_cmptlevel=140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Nu65DB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Nu65DB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Nu65DB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Nu65DB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Nu65DB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Nu65DB] SET ARITHABORT OFF 
GO
ALTER DATABASE [Nu65DB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Nu65DB] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [Nu65DB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Nu65DB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Nu65DB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Nu65DB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Nu65DB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Nu65DB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Nu65DB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Nu65DB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Nu65DB] SET DISABLE_BROKER 
GO
ALTER DATABASE [Nu65DB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Nu65DB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Nu65DB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Nu65DB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Nu65DB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Nu65DB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Nu65DB] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Nu65DB] SET MULTI_USER 
GO
ALTER DATABASE [Nu65DB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Nu65DB] SET DB_CHAINING OFF 
GO


-- �������� ������� [���������]
USE [Nu65DB]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products] (
	[id] bigint IDENTITY(1, 1) NOT NULL, 
	[codeProduct] bigint NOT NULL UNIQUE,
	[name] nvarchar(50) NOT NULL, 
	[mark] nvarchar(40) NOT NULL, 
	[SERVICE_CREATE_DATETIME] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP, 
	[SERVICE_CREATE_USER] [nvarchar](150) NOT NULL DEFAULT SUSER_SNAME(), 
	[SERVICE_LAST_MODIFY_DATETIME] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP, 
	[SERVICE_LAST_MODIFY_USER] [nvarchar](150) NOT NULL DEFAULT SUSER_SNAME(), 
CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED (
	[id] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, 
	ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo', @level1type=N'TABLE',
	@level1name=N'Products',
	@value=N'������� �������'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Products', 
	@level2name = N'id', 
	@value = N'���������� �������������'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Products', 
	@level2name = N'codeProduct', 
	@value = N'��� ��������'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Products', 
	@level2name = N'name', 
	@value = N'�������� �������'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Products', 
	@level2name = N'mark', 
	@value = N'����� �������'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Products',
	@level2name=N'SERVICE_CREATE_USER',
	@value=N'������������ ��������� ������'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Products',
	@level2name=N'SERVICE_LAST_MODIFY_DATETIME',
	@value=N'���� ���������� ��������� ������'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Products',
	@level2name=N'SERVICE_LAST_MODIFY_USER',
	@value=N'��������� ������������ ���������� ������'
GO

-- ������� ���������� ����-������� ���������� �������������� � ������������ ����������� ������ ���������
CREATE TRIGGER [TriggerUpdateProducts] ON [Products] 
FOR UPDATE 
AS
	BEGIN 
		IF @@ROWCOUNT = 0 RETURN 
		IF TRIGGER_NESTLEVEL(object_ID('TriggerUpdateProducts')) > 1 RETURN 
		SET NOCOUNT ON 

		UPDATE [Products] 
		SET [SERVICE_LAST_MODIFY_DATETIME] = CURRENT_TIMESTAMP, [SERVICE_LAST_MODIFY_USER] = SUSER_SNAME() 
		WHERE [id] IN (SELECT DISTINCT [id] FROM [INSERTED]) 
	END
GO


-- �������� ������� [����������]
USE [Nu65DB]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Materials] (
	[id] bigint IDENTITY(1, 1) NOT NULL, 
	[codeMaterial] bigint NOT NULL UNIQUE, 
	[name] nvarchar(50) NOT NULL, 
	[profile] nvarchar(50) NOT NULL, 
	[gost] nvarchar(40) NOT NULL, 	
	[SERVICE_CREATE_DATETIME] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP, 
	[SERVICE_CREATE_USER] [nvarchar](150) NOT NULL DEFAULT SUSER_SNAME(), 
	[SERVICE_LAST_MODIFY_DATETIME] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP, 
	[SERVICE_LAST_MODIFY_USER] [nvarchar](150) NOT NULL DEFAULT SUSER_SNAME(), 
CONSTRAINT [PK_Materials] PRIMARY KEY CLUSTERED (
	[id] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, 
	ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo', @level1type=N'TABLE',
	@level1name=N'Materials',
	@value=N'������� ����������'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Materials', 
	@level2name = N'id', 
	@value = N'���������� �������������'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Materials', 
	@level2name = N'codeMaterial', 
	@value = N'��� ���������'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Materials', 
	@level2name = N'name', 
	@value = N'�������� ���������'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Materials', 
	@level2name = N'profile', 
	@value = N'������� ���������'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Materials', 
	@level2name = N'gost', 
	@value = N'���� ���������'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Materials',
	@level2name=N'SERVICE_CREATE_DATETIME',
	@value=N'���� �������� ������'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Materials',
	@level2name=N'SERVICE_CREATE_USER',
	@value=N'������������ ��������� ������'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Materials',
	@level2name=N'SERVICE_LAST_MODIFY_DATETIME',
	@value=N'���� ���������� ��������� ������'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Materials',
	@level2name=N'SERVICE_LAST_MODIFY_USER',
	@value=N'��������� ������������ ���������� ������'
GO

-- ������� ���������� ����-������� ���������� �������������� � ������������ ����������� ������ ���������
CREATE TRIGGER [TriggerUpdateMaterials] ON [Materials] 
FOR UPDATE 
AS
	BEGIN 
		IF @@ROWCOUNT = 0 RETURN 
		IF TRIGGER_NESTLEVEL(object_ID('TriggerUpdateMaterials')) > 1 RETURN 
		SET NOCOUNT ON 

		UPDATE [Materials] 
		SET [SERVICE_LAST_MODIFY_DATETIME] = CURRENT_TIMESTAMP, [SERVICE_LAST_MODIFY_USER] = SUSER_SNAME() 
		WHERE [id] IN (SELECT DISTINCT [id] FROM [INSERTED]) 
	END
GO


-- �������� ������� [��. ���.]
USE [Nu65DB]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Measures] (
	[id] bigint IDENTITY(1, 1) NOT NULL, 
	[oldDbCode] int NOT NULL, 
	[name] nvarchar(20) NOT NULL, 
	[shortName] nvarchar(11) NOT NULL, 
	[SERVICE_CREATE_DATETIME] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP, 
	[SERVICE_CREATE_USER] [nvarchar](150) NOT NULL DEFAULT SUSER_SNAME(), 
	[SERVICE_LAST_MODIFY_DATETIME] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP, 
	[SERVICE_LAST_MODIFY_USER] [nvarchar](150) NOT NULL DEFAULT SUSER_SNAME(), 
CONSTRAINT [PK_Measure] PRIMARY KEY CLUSTERED (
	[id] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, 
	ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo', @level1type=N'TABLE',
	@level1name=N'Measures',
	@value=N'������� ��. ���.'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Measures', 
	@level2name = N'id', 
	@value = N'���������� �������������'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Measures', 
	@level2name = N'oldDbCode', 
	@value = N'��� ������� ��������� � ������ ��'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Measures', 
	@level2name = N'name', 
	@value = N'�������� ��. ���.'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Measures', 
	@level2name = N'shortName', 
	@value = N'������� �������� ��. ���.'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Measures',
	@level2name=N'SERVICE_CREATE_DATETIME',
	@value=N'���� �������� ������'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Measures',
	@level2name=N'SERVICE_CREATE_USER',
	@value=N'������������ ��������� ������'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Measures',
	@level2name=N'SERVICE_LAST_MODIFY_DATETIME',
	@value=N'���� ���������� ��������� ������'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Measures',
	@level2name=N'SERVICE_LAST_MODIFY_USER',
	@value=N'��������� ������������ ���������� ������'
GO

-- ���� ������������ ���� ����� [name]
ALTER TABLE [dbo].[Measures]
	ADD CONSTRAINT [UQ_Measures_OldDbCode] UNIQUE NONCLUSTERED ([oldDbCode]) ON [PRIMARY]
GO

-- ������� ���������� ����-������� ���������� �������������� � ������������ ����������� ������ ���������
CREATE TRIGGER [TriggerUpdateMeasures] ON [Measures] 
FOR UPDATE 
AS
	BEGIN 
		IF @@ROWCOUNT = 0 RETURN 
		IF TRIGGER_NESTLEVEL(object_ID('TriggerUpdateMeasures')) > 1 RETURN 
		SET NOCOUNT ON 

		UPDATE [Measures] 
		SET [SERVICE_LAST_MODIFY_DATETIME] = CURRENT_TIMESTAMP, [SERVICE_LAST_MODIFY_USER] = SUSER_SNAME() 
		WHERE [id] IN (SELECT DISTINCT [id] FROM [INSERTED]) 
	END
GO


-- �������� ������� [Nu65]
USE [Nu65DB]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Nu65Table] (
	[id] bigint IDENTITY(1, 1) NOT NULL, 
	[productsId] bigint NOT NULL, 
	[materialsId] bigint NOT NULL, 
    [measuresId] bigint NOT NULL, 
	[auxiliaryMaterialConsumptionRate] nvarchar(15) NOT NULL, 
	[workGuildId] nvarchar(2) NOT NULL, 
	[signMaterial] nvarchar(1) NOT NULL, 
	[parcelId] nvarchar(2) NOT NULL, 
	[unitValidation] nvarchar(4) NOT NULL, 
	[date] datetime NOT NULL,
	[flowRate] nvarchar(21), 
	[SERVICE_CREATE_DATETIME] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP, 
	[SERVICE_CREATE_USER] [nvarchar](150) NOT NULL DEFAULT SUSER_SNAME(), 
	[SERVICE_LAST_MODIFY_DATETIME] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP, 
	[SERVICE_LAST_MODIFY_USER] [nvarchar](150) NOT NULL DEFAULT SUSER_SNAME(), 
CONSTRAINT [PK_Nu65Table] PRIMARY KEY CLUSTERED (
	[id] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, 
	ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo', @level1type=N'TABLE',
	@level1name=N'Nu65Table',
	@value=N'������� Nu65'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Nu65Table', 
	@level2name = N'id', 
	@value = N'���������� �������������'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Nu65Table', 
	@level2name = N'productsId', 
	@value = N'��������� ���� �� ������� ���������'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Nu65Table', 
	@level2name = N'materialsId', 
	@value = N'��������� ���� �� ������� ����������'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Nu65Table', 
	@level2name = N'measuresId', 
	@value = N'��������� ���� �� ������� ��. ���.'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Nu65Table', 
	@level2name = N'auxiliaryMaterialConsumptionRate', 
	@value = N'����� ������� ���������������� ��������'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Nu65Table', 
	@level2name = N'workGuildId', 
	@value = N'��� ����'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Nu65Table', 
	@level2name = N'signMaterial', 
	@value = N'������� ���������'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Nu65Table', 
	@level2name = N'parcelId', 
	@value = N'��� �������'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Nu65Table', 
	@level2name = N'unitValidation', 
	@value = N'������� ������������'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Nu65Table', 
	@level2name = N'date', 
	@value = N'���� ����������'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Nu65Table', 
	@level2name = N'flowRate', 
	@value = N'����� ���������'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN', 
	@level1name=N'Nu65Table', 
	@level2name=N'SERVICE_CREATE_DATETIME', 
	@value=N'���� �������� ������'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Nu65Table',
	@level2name=N'SERVICE_CREATE_USER',
	@value=N'������������ ��������� ������'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Nu65Table',
	@level2name=N'SERVICE_LAST_MODIFY_DATETIME',
	@value=N'���� ���������� ��������� ������'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Nu65Table',
	@level2name=N'SERVICE_LAST_MODIFY_USER',
	@value=N'��������� ������������ ���������� ������'
GO

-- ��������� ���� �� ������� [���������] 
ALTER TABLE [dbo].[Nu65Table] WITH CHECK
ADD CONSTRAINT [FK_Nu65TableToProducts] FOREIGN KEY([productsId])
REFERENCES [dbo].[Products] ([id])
	ON UPDATE NO ACTION
	ON DELETE NO ACTION
GO
ALTER TABLE [dbo].[Nu65Table] CHECK CONSTRAINT [FK_Nu65TableToProducts]
GO

-- ��������� ���� �� ������� [����������] 
ALTER TABLE [dbo].[Nu65Table] WITH CHECK
ADD CONSTRAINT [FK_Nu65TableToMaterials] FOREIGN KEY([materialsId])
REFERENCES [dbo].[Materials] ([id])
	ON UPDATE NO ACTION
	ON DELETE NO ACTION
GO
ALTER TABLE [dbo].[Nu65Table] CHECK CONSTRAINT [FK_Nu65TableToMaterials]
GO

-- ��������� ���� �� ������� [��. ���.] 
ALTER TABLE [dbo].[Nu65Table] WITH CHECK
ADD CONSTRAINT [FK_Nu65TableToMeasures] FOREIGN KEY([measuresId])
REFERENCES [dbo].[Measures] ([id])
	ON UPDATE NO ACTION
	ON DELETE NO ACTION
GO
ALTER TABLE [dbo].[Nu65Table] CHECK CONSTRAINT [FK_Nu65TableToMeasures]
GO

-- ������� ���������� ����-������� ���������� �������������� � ������������ ����������� ������ ���������
CREATE TRIGGER [TriggerUpdateNu65Table] ON [Nu65Table] 
FOR UPDATE 
AS
	BEGIN 
		IF @@ROWCOUNT = 0 RETURN 
		IF TRIGGER_NESTLEVEL(object_ID('TriggerUpdateNu65Table')) > 1 RETURN 
		SET NOCOUNT ON 

		UPDATE [Nu65Table] 
		SET [SERVICE_LAST_MODIFY_DATETIME] = CURRENT_TIMESTAMP, [SERVICE_LAST_MODIFY_USER] = SUSER_SNAME() 
		WHERE [id] IN (SELECT DISTINCT [id] FROM [INSERTED]) 
	END
GO


