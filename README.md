# IOT-Home-Security
Kaloyan Dinev [kddinev18] All Rights Reserved.

This repository and its contents are protected by copyright law. You are not permitted to copy, modify, or distribute the repository or its contents in any way without express written permission from Kaloyan Dinev [kddinev18].


За стартиране на приложението трябва да отворите приложенеито в Visual Studio. Трябва да конфигурирате стартиращите проекти да са: BridgeAPI, WebApp, LocalSerevrGUI. Трябва да имате следните бази създадени: "IOTHomeSecurity", таблиците в тази база се зъдават автоматично от персонализирания обектно релационен картограф и "BridgeAPI", таблиците в тази база се създават от EFCore. За да се създаде таблиците в "BridgeAPI" трябва да конфигурирате сартиращия проект да е BridgeAPI.DLL и в "Package-Manager console" да се въведе командата "Update-Database".

За да може да използвате приложението трябва да се регистрирате в сайта и в локалия сървър. След което трябва са се вържете към BridgeAPI-то в локания сървър и в локания сървър от уеб сайта. След регистрацията ви в локания сървър, ще излезе форма, с която трябва да се свържете в BridgeAPI. Там въвежадте креденциалите, с които сте се вписали в уеб приложението. След което, потребителя трябва да се свърже с локания сървър от уеб сайта, като отново след вписването на потребителя в уеб сайта, на потребителя ще се покаже форма, в която потребителя трябва да въведе креденциалите си, с които той се е регистрирал в локания сървър. 

Ако нямате esp устройство, което е конфигурирано да работи с апликацията, трябва да бъде симулирано такова. За целта трябва да се изпълне следната заявка:
USE [IOTHomeSecurity]
GO

INSERT INTO [dbo].[Devices]
           ([Id]
           ,[IPv4Address]
           ,[Name]
           ,[IsAprooved])
     VALUES
           (NEWID()
           ,'127.0.0.1'
           ,'Temperature'
           ,'true')
GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Temperature](
	[Id] [nvarchar](36) NOT NULL,
	[Created] [datetime2](7) NOT NULL,
	[DeviceId] [nvarchar](36) NOT NULL,
	[Temperature] [int] NOT NULL,
 CONSTRAINT [PK_Temperature] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Temperature]  WITH CHECK ADD  CONSTRAINT [FK_Temperature_Devices] FOREIGN KEY([DeviceId])
REFERENCES [dbo].[Devices] ([Id])
GO

ALTER TABLE [dbo].[Temperature] CHECK CONSTRAINT [FK_Temperature_Devices]
GO

След каот сте създали таблицата Temperature трябва да изпълнете следната заявка:

USE [IOTHomeSecurity]
GO

INSERT INTO [dbo].[Permissions]
           ([RoleId]
           ,[DeviceId]
           ,[CanCreate]
           ,[CanRead]
           ,[CanUpdate]
           ,[CanDelete])
     VALUES
           ('ID на ролята админ'
           ,'ID на устройството, което създадохме преди малко'
           ,'true'
           ,'true'
           ,'true'
           ,'true')
GO

INSERT INTO [dbo].[Temperature]
           ([Id]
           ,[Created]
           ,[DeviceId]
           ,[Temperature])
     VALUES
           (NEWID(),GETDATE(),'ID на устройството, което създадохме преди малко',15),
		   (NEWID(),GETDATE(),'ID на устройството, което създадохме преди малко',14),
		   (NEWID(),GETDATE(),'ID на устройството, което създадохме преди малко',13),
		   (NEWID(),GETDATE(),'ID на устройството, което създадохме преди малко',12),
		   (NEWID(),GETDATE(),'ID на устройството, което създадохме преди малко',11),
		   (NEWID(),GETDATE(),'ID на устройството, което създадохме преди малко',10),
		   (NEWID(),GETDATE(),'ID на устройството, което създадохме преди малко',9),
		   (NEWID(),GETDATE(),'ID на устройството, което създадохме преди малко',8),
		   (NEWID(),GETDATE(),'ID на устройството, което създадохме преди малко',7),
		   (NEWID(),GETDATE(),'ID на устройството, което създадохме преди малко',6),
		   (NEWID(),GETDATE(),'ID на устройството, което създадохме преди малко',5),
		   (NEWID(),GETDATE(),'ID на устройството, което създадохме преди малко',4)
GO

Като естествено заменяте "ID на устройството, което създадохме преди малко" с Id-то на устройството, което създадохме с предишната заявка и "ID на ролята админ" с Id-то на ролята админ, която можете да намерите в таблицата "Roles".

След като сте готови рестартирайте апликацията. Изпълнете процеса на аутентикация отново и всичко трябва да работи. Ако има някакви проблеми при стартирането моля свържете се с kddinev18.
