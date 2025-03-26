# PadelBookingApp Frontend

This is the React frontend for the PadelBookingApp application.

## Overview

The frontend provides:
- A login/registration page for users.
- A booking page with a calendar and time slot selection.
- A booking history view.
- Conditional navigation options (normal booking history or admin booking history) based on your credentials.

## Getting Started

1. Install dependencies:

   ```bash
   npm install
   ```

2. Create a `.env` file in this directory and set the base URL for your API:
   
   ```env
   REACT_APP_API_BASE_URL=http://localhost:5000
   ```
   
3. Run the development server:

   ```bash
   npm start
   ```

## Running Unit Tests

The frontend uses Jest and React Testing Library for unit testing. To run the tests:

1. Ensure all dependencies are installed (including testing libraries):

   ```bash
   npm install
   ```

2. Run the tests with the following command:

   ```bash
   npm test
   ```

This command will launch Jest in watch mode. It automatically picks up any files with names ending in `.test.js` or `.test.jsx`.

## Authentication & Admin User

- To test admin capabilities, log in with the default admin account:
  - **Username:** `admin@admin.com`
  - **Password:** `admin1234`

When logged in as an admin, you will see an **Admin Booking History** link in the navbar. This page will allow you to view all bookings and update statuses.

## Additional Configuration

Ensure that your backend (API) has been configured to use environment variables for the connection string. Refer to the main README in the root folder for more details.