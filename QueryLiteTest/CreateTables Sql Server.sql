﻿
CREATE TABLE AllTypes (
	
	taId INT IDENTITY NOT NULL,
	taGuid UNIQUEIDENTIFIER NOT NULL,
	taString NVARCHAR(100) NOT NULL,
	taSmallInt SMALLINT NOT NULL,
	taInt INT NOT NULL,
	taBigInt BIGINT NOT NULL,
	taDecimal DECIMAL(19, 8) NOT NULL,
	taFloat REAL NOT NULL,
	taDouble FLOAT NOT NULL,
	taBoolean TINYINT NOT NULL,
	taBytes VARBINARY(MAX) NOT NULL,
	taDateTime DATETIME NOT NULL,
	taDateTimeOffset DATETIMEOFFSET NOT NULL,
	taEnum SMALLINT NOT NULL,
	taDateOnly DATE NOT NULL,

	CONSTRAINT pk_TableA PRIMARY KEY(taId)
);