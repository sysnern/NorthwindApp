import api from './axios';
import { Supplier } from '../types';

export const getSuppliers = async () => {
  const { data } = await api.get<{ data: Supplier[] }>('/Supplier/list');
  return data.data;
};
