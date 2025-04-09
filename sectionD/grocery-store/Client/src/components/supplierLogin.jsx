import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import { TextField, Button, Typography, Container } from '@mui/material';

export default function Login() {
    const [allSuppliers, setAllSuppliers] = useState([]); // List of all suppliers
    const [supplierName, setSupplierName] = useState(''); // Supplier's name input
    const [supplierPhone, setSupplierPhone] = useState(''); // Supplier's phone input
    const navigate = useNavigate(); // Navigation hook

    useEffect(() => {
        // Fetch suppliers from the API
        axios.get('http://localhost:5000/suppliers')
            .then((response) => {
                setAllSuppliers(response.data); // Update state with supplier data
            })
            .catch((error) => {
                console.error('Error fetching suppliers:', error); 
            });
    }, []); // Runs once when the component mounts

    const handleSubmit = (e) => {
        e.preventDefault(); // Prevent form default behavior
        const newSupplier = { companyName: supplierName, phone: supplierPhone };
        const existSupplier = allSuppliers.find(s => s.companyName === newSupplier.companyName &&
            s.phone === newSupplier.phone);
        if (existSupplier) {
            navigate('/listOrders', { state: { existSupplier } }); 
        } else {
            alert("you are a new supplier click to register!"); 
            navigate('/register', { state: { newSupplier } }); 
        }
    };

    return (
        <Container maxWidth="sm">
            <Typography variant="h4" gutterBottom>
                Login Supplier
            </Typography>
            <form onSubmit={handleSubmit}>
                <TextField
                    label="Supplier Name"
                    variant="outlined"
                    fullWidth
                    margin="normal"
                    required
                    onBlur={(e) => setSupplierName(e.target.value)} 
                />
                <TextField
                    label="Phone"
                    variant="outlined"
                    fullWidth
                    margin="normal"
                    required
                    onBlur={(e) => setSupplierPhone(e.target.value)} 
                />
                <Button type="submit" variant="contained" color="primary" fullWidth>
                    Submit
                </Button>
            </form>
        </Container>
    );
}