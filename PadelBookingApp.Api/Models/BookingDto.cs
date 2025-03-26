using System;
using System.Collections.Generic;

namespace PadelBookingApp.Api.Models
{
    public class BookingDto
    {
        public DateTime BookingStart { get; set; }
        public double DurationInHours { get; set; }
        public List<string> ParticipantEmails { get; set; } = new List<string>();
    }
}