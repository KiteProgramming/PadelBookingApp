import React, { useState, useEffect } from 'react';
import { Card, Table, Button, Form } from 'react-bootstrap';
import axios from 'axios';

const AdminBookingHistory = () => {
    const [bookings, setBookings] = useState([]);
    const [loading, setLoading] = useState(true);
    const [selectedRows, setSelectedRows] = useState([]);
    const [massStatus, setMassStatus] = useState("Pending");
    const [message, setMessage] = useState("");

    // Fetch all bookings on mount
    useEffect(() => {
        axios.get(`${process.env.REACT_APP_API_BASE_URL}/api/booking`, {
            headers: { Authorization: "Bearer " + localStorage.getItem('accessToken') }
        })
        .then((res) => {
            setBookings(res.data);
            setLoading(false);
        })
        .catch((err) => {
            console.error(err);
            setLoading(false);
        });
    }, []);

    const handleRowSelect = (bookingId, checked) => {
        if (checked) {
            setSelectedRows([...selectedRows, bookingId]);
        } else {
            setSelectedRows(selectedRows.filter(id => id !== bookingId));
        }
    };

    const handleMassUpdate = async () => {
        if (!selectedRows.length) {
            setMessage("No bookings selected for update.");
            return;
        }
        try {
            await axios.put(`${process.env.REACT_APP_API_BASE_URL}/api/adminbooking/update-status`, 
            { bookingIds: selectedRows, status: massStatus },
            { headers: { Authorization: "Bearer " + localStorage.getItem('accessToken') } });
            setMessage("Mass update successful.");
            // Refresh bookings
            const res = await axios.get(`${process.env.REACT_APP_API_BASE_URL}/api/booking`, {
                headers: { Authorization: "Bearer " + localStorage.getItem('accessToken') }
            });
            setBookings(res.data);
            setSelectedRows([]);
        } catch (err) {
            console.error(err);
            setMessage("Mass update failed.");
        }
    };

    const handleInlineStatusChange = async (bookingId, newStatus) => {
        try {
            await axios.put(`${process.env.REACT_APP_API_BASE_URL}/api/adminbooking/update-status`, 
            { bookingIds: [bookingId], status: newStatus },
            { headers: { Authorization: "Bearer " + localStorage.getItem('accessToken') } });
            setMessage("Status updated successfully.");
            // Update local state
            setBookings(bookings.map(b => b.id === bookingId ? { ...b, status: newStatus } : b));
        } catch (err) {
            console.error(err);
            setMessage("Failed to update status.");
        }
    };

    if (loading) return <p>Loading...</p>;
    
    return (
        <Card className="mt-5">
            <Card.Body>
                <Card.Title>Admin Booking History</Card.Title>
                {message && <p>{message}</p>}
                <div className="mb-3 d-flex align-items-center">
                    <Form.Label className="me-2">Mass Update Status:</Form.Label>
                    <Form.Select value={massStatus} onChange={(e) => setMassStatus(e.target.value)} style={{ width: '150px' }}>
                        <option value="Pending">Pending</option>
                        <option value="Approved">Approved</option>
                        <option value="Rejected">Rejected</option>
                    </Form.Select>
                    <Button variant="primary" className="ms-2" onClick={handleMassUpdate}>
                        Update Selected
                    </Button>
                </div>
                <Table striped bordered hover responsive>
                    <thead>
                        <tr>
                            <th>Select</th>
                            <th>Date &amp; Time</th>
                            <th>Duration</th>
                            <th>Participants</th>
                            <th>Status</th>
                            <th>Change Status</th>
                        </tr>
                    </thead>
                    <tbody>
                        {bookings.map(booking => (
                            <tr key={booking.id}>
                                <td>
                                    <Form.Check 
                                        type="checkbox" 
                                        checked={selectedRows.includes(booking.id)}
                                        onChange={(e) => handleRowSelect(booking.id, e.target.checked)} 
                                    />
                                </td>
                                <td>{new Date(booking.bookingStart).toLocaleString()}</td>
                                <td>{booking.durationInHours} hour(s)</td>
                                <td>{booking.participants}</td>
                                <td>{booking.status}</td>
                                <td>
                                    <Form.Select value={booking.status} onChange={(e) => handleInlineStatusChange(booking.id, e.target.value)}>
                                        <option value="Pending">Pending</option>
                                        <option value="Approved">Approved</option>
                                        <option value="Rejected">Rejected</option>
                                    </Form.Select>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </Table>
            </Card.Body>
        </Card>
    );
};

export default AdminBookingHistory;