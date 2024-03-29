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
	taTimeOnly TIME WITHOUT TIME ZONE NOT NULL,

	CONSTRAINT pk_TableA PRIMARY KEY(taId)
);

COMMENT ON TABLE alltypes IS 'All types table used for automated testing';

CREATE TABLE Parent (
	Id UUID NOT NULL,
	Id2 UUID NOT NULL,
	CONSTRAINT pk_Parent PRIMARY KEY(Id),
	CONSTRAINT unq_Parent UNIQUE(Id2)
);

CREATE TABLE Child (
	Id UUID NOT NULL,
	ParentId UUID NOT NULL,
	CONSTRAINT pk_Child PRIMARY KEY(Id),
	CONSTRAINT fk_Child_Parent FOREIGN KEY(ParentId) REFERENCES Parent(Id),
	CONSTRAINT fk_Child_Parent_Id2 FOREIGN KEY(ParentId) REFERENCES Parent(Id2)
);

CREATE TABLE EnumTestTable (

	etByteEnum SMALLINT NOT NULL,	
	etShortEnum SMALLINT NOT NULL,	
	etIntEnum INTEGER NOT NULL,	
	etLongEnum BIGINT NOT NULL,
	etByteNullEnum SMALLINT NULL,	
	etShortNullEnum SMALLINT NULL,	
	etIntNullEnum INTEGER NULL,	
	etLongNullEnum BIGINT NULL
);