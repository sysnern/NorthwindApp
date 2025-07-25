import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { getProducts, deleteProduct } from '../api/productService';
import { Product } from '../types';
import ConfirmModal from '../components/ConfirmModal';
import { toast } from 'react-toastify';

const ProductList: React.FC = () => {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedId, setSelectedId] = useState<number | null>(null);

  const load = async () => {
    try {
      const data = await getProducts();
      setProducts(data);
    } catch {
      setError('Failed to load');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  const handleDelete = async () => {
    if (selectedId == null) return;
    try {
      await deleteProduct(selectedId);
      toast.success('Deleted');
      setSelectedId(null);
      load();
    } catch {
      toast.error('Delete failed');
    }
  };

  if (loading) return <p>Loading...</p>;
  if (error) return <p>{error}</p>;

  return (
    <div>
      <div className="d-flex justify-content-between mb-2">
        <h2>Products</h2>
        <Link to="/products/new" className="btn btn-primary">Add</Link>
      </div>
      <table className="table table-bordered">
        <thead>
          <tr>
            <th>Name</th>
            <th>Price</th>
            <th>Stock</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {products.map(p => (
            <tr key={p.productId}>
              <td>{p.productName}</td>
              <td>{p.unitPrice}</td>
              <td>{p.unitsInStock}</td>
              <td>
                <Link className="btn btn-sm btn-warning me-2" to={`/products/${p.productId}`}>Edit</Link>
                <button className="btn btn-sm btn-danger" onClick={() => setSelectedId(p.productId)}>Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      <ConfirmModal
        show={selectedId != null}
        onClose={() => setSelectedId(null)}
        onConfirm={handleDelete}
      />
    </div>
  );
};

export default ProductList;
