import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';

import { CommonModule } from '@angular/common';

import {
  CategoryService
} from '../../../../core/services/category.service';

import {
  Category
} from '../../../../core/models/category.model';

import {
  CategoryFormComponent
} from '../category-form/category-form.component';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [
    CommonModule,
    CategoryFormComponent
  ],
  templateUrl: './category-list.component.html',
  styleUrl: './category-list.component.css'
})
export class CategoryListComponent implements OnInit {

  categories: Category[] = [];

  loading = false;

  errorMessage = '';

  successMessage = '';

  showForm = false;

  selectedCategory: Category | null = null;

  constructor(
    private categoryService: CategoryService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {

    this.loading = true;

    this.errorMessage = '';

    this.categoryService
      .getAll()
      .subscribe({

        next: (categories) => {

          this.categories = categories;

          this.loading = false;

          this.cdr.markForCheck();
        },

        error: (error) => {

          this.categories = [];

          this.loading = false;

          this.errorMessage =
            error?.error?.message ||
            'Unable to load categories.';

          this.cdr.markForCheck();
        }

      });
  }

  addCategory(): void {

    this.selectedCategory = null;

    this.showForm = true;
  }

  onCategorySaved(): void {

    this.showForm = false;

    this.selectedCategory = null;

    this.loadCategories();

    this.cdr.markForCheck();
  }

  onCategoryCancelled(): void {

    this.showForm = false;

    this.selectedCategory = null;
  }

  editCategory(
    categoryId: number
  ): void {

    const category =
      this.categories.find(
        c => c.categoryId === categoryId
      );

    if (category) {

      this.selectedCategory = category;

      this.showForm = true;
    }
  }

  deleteCategory(
    categoryId: number
  ): void {

    const confirmed =
      window.confirm(
        'Are you sure you want to delete this category?'
      );

    if (!confirmed) {
      return;
    }

    this.errorMessage = '';

    this.successMessage = '';

    this.categoryService
      .delete(categoryId)
      .subscribe({

        next: () => {

          this.successMessage =
            'Category deleted successfully.';

          this.loadCategories();

          this.cdr.markForCheck();
        },

        error: (error) => {

          this.errorMessage =
            error?.error?.message ||
            'Unable to delete category.';

          this.cdr.markForCheck();
        }

      });
  }
}