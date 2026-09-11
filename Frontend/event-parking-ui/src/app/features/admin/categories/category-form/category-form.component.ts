import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { CategoryService } from '../../../../core/services/category.service';

import {
  Category,
  CategoryCreate,
  CategoryUpdate
} from '../../../../core/models/category.model';

@Component({
  selector: 'app-category-form',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './category-form.component.html',
  styleUrl: './category-form.component.css'
})
export class CategoryFormComponent implements OnChanges {

  @Input()
  category: Category | null = null;

  @Output()
  saved = new EventEmitter<void>();

  @Output()
  cancelled = new EventEmitter<void>();

  formData: CategoryCreate = {
    categoryName: '',
    description: null
  };

  submitting = false;
  errorMessage = '';
  successMessage = '';

  constructor(
    private categoryService: CategoryService
  ) {}

  ngOnChanges(): void {
    if (this.category) {
      this.formData = {
        categoryName: this.category.categoryName,
        description: this.category.description
      };
    } else {
      this.resetForm();
    }
  }

  saveCategory(): void {
    this.submitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    if (this.category) {

      const updateData: CategoryUpdate = {
        ...this.formData
      };

      this.categoryService
        .update(this.category.categoryId, updateData)
        .subscribe({
          next: () => {
            this.successMessage =
              'Category updated successfully.';

            this.submitting = false;
            this.saved.emit();
          },
          error: (error) => {
            this.errorMessage =
              error?.error?.message ||
              'Unable to update category.';

            this.submitting = false;
          }
        });

    } else {

      this.categoryService
        .create(this.formData)
        .subscribe({
          next: () => {
            this.successMessage =
              'Category created successfully.';

            this.submitting = false;
            this.resetForm();
            this.saved.emit();
          },
          error: (error) => {
            this.errorMessage =
              error?.error?.message ||
              'Unable to create category.';

            this.submitting = false;
          }
        });
    }
  }

  cancel(): void {
    this.resetForm();
    this.cancelled.emit();
  }

  resetForm(): void {
    this.formData = {
      categoryName: '',
      description: null
    };

    this.errorMessage = '';
  }
}