import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import { Form, Button, Card, Spinner } from 'react-bootstrap';

function Register() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [loading, setLoading] = useState(false);
    const [errorMessage, setErrorMessage] = useState('');
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setErrorMessage('');
        try {
            const response = await axios.post(`${process.env.REACT_APP_API_BASE_URL}/api/auth/register`, {
                email,
                password
            });
            setTimeout(() => {
                navigate('/login');
            }, 1000);
        } catch (error) {
            setErrorMessage('Registration failed. The user may already exist.');
            setLoading(false);
        }
    };

    return (
        <Card className="mx-auto mt-5" style={{ maxWidth: '400px' }}>
            <Card.Body>
                <Card.Title className="text-center mb-4">
                    <i className="fas fa-user-plus"></i> Register
                </Card.Title>
                <Form onSubmit={handleSubmit}>
                    <Form.Group controlId="formEmail" className="mb-3">
                        <Form.Label>Email</Form.Label>
                        <Form.Control 
                            type="email" 
                            value={email} 
                            onChange={(e) => setEmail(e.target.value)}
                            placeholder="Enter your email" 
                            required 
                        />
                    </Form.Group>
                    <Form.Group controlId="formPassword" className="mb-3">
                        <Form.Label>Password</Form.Label>
                        <Form.Control 
                            type="password" 
                            value={password} 
                            onChange={(e) => setPassword(e.target.value)}
                            placeholder="Enter your password" 
                            required 
                        />
                    </Form.Group>
                    <Button variant="success" type="submit" className="w-100" disabled={loading}>
                        {loading ? (
                            <>
                                <Spinner animation="border" size="sm" className="me-2" />
                                Registering...
                            </>
                        ) : (
                            "Register"
                        )}
                    </Button>
                </Form>
                {errorMessage && <p className="mt-3 text-center text-danger">{errorMessage}</p>}
            </Card.Body>
        </Card>
    );
}

export default Register;