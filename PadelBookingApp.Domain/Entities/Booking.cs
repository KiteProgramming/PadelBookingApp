namespace PadelBookingApp.Domain.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public DateTime BookingStart { get; set; }
        public double DurationInHours { get; set; } // Make sure this is exactly defined.
        public required string Participants { get; set; }
    }
}