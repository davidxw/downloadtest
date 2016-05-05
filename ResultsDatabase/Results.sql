CREATE TABLE [dbo].[Results]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[ClientId] NVARCHAR(100),
	[Uri] NVARCHAR(1000) NOT NULL,
	[StartTime] DATETIME NOT NULL,
	[Message] NVARCHAR(1000) NOT NULL,
	[HttpResonse] NVARCHAR(4),
	[DurationInMilliseconds] DECIMAL,
	[SizeInBytes] DECIMAL,
	[Mbps] DECIMAL(15,3) 

)
