
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

	etByteEnum INTEGER NOT NULL,	
	etShortEnum INTEGER NOT NULL,	
	etIntEnum INTEGER NOT NULL,	
	etLongEnum INTEGER NOT NULL,
	etByteNullEnum INTEGER NULL,	
	etShortNullEnum INTEGER NULL,	
	etIntNullEnum INTEGER NULL,	
	etLongNullEnum INTEGER NULL
);

CREATE TABLE CustomTypes (

    ctGuid TEXT NOT NULL,
    ctIdentifier INTEGER PRIMARY KEY AUTOINCREMENT,
    ctShort INTEGER NOT NULL,
    ctInt INTEGER NOT NULL,
    ctLong INTEGER NOT NULL,
    ctString TEXT NOT NULL,
    ctBool INTEGER NOT NULL,
    
    ctDecimal NUMERIC NOT NULL,
    ctDateTime TEXT NOT NULL,
    ctDateTimeOffset TEXT NOT NULL,
    ctDateOnly TEXT NOT NULL,
    ctTimeOnly TEXT NOT NULL,
    ctFloat REAL NOT NULL,
    ctDouble REAL NOT NULL,

    ctNGuid TEXT NULL,
    ctNShort INTEGER NULL,
    ctNInt INTEGER NULL,
    ctNLong INTEGER NULL,
    ctNString TEXT NULL,
    ctNBool INTEGER NULL,
    ctNDecimal NUMERIC NULL,
    ctNDateTime TEXT NULL,
    ctNDateTimeOffset TEXT NULL,
    ctNDateOnly TEXT NULL,
    ctNTimeOnly TEXT NULL,
    ctNFloat REAL NULL,
    ctNDouble REAL NULL,
);

--CREATE TABLE JsonTable (

--	id UUID NOT NULL,
--	detail JSONB NOT NULL,

--	CONSTRAINT pk_JsonTable PRIMARY KEY(id)
--);