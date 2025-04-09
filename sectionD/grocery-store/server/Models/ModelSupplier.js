const mongoose = require('mongoose');

const SupplierSchema = new mongoose.Schema({
    companyName: {
        type: String,
        required: true,
        trim: true
    },
    phone: {
        type: String,
        required: true,
        trim: true
    },
    representative: {
        type: String,
        required: true,
        trim: true
    },

    products: [
        {
            name: { type: String, required: true },
            price: { type: Number, required: true },
            minQuantity: { type: Number, required: true },
        }
    ]
}, { timestamps: true }); 

const suppliers = mongoose.model('suppliers', SupplierSchema);
module.exports = suppliers;

