import React, { useEffect, useState } from 'react';
import { getCategories } from '../api/categoryService';
import { Category } from '../types';

const CategoryList: React.FC = () => {
  const [categories, setCategories] = useState<Category[]>([]);

  useEffect(() => {
    getCategories().then(setCategories);
  }, []);

  return (
    <div>
      <h2>Categories</h2>
      <ul className="list-group">
        {categories.map(c => (
          <li key={c.categoryId} className="list-group-item">
            {c.categoryName}
          </li>
        ))}
      </ul>
    </div>
  );
};

export default CategoryList;
