import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import BookingPage from './BookingPage';
import axios from 'axios';

jest.mock('axios');

describe('BookingPage', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  test('renders booking page title', () => {
    render(
      <BrowserRouter>
        <BookingPage />
      </BrowserRouter>
    );
    expect(screen.getByText(/Book a Court/i)).toBeInTheDocument();
  });

  test('displays error if incorrect number of participants selected', async () => {
    axios.get.mockResolvedValue({
      data: [
        { email: 'user1@test.com' },
        { email: 'user2@test.com' }
      ]
    });

    render(
      <BrowserRouter>
        <BookingPage />
      </BrowserRouter>
    );
    
    await waitFor(() => {
      expect(axios.get).toHaveBeenCalled();
    });
    
    const bookButton = screen.getByRole('button', { name: /Book Court/i });
    fireEvent.click(bookButton);
    
    await waitFor(() => {
      expect(screen.getByText(/Please select exactly 4 participants./i)).toBeInTheDocument();
    });
  });
});