USE master;
-- Create Microservice_User database if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Microservice_User')
BEGIN
    CREATE DATABASE Microservice_User;
END
GO

-- Use Microservice_User database
USE Microservice_User;
GO

-- Create Users table
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(255) NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    HasReadAccess BIT NOT NULL DEFAULT 1,
    HasCreateAccess BIT NOT NULL DEFAULT 1,
    HasWriteAccess BIT NOT NULL DEFAULT 1,
    HasDeleteAccess BIT NOT NULL DEFAULT 1
);
GO

-- Insert sample records
INSERT INTO Users (Email, Password, HasReadAccess, HasCreateAccess, HasWriteAccess, HasDeleteAccess)
VALUES 
    ('admin@example.com', 'password123', 1, 1, 1, 1),
    ('user1@example.com', 'password123', 1, 0, 1, 1),
    ('user2@example.com', 'password123', 1, 1, 0, 0),
    ('user3@example.com', 'password123', 0, 1, 1, 0),
    ('user4@example.com', 'password123', 1, 0, 0, 0),
    ('user5@example.com', 'password123', 1, 0, 0, 1);
GO


USE master;
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Microservice_Product')
BEGIN
    CREATE DATABASE Microservice_Product;
END
GO

USE Microservice_Product;

CREATE TABLE Products (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL
);

INSERT INTO Products (Name) VALUES ('Product 1');
INSERT INTO Products (Name) VALUES ('Product 2');
INSERT INTO Products (Name) VALUES ('Product 3');
INSERT INTO Products (Name) VALUES ('Product 4');
INSERT INTO Products (Name) VALUES ('Product 5');

