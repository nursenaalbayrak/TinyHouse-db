
DROP TABLE IF EXISTS Payments;
DROP TABLE IF EXISTS Reviews;
DROP TABLE IF EXISTS Reservations;
DROP TABLE IF EXISTS TinyHouses;
DROP TABLE IF EXISTS Users;
DROP TABLE IF EXISTS DeletedUsersLog;


CREATE TABLE Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    FirstName TEXT NOT NULL,
    LastName TEXT NOT NULL,
    Email TEXT NOT NULL UNIQUE,
    Password TEXT NOT NULL,
    Phone TEXT NOT NULL,
    Role INTEGER NOT NULL CHECK (Role IN (0, 1, 2)),
    IsActive BOOLEAN DEFAULT 1,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);


CREATE TABLE TinyHouses (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Description TEXT NOT NULL,
    Price DECIMAL NOT NULL,
    Location TEXT NOT NULL,
    SquareMeters INTEGER NOT NULL,
    OwnerId INTEGER NOT NULL,
    IsActive BOOLEAN DEFAULT 1,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (OwnerId) REFERENCES Users(Id) ON DELETE CASCADE
);


CREATE TABLE Reservations (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    TinyHouseId INTEGER NOT NULL,
    UserId INTEGER NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    TotalPrice DECIMAL NOT NULL,
    Status TEXT NOT NULL DEFAULT 'Beklemede',
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (TinyHouseId) REFERENCES TinyHouses(Id) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);


CREATE TABLE Reviews (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    TinyHouseId INTEGER NOT NULL,
    UserId INTEGER NOT NULL,
    Rating INTEGER NOT NULL CHECK (Rating BETWEEN 1 AND 5),
    Comment TEXT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (TinyHouseId) REFERENCES TinyHouses(Id) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);


CREATE TABLE Payments (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ReservationId INTEGER NOT NULL,
    Amount DECIMAL NOT NULL,
    Status TEXT NOT NULL CHECK (Status IN ('Beklemede', 'Tamamlandı', 'İptalEdildi')),
    PaymentMethod TEXT NOT NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ReservationId) REFERENCES Reservations(Id) ON DELETE CASCADE
);


CREATE TABLE IF NOT EXISTS DeletedUsersLog (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER,
    FirstName TEXT,
    LastName TEXT,
    Email TEXT,
    DeletedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);


DELIMITER //
CREATE TRIGGER IF NOT EXISTS CreatePaymentAfterReservation
AFTER INSERT ON Reservations
BEGIN
    INSERT INTO Payments (ReservationId, Amount, Status, PaymentMethod)
    VALUES (NEW.Id, NEW.TotalPrice, 'Beklemede', 'Kredi Kartı');
END;//
DELIMITER ;


INSERT INTO Users (FirstName, LastName, Email, Password, Phone, Role, IsActive)
VALUES ('Admin', 'User', 'admin@tinyhouse.com', 'Admin123!', '5551234567', 0, 1);


PRAGMA foreign_keys = ON;

CREATE PROCEDURE GetUserReservations
    @UserID INT
AS
BEGIN
    SELECT r.ReservationID, r.HouseID, r.StartDate, r.EndDate, r.Status
    FROM Reservations r
    WHERE r.TenantID = @UserID
END;

CREATE PROCEDURE GetHouseAvailability
    @HouseID INT,
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
    SELECT COUNT(*) AS ReservationCount
    FROM Reservations
    WHERE HouseID = @HouseID
      AND Status = 'Onaylandı'
      AND (
            (StartDate <= @EndDate AND EndDate >= @StartDate)
          )
END;

CREATE FUNCTION GetHouseAverageRating (@HouseID INT)
RETURNS FLOAT
AS
BEGIN
    DECLARE @AvgRating FLOAT;
    SELECT @AvgRating = AVG(CAST(Rating AS FLOAT))
    FROM Reviews
    WHERE HouseID = @HouseID;
    RETURN @AvgRating;
END;

CREATE FUNCTION GetUserReservationCount (@UserID INT)
RETURNS INT
AS
BEGIN
    DECLARE @Count INT;
    SELECT @Count = COUNT(*)
    FROM Reservations
    WHERE TenantID = @UserID;
    RETURN @Count;
END;

CREATE TRIGGER IF NOT EXISTS DeletedUser
BEFORE DELETE ON Users
FOR EACH ROW
BEGIN
    INSERT INTO DeletedUsersLog (UserId, FirstName, LastName, Email)
    VALUES (OLD.Id, OLD.FirstName, OLD.LastName, OLD.Email);
END; 