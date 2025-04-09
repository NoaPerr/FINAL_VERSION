// Create a new order in the database.
exports.createOrder = async (req, res) => {
  try {
    const { supplier, items } = req.body;
    const foundSupplier = await suppliers.findById(supplier);
    if (!foundSupplier)
      return res.status(404).json({ message: "Supplier not found" });

    const order = new Order({
      foundSupplier: supplier,
      items,
    });

    await order.save();
    res.status(201).json(order); 
  } catch (error) {
    res.status(400).json({ error: error.message }); 
  }
};

// Get all orders from the database.
exports.getAllOrders = async (req, res) => {
  try {
    console.log("get orders");
    const orders = await Order.find(); 
    console.log(30, orders);
    res.json(orders); 
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
};

// Get orders with a status of "Pending" or "In Process" from the database.
exports.getFilteredOrders = async (req, res) => {
  try {
    console.log("get filtered orders");

    const orders = await Order.find({
      status: { $in: ["Pending", "In Process"] }
    });

    console.log(30, orders);
    res.json(orders); 
  } catch (error) {
    res.status(500).json({ error: error.message }); 
  }
};

// Get orders for a specific supplier by their supplier ID from the database.
exports.getAllOrdersBySupplierID = async (req, res) => {
  try {
    const supplierId = req.params.id;
    const orders = await Order.find({ supplier: supplierId });
    if (orders.length === 0) {
      return res
        .status(404)
        .json({ message: "No orders found for this supplier" });
    }
    res.status(200).json(orders); 
  } catch (error) {
    res.status(500).json({ error: error.message }); 
  }
};

// Update the status of a specific order to "In Process"
exports.updateOrderStatus = async (req, res) => {
  try {
    const order = await Order.findByIdAndUpdate(
      req.params.id,
      { status: "In Process" },
      { new: true }
    );

    if (!order) return res.status(404).json({ message: "Order not found" });
    res.json(order); 
  } catch (error) {
    res.status(400).json({ error: error.message }); 
  }
};
