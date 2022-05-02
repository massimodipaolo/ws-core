use [ws-core]

create table [User](
	Id int primary key clustered identity(1,1),
	[Name] nvarchar(255),
	Username nvarchar(255),
	Email varchar(255),
	[Address] nvarchar(max),
	Phone nvarchar(255),
	Website nvarchar(255),
	Company nvarchar(max)
	)
go

create table Post(
	Id int primary key clustered identity(1,1),
	UserId int,
	Title nvarchar(255),
	Body nvarchar(4000)
)
go

create table Comment(
	Id int primary key clustered identity(1,1),
	PostId int,
	[Name] nvarchar(255),
	Email varchar(255),
	Body nvarchar(4000)
)
go

create table Album(
	Id int primary key clustered identity(1,1),
	UserId int,
	Title nvarchar(255)
)
go

create table Photo(
	Id int primary key clustered identity(1,1),
	AlbumId int,
	Title nvarchar(255),
	[Url] nvarchar(255),
	ThumbnailUrl nvarchar(255)
)
go

create table Todo(
	Id int primary key clustered identity(1,1),
	UserId int,
	Title nvarchar(255),
	Completed bit
)
go