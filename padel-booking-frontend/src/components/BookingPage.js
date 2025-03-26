import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { Form, Button, Card, Spinner } from 'react-bootstrap';
import DatePicker from 'react-datepicker';
import "react-datepicker/dist/react-datepicker.css";
import { Typeahead } from 'react-bootstrap-typeahead';
import 'react-bootstrap-typeahead/css/Typeahead.css';
import { useNavigate } from 'react-router-dom';

const BookingPage = () => {
    const navigate = useNavigate();
    const [selectedDate, setSelectedDate] = useState(new Date());
    const [participantsOptions, setParticipantsOptions] = useState([]);
    const [participants, setParticipants] = useState([]); // Expect objects: { email: "..." }
    const [duration, setDuration] = useState(1);
    const [loading, setLoading] = useState(false);
    const [message, setMessage] = useState('');

    // Fetch user list for auto-suggestions with auth header (filtering out admin email).
    useEffect(() => {
        axios.get(`${process.env.REACT_APP_API_BASE_URL}/api/users`, {
            headers: {
                "Authorization": "Bearer " + localStorage.getItem('accessToken')
            }
        })
        .then(response => {
            // Filter out admin account (assuming admin email is admin@admin.com)
            const nonAdminUsers = response.data.filter(user => 
                user.email.toLowerCase() !== "admin@admin.com"
            );
            setParticipantsOptions(nonAdminUsers);
        })
        .catch(error => console.log(error));
    }, []);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setMessage('');

        // Since court logic is removed, we require only participant count validation.
        if (participants.length !== 4) {
            setMessage("Please select exactly 4 participants.");
            setLoading(false);
            return;
        }

        // Build booking date string from selectedDate.
        const bookingDateISOString = selectedDate.toISOString();

        const bookingData = {
            BookingStart: bookingDateISOString, // expects a UTC ISO DateTime string
            DurationInHours: parseFloat(duration),
            // Map selected objects to emails.
            ParticipantEmails: participants.map(p => p.email)
        };

        try {
            const response = await axios.post(`${process.env.REACT_APP_API_BASE_URL}/api/booking`, bookingData, {
                headers: {
                    "Authorization": "Bearer " + localStorage.getItem('accessToken')
                }
            });
            // On success, redirect to PaymentPage.
            navigate('/payment', { state: { bookingId: response.data.bookingId }});
        } catch (error) {
            setMessage(error.response?.data || "Booking failed.");
        } finally {
            setLoading(false);
        }
    };

    // For 24-hour time format with booking limits:
    const minTime = new Date(selectedDate.getFullYear(), selectedDate.getMonth(), selectedDate.getDate(), 7, 0, 0);
    const maxTime = new Date(selectedDate.getFullYear(), selectedDate.getMonth(), selectedDate.getDate(), 23, 0, 0);

    return (
        <Card className="mx-auto mt-5" style={{ maxWidth: '800px' }}>
            <Card.Body>
                <Card.Title className="text-center mb-4">Book a Court</Card.Title>
                <Form onSubmit={handleSubmit}>
                    <Form.Group controlId="formDate" className="mb-3">
                        <Form.Label>Select Date &amp; Time</Form.Label>
                        <DatePicker
                            selected={selectedDate}
                            onChange={date => setSelectedDate(date)}
                            showTimeSelect
                            timeFormat="HH:mm" // 24-hour time format
                            timeIntervals={30}
                            minTime={minTime}
                            maxTime={maxTime}
                            dateFormat="Pp"
                            className="form-control"
                        />
                        <Form.Text className="text-muted">
                            Courts are available between 7:00 AM and 11:00 PM.
                        </Form.Text>
                    </Form.Group>
                    <Form.Group controlId="formDuration" className="mb-3">
                        <Form.Label>Duration (Hours)</Form.Label>
                        <Form.Select value={duration} onChange={e => setDuration(e.target.value)} required>
                            <option value={1}>1 Hour</option>
                            <option value={1.5}>1.5 Hours</option>
                        </Form.Select>
                    </Form.Group>
                    <Form.Group controlId="formParticipants" className="mb-3">
                        <Form.Label>Participants</Form.Label>
                        <Typeahead
                            id="participants-typeahead"
                            labelKey="email"
                            multiple
                            onChange={setParticipants}
                            options={participantsOptions}
                            placeholder="Type participant's email..."
                            selected={participants}
                            filterBy={(option, props) =>
                                option.email.toLowerCase().includes(props.text.toLowerCase())
                            }
                        />
                        <Form.Text className="text-muted">
                            Select exactly 4 registered participants.
                        </Form.Text>
                    </Form.Group>
                    <Button variant="primary" type="submit" className="w-100" disabled={loading}>
                        {loading ? (
                            <>
                                <Spinner animation="border" size="sm" className="me-2" /> Booking...
                            </>
                        ) : "Book Court"}
                    </Button>
                </Form>
                {message && <p className="mt-3 text-center text-danger">{message}</p>}
            </Card.Body>
        </Card>
    );
};

export default BookingPage;