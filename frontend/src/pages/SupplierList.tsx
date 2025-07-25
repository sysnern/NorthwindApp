import React, { useEffect, useState } from 'react';
import { getSuppliers } from '../api/supplierService';
import { Supplier } from '../types';

const SupplierList: React.FC = () => {
  const [suppliers, setSuppliers] = useState<Supplier[]>([]);

  useEffect(() => {
    getSuppliers().then(setSuppliers);
  }, []);

  return (
    <div>
      <h2>Suppliers</h2>
      <ul className="list-group">
        {suppliers.map(s => (
          <li key={s.supplierId} className="list-group-item">
            {s.companyName}
          </li>
        ))}
      </ul>
    </div>
  );
};

export default SupplierList;
