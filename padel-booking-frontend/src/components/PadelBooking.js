import React, { useState } from 'react';
import axios from 'axios';
import { Form, Button, Card, Spinner } from 'react-bootstrap';

const PadelBooking = () => {
    const [bookingStart, setBookingStart] = useState('');
    const [duration, setDuration] = useState(1);
    const [participants, setParticipants] = useState(['', '', '', '']);
    const [loading, setLoading] = useState(false);
    const [message, setMessage] = useState('');

    const handleParticipantChange = (index, value) => {
        const newParticipants = [...participants];
        newParticipants[index] = value;
        setParticipants(newParticipants);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setMessage('');
        try {
            const bookingData = {
                BookingStart: bookingStart, // Provide a valid ISO DateTime string.
                DurationInHours: parseFloat(duration),
                ParticipantEmails: participants
            };
            const response = await axios.post(`${process.env.REACT_APP_API_BASE_URL}/api/booking`, bookingData, {
                headers: {
                    "Authorization": "Bearer " + localStorage.getItem('accessToken')
                }
            });
            setMessage(response.data);
        } catch (error) {            
            setMessage(error.response?.data || "Booking failed.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <Card className="mx-auto mt-5" style={{ maxWidth: '600px' }}>
            <Card.Body>
                <Card.Title className="text-center mb-4">
                    <i className="fas fa-calendar-alt"></i> Book a Court
                </Card.Title>
                <Form onSubmit={handleSubmit}>
                    <Form.Group controlId="formBookingStart" className="mb-3">
                        <Form.Label>Date & Time</Form.Label>
                        <Form.Control 
                            type="datetime-local" 
                            value={bookingStart} 
                            onChange={(e) => setBookingStart(e.target.value)}
                            required 
                        />
                        <Form.Text className="text-muted">
                            Club hours: 8:00 AM to 11:00 PM.
                        </Form.Text>
                    </Form.Group>
                    <Form.Group controlId="formDuration" className="mb-3">
                        <Form.Label>Duration (Hours)</Form.Label>
                        <Form.Select value={duration} onChange={(e) => setDuration(e.target.value)} required>
                            <option value={1}>1 Hour</option>
                            <option value={1.5}>1.5 Hours</option>
                        </Form.Select>
                    </Form.Group>
                    {participants.map((email, index) => (
                        <Form.Group key={index} controlId={`participant${index}`} className="mb-3">
                            <Form.Label>Participant {index + 1} Email</Form.Label>
                            <Form.Control 
                                type="email" 
                                value={email} 
                                onChange={(e) => handleParticipantChange(index, e.target.value)}
                                placeholder={`Enter email for participant ${index + 1}`}
                                required 
                            />
                        </Form.Group>
                    ))}
                    <Button variant="primary" type="submit" className="w-100" disabled={loading}>
                        {loading ? (
                            <>
                                <Spinner animation="border" size="sm" className="me-2" /> Booking...
                            </>
                        ) : (
                            "Book Court"
                        )}
                    </Button>
                </Form>
                {message && <p className="mt-3 text-center">{message}</p>}
            </Card.Body>
        </Card>
    );
};

export default PadelBooking;