export interface Category {
  categoryId: number;
  categoryName: string;
  description: string | null;
  createdAt: string;
  updatedAt: string | null;
}

export interface CategoryCreate {
  categoryName: string;
  description: string | null;
}

export interface CategoryUpdate {
  categoryName: string;
  description: string | null;
}