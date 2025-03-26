import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Navbar as BSNavbar, Nav, Container, Button } from 'react-bootstrap';

const Navbar = () => {
    const navigate = useNavigate();
    const token = localStorage.getItem('accessToken');

    const handleLogout = () => {
        localStorage.removeItem('accessToken');
        navigate('/login');
    };

    return (
        <BSNavbar bg="dark" variant="dark" expand="lg">
            <Container>
                <BSNavbar.Brand as={Link} to="/">Padel Booking</BSNavbar.Brand>
                <BSNavbar.Toggle aria-controls="basic-navbar-nav" />
                <BSNavbar.Collapse id="basic-navbar-nav">
                    <Nav className="me-auto">
                        {token && (
                            <Nav.Link as={Link} to="/booking">
                                <i className="fas fa-calendar-alt"></i> Booking
                            </Nav.Link>
                        )}
                    </Nav>
                    <Nav>
                        {!token && (
                            <>
                                <Nav.Link as={Link} to="/login">
                                    <i className="fas fa-sign-in-alt"></i> Login
                                </Nav.Link>
                                <Nav.Link as={Link} to="/register">
                                    <i className="fas fa-user-plus"></i> Register
                                </Nav.Link>
                            </>
                        )}
                        {token && (
                            <Button variant="outline-light" onClick={handleLogout}>
                                <i className="fas fa-sign-out-alt"></i> Logout
                            </Button>
                        )}
                    </Nav>
                </BSNavbar.Collapse>
            </Container>
        </BSNavbar>
    );
};

export default Navbar;