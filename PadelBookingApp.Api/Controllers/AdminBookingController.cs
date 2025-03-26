using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PadelBookingApp.Infrastructure;
using PadelBookingApp.Domain.Entities;

namespace PadelBookingApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminBookingController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        
        public AdminBookingController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _dbContext.Bookings.ToListAsync();
            return Ok(bookings);
        }
        
        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateBookingStatus([FromBody] UpdateBookingStatusDto dto)
        {
            if(dto?.BookingIds == null || dto.BookingIds.Count == 0)
                return BadRequest("At least one booking id must be provided.");
            
            var bookings = await _dbContext.Bookings
                .Where(b => dto.BookingIds.Contains(b.Id))
                .ToListAsync();
            
            if(bookings.Count == 0)
                return NotFound("No bookings found.");
            
            foreach(var booking in bookings)
            {
                booking.Status = dto.Status; // e.g., "Approved" or "Rejected"
            }
            await _dbContext.SaveChangesAsync();
            return Ok("Booking statuses updated successfully.");
        }
    }
    
    public class UpdateBookingStatusDto
    {
       public required List<int> BookingIds { get; set; }
       public required string Status { get; set; }
    }
}