create table Record (
	ID int IDENTITY(1,1) PRIMARY KEY,
    Label varchar(255) NOT NULL,
    FilePath varchar(255)
);

create table RecordFrame (
	ID int IDENTITY(1,1) PRIMARY KEY,
    Label varchar(255) NOT NULL,
    RecordId int FOREIGN KEY REFERENCES Record(ID)
);

create table Coefficient (
	ID int IDENTITY(1,1) PRIMARY KEY,
    Coefficient float NOT NULL,
    FrameId int FOREIGN KEY REFERENCES RecordFrame(ID)	
)