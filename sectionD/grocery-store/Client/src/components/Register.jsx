import axios from "axios";
import React, { useState } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { TextField, Button, Typography, Container, Grid } from "@mui/material";

export default function Register() {
  const { state } = useLocation(); // Get supplier data passed from the previous page (if any)
  const navigate = useNavigate(); // Used for redirection after registration

  const [supplierName, setSupplierName] = useState(""); 
  const [supplierPhone, setSupplierPhone] = useState(""); 
  const [representative, setRepresentative] = useState(""); 
  const [products, setProducts] = useState([ 
    { name: "", price: "", minQuantity: "" },
  ]);

  
  const addProduct = () => {
    setProducts([...products, { name: "", price: "", minQuantity: "" }]);
  };

  
  const handleProductChange = (index, event) => {
    const values = [...products];
    values[index][event.target.name] = event.target.value; // Update the specific product field
    setProducts(values); // Update state with modified product list
  };

  // Submit the form and register the supplier
  const handleSubmit = (e) => {
    e.preventDefault(); 

    // POST request to create a new supplier
    axios
      .post("http://localhost:5000/suppliers", {
        companyName: state.newSupplier.companyName || supplierName, 
        phone: state.newSupplier.phone || supplierPhone, 
        representative, // Add representative name
        products, // Add products data
      })
      .then((response) => {
        const id = response.data._id; // Extract the new supplier's ID
        alert("Supplier registered successfully!"); 
        navigate('/listOrders', { state: { id } }); // Redirect to orders page with the supplier ID
      })
      .catch((error) => {
        console.error("Error creating supplier:", error); 
      });
  };

  return (
    <Container maxWidth="sm">
      <Typography variant="h4" gutterBottom>
        New Supplier Form
      </Typography>
      <form onSubmit={handleSubmit}>
        <TextField
          defaultValue={state.newSupplier.companyName} 
          label="Supplier Name"
          variant="outlined"
          fullWidth
          margin="normal"
          required
          onBlur={(e) => setSupplierName(e.target.value)} 
        />
        <TextField
          defaultValue={state.newSupplier.phone} 
          label="Phone"
          variant="outlined"
          fullWidth
          margin="normal"
          required
          onBlur={(e) => setSupplierPhone(e.target.value)} 
        />
        <TextField
          label="Representative"
          variant="outlined"
          fullWidth
          margin="normal"
          required
          onBlur={(e) => setRepresentative(e.target.value)} 
        />

        <Typography variant="h6" gutterBottom>
          Products
        </Typography>
        {products.map((product, index) => (
          <Grid container spacing={2} key={index}>
            <Grid item xs={4}>
              <TextField
                label="Product Name"
                name="name"
                variant="outlined"
                fullWidth
                value={product.name}
                onChange={(e) => handleProductChange(index, e)} 
                required
              />
            </Grid>
            <Grid item xs={4}>
              <TextField
                label="Price"
                name="price"
                type="number"
                variant="outlined"
                fullWidth
                value={product.price}
                onChange={(e) => handleProductChange(index, e)} 
                required
              />
            </Grid>
            <Grid item xs={4}>
              <TextField
                label="Minimum Quantity"
                name="minQuantity"
                type="number"
                variant="outlined"
                fullWidth
                value={product.minQuantity}
                onChange={(e) => handleProductChange(index, e)} 
                required
              />
            </Grid>
          </Grid>
        ))}
        <Button variant="outlined" onClick={addProduct}>
          + Add Product
        </Button>

        <Button
          type="submit"
          variant="contained"
          color="primary"
          fullWidth
          style={{ marginTop: "16px" }}
        >
          Submit
        </Button>
      </form>
    </Container>
  );
}
