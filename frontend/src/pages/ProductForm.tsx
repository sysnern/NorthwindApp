import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useFormik } from 'formik';
import * as Yup from 'yup';
import { getProduct, createProduct, updateProduct } from '../api/productService';
import { getCategories } from '../api/categoryService';
import { getSuppliers } from '../api/supplierService';
import { Category, Supplier } from '../types';
import { toast } from 'react-toastify';

const schema = Yup.object({
  productName: Yup.string().required('Required'),
  unitPrice: Yup.number().required('Required'),
  unitsInStock: Yup.number().required('Required'),
  categoryId: Yup.number().required('Required'),
  supplierId: Yup.number().required('Required'),
});

const ProductForm: React.FC = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [categories, setCategories] = useState<Category[]>([]);
  const [suppliers, setSuppliers] = useState<Supplier[]>([]);

  const formik = useFormik({
    initialValues: {
      productId: 0,
      productName: '',
      unitPrice: 0,
      unitsInStock: 0,
      categoryId: 0,
      supplierId: 0,
      discontinued: false,
    },
    validationSchema: schema,
    onSubmit: async values => {
      try {
        if (values.productId) {
          await updateProduct(values as any);
          toast.success('Updated');
        } else {
          await createProduct(values);
          toast.success('Created');
        }
        navigate('/products');
      } catch {
        toast.error('Save failed');
      }
    },
  });

  useEffect(() => {
    (async () => {
      const [cats, sups] = await Promise.all([getCategories(), getSuppliers()]);
      setCategories(cats);
      setSuppliers(sups);
    })();
  }, []);

  useEffect(() => {
    if (id) {
      getProduct(Number(id)).then(p => formik.setValues(p));
    }
  }, [id]);

  return (
    <form onSubmit={formik.handleSubmit}>
      <h2>{formik.values.productId ? 'Edit' : 'Add'} Product</h2>
      <div className="mb-3">
        <label className="form-label">Name</label>
        <input
          type="text"
          className="form-control"
          {...formik.getFieldProps('productName')}
        />
        {formik.touched.productName && formik.errors.productName && (
          <div className="text-danger">{formik.errors.productName}</div>
        )}
      </div>
      <div className="mb-3">
        <label className="form-label">Price</label>
        <input type="number" className="form-control" {...formik.getFieldProps('unitPrice')} />
        {formik.touched.unitPrice && formik.errors.unitPrice && (
          <div className="text-danger">{formik.errors.unitPrice}</div>
        )}
      </div>
      <div className="mb-3">
        <label className="form-label">Stock</label>
        <input type="number" className="form-control" {...formik.getFieldProps('unitsInStock')} />
        {formik.touched.unitsInStock && formik.errors.unitsInStock && (
          <div className="text-danger">{formik.errors.unitsInStock}</div>
        )}
      </div>
      <div className="mb-3">
        <label className="form-label">Category</label>
        <select className="form-select" {...formik.getFieldProps('categoryId')}>
          <option value="">Select</option>
          {categories.map(c => (
            <option key={c.categoryId} value={c.categoryId}>{c.categoryName}</option>
          ))}
        </select>
        {formik.touched.categoryId && formik.errors.categoryId && (
          <div className="text-danger">{formik.errors.categoryId}</div>
        )}
      </div>
      <div className="mb-3">
        <label className="form-label">Supplier</label>
        <select className="form-select" {...formik.getFieldProps('supplierId')}>
          <option value="">Select</option>
          {suppliers.map(s => (
            <option key={s.supplierId} value={s.supplierId}>{s.companyName}</option>
          ))}
        </select>
        {formik.touched.supplierId && formik.errors.supplierId && (
          <div className="text-danger">{formik.errors.supplierId}</div>
        )}
      </div>
      <div className="form-check mb-3">
        <input type="checkbox" className="form-check-input" {...formik.getFieldProps('discontinued')} />
        <label className="form-check-label">Discontinued</label>
      </div>
      <button className="btn btn-primary" type="submit">Save</button>
    </form>
  );
};

export default ProductForm;
