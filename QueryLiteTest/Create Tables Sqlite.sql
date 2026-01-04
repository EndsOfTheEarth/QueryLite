
CREATE TABLE AllTypes (
	
	taId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    taGuid BLOB NOT NULL,
    taString TEXT NOT NULL,
    taSmallInt INTEGER NOT NULL,
    taInt INTEGER NOT NULL,
    taBigInt INTEGER NOT NULL,
    taDecimal NUMERIC NOT NULL,
    taFloat REAL NOT NULL,
    taDouble REAL NOT NULL,
    taBoolean INTEGER NOT NULL,
    taBytes BLOB NOT NULL,
    taDateTime TEXT NOT NULL,
    taDateTimeOffset TEXT NOT NULL,
    taEnum INTEGER NOT NULL,
    taDateOnly TEXT NOT NULL,
    taTimeOnly TEXT NOT NULL
);

--COMMENT ON TABLE alltypes IS 'All types table used for automated testing';

--CREATE TABLE Parent (
--	Id UUID NOT NULL,
--	Id2 UUID NOT NULL,
--	CONSTRAINT pk_Parent PRIMARY KEY(Id),
--	CONSTRAINT unq_Parent UNIQUE(Id2)
--);

--CREATE TABLE Child (
--	Id UUID NOT NULL,
--	ParentId UUID NOT NULL,
--	CONSTRAINT pk_Child PRIMARY KEY(Id),
--	CONSTRAINT fk_Child_Parent FOREIGN KEY(ParentId) REFERENCES Parent(Id),
--	CONSTRAINT fk_Child_Parent_Id2 FOREIGN KEY(ParentId) REFERENCES Parent(Id2)
--);

--CREATE TABLE EnumTestTable (

--	etByteEnum SMALLINT NOT NULL,	
--	etShortEnum SMALLINT NOT NULL,	
--	etIntEnum INTEGER NOT NULL,	
--	etLongEnum BIGINT NOT NULL,
--	etByteNullEnum SMALLINT NULL,	
--	etShortNullEnum SMALLINT NULL,	
--	etIntNullEnum INTEGER NULL,	
--	etLongNullEnum BIGINT NULL
--);

--CREATE TABLE CustomTypes (

--	ctGuid UUID NOT NULL,
--	ctIdentifier BIGSERIAL NOT NULL,
--	ctShort SMALLINT NOT NULL,
--	ctInt INTEGER NOT NULL,
--	ctLong BIGINT NOT NULL,
--	ctString VARCHAR(100) NOT NULL,
--	ctBool BOOLEAN NOT NULL,
--	ctDecimal DECIMAL(19,8) NOT NULL,
--	ctDateTime TIMESTAMP WITHOUT TIME ZONE NOT NULL,
--	ctDateTimeOffset TIMESTAMP WITH TIME ZONE NOT NULL,
--	ctDateOnly DATE NOT NULL,
--	ctTimeOnly TIME WITHOUT TIME ZONE NOT NULL,	
--	ctFloat REAL NOT NULL,
--	ctDouble DOUBLE PRECISION NOT NULL,

--	ctNGuid UUID NULL,
--	ctNShort SMALLINT NULL,
--	ctNInt INTEGER NULL,
--	ctNLong BIGINT NULL,
--	ctNString VARCHAR(100) NULL,
--	ctNBool BOOLEAN NULL,
--	ctNDecimal DECIMAL(19,8) NULL,
--	ctNDateTime TIMESTAMP WITHOUT TIME ZONE NULL,
--	ctNDateTimeOffset TIMESTAMP WITH TIME ZONE NULL,
--	ctNDateOnly DATE NULL,
--	ctNTimeOnly TIME WITHOUT TIME ZONE NULL,
--	ctNFloat REAL NULL,
--	ctNDouble DOUBLE PRECISION NULL
--);

--CREATE TABLE JsonTable (

--	id UUID NOT NULL,
--	detail JSONB NOT NULL,

--	CONSTRAINT pk_JsonTable PRIMARY KEY(id)
--);