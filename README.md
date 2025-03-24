# PadelBookingApp

## Overview

This project is a full-stack Padel Court Booking System featuring an ASP.NET Core Web API backend (with JWT authentication, Swagger API documentation, etc.) and a ReactJS frontend.

## Dependencies

This project uses the following key NuGet packages:

- [Microsoft.AspNetCore.Authentication.JwtBearer](https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.JwtBearer)
- [System.IdentityModel.Tokens.Jwt](https://www.nuget.org/packages/System.IdentityModel.Tokens.Jwt) (if needed)
- [Swashbuckle.AspNetCore](https://www.nuget.org/packages/Swashbuckle.AspNetCore)

These dependencies are automatically restored when you build the solution.

## Setup Instructions

1. Clone the repository:
   ```bash
   git clone https://github.com/YourUsername/PadelBookingApp.git
   ```
2. Navigate to the API project directory:
   ```bash
   cd PadelBookingApp.Api
   ```
3. Restore the NuGet packages:
   ```bash
   dotnet restore
   ```
4. Run the API:
   ```bash
   dotnet run
   ```
5. Browse to `https://localhost:{PORT}/swagger` to see the API documentation (if in Development mode).

## Notes

- The JWT secret key is generated automatically at runtime if not provided via the `JWT_SECRET_KEY` environment variable.
- For production use, consider using a secure secret storage solution (e.g., environment variables, Azure Key Vault, etc.).