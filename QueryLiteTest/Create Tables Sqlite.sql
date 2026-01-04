CREATE TABLE AllTypes (	
	taId INTEGER CONSTRAINT pk_TableA PRIMARY KEY AUTOINCREMENT NOT NULL,
    taGuid BLOB NOT NULL,
    taString TEXT NOT NULL,
    taSmallInt INTEGER NOT NULL,
    taInt INTEGER NOT NULL,
    taBigInt INTEGER NOT NULL,
    taDecimal TEXT NOT NULL,
    taFloat REAL NOT NULL,
    taDouble REAL NOT NULL,
    taBoolean INTEGER NOT NULL,
    taBytes BLOB NOT NULL,
    taDateTime TEXT NOT NULL,
    taDateTimeOffset TEXT NOT NULL,
    taEnum INTEGER NOT NULL,
    taDateOnly TEXT NOT NULL,
    taTimeOnly TEXT NOT NULL,

    CONSTRAINT pk_TableA PRIMARY KEY(taId)
) STRICT;

--COMMENT ON TABLE alltypes IS 'All types table used for automated testing';

CREATE TABLE CustomTypes (

    ctGuid BLOB NOT NULL,
    ctIdentifier INTEGER PRIMARY KEY AUTOINCREMENT,
    ctShort INTEGER NOT NULL,
    ctInt INTEGER NOT NULL,
    ctLong INTEGER NOT NULL,
    ctString TEXT NOT NULL,
    ctBool INTEGER NOT NULL,
    
    ctDecimal TEXT NOT NULL,
    ctDateTime TEXT NOT NULL,
    ctDateTimeOffset TEXT NOT NULL,
    ctDateOnly TEXT NOT NULL,
    ctTimeOnly TEXT NOT NULL,
    ctFloat REAL NOT NULL,
    ctDouble REAL NOT NULL,

    ctNGuid BLOB NULL,
    ctNShort INTEGER NULL,
    ctNInt INTEGER NULL,
    ctNLong INTEGER NULL,
    ctNString TEXT NULL,
    ctNBool INTEGER NULL,
    ctNDecimal TEXT NULL,
    ctNDateTime TEXT NULL,
    ctNDateTimeOffset TEXT NULL,
    ctNDateOnly TEXT NULL,
    ctNTimeOnly TEXT NULL,
    ctNFloat REAL NULL,
    ctNDouble REAL NULL
) STRICT;

CREATE TABLE Parent (
    Id BLOB NOT NULL,
    Id2 BLOB NOT NULL,
    CONSTRAINT pk_Parent PRIMARY KEY (Id),
    CONSTRAINT unq_Parent UNIQUE (Id2)
);

CREATE TABLE Child (
    Id BLOB NOT NULL,
    ParentId BLOB NOT NULL,
    CONSTRAINT pk_Child PRIMARY KEY (Id),
    CONSTRAINT fk_Child_Parent FOREIGN KEY (ParentId) REFERENCES Parent (Id),
    CONSTRAINT fk_Child_Parent_Id2 FOREIGN KEY (ParentId) REFERENCES Parent (Id2)
) STRICT;

CREATE TABLE EnumTestTable (

	etByteEnum INTEGER NOT NULL,	
	etShortEnum INTEGER NOT NULL,	
	etIntEnum INTEGER NOT NULL,	
	etLongEnum INTEGER NOT NULL,
	etByteNullEnum INTEGER NULL,	
	etShortNullEnum INTEGER NULL,	
	etIntNullEnum INTEGER NULL,	
	etLongNullEnum INTEGER NULL
) STRICT;

CREATE TABLE JsonTable (

	id BLOB NOT NULL,
	detail TEXT NOT NULL,

	CONSTRAINT pk_JsonTable PRIMARY KEY(id)
) STRICT;