CREATE TABLE [dbo].[PortalUser](
                                   [Id] [nvarchar](100) NOT NULL,
                                   [UserName] [nvarchar](256) NULL,
                                   [NormalizedUserName] [nvarchar](256) NULL,
                                   [Email] [nvarchar](256) NULL,
                                   [NormalizedEmail] [nvarchar](256) NULL,
                                   [EmailConfirmed] [bit] NOT NULL,
                                   [PasswordHash] [nvarchar](max) NULL,
                                   [SecurityStamp] [nvarchar](max) NULL,
                                   [ConcurrencyStamp] [nvarchar](max) NULL,
                                   [PhoneNumber] [nvarchar](max) NULL,
                                   [PhoneNumberConfirmed] [bit] NOT NULL,
                                   [TwoFactorEnabled] [bit] NOT NULL,
                                   [LockoutEnd] [datetimeoffset](7) NULL,
                                   [LockoutEnabled] [bit] NOT NULL,
                                   [AccessFailedCount] [int] NOT NULL,
                                   [FirstName] [nvarchar](50) NOT NULL,
                                   [LastName] [nvarchar](50) NOT NULL,
                                   [DateOfBirth] [date] NOT NULL,
                                   [StreetAddress] [nvarchar](100) NULL,
                                   [City] [nvarchar](50) NULL,
                                   [State] [nvarchar](50) NULL,
                                   [PostalCode] [nvarchar](10) NULL,
                                   CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
)

GO

ALTER TABLE [dbo].[PortalUser] ADD CONSTRAINT [CK_PortalUser_FirstName] CHECK (LEN([FirstName]) > 0)
ALTER TABLE [dbo].[PortalUser] ADD CONSTRAINT [CK_PortalUser_LastName] CHECK (LEN([LastName]) > 0)
ALTER TABLE [dbo].[PortalUser] ADD CONSTRAINT [CK_PortalUser_DateOfBirth] CHECK ([DateOfBirth] <= GETDATE())

alter table PortalUser add Avatar nvarchar(256),
    Provider nvarchar(20),
    SuperAdmin bit,
    RoleId int,
    GoogleUserId nvarchar(100),
    FacebookUserId nvarchar(100),
    CreatedAt datetime,
    Otp int,
    OtpExpiryAt datetime,
    LastLoginAt datetime;
    