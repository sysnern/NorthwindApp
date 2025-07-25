import api from './axios';
import { Category } from '../types';

export const getCategories = async () => {
  const { data } = await api.get<{ data: Category[] }>('/Category/list');
  return data.data;
};
