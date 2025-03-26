# PadelBookingApp

This is a full-stack Padel Booking application with a .NET backend and a React frontend.

## Overview

The application provides the following key features:
- User registration/login and JWT-based authentication.
- Booking a court with time slot and participant selection.
- Viewing booking history.
- For administrators: Ability to view and update booking statuses (Approved, Rejected, Pending).

## Admin User

On first run, the application seeds a default admin user if one does not exist:
- **Username:** `admin@admin.com`
- **Password:** `admin1234`

Use these credentials to log in as an admin and access the Admin Booking History page, where you can view all bookings and update their statuses.

## Database Setup and Connection

**Important:** Do not store your database credentials in the configuration files. Instead, use environment variables for sensitive data.

### Changing Database Credentials

1. Open the `appsettings.json` file in the `PadelBookingApp.Api` project.
2. Remove or replace the static values for the database username, password, and database name with environment variable references.

For example, update your connection string like this:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=${DB_NAME};Username=${DB_USER};Password=${DB_PASSWORD}"
  },
  "...": "..."
}
```

Or configure it in your startup code (e.g. using `Environment.GetEnvironmentVariable("DB_USER")`, etc.).  
Make sure you update the `.env` file (or configure your environment variables) on your local machine with the following keys:

```
DB_NAME=your_database_name
DB_USER=your_username
DB_PASSWORD=your_password
```

### Setting Up Environment Variables

For local development, consider using a tool like [dotenv](https://github.com/joho/godotenv) (for .NET you might use the [dotnet user-secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows)) to manage these values without including them in your repository.

## Running the Application

- **Backend:** Navigate to the `PadelBookingApp.Api` directory and run:
  ```bash
  dotnet run
  ```
- **Frontend:** Navigate to the React app directory (e.g. `client` or `frontend`) and run:
  ```bash
  npm install
  npm start
  ```
  
For more details on installation and running, refer to the respective README in the frontend folder.

## Running Backend Unit Tests

Backend unit tests are implemented using xUnit and the In-Memory Database provider. To run the backend tests:

1. Navigate to the backend test project directory (commonly named `PadelBookingApp.Tests`).
2. Ensure required packages are installed (for example, verify `Microsoft.EntityFrameworkCore.InMemory` is in your project).
3. Open a terminal in the test project folder and run:
   ```bash
   dotnet test
   ```
   
This command will build your test project and execute all tests, with the results displayed in your terminal.