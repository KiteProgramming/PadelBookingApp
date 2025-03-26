import React from 'react';
import { Button } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';

function Home() {
    const navigate = useNavigate();

    return (
        <div className="d-flex align-items-center justify-content-center min-vh-100 text-center">
            <div>
                <h1>Welcome to the Padel Booking Portal</h1>
                <p className="lead">
                    <strong>Book your court quickly with our top-notch services.</strong>
                </p>
                <Button variant="primary" onClick={() => navigate('/booking')}>
                    Book Now!
                </Button>
            </div>
        </div>
    );
}

export default Home;