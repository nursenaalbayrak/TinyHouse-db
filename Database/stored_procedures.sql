
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
