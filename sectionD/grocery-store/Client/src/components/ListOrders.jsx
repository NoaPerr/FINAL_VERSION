import React, { useState, useEffect } from "react";
import { useLocation } from "react-router-dom";
import axios from "axios";
import { Card, CardContent, Typography, Grid, Button } from "@mui/material";

export default function ListOrders() {
  const [allOrders, setAllOrders] = useState([]); 
  const { state } = useLocation(); // Fetch supplier details from location state

  useEffect(() => {
    // Fetch orders only if a supplier exists in the state
    if (state.existSupplier) {
      axios
        .get(
          `http://localhost:5000/orders/getAllOrdersBySId/${state.existSupplier._id}`
        ) // Fetch orders by supplier ID
        .then((response) => {
          setAllOrders(response.data); // Set orders to state when fetched
        })
        .catch((error) => {
          console.error("Error fetching orders:", error); 
        });
    }
  }, []); 

  const changeStatus = (orderId) => {
    axios
      .put(`http://localhost:5000/orders/updateOrderStatus/${orderId}`) // Send request to update status
      .then((response) => {
        alert("The status has been changed"); 
        setAllOrders((prevOrders) =>
          prevOrders.map((order) =>
            order._id === orderId ? { ...order, status: "In Process" } : order
          ) 
        );
      })
      .catch((error) => {
        console.error("Error updating order status:", error); 
      });
  };

  return (
    <div>
      <h1>Your Orders:</h1>
      <Grid container spacing={2}>
        {allOrders.length > 0 ? (
          allOrders.map((order) => (
            <Grid item xs={12} sm={6} md={4} key={order._id}>
              <Card>
                <CardContent>
                  <Typography variant="h5">Order ID: {order._id}</Typography>
                  <Typography variant="body2">
                    Status: {order.status} {/* Display the status of the order */}
                  </Typography>
                  <Button
                    variant="contained"
                    color="primary"
                    onClick={() => changeStatus(order._id)} 
                  >
                    Confirm Order
                  </Button>
                  <Typography variant="body2">
                    Created At: {new Date(order.createdAt).toLocaleString()} 
                  </Typography>
                  <Typography variant="body2">
                    Updated At: {new Date(order.updatedAt).toLocaleString()} 
                  </Typography>
                  <Typography variant="h6">Items:</Typography>
                  {/* Display order items, mapping through them */}
                  {order.items.map((item) => (
                    <Typography key={item._id}>
                      {item.name} - Quantity: {item.quantity} - Price per Unit: {item.pricePerUnit}₪
                    </Typography>
                  ))}
                </CardContent>
              </Card>
            </Grid>
          ))
        ) : (
          <div style={{ color: "red", fontWeight: "bold" }}>
            Please note: You have no orders yet! 
          </div>
        )}
      </Grid>
    </div>
  );
}
