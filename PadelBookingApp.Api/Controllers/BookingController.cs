using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PadelBookingApp.Api.Models;
using PadelBookingApp.Domain.Entities;
using PadelBookingApp.Infrastructure;

namespace PadelBookingApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]  // Only authenticated users may access this endpoint.
    public class BookingController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        public BookingController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingDto dto)
        {
            if(dto.ParticipantEmails == null || dto.ParticipantEmails.Count != 4)
                return BadRequest("Exactly 4 participant emails are required.");

            var start = dto.BookingStart;
            var end = start.AddHours(dto.DurationInHours);

            // Club operating hours for the same day: 8 AM to 11 PM.
            var openTime = new DateTime(start.Year, start.Month, start.Day, 8, 0, 0);
            var closeTime = new DateTime(start.Year, start.Month, start.Day, 23, 0, 0);

            if(start < openTime || end > closeTime)
                return BadRequest("Booking must be within operating hours: 8 AM to 11 PM.");

            // Verify no other booking overlaps with this time slot.
            var conflict = await _dbContext.Bookings.AnyAsync(b =>
                b.BookingStart < end &&
                b.BookingStart.AddHours(b.DurationInHours) > start
            );
            if(conflict)
                return Conflict("This booking slot is already taken.");

            var booking = new Booking
            {
                BookingStart = start,
                DurationInHours = dto.DurationInHours,
                Participants = string.Join(",", dto.ParticipantEmails)
            };

            _dbContext.Bookings.Add(booking);
            await _dbContext.SaveChangesAsync();
            return Ok("Booking created successfully.");
        }

        [HttpGet]
        public async Task<IActionResult> GetBookings()
        {
            var bookings = await _dbContext.Bookings.ToListAsync();
            return Ok(bookings);
        }
    }
}