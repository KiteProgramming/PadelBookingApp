using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PadelBookingApp.Api.Controllers;
using PadelBookingApp.Api.Models;
using PadelBookingApp.Domain.Entities;
using PadelBookingApp.Infrastructure;
using Xunit;

namespace PadelBookingApp.Tests
{
    public class BookingControllerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly BookingController _controller;

        public BookingControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            _context.Users.Add(new User { Email = "user1@test.com", Password = "dummy", Role = "Customer" });
            _context.Users.Add(new User { Email = "user2@test.com", Password = "dummy", Role = "Customer" });
            _context.Users.Add(new User { Email = "user3@test.com", Password = "dummy", Role = "Customer" });
            _context.Users.Add(new User { Email = "user4@test.com", Password = "dummy", Role = "Customer" });
            _context.Users.Add(new User { Email = "admin@admin.com", Password = "dummy", Role = "Admin" });            
            _context.Bookings.Add(new Booking {
                BookingStart = new DateTime(2025, 3, 27, 10, 0, 0, DateTimeKind.Utc),
                DurationInHours = 1,
                Participants = "user1@test.com,user2@test.com,user3@test.com,user4@test.com",
                Status = "Pending"
            });
            _context.SaveChanges();

            _controller = new BookingController(_context);
            
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "user1@test.com"),
                new Claim(ClaimTypes.Role, "Customer")
            }, "mock"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetBookings_ReturnsBookingsForLoggedInUser()
        {            
            var result = await _controller.GetBookings();
            
            var okResult = Assert.IsType<OkObjectResult>(result);
            var bookings = Assert.IsAssignableFrom<List<Booking>>(okResult.Value);
            
            Assert.Single(bookings);
            Assert.Contains("user1@test.com", bookings[0].Participants);
        }

        [Fact]
        public async Task CreateBooking_Successful()
        {            
            var bookingDto = new BookingDto
            {
                BookingStart = new DateTime(2025, 3, 27, 12, 0, 0, DateTimeKind.Utc),
                DurationInHours = 1,
                ParticipantEmails = new List<string>
                {
                    "user1@test.com",
                    "user2@test.com",
                    "user3@test.com",
                    "user4@test.com"
                }
            };
            
            var result = await _controller.CreateBooking(bookingDto);
            
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic response = okResult.Value;
            Assert.NotNull(response.bookingId);
            
            var bookingInDb = await _context.Bookings.FindAsync(response.bookingId);
            Assert.NotNull(bookingInDb);
            Assert.Equal("Pending", bookingInDb.Status);
        }

        [Fact]
        public async Task CreateBooking_FailsWithInvalidParticipantCount()
        {            
            var bookingDto = new BookingDto
            {
                BookingStart = new DateTime(2025, 3, 27, 12, 0, 0, DateTimeKind.Utc),
                DurationInHours = 1,
                ParticipantEmails = new List<string>
                {
                    "user1@test.com",
                    "user2@test.com",
                    "user3@test.com"
                }
            };
            
            var result = await _controller.CreateBooking(bookingDto);
            
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Exactly 4 participant emails are required.", badResult.Value);
        }

        [Fact]
        public async Task CreateBooking_FailsWhenBookingOutsideOperatingHours()
        {            
            var bookingDto = new BookingDto
            {
                BookingStart = new DateTime(2025, 3, 27, 6, 0, 0, DateTimeKind.Utc),
                DurationInHours = 1,
                ParticipantEmails = new List<string>
                {
                    "user1@test.com",
                    "user2@test.com",
                    "user3@test.com",
                    "user4@test.com"
                }
            };
            
            var result = await _controller.CreateBooking(bookingDto);
            
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Booking must be within operating hours", badResult.Value.ToString());
        }

        [Fact]
        public async Task CreateBooking_FailsDueToConflict()
        {            
            var bookingDto = new BookingDto
            {
                BookingStart = new DateTime(2025, 3, 27, 10, 30, 0, DateTimeKind.Utc),
                DurationInHours = 1,
                ParticipantEmails = new List<string>
                {
                    "user1@test.com",
                    "user2@test.com",
                    "user3@test.com",
                    "user4@test.com"
                }
            };

            var result = await _controller.CreateBooking(bookingDto);

            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Contains("already taken", conflictResult.Value.ToString());
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}