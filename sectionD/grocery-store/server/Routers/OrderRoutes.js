const express = require('express');
const router = express.Router();
const orderController = require('../Controllers/OrderController');
console.log('order route');

router.post('/', orderController.createOrder);
router.get('/', orderController.getAllOrders);
router.get('/getFilteredOrders', orderController.getFilteredOrders);
router.get('/getAllOrdersBySId/:id',orderController.getAllOrdersBySupplierID);
router.put('/updateOrderStatus/:id', orderController.updateOrderStatus);

module.exports = router;

