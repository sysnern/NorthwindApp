import api from './axios';
import { Product } from '../types';

export const getProducts = async () => {
  const { data } = await api.get<{ data: Product[] }>('/Product/list');
  return data.data;
};

export const getProduct = async (id: number) => {
  const { data } = await api.get<{ data: Product }>(`/Product/${id}`);
  return data.data;
};

export const createProduct = async (product: Partial<Product>) => {
  await api.post('/Product', product);
};

export const updateProduct = async (product: Product) => {
  await api.put('/Product', product);
};

export const deleteProduct = async (id: number) => {
  await api.delete(`/Product/${id}`);
};
