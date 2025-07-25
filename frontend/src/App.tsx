import React from 'react';
import { Route, Routes, Navigate } from 'react-router-dom';
import Navbar from './components/Navbar';
import ProductList from './pages/ProductList';
import ProductForm from './pages/ProductForm';
import CategoryList from './pages/CategoryList';
import SupplierList from './pages/SupplierList';
import { ToastContainer } from 'react-toastify';

const App: React.FC = () => (
  <>
    <Navbar />
    <div className="container mt-3">
      <Routes>
        <Route path="/" element={<Navigate to="/products" />} />
        <Route path="/products" element={<ProductList />} />
        <Route path="/products/new" element={<ProductForm />} />
        <Route path="/products/:id" element={<ProductForm />} />
        <Route path="/categories" element={<CategoryList />} />
        <Route path="/suppliers" element={<SupplierList />} />
      </Routes>
    </div>
    <ToastContainer />
  </>
);

export default App;
