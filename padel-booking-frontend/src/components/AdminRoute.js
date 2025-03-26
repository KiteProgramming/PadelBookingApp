import React from 'react';
import { Navigate } from 'react-router-dom';

// A helper to decode the JWT (this is a simplistic example—consider using a proper library)
const getRoleFromToken = () => {
    const token = localStorage.getItem('accessToken');
    if (!token) return null;
    try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        return payload.role;
    } catch (e) {
        return null;
    }
};

const AdminRoute = ({ children }) => {
    const role = getRoleFromToken();    
    if (role && role.toLowerCase() === 'admin') {
        return children;
    }
    // If not admin, redirect to Home (or login)
    return <Navigate to="/login" />;
};

export default AdminRoute;