-- DİKKAT: Bu dosyadaki Function örnekleri MSSQL/PostgreSQL içindir.
-- SQLite bu komutları desteklemez, sadece örnek olarak eklenmiştir.

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
