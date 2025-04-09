const mongoose = require("mongoose");

const ordersSchema = new mongoose.Schema({
  supplier: {
    type: mongoose.Schema.Types.ObjectId,
    ref: "suppliers",
  }, 
  items: [
    {
      name: { type: String, required: true },
      quantity: { type: Number, required: true },
      pricePerUnit: { type: Number, required: true },
    },
  
  ],
  status: {
    type: String,
    enum: ["Pending", "In Process", "Completed"],
    default: "Pending",
  }, 
  createdAt: { type: Date, default: Date.now },
  updatedAt: { type: Date, default: Date.now },
});

const orders = mongoose.model("orders", ordersSchema);
module.exports = orders;
