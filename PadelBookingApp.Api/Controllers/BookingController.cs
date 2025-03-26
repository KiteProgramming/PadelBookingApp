using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PadelBookingApp.Api.Models;
using PadelBookingApp.Domain.Entities;
using PadelBookingApp.Infrastructure;
using System.Security.Claims;

namespace PadelBookingApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
            
            var distinctEmails = dto.ParticipantEmails.Select(e => e.Trim().ToLower()).Distinct().ToList();
            if(distinctEmails.Count != dto.ParticipantEmails.Count)
                return BadRequest("Duplicate participant emails are not allowed.");

            foreach (var email in distinctEmails)
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email);
                if(user == null)
                    return BadRequest($"Participant with email '{email}' is not registered.");
            }

            var start = DateTime.SpecifyKind(dto.BookingStart, DateTimeKind.Utc);
            var end = start.AddHours(dto.DurationInHours);

            var openTime = new DateTime(start.Year, start.Month, start.Day, 7, 0, 0, DateTimeKind.Utc);
            var closeTime = new DateTime(start.Year, start.Month, start.Day, 23, 0, 0, DateTimeKind.Utc);
            if(start < openTime || end > closeTime)
                return BadRequest("Booking must be within operating hours: 7 AM to 11 PM.");

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
                Participants = string.Join(",", dto.ParticipantEmails),
                Status = "Pending" // New bookings start as Pending.
            };

            _dbContext.Bookings.Add(booking);
            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "Booking created successfully.", bookingId = booking.Id });
        }

        [HttpGet]
        public async Task<IActionResult> GetBookings()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Name);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized("Your email claim is missing");
            }

            List<Booking> bookings;
            if (!string.IsNullOrEmpty(userRole) && userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                // Admin can see all bookings.
                bookings = await _dbContext.Bookings.ToListAsync();
            }
            else
            {
                // Other users see only bookings where their email is listed.
                bookings = await _dbContext.Bookings
                    .Where(b => b.Participants.ToLower().Contains(userEmail.ToLower()))
                    .ToListAsync();
            }

            return Ok(bookings);
        }
    }
}