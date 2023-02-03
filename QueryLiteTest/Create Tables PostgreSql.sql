﻿
CREATE TABLE AllTypes (
	
	taId SERIAL NOT NULL,
	taGuid UUID NOT NULL,
	taString VARCHAR(100) NOT NULL,
	taSmallInt SMALLINT NOT NULL,
	taInt INT NOT NULL,
	taBigInt BIGINT NOT NULL,
	taDecimal DECIMAL(19, 8) NOT NULL,
	taFloat REAL NOT NULL,
	taDouble DOUBLE PRECISION NOT NULL,
	taBoolean BOOLEAN NOT NULL,
	taBytes BYTEA NOT NULL,
	taDateTime TIMESTAMP WITHOUT TIME ZONE NOT NULL,
	taDateTimeOffset TIMESTAMP WITH TIME ZONE NOT NULL,
	taEnum SMALLINT NOT NULL,
	taDateOnly DATE NOT NULL,

	CONSTRAINT pk_TableA PRIMARY KEY(taId)
);