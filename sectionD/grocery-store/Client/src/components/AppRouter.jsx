import React from 'react'
import { Route, Routes } from 'react-router-dom'
import Register from './Register'
import ListOrders from './ListOrders'
import Login from './supplierLogin'


export default function AppRouter() {
  return (
    <div>
        <Routes>
            <Route path='/' element={<Login />} />
            <Route path='/register' element={<Register />} />
            <Route path='/listOrders' element={<ListOrders></ListOrders>} />
        </Routes>
    </div>
  )
}
