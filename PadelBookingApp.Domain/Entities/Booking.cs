namespace PadelBookingApp.Domain.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public DateTime BookingStart { get; set; }
        public double DurationInHours { get; set; }
        public required string Participants { get; set; }
        // Add new property for status (you could set a default in your migration)
        public required string Status { get; set; } 
    }
}