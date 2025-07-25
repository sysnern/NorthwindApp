import React from 'react';
import { Link } from 'react-router-dom';

const Navbar: React.FC = () => (
  <nav className="navbar navbar-expand navbar-dark bg-dark">
    <div className="container">
      <Link className="navbar-brand" to="/">Northwind</Link>
      <ul className="navbar-nav">
        <li className="nav-item">
          <Link className="nav-link" to="/products">Products</Link>
        </li>
        <li className="nav-item">
          <Link className="nav-link" to="/categories">Categories</Link>
        </li>
        <li className="nav-item">
          <Link className="nav-link" to="/suppliers">Suppliers</Link>
        </li>
      </ul>
    </div>
  </nav>
);

export default Navbar;
