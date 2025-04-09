
const suppliers = require('../Models/ModelSupplier');

// Create a new supplier in the database  
exports.createSupplier = async (req, res) => {
  
  try {
    const supplier = new suppliers(req.body);
    console.log(req.body);
    
    await supplier.save();
    res.status(201).json(supplier);
  } catch (error) {
    res.status(400).json({ error: error.message });
  }
};

//Get all suppliers from the database  
exports.getAllSuppliers = async (req, res) => {
  try {
    console.log("get all suppliers");

    const allsuppliers = await suppliers.find();
    console.log(28, allsuppliers);
    
    res.json(allsuppliers);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
};

