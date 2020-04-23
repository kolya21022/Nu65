-- Создание/инициализация БД Nu65DB [2018.11.26]


 ---- Удаление базы данных, если она существует
 --USE [master]
 --IF EXISTS (select * from [sys].[databases] where [name]='Nu65DB')
 --	DROP DATABASE [Nu65DB]
 --GO

 ---- Удаление таблиц базы данных, если они существуют (сначала дочерние по вторичным ключам, затем родительские) 
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


-- Создание базы данных
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


-- Создание таблицы [Продуктов]
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
	@value=N'Таблица изделий'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Products', 
	@level2name = N'id', 
	@value = N'Уникальный идентификатор'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Products', 
	@level2name = N'codeProduct', 
	@value = N'Код продукта'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Products', 
	@level2name = N'name', 
	@value = N'Название изделия'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Products', 
	@level2name = N'mark', 
	@value = N'Марка изделия'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Products',
	@level2name=N'SERVICE_CREATE_USER',
	@value=N'Пользователь создавший запись'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Products',
	@level2name=N'SERVICE_LAST_MODIFY_DATETIME',
	@value=N'Дата последнего изменения записи'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Products',
	@level2name=N'SERVICE_LAST_MODIFY_USER',
	@value=N'Последний пользователь изменявший запись'
GO

-- Триггер обновления даты-времени последнего редактирования и пользователя изменившего запись последним
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


-- Создание таблицы [Материалов]
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
	@value=N'Таблица материалов'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Materials', 
	@level2name = N'id', 
	@value = N'Уникальный идентификатор'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Materials', 
	@level2name = N'codeMaterial', 
	@value = N'Код материала'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Materials', 
	@level2name = N'name', 
	@value = N'Название материала'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Materials', 
	@level2name = N'profile', 
	@value = N'Профиль материала'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Materials', 
	@level2name = N'gost', 
	@value = N'ГОСТ материала'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Materials',
	@level2name=N'SERVICE_CREATE_DATETIME',
	@value=N'Дата создания записи'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Materials',
	@level2name=N'SERVICE_CREATE_USER',
	@value=N'Пользователь создавший запись'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Materials',
	@level2name=N'SERVICE_LAST_MODIFY_DATETIME',
	@value=N'Дата последнего изменения записи'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Materials',
	@level2name=N'SERVICE_LAST_MODIFY_USER',
	@value=N'Последний пользователь изменявший запись'
GO

-- Триггер обновления даты-времени последнего редактирования и пользователя изменившего запись последним
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


-- Создание таблицы [Ед. изм.]
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
	@value=N'Таблица ед. изм.'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Measures', 
	@level2name = N'id', 
	@value = N'Уникальный идентификатор'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Measures', 
	@level2name = N'oldDbCode', 
	@value = N'Код единицы измерения в старой БД'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Measures', 
	@level2name = N'name', 
	@value = N'Название ед. изм.'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Measures', 
	@level2name = N'shortName', 
	@value = N'Краткое название ед. изм.'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Measures',
	@level2name=N'SERVICE_CREATE_DATETIME',
	@value=N'Дата создания записи'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Measures',
	@level2name=N'SERVICE_CREATE_USER',
	@value=N'Пользователь создавший запись'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Measures',
	@level2name=N'SERVICE_LAST_MODIFY_DATETIME',
	@value=N'Дата последнего изменения записи'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Measures',
	@level2name=N'SERVICE_LAST_MODIFY_USER',
	@value=N'Последний пользователь изменявший запись'
GO

-- Ключ уникальности пары полей [name]
ALTER TABLE [dbo].[Measures]
	ADD CONSTRAINT [UQ_Measures_OldDbCode] UNIQUE NONCLUSTERED ([oldDbCode]) ON [PRIMARY]
GO

-- Триггер обновления даты-времени последнего редактирования и пользователя изменившего запись последним
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


-- Создание таблицы [Nu65]
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
	@value=N'Таблица Nu65'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Nu65Table', 
	@level2name = N'id', 
	@value = N'Уникальный идентификатор'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Nu65Table', 
	@level2name = N'productsId', 
	@value = N'Вторичный ключ на таблицу продуктов'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Nu65Table', 
	@level2name = N'materialsId', 
	@value = N'Вторичный ключ на таблицу материалов'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Nu65Table', 
	@level2name = N'measuresId', 
	@value = N'Вторичный ключ на таблицу ед. изм.'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Nu65Table', 
	@level2name = N'auxiliaryMaterialConsumptionRate', 
	@value = N'Норма расхода вспомогательного продукта'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Nu65Table', 
	@level2name = N'workGuildId', 
	@value = N'Код цеха'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Nu65Table', 
	@level2name = N'signMaterial', 
	@value = N'Признак материала'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Nu65Table', 
	@level2name = N'parcelId', 
	@value = N'Код участка'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Nu65Table', 
	@level2name = N'unitValidation', 
	@value = N'Единица нормирования'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Nu65Table', 
	@level2name = N'date', 
	@value = N'Дата проведения'
GO
EXEC [sys].[sp_addextendedproperty] @name=N'MS_Description', @level0type=N'SCHEMA', 
	@level0name = N'dbo', @level1type = N'TABLE', @level2type = N'COLUMN', 
	@level1name = N'Nu65Table', 
	@level2name = N'flowRate', 
	@value = N'Номер извещания'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN', 
	@level1name=N'Nu65Table', 
	@level2name=N'SERVICE_CREATE_DATETIME', 
	@value=N'Дата создания записи'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Nu65Table',
	@level2name=N'SERVICE_CREATE_USER',
	@value=N'Пользователь создавший запись'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Nu65Table',
	@level2name=N'SERVICE_LAST_MODIFY_DATETIME',
	@value=N'Дата последнего изменения записи'
GO
EXEC [sys].[sp_addextendedproperty]	@name=N'MS_Description', @level0type=N'SCHEMA',
	@level0name=N'dbo',	@level1type=N'TABLE', @level2type=N'COLUMN',
	@level1name=N'Nu65Table',
	@level2name=N'SERVICE_LAST_MODIFY_USER',
	@value=N'Последний пользователь изменявший запись'
GO

-- Вторичный ключ на таблицу [Продуктов] 
ALTER TABLE [dbo].[Nu65Table] WITH CHECK
ADD CONSTRAINT [FK_Nu65TableToProducts] FOREIGN KEY([productsId])
REFERENCES [dbo].[Products] ([id])
	ON UPDATE NO ACTION
	ON DELETE NO ACTION
GO
ALTER TABLE [dbo].[Nu65Table] CHECK CONSTRAINT [FK_Nu65TableToProducts]
GO

-- Вторичный ключ на таблицу [Материалов] 
ALTER TABLE [dbo].[Nu65Table] WITH CHECK
ADD CONSTRAINT [FK_Nu65TableToMaterials] FOREIGN KEY([materialsId])
REFERENCES [dbo].[Materials] ([id])
	ON UPDATE NO ACTION
	ON DELETE NO ACTION
GO
ALTER TABLE [dbo].[Nu65Table] CHECK CONSTRAINT [FK_Nu65TableToMaterials]
GO

-- Вторичный ключ на таблицу [Ед. изм.] 
ALTER TABLE [dbo].[Nu65Table] WITH CHECK
ADD CONSTRAINT [FK_Nu65TableToMeasures] FOREIGN KEY([measuresId])
REFERENCES [dbo].[Measures] ([id])
	ON UPDATE NO ACTION
	ON DELETE NO ACTION
GO
ALTER TABLE [dbo].[Nu65Table] CHECK CONSTRAINT [FK_Nu65TableToMeasures]
GO

-- Триггер обновления даты-времени последнего редактирования и пользователя изменившего запись последним
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


