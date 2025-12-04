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
-- 2. Move AUTHENTICATION-related tables to Autenticacion
------------------------------------------------------------

-- Users
IF OBJECT_ID('SECURITY_SYSTEM.Users', 'U') IS NOT NULL
BEGIN
    ALTER SCHEMA Autenticacion TRANSFER SECURITY_SYSTEM.Users;
END;
GO

-- RefreshTokens
IF OBJECT_ID('SECURITY_SYSTEM.RefreshTokens', 'U') IS NOT NULL
BEGIN
    ALTER SCHEMA Autenticacion TRANSFER SECURITY_SYSTEM.RefreshTokens;
END;
GO

-- RevokedTokens
IF OBJECT_ID('SECURITY_SYSTEM.RevokedTokens', 'U') IS NOT NULL
BEGIN
    ALTER SCHEMA Autenticacion TRANSFER SECURITY_SYSTEM.RevokedTokens;
END;
GO

-- LoginAttempts
IF OBJECT_ID('SECURITY_SYSTEM.LoginAttempts', 'U') IS NOT NULL
BEGIN
    ALTER SCHEMA Autenticacion TRANSFER SECURITY_SYSTEM.LoginAttempts;
END;
GO

-- LoginAudit
IF OBJECT_ID('SECURITY_SYSTEM.LoginAudit', 'U') IS NOT NULL
BEGIN
    ALTER SCHEMA Autenticacion TRANSFER SECURITY_SYSTEM.LoginAudit;
END;
GO

-- KnownDevices
IF OBJECT_ID('SECURITY_SYSTEM.KnownDevices', 'U') IS NOT NULL
BEGIN
    ALTER SCHEMA Autenticacion TRANSFER SECURITY_SYSTEM.KnownDevices;
END;
GO

-- CryptoKeys
IF OBJECT_ID('SECURITY_SYSTEM.CryptoKeys', 'U') IS NOT NULL
BEGIN
    ALTER SCHEMA Autenticacion TRANSFER SECURITY_SYSTEM.CryptoKeys;
END;
GO

------------------------------------------------------------
-- 3. Move AUTHORIZATION-related tables to Autorizacion
------------------------------------------------------------

-- Applications
IF OBJECT_ID('SECURITY_SYSTEM.Applications', 'U') IS NOT NULL
BEGIN
    ALTER SCHEMA Autorizacion TRANSFER SECURITY_SYSTEM.Applications;
END;
GO

-- Roles
IF OBJECT_ID('SECURITY_SYSTEM.Roles', 'U') IS NOT NULL
BEGIN
    ALTER SCHEMA Autorizacion TRANSFER SECURITY_SYSTEM.Roles;
END;
GO

-- RoleUsers
IF OBJECT_ID('SECURITY_SYSTEM.RoleUsers', 'U') IS NOT NULL
BEGIN
    ALTER SCHEMA Autorizacion TRANSFER SECURITY_SYSTEM.RoleUsers;
END;
GO

-- Resources
IF OBJECT_ID('SECURITY_SYSTEM.Resources', 'U') IS NOT NULL
BEGIN
    ALTER SCHEMA Autorizacion TRANSFER SECURITY_SYSTEM.Resources;
END;
GO

-- ResourceMenus
IF OBJECT_ID('SECURITY_SYSTEM.ResourceMenus', 'U') IS NOT NULL
BEGIN
    ALTER SCHEMA Autorizacion TRANSFER SECURITY_SYSTEM.ResourceMenus;
END;
GO

-- ResourceEndpoints
IF OBJECT_ID('SECURITY_SYSTEM.ResourceEndpoints', 'U') IS NOT NULL
BEGIN
    ALTER SCHEMA Autorizacion TRANSFER SECURITY_SYSTEM.ResourceEndpoints;
END;
GO

-- RoleEndpoints
IF OBJECT_ID('SECURITY_SYSTEM.RoleEndpoints', 'U') IS NOT NULL
BEGIN
    ALTER SCHEMA Autorizacion TRANSFER SECURITY_SYSTEM.RoleEndpoints;
END;
GO

-- RoleResourceMenus
IF OBJECT_ID('SECURITY_SYSTEM.RoleResourceMenus', 'U') IS NOT NULL
BEGIN
    ALTER SCHEMA Autorizacion TRANSFER SECURITY_SYSTEM.RoleResourceMenus;
END;
GO
