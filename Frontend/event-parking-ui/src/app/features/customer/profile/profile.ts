import { CommonModule } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';

import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import {
  RouterLink
} from '@angular/router';

import {
  CustomerService
} from '../../../core/services/customer.service';

import {
  Customer
} from '../../../core/models/customer.model';

import {
  CustomerUpdate
} from '../../../core/models/customer-update.model';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink
  ],
  templateUrl: './profile.html',
  styleUrl: './profile.css'
})
export class Profile implements OnInit {

  customer: Customer | null = null;

  isLoading = true;

  isSaving = false;

  errorMessage = '';

  successMessage = '';

  profileForm;

  constructor(
    private formBuilder: FormBuilder,
    private customerService: CustomerService,
    private cdr: ChangeDetectorRef
  ) {

    this.profileForm =
      this.formBuilder.group({

        fullName: [
          '',
          [
            Validators.required,
            Validators.minLength(2),
            Validators.maxLength(100)
          ]
        ],

        phone: [
          '',
          [
            Validators.required
          ]
        ]

      });
  }

  ngOnInit(): void {
    this.loadProfile();
  }

  get fullName() {
    return this.profileForm.controls.fullName;
  }

  get phone() {
    return this.profileForm.controls.phone;
  }

  loadProfile(): void {

    this.isLoading = true;

    this.errorMessage = '';

    this.customerService
      .getMyProfile()
      .subscribe({

        next: response => {

          this.isLoading = false;

          if (
            !response.success ||
            !response.data
          ) {

            this.errorMessage =
              response.message ||
              'Unable to load your profile.';

            this.cdr.markForCheck();

            return;
          }

          this.customer =
            response.data;

          this.profileForm.patchValue({

            fullName:
              this.customer.fullName,

            phone:
              this.customer.phone

          });

          this.cdr.markForCheck();
        },

        error: error => {

          this.isLoading = false;

          this.errorMessage =
            error?.error?.message ||
            'Unable to load your profile.';

          this.cdr.markForCheck();
        }

      });
  }

  onSubmit(): void {

    this.errorMessage = '';

    this.successMessage = '';

    if (this.profileForm.invalid) {

      this.profileForm.markAllAsTouched();

      return;
    }

    const request: CustomerUpdate = {

      fullName:
        this.fullName.value ?? '',

      phone:
        this.phone.value ?? ''

    };

    this.isSaving = true;

    this.customerService
      .updateMyProfile(request)
      .subscribe({

        next: response => {

          this.isSaving = false;

          if (
            !response.success ||
            !response.data
          ) {

            this.errorMessage =
              response.message ||
              'Unable to update your profile.';

            this.cdr.markForCheck();

            return;
          }

          this.customer =
            response.data;

          this.profileForm.patchValue({

            fullName:
              this.customer.fullName,

            phone:
              this.customer.phone

          });

          this.successMessage =
            response.message ||
            'Profile updated successfully.';

          this.cdr.markForCheck();
        },

        error: error => {

          this.isSaving = false;

          this.errorMessage =
            error?.error?.message ||
            'Unable to update your profile. Please try again.';

          this.cdr.markForCheck();
        }

      });
  }

  resetForm(): void {

    if (!this.customer) {
      return;
    }

    this.profileForm.patchValue({

      fullName:
        this.customer.fullName,

      phone:
        this.customer.phone

    });

    this.errorMessage = '';

    this.successMessage = '';
  }
}