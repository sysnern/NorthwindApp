import React from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';
import App from './App';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'react-toastify/dist/ReactToastify.css';

const container = document.getElementById('root');
const root = createRoot(container!);
root.render(
  <BrowserRouter>
    <App />
  </BrowserRouter>
);
