import React, { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Navbar as BSNavbar, Nav, Button, Container } from 'react-bootstrap';

const Navbar = () => {
  const navigate = useNavigate();
  const [token, setToken] = useState(localStorage.getItem('accessToken'));
  const [role, setRole] = useState(null);

  useEffect(() => {
    const handleAuthChange = () => {
      const t = localStorage.getItem('accessToken');
      setToken(t);
      if (t) {
        try {
          const payload = JSON.parse(atob(t.split('.')[1]));
          setRole(payload.role);
        } catch (error) {
          console.error('Invalid token format', error);
          setRole(null);
        }
      } else {
        setRole(null);
      }
    };
  
    window.addEventListener('authChange', handleAuthChange);
    window.addEventListener('storage', handleAuthChange);
    return () => {
      window.removeEventListener('authChange', handleAuthChange);
      window.removeEventListener('storage', handleAuthChange);
    };
  }, []);

  const handleLogout = () => {
    localStorage.removeItem('accessToken');
    setToken(null);
    setRole(null);
    navigate('/login');
  };

  return (
    <BSNavbar bg="dark" variant="dark" expand="lg">
      <Container>
        <BSNavbar.Brand as={Link} to="/">PadelBookingApp</BSNavbar.Brand>
        <BSNavbar.Toggle aria-controls="basic-navbar-nav" />
        <BSNavbar.Collapse id="basic-navbar-nav">
          <Nav className="me-auto">
            {token && (
              <>
                <Nav.Link as={Link} to="/booking">
                  <i className="fas fa-calendar-alt"></i> Booking
                </Nav.Link>
                { role && role.toLowerCase() === 'admin' ? (
                  <Nav.Link as={Link} to="/admin-booking-history">
                    <i className="fas fa-history"></i> Admin Booking History
                  </Nav.Link>
                ) : (
                  <Nav.Link as={Link} to="/booking-history">
                    <i className="fas fa-history"></i> Booking History
                  </Nav.Link>
                )}
              </>
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