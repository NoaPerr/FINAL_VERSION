const express = require('express');
const bodyParser = require('body-parser');
const cors = require('cors');
const routerOrders = require('./Routers/OrderRoutes');
const routerSuppliers = require('./Routers/SupplierRoutes');
const app = express();

// Middleware
app.use(bodyParser.json());
app.use(cors());

// Example route
app.use('/orders',routerOrders);
app.use('/suppliers', routerSuppliers);

// app.get('/', (req, res) => {
//     res.send('Server is running!');
// });


// Start the server
const PORT = 5000;
app.listen(PORT, () => {
    console.log(`Server is running on http://localhost:${PORT}`);
});

//connection to mongodb:
const mongoose = require('mongoose');

const mongoURI = "mongodb+srv://noaP25:np654123@cluster0.yjkunp3.mongodb.net/GroceryStore?retryWrites=true&w=majority";

// פונקציה לחיבור למסד הנתונים
const connectDB = async () => {
    try {
      await mongoose.connect(mongoURI);
      console.log(" MongoDB connected successfully!");
    } catch (error) {
      console.error(" MongoDB connection failed:", error.message);
    }
  };

// קריאה לפונקציה
connectDB();

