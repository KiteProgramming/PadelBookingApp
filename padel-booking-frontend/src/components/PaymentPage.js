import React, { useState } from 'react';
import { Form, Button, Card } from 'react-bootstrap';
import { useNavigate, useLocation } from 'react-router-dom';

const PaymentPage = () => {
    const navigate = useNavigate();
    const location = useLocation();
    // You can pass any booking info via state
    const { bookingId } = location.state || {};

    const [cardNumber, setCardNumber] = useState('');
    const [expiry, setExpiry] = useState('');
    const [cvv, setCvv] = useState('');

    const handlePaymentSubmit = (e) => {
        e.preventDefault();
        // This is a mock payment. In a real app you'd process the payment.
        navigate('/booking-history');
    };

    return (
        <Card className="mx-auto mt-5" style={{ maxWidth: '500px' }}>
            <Card.Body>
                <Card.Title className="text-center mb-4">Payment</Card.Title>
                <Form onSubmit={handlePaymentSubmit}>
                    <Form.Group controlId="formCardNumber" className="mb-3">
                        <Form.Label>Card Number</Form.Label>
                        <Form.Control 
                            type="text" 
                            placeholder="Enter card number" 
                            value={cardNumber} 
                            onChange={(e) => setCardNumber(e.target.value)} 
                            required 
                        />
                    </Form.Group>
                    <Form.Group controlId="formExpiry" className="mb-3">
                        <Form.Label>Expiry Date</Form.Label>
                        <Form.Control 
                            type="text" 
                            placeholder="MM/YY" 
                            value={expiry} 
                            onChange={(e) => setExpiry(e.target.value)} 
                            required 
                        />
                    </Form.Group>
                    <Form.Group controlId="formCvv" className="mb-3">
                        <Form.Label>CVV</Form.Label>
                        <Form.Control 
                            type="text" 
                            placeholder="CVV" 
                            value={cvv} 
                            onChange={(e) => setCvv(e.target.value)} 
                            required 
                        />
                    </Form.Group>
                    <Button variant="success" type="submit" className="w-100">Pay</Button>
                </Form>
            </Card.Body>
        </Card>
    );
};

export default PaymentPage;