import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Navbar from './components/Navbar';
import Home from './components/Home';
import Login from './components/Login';
import Register from './components/Register';
import BookingPage from './components/BookingPage';
import PaymentPage from './components/PaymentPage';
import BookingHistory from './components/BookingHistory';
import AdminBookingHistory from './components/AdminBookingHistory';
import Logout from './components/Logout';
import ProtectedRoute from './components/ProtectedRoute';
import AdminRoute from './components/AdminRoute';

function App() {
  return (
    <Router>
      <Navbar />
      <div className="container mt-3">
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route path="/logout" element={<Logout />} />

          <Route 
             path="/booking" 
             element={
                <ProtectedRoute>
                   <BookingPage />
                </ProtectedRoute>
             }
          />

          <Route 
             path="/payment" 
             element={
                <ProtectedRoute>
                   <PaymentPage />
                </ProtectedRoute>
             }
          />

          <Route 
             path="/booking-history" 
             element={
                <ProtectedRoute>
                   <BookingHistory />
                </ProtectedRoute>
             }
          />

          {/* Admin route: Only accessible for admins */}
          <Route 
             path="/admin-booking-history" 
             element={
                <AdminRoute>
                   <AdminBookingHistory />
                </AdminRoute>
             }
          />
          
        </Routes>
      </div>
    </Router>
  );
}

export default App;