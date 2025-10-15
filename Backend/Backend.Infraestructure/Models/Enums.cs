namespace Backend.Infraestructure.Models
{
    public enum ReservationStatus
    {
        reserved,
        checked_in,
        completed,
        canceled
    }

    public enum EconomicLevel
    {
        low,
        medium,
        high
    }

    public enum UserRole
    {
        user,
        admin
    }


}
