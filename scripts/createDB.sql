-- crear base de datos
CREATE DATABASE GuitarDB;
GO

-- usar base de datos
USE GuitarDB;
GO

-- crear tabla Guitar
CREATE TABLE Guitar (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(255),
    Model NVARCHAR(255),
    Brand NVARCHAR(255)
);
GO

-- insertar 20 registros a tabla Guitar
INSERT INTO Guitar (Name, Model, Brand)
VALUES 
    ('Stratocaster', 'American Standard', 'Fender'),
    ('Les Paul', 'Standard', 'Gibson'),
    ('SG', 'Special', 'Gibson'),
    ('Telecaster', 'Deluxe', 'Fender'),
    ('Explorer', 'Pro', 'Epiphone'),
    ('Jazzmaster', 'Classic Player', 'Fender'),
    ('Flying V', 'Modern', 'Gibson'),
    ('Mustang', 'American Performer', 'Fender'),
    ('Jaguar', 'Vintage Modified', 'Squier'),
    ('Casino', 'Inspired by John Lennon', 'Epiphone'),
    ('Dreadnought', 'D-28', 'Martin'),
    ('Super Strat', 'RG550', 'Ibanez'),
    ('Cutlass', 'CT50', 'Sterling by Music Man'),
    ('SG', 'Custom', 'Epiphone'),
    ('Falcon', 'G6136T', 'Gretsch'),
    ('Offset', 'Jazzmaster', 'Squier'),
    ('Acoustic', 'FG800', 'Yamaha'),
    ('Semi-Hollow', 'ES-335', 'Gibson'),
    ('Single Cut', 'SE Custom 24', 'PRS'),
    ('Electromatic', 'G5425', 'Gretsch');
GO

-- Mostrar todos los registros de tabla Guitar
SELECT * FROM Guitar;