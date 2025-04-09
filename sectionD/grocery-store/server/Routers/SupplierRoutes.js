const express = require('express');
const router = express.Router();
const supplierController = require('../Controllers/SupplierController');

router.post('/', supplierController.createSupplier);
router.get('/', supplierController.getAllSuppliers);


module.exports = router;
