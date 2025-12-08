------------------------------------------------------------
-- 1. Create schemas if they do not exist
------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Autenticacion')
BEGIN
    EXEC('CREATE SCHEMA Autenticacion');
END;
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Autorizacion')
BEGIN
    EXEC('CREATE SCHEMA Autorizacion');
END;
GO

------------------------------------------------------------
-- 2. AUTHORIZATION TABLES (schema: Autorizacion)
------------------------------------------------------------

------------------------------------------------------------
-- 2.1 Applications
------------------------------------------------------------
IF OBJECT_ID('Autorizacion.Applications', 'U') IS NOT NULL
    DROP TABLE Autorizacion.Applications;
GO

CREATE TABLE Autorizacion.Applications (
    Id           INT IDENTITY(1,1) CONSTRAINT PK_Applications PRIMARY KEY,
    Code         NVARCHAR(25)  NOT NULL,     -- short code, e.g. "ATACADO"
    Description         NVARCHAR(250) NOT NULL,     -- descriptive name
    Url          NVARCHAR(250) NULL,
    Icon         NVARCHAR(50)  NULL,
    RecordStatus INT           NOT NULL DEFAULT 1,
    CreatedAt    DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy    NVARCHAR(100) NOT NULL DEFAULT SYSTEM_USER
);
GO

------------------------------------------------------------
-- 2.2 Users (AUTHENTICATION USERS: schema Autenticacion)
--     (se crea aquí por dependencia de FKs de Autorizacion.RoleUsers)
------------------------------------------------------------
IF OBJECT_ID('Autenticacion.Users', 'U') IS NOT NULL
    DROP TABLE Autenticacion.Users;
GO

CREATE TABLE Autenticacion.Users (
    Id                 INT IDENTITY(1,1) CONSTRAINT PK_Users PRIMARY KEY,
    ExternalUserId     INT             NULL,      -- optional link to another system

    Username           NVARCHAR(50)    NOT NULL,
    Email              NVARCHAR(150)   NULL,

    PasswordHash       NVARCHAR(300)   NOT NULL,
    LastPasswordChange DATETIME2       NULL,

    IsLocked           BIT             NOT NULL DEFAULT 0,
    LockDate           DATETIME2       NULL,

    IsNewUser          BIT             NOT NULL DEFAULT 0,
    KeepLoggedIn       BIT             NOT NULL DEFAULT 0,

    RecordStatus       INT             NOT NULL DEFAULT 1,
    CreatedAt          DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy          NVARCHAR(100)   NOT NULL DEFAULT SYSTEM_USER,

    CONSTRAINT UQ_Users_Username UNIQUE (Username),
    CONSTRAINT UQ_Users_Email    UNIQUE (Email)
);
GO

------------------------------------------------------------
-- 2.3 Roles (per Application)
------------------------------------------------------------
IF OBJECT_ID('Autorizacion.Roles', 'U') IS NOT NULL
    DROP TABLE Autorizacion.Roles;
GO

CREATE TABLE Autorizacion.Roles (
    Id            INT IDENTITY(1,1) CONSTRAINT PK_Roles PRIMARY KEY,
    ApplicationId INT           NOT NULL,
    Name          NVARCHAR(50)  NOT NULL,
    Description   NVARCHAR(100) NOT NULL,
    RecordStatus  INT           NOT NULL DEFAULT 1,
    CreatedAt     DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy     NVARCHAR(100) NOT NULL DEFAULT SYSTEM_USER,

    CONSTRAINT FK_Roles_Applications FOREIGN KEY (ApplicationId)
        REFERENCES Autorizacion.Applications(Id),

    CONSTRAINT UQ_Roles_Application_Name UNIQUE (ApplicationId, Name)
);
GO

------------------------------------------------------------
-- 2.4 Resources
------------------------------------------------------------
IF OBJECT_ID('Autorizacion.Resources', 'U') IS NOT NULL
    DROP TABLE Autorizacion.Resources;
GO

CREATE TABLE Autorizacion.Resources (
    Id            INT IDENTITY(1,1) CONSTRAINT PK_Resources PRIMARY KEY,
    ApplicationId INT           NOT NULL,
    Page          NVARCHAR(50)  NULL,          -- route or page name
    Name          NVARCHAR(100) NOT NULL,      -- short description
    Description   NVARCHAR(350) NOT NULL,      -- detailed description
    ResourceType  INT           NOT NULL,      -- 1: Node, 2: Page, etc.
    IconName      NVARCHAR(100) NOT NULL,
    IsNew         BIT           NOT NULL DEFAULT 0,
    RecordStatus  INT           NOT NULL DEFAULT 1,
    CreatedAt     DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy     NVARCHAR(100) NOT NULL DEFAULT SYSTEM_USER,

    CONSTRAINT FK_Resources_Applications FOREIGN KEY (ApplicationId)
        REFERENCES Autorizacion.Applications(Id)
);
GO

------------------------------------------------------------
-- 2.5 ResourceMenus
------------------------------------------------------------
IF OBJECT_ID('Autorizacion.ResourceMenus', 'U') IS NOT NULL
    DROP TABLE Autorizacion.ResourceMenus;
GO

CREATE TABLE Autorizacion.ResourceMenus (
    Id           INT IDENTITY(1,1) CONSTRAINT PK_ResourceMenus PRIMARY KEY,
    ResourceId   INT           NOT NULL,
    Level        INT           NOT NULL,
    IndentLevel  INT           NOT NULL,
    RecordStatus INT           NOT NULL DEFAULT 1,
    CreatedAt    DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy    NVARCHAR(100) NOT NULL DEFAULT SYSTEM_USER,

    CONSTRAINT FK_ResourceMenus_Resources FOREIGN KEY (ResourceId)
        REFERENCES Autorizacion.Resources(Id)
    -- If you want each resource to appear only once in the menu:
    -- ,CONSTRAINT UQ_ResourceMenus_Resource UNIQUE (ResourceId)
);
GO

------------------------------------------------------------
-- 2.6 ResourceEndpoints
------------------------------------------------------------
IF OBJECT_ID('Autorizacion.ResourceEndpoints', 'U') IS NOT NULL
    DROP TABLE Autorizacion.ResourceEndpoints;
GO

CREATE TABLE Autorizacion.ResourceEndpoints (
    Id           INT IDENTITY(1,1) CONSTRAINT PK_ResourceEndpoints PRIMARY KEY,
    ResourceId   INT           NOT NULL,
    ServiceType  INT           NOT NULL,          -- e.g. 1=GET, 2=POST or custom code
    Endpoint     NVARCHAR(350) NOT NULL,          -- route / URL / action
    Description  NVARCHAR(500) NOT NULL,
    RecordStatus INT           NOT NULL DEFAULT 1,
    CreatedAt    DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy    NVARCHAR(100) NOT NULL DEFAULT SYSTEM_USER,

    CONSTRAINT FK_ResourceEndpoints_Resources FOREIGN KEY (ResourceId)
        REFERENCES Autorizacion.Resources(Id)
);
GO

------------------------------------------------------------
-- 2.7 RoleUsers
------------------------------------------------------------
IF OBJECT_ID('Autorizacion.RoleUsers', 'U') IS NOT NULL
    DROP TABLE Autorizacion.RoleUsers;
GO

CREATE TABLE Autorizacion.RoleUsers (
    Id           INT IDENTITY(1,1) CONSTRAINT PK_RoleUsers PRIMARY KEY,
    RoleId       INT           NOT NULL,
    UserId       INT           NOT NULL,
    IsInspector  BIT           NOT NULL DEFAULT 0,  -- agregado
    RecordStatus INT           NOT NULL DEFAULT 1,
    CreatedAt    DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy    NVARCHAR(100) NOT NULL DEFAULT SYSTEM_USER,

    CONSTRAINT FK_RoleUsers_Roles FOREIGN KEY (RoleId)
        REFERENCES Autorizacion.Roles(Id),

    CONSTRAINT FK_RoleUsers_Users FOREIGN KEY (UserId)
        REFERENCES Autenticacion.Users(Id),

    CONSTRAINT UQ_RoleUsers_Role_User UNIQUE (RoleId, UserId)
);
GO

------------------------------------------------------------
-- 2.8 RoleResourceMenus
------------------------------------------------------------
IF OBJECT_ID('Autorizacion.RoleResourceMenus', 'U') IS NOT NULL
    DROP TABLE Autorizacion.RoleResourceMenus;
GO

CREATE TABLE Autorizacion.RoleResourceMenus (
    Id           INT IDENTITY(1,1) CONSTRAINT PK_RoleResourceMenus PRIMARY KEY,
    RoleId       INT           NOT NULL,
    ResourceId   INT           NOT NULL,
    RecordStatus INT           NOT NULL DEFAULT 1,
    CreatedAt    DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy    NVARCHAR(100) NOT NULL DEFAULT SYSTEM_USER,

    CONSTRAINT FK_RoleResourceMenus_Roles FOREIGN KEY (RoleId)
        REFERENCES Autorizacion.Roles(Id),

    CONSTRAINT FK_RoleResourceMenus_Resources FOREIGN KEY (ResourceId)
        REFERENCES Autorizacion.Resources(Id),

    CONSTRAINT UQ_RoleResourceMenus_Role_Resource UNIQUE (RoleId, ResourceId)
);
GO

------------------------------------------------------------
-- 2.9 RoleEndpoints
------------------------------------------------------------
IF OBJECT_ID('Autorizacion.RoleEndpoints', 'U') IS NOT NULL
    DROP TABLE Autorizacion.RoleEndpoints;
GO

CREATE TABLE Autorizacion.RoleEndpoints (
    Id                 INT IDENTITY(1,1) CONSTRAINT PK_RoleEndpoints PRIMARY KEY,
    RoleId             INT           NOT NULL,
    ResourceEndpointId INT           NOT NULL,
    ServiceType        INT           NULL,          -- agregado
    Endpoint           NVARCHAR(500) NULL,          -- agregado
    PageName           NVARCHAR(350) NULL,          -- agregado
    RecordStatus       INT           NOT NULL DEFAULT 1,
    CreatedAt          DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy          NVARCHAR(100) NOT NULL DEFAULT SYSTEM_USER,

    CONSTRAINT FK_RoleEndpoints_Roles FOREIGN KEY (RoleId)
        REFERENCES Autorizacion.Roles(Id),

    CONSTRAINT FK_RoleEndpoints_ResourceEndpoints FOREIGN KEY (ResourceEndpointId)
        REFERENCES Autorizacion.ResourceEndpoints(Id),

    CONSTRAINT UQ_RoleEndpoints_Role_Endpoint UNIQUE (RoleId, ResourceEndpointId)
);
GO

------------------------------------------------------------
-- 3. AUTHENTICATION TABLES (schema: Autenticacion)
------------------------------------------------------------

------------------------------------------------------------
-- 3.1 RefreshTokens
------------------------------------------------------------
IF OBJECT_ID('Autenticacion.RefreshTokens', 'U') IS NOT NULL
    DROP TABLE Autenticacion.RefreshTokens;
GO

CREATE TABLE Autenticacion.RefreshTokens (
    Id               UNIQUEIDENTIFIER CONSTRAINT PK_RefreshTokens PRIMARY KEY,
    UserId           INT           NOT NULL,
    TokenHash        NVARCHAR(500) NOT NULL,   -- hashed or encrypted refresh token
    ExpiresAt        DATETIME2     NOT NULL,
    Used             BIT           NOT NULL DEFAULT 0,
    Revoked          BIT           NOT NULL DEFAULT 0,
    TokenCreatedAt   DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    IPAddress        NVARCHAR(100) NULL,
    UserAgent        NVARCHAR(500) NULL,
    RecordStatus     INT           NOT NULL DEFAULT 1,
    CreatedAt        DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy        NVARCHAR(100) NOT NULL DEFAULT SYSTEM_USER,

    CONSTRAINT FK_RefreshTokens_Users FOREIGN KEY (UserId)
        REFERENCES Autenticacion.Users(Id)
);
GO

CREATE INDEX IX_RefreshTokens_UserId
    ON Autenticacion.RefreshTokens (UserId);
GO

------------------------------------------------------------
-- 3.2 LoginAttempts
------------------------------------------------------------
IF OBJECT_ID('Autenticacion.LoginAttempts', 'U') IS NOT NULL
    DROP TABLE Autenticacion.LoginAttempts;
GO

CREATE TABLE Autenticacion.LoginAttempts (
    Id           INT IDENTITY(1,1) CONSTRAINT PK_LoginAttempts PRIMARY KEY,
    UserId       INT           NULL,       -- NULL if user could not be resolved
    Username     NVARCHAR(50)  NOT NULL,
    IPAddress    NVARCHAR(100) NOT NULL,
    UserAgent    NVARCHAR(500) NULL,
    IsSuccessful BIT           NOT NULL DEFAULT 0,
    AttemptedAt  DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    RecordStatus INT           NOT NULL DEFAULT 1,
    CreatedAt    DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy    NVARCHAR(100) NOT NULL DEFAULT SYSTEM_USER,

    CONSTRAINT FK_LoginAttempts_Users FOREIGN KEY (UserId)
        REFERENCES Autenticacion.Users(Id)
);
GO

CREATE INDEX IX_LoginAttempts_Username_AttemptedAt
    ON Autenticacion.LoginAttempts (Username, AttemptedAt);
GO

------------------------------------------------------------
-- 3.3 RevokedTokens
------------------------------------------------------------
IF OBJECT_ID('Autenticacion.RevokedTokens', 'U') IS NOT NULL
    DROP TABLE Autenticacion.RevokedTokens;
GO

CREATE TABLE Autenticacion.RevokedTokens (
    Jti          UNIQUEIDENTIFIER CONSTRAINT PK_RevokedTokens PRIMARY KEY,
    UserId       INT           NOT NULL,
    Reason       NVARCHAR(250) NULL,
    RevokedAt    DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    RecordStatus INT           NOT NULL DEFAULT 1,
    CreatedAt    DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy    NVARCHAR(100) NOT NULL DEFAULT SYSTEM_USER,

    CONSTRAINT FK_RevokedTokens_Users FOREIGN KEY (UserId)
        REFERENCES Autenticacion.Users(Id)
);
GO

CREATE INDEX IX_RevokedTokens_UserId
    ON Autenticacion.RevokedTokens (UserId);
GO

------------------------------------------------------------
-- 3.4 KnownDevices
------------------------------------------------------------
IF OBJECT_ID('Autenticacion.KnownDevices', 'U') IS NOT NULL
    DROP TABLE Autenticacion.KnownDevices;
GO

CREATE TABLE Autenticacion.KnownDevices (
    Id              INT IDENTITY(1,1) CONSTRAINT PK_KnownDevices PRIMARY KEY,
    UserId          INT           NOT NULL,
    FingerprintHash NVARCHAR(300) NOT NULL,
    DeviceName      NVARCHAR(100) NULL,
    UserAgent       NVARCHAR(500) NULL,
    IPAddress       NVARCHAR(100) NULL,
    RecordStatus    INT           NOT NULL DEFAULT 1,
    CreatedAt       DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy       NVARCHAR(100) NOT NULL DEFAULT SYSTEM_USER,

    CONSTRAINT FK_KnownDevices_Users FOREIGN KEY (UserId)
        REFERENCES Autenticacion.Users(Id),

    CONSTRAINT UQ_KnownDevices_User_Fingerprint UNIQUE (UserId, FingerprintHash)
);
GO

------------------------------------------------------------
-- 3.5 LoginAudit
------------------------------------------------------------
IF OBJECT_ID('Autenticacion.LoginAudit', 'U') IS NOT NULL
    DROP TABLE Autenticacion.LoginAudit;
GO

CREATE TABLE Autenticacion.LoginAudit (
    Id           INT IDENTITY(1,1) CONSTRAINT PK_LoginAudit PRIMARY KEY,
    UserId       INT           NULL,
    Username     NVARCHAR(50)  NULL,
    IPAddress    NVARCHAR(100) NULL,
    UserAgent    NVARCHAR(500) NULL,
    IsSuccessful BIT           NULL,
    Message      NVARCHAR(250) NULL,
    LoggedAt     DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    RecordStatus INT           NOT NULL DEFAULT 1,
    CreatedAt    DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy    NVARCHAR(100) NOT NULL DEFAULT SYSTEM_USER,

    CONSTRAINT FK_LoginAudit_Users FOREIGN KEY (UserId)
        REFERENCES Autenticacion.Users(Id)
);
GO

CREATE INDEX IX_LoginAudit_Username_LoggedAt
    ON Autenticacion.LoginAudit (Username, LoggedAt);
GO

------------------------------------------------------------
-- 3.6 CryptoKeys
------------------------------------------------------------
IF OBJECT_ID('Autenticacion.CryptoKeys', 'U') IS NOT NULL
    DROP TABLE Autenticacion.CryptoKeys;
GO

CREATE TABLE Autenticacion.CryptoKeys (
    Id                  INT IDENTITY(1,1) CONSTRAINT PK_CryptoKeys PRIMARY KEY,

    Name                NVARCHAR(100) NOT NULL,  -- e.g. 'Auth.JwtMain', 'Auth.Refresh'
    KeyType             TINYINT       NOT NULL,  -- 1=RSA signing, 2=RSA encryption, 3=AES, etc.
    Version             INT           NOT NULL DEFAULT 1,

    ApplicationId       INT           NULL,      -- optional: key associated to an application

    PublicKeyPem        NVARCHAR(MAX)   NOT NULL,  -- -----BEGIN PUBLIC KEY-----
    EncryptedPrivateKey VARBINARY(MAX)  NOT NULL,  -- private key encrypted

    IsActive            BIT           NOT NULL DEFAULT 1,
    StartDate           DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    EndDate             DATETIME2     NULL,

    Thumbprint          NVARCHAR(128) NULL,      -- key fingerprint / hash

    RecordStatus        INT           NOT NULL DEFAULT 1,
    CreatedAt           DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy           NVARCHAR(100) NOT NULL DEFAULT SYSTEM_USER,

    CONSTRAINT FK_CryptoKeys_Applications FOREIGN KEY (ApplicationId)
        REFERENCES Autorizacion.Applications(Id),

    CONSTRAINT UQ_CryptoKeys_Name_Version UNIQUE (Name, Version)
);
GO
