------------------------------------------------------------
-- 1. Create schema SECURITY_SYSTEM (if not exists)
------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'SECURITY_SYSTEM')
BEGIN
    EXEC('CREATE SCHEMA SECURITY_SYSTEM');
END;
GO

------------------------------------------------------------
-- 2. Table: Applications
--    (Modules / systems that use this auth engine)
------------------------------------------------------------
CREATE TABLE SECURITY_SYSTEM.Applications (
    Id           INT IDENTITY(1,1) CONSTRAINT PK_Applications PRIMARY KEY,
    Code         NVARCHAR(25)  NOT NULL,     -- short code, e.g. "ATACADO"
    Name         NVARCHAR(250) NOT NULL,     -- descriptive name
    Url          NVARCHAR(250) NULL,
    Icon         NVARCHAR(50)  NULL,
    RecordStatus INT           NOT NULL DEFAULT 1,
    CreatedAt    DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy    NVARCHAR(100) NOT NULL DEFAULT SYSTEM_USER
);
GO

------------------------------------------------------------
-- 3. Table: Users (internal users of the auth system)
------------------------------------------------------------
CREATE TABLE SECURITY_SYSTEM.Users (
    Id               INT IDENTITY(1,1) CONSTRAINT PK_Users PRIMARY KEY,
    ExternalUserId   INT           NULL,      -- optional link to another system

    Username         NVARCHAR(50)  NOT NULL,
    Email            NVARCHAR(150) NULL,

    PasswordHash     NVARCHAR(300) NOT NULL,
    LastPasswordChange DATETIME2   NULL,

    IsLocked         BIT           NOT NULL DEFAULT 0,
    LockDate         DATETIME2     NULL,

    IsNewUser        BIT           NOT NULL DEFAULT 0,
    KeepLoggedIn     BIT           NOT NULL DEFAULT 0,

    RecordStatus     INT           NOT NULL DEFAULT 1,
    CreatedAt        DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy        NVARCHAR(100) NOT NULL DEFAULT SYSTEM_USER,

    CONSTRAINT UQ_Users_Username UNIQUE (Username),
    CONSTRAINT UQ_Users_Email    UNIQUE (Email)
);
GO

------------------------------------------------------------
-- 4. Table: Roles (per application)
------------------------------------------------------------
CREATE TABLE SECURITY_SYSTEM.Roles (
    Id           INT IDENTITY(1,1) CONSTRAINT PK_Roles PRIMARY KEY,
    ApplicationId INT          NOT NULL,
    Name         NVARCHAR(50)  NOT NULL,
    Description  NVARCHAR(100) NOT NULL,
    RecordStatus INT           NOT NULL DEFAULT 1,
    CreatedAt    DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy    NVARCHAR(100) NOT NULL DEFAULT SYSTEM_USER,

    CONSTRAINT FK_Roles_Applications FOREIGN KEY (ApplicationId)
        REFERENCES SECURITY_SYSTEM.Applications(Id),

    CONSTRAINT UQ_Roles_Application_Name UNIQUE (ApplicationId, Name)
);
GO

------------------------------------------------------------
-- 5. Table: Resources
--    (Pages / nodes / components for menu & permissions)
------------------------------------------------------------
CREATE TABLE SECURITY_SYSTEM.Resources (
    Id           INT IDENTITY(1,1) CONSTRAINT PK_Resources PRIMARY KEY,
    ApplicationId INT          NOT NULL,
    Page         NVARCHAR(50)  NULL,          -- route or page name
    Name         NVARCHAR(100) NOT NULL,      -- short description
    Description  NVARCHAR(350) NOT NULL,      -- detailed description
    ResourceType INT           NOT NULL,      -- 1: Node, 2: Page, etc.
    IconName     NVARCHAR(100) NOT NULL,
    IsNew        BIT           NOT NULL DEFAULT 0,
    RecordStatus INT           NOT NULL DEFAULT 1,
    CreatedAt    DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy    NVARCHAR(100) NOT NULL DEFAULT SYSTEM_USER,

    CONSTRAINT FK_Resources_Applications FOREIGN KEY (ApplicationId)
        REFERENCES SECURITY_SYSTEM.Applications(Id)
);
GO

------------------------------------------------------------
-- 6. Table: ResourceMenus
--    (Position of resources in the menu tree)
------------------------------------------------------------
CREATE TABLE SECURITY_SYSTEM.ResourceMenus (
    Id           INT IDENTITY(1,1) CONSTRAINT PK_ResourceMenus PRIMARY KEY,
    ResourceId   INT           NOT NULL,
    Level        INT           NOT NULL,
    IndentLevel  INT           NOT NULL,
    RecordStatus INT           NOT NULL DEFAULT 1,
    CreatedAt    DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy    NVARCHAR(100) NOT NULL DEFAULT SYSTEM_USER,

    CONSTRAINT FK_ResourceMenus_Resources FOREIGN KEY (ResourceId)
        REFERENCES SECURITY_SYSTEM.Resources(Id)
    -- If you want each resource to appear only once in the menu:
    -- ,CONSTRAINT UQ_ResourceMenus_Resource UNIQUE (ResourceId)
);
GO

------------------------------------------------------------
-- 7. Table: ResourceEndpoints
--    (API endpoints / actions linked to a resource)
------------------------------------------------------------
CREATE TABLE SECURITY_SYSTEM.ResourceEndpoints (
    Id           INT IDENTITY(1,1) CONSTRAINT PK_ResourceEndpoints PRIMARY KEY,
    ResourceId   INT           NOT NULL,
    ServiceType  INT           NOT NULL,          -- e.g. 1=GET, 2=POST or custom code
    Endpoint     NVARCHAR(350) NOT NULL,          -- route / URL / action
    Description  NVARCHAR(500) NOT NULL,
    RecordStatus INT           NOT NULL DEFAULT 1,
    CreatedAt    DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy    NVARCHAR(100) NOT NULL DEFAULT SYSTEM_USER,

    CONSTRAINT FK_ResourceEndpoints_Resources FOREIGN KEY (ResourceId)
        REFERENCES SECURITY_SYSTEM.Resources(Id)
);
GO

------------------------------------------------------------
-- 8. Table: RoleUsers
--    (Assignment of roles to users)
------------------------------------------------------------
CREATE TABLE SECURITY_SYSTEM.RoleUsers (
    Id           INT IDENTITY(1,1) CONSTRAINT PK_RoleUsers PRIMARY KEY,
    RoleId       INT           NOT NULL,
    UserId       INT           NOT NULL,
    RecordStatus INT           NOT NULL DEFAULT 1,
    CreatedAt    DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy    NVARCHAR(100) NOT NULL DEFAULT SYSTEM_USER,

    CONSTRAINT FK_RoleUsers_Roles FOREIGN KEY (RoleId)
        REFERENCES SECURITY_SYSTEM.Roles(Id),

    CONSTRAINT FK_RoleUsers_Users FOREIGN KEY (UserId)
        REFERENCES SECURITY_SYSTEM.Users(Id),

    CONSTRAINT UQ_RoleUsers_Role_User UNIQUE (RoleId, UserId)
);
GO

------------------------------------------------------------
-- 9. Table: RoleResourceMenus
--    (Which role can see which resource in the menu)
------------------------------------------------------------
CREATE TABLE SECURITY_SYSTEM.RoleResourceMenus (
    Id           INT IDENTITY(1,1) CONSTRAINT PK_RoleResourceMenus PRIMARY KEY,
    RoleId       INT           NOT NULL,
    ResourceId   INT           NOT NULL,
    RecordStatus INT           NOT NULL DEFAULT 1,
    CreatedAt    DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy    NVARCHAR(100) NOT NULL DEFAULT SYSTEM_USER,

    CONSTRAINT FK_RoleResourceMenus_Roles FOREIGN KEY (RoleId)
        REFERENCES SECURITY_SYSTEM.Roles(Id),

    CONSTRAINT FK_RoleResourceMenus_Resources FOREIGN KEY (ResourceId)
        REFERENCES SECURITY_SYSTEM.Resources(Id),

    CONSTRAINT UQ_RoleResourceMenus_Role_Resource UNIQUE (RoleId, ResourceId)
);
GO

------------------------------------------------------------
-- 10. Table: RoleEndpoints
--      (Permissions at endpoint/action level)
--      Normalized: references ResourceEndpoints
------------------------------------------------------------
CREATE TABLE SECURITY_SYSTEM.RoleEndpoints (
    Id                INT IDENTITY(1,1) CONSTRAINT PK_RoleEndpoints PRIMARY KEY,
    RoleId            INT           NOT NULL,
    ResourceEndpointId INT          NOT NULL,
    RecordStatus      INT           NOT NULL DEFAULT 1,
    CreatedAt         DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy         NVARCHAR(100) NOT NULL DEFAULT SYSTEM_USER,

    CONSTRAINT FK_RoleEndpoints_Roles FOREIGN KEY (RoleId)
        REFERENCES SECURITY_SYSTEM.Roles(Id),

    CONSTRAINT FK_RoleEndpoints_ResourceEndpoints FOREIGN KEY (ResourceEndpointId)
        REFERENCES SECURITY_SYSTEM.ResourceEndpoints(Id),

    CONSTRAINT UQ_RoleEndpoints_Role_Endpoint UNIQUE (RoleId, ResourceEndpointId)
);
GO

------------------------------------------------------------
-- 11. Table: RefreshTokens
--      (Refresh tokens for JWT, with hash, IP, UA, etc.)
------------------------------------------------------------
CREATE TABLE SECURITY_SYSTEM.RefreshTokens (
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
        REFERENCES SECURITY_SYSTEM.Users(Id)
);
GO

CREATE INDEX IX_RefreshTokens_UserId
    ON SECURITY_SYSTEM.RefreshTokens (UserId);
GO

------------------------------------------------------------
-- 12. Table: LoginAttempts
--      (Record of login attempts, successful or not)
------------------------------------------------------------
CREATE TABLE SECURITY_SYSTEM.LoginAttempts (
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
        REFERENCES SECURITY_SYSTEM.Users(Id)
);
GO

CREATE INDEX IX_LoginAttempts_Username_AttemptedAt
    ON SECURITY_SYSTEM.LoginAttempts (Username, AttemptedAt);
GO

------------------------------------------------------------
-- 13. Table: RevokedTokens
--      (JWT blacklist by JTI)
------------------------------------------------------------
CREATE TABLE SECURITY_SYSTEM.RevokedTokens (
    Jti          UNIQUEIDENTIFIER CONSTRAINT PK_RevokedTokens PRIMARY KEY,
    UserId       INT           NOT NULL,
    Reason       NVARCHAR(250) NULL,
    RevokedAt    DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    RecordStatus INT           NOT NULL DEFAULT 1,
    CreatedAt    DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy    NVARCHAR(100) NOT NULL DEFAULT SYSTEM_USER,

    CONSTRAINT FK_RevokedTokens_Users FOREIGN KEY (UserId)
        REFERENCES SECURITY_SYSTEM.Users(Id)
);
GO

CREATE INDEX IX_RevokedTokens_UserId
    ON SECURITY_SYSTEM.RevokedTokens (UserId);
GO

------------------------------------------------------------
-- 14. Table: KnownDevices
--      (Device fingerprint / trusted devices)
------------------------------------------------------------
CREATE TABLE SECURITY_SYSTEM.KnownDevices (
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
        REFERENCES SECURITY_SYSTEM.Users(Id),

    CONSTRAINT UQ_KnownDevices_User_Fingerprint UNIQUE (UserId, FingerprintHash)
);
GO

------------------------------------------------------------
-- 15. Table: LoginAudit
--      (Audit of logins: success, error message, etc.)
------------------------------------------------------------
CREATE TABLE SECURITY_SYSTEM.LoginAudit (
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
        REFERENCES SECURITY_SYSTEM.Users(Id)
);
GO

CREATE INDEX IX_LoginAudit_Username_LoggedAt
    ON SECURITY_SYSTEM.LoginAudit (Username, LoggedAt);
GO

------------------------------------------------------------
-- 16. Table: CryptoKeys
--      (Management of RSA / crypto keys, no passphrase stored)
--      EncryptedPrivateKey is decrypted by the app using a passphrase
--      taken from environment variables / vault (not from DB).
------------------------------------------------------------
CREATE TABLE SECURITY_SYSTEM.CryptoKeys (
    Id                 INT IDENTITY(1,1) CONSTRAINT PK_CryptoKeys PRIMARY KEY,

    Name               NVARCHAR(100) NOT NULL,  -- e.g. 'Auth.JwtMain', 'Auth.Refresh'
    KeyType            TINYINT       NOT NULL,  -- 1=RSA signing, 2=RSA encryption, 3=AES, etc.
    Version            INT           NOT NULL DEFAULT 1,

    ApplicationId      INT           NULL,      -- optional: key associated to an application

    PublicKeyPem       NVARCHAR(MAX) NOT NULL,  -- -----BEGIN PUBLIC KEY-----
    EncryptedPrivateKey VARBINARY(MAX) NOT NULL, -- private key encrypted

    IsActive           BIT           NOT NULL DEFAULT 1,
    StartDate          DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    EndDate            DATETIME2     NULL,

    Thumbprint         NVARCHAR(128) NULL,      -- key fingerprint / hash

    RecordStatus       INT           NOT NULL DEFAULT 1,
    CreatedAt          DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy          NVARCHAR(100) NOT NULL DEFAULT SYSTEM_USER,

    CONSTRAINT FK_CryptoKeys_Applications FOREIGN KEY (ApplicationId)
        REFERENCES SECURITY_SYSTEM.Applications(Id),

    CONSTRAINT UQ_CryptoKeys_Name_Version UNIQUE (Name, Version)
);
GO


ALTER TABLE Autorizacion.RoleUsers
ADD IsInspector BIT NOT NULL DEFAULT(0);


ALTER TABLE AUTORIZACION.RoleEndpoints
ADD 
    ServiceType   INT             NULL,
    Endpoint      NVARCHAR(500)   NULL,
    PageName      NVARCHAR(350)   NULL;
GO
