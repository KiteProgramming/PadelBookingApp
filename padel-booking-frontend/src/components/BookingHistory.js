import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { Card, Table } from 'react-bootstrap';

const BookingHistory = () => {
    const [bookings, setBookings] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        axios.get(`${process.env.REACT_APP_API_BASE_URL}/api/booking`, {
            headers: { Authorization: "Bearer " + localStorage.getItem('accessToken') }
        })
        .then(response => {
            setBookings(response.data);
            setLoading(false);
        })
        .catch(error => {
            console.error(error);
            setLoading(false);
        });
    }, []);

    return (
        <Card className="mx-auto mt-5" style={{ maxWidth: '800px' }}>
            <Card.Body>
                <Card.Title className="text-center mb-4">My Booking History</Card.Title>
                {loading ? (
                    <p>Loading...</p>
                ) : (
                    <Table striped bordered hover responsive>
                        <thead>
                            <tr>
                                <th>Date &amp; Time</th>
                                <th>Duration</th>
                                <th>Participants</th>
                                <th>Status</th>
                            </tr>
                        </thead>
                        <tbody>
                            {bookings.map((booking, index) => (
                                <tr key={index}>
                                    <td>{new Date(booking.bookingStart).toLocaleString()}</td>
                                    <td>{booking.durationInHours} hour(s)</td>
                                    <td>{booking.participants}</td>
                                    <td>{booking.status || "Pending"}</td>
                                </tr>
                            ))}
                        </tbody>
                    </Table>
                )}
            </Card.Body>
        </Card>
    );
};

export default BookingHistory;