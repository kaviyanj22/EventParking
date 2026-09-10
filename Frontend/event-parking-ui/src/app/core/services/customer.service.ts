import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Customer } from '../models/customer.model';
import { CustomerUpdate } from '../models/customer-update.model';
import { ServiceResult } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {

  private readonly apiUrl =
  'https://localhost:7168/api/customers';

  constructor(
    private http: HttpClient
  ) {}

  // =========================
  // GET MY PROFILE
  // GET /api/customers/me
  // =========================

  getMyProfile():
    Observable<ServiceResult<Customer>> {

    return this.http.get<
      ServiceResult<Customer>
    >(
      `${this.apiUrl}/me`
    );
  }

  // =========================
  // UPDATE MY PROFILE
  // PUT /api/customers/me
  // =========================

  updateMyProfile(
    request: CustomerUpdate
  ): Observable<ServiceResult<Customer>> {

    return this.http.put<
      ServiceResult<Customer>
    >(
      `${this.apiUrl}/me`,
      request
    );
  }

  // =========================
  // SEARCH CUSTOMERS
  // ADMIN ONLY
  // GET /api/customers?search=
  // =========================

  searchCustomers(
    search?: string
  ): Observable<Customer[]> {

    let params = new HttpParams();

    if (
      search &&
      search.trim().length > 0
    ) {
      params = params.set(
        'search',
        search.trim()
      );
    }

    return this.http.get<Customer[]>(
      this.apiUrl,
      { params }
    );
  }

  // =========================
  // GET CUSTOMER BY ID
  // ADMIN ONLY
  // GET /api/customers/{id}
  // =========================

  getCustomerById(
    customerId: number
  ): Observable<ServiceResult<Customer>> {

    return this.http.get<
      ServiceResult<Customer>
    >(
      `${this.apiUrl}/${customerId}`
    );
  }

  // =========================
  // DEACTIVATE CUSTOMER
  // ADMIN ONLY
  // PUT /api/customers/{id}/deactivate
  // =========================

  deactivateCustomer(
    customerId: number
  ): Observable<ServiceResult<object>> {

    return this.http.put<
      ServiceResult<object>
    >(
      `${this.apiUrl}/${customerId}/deactivate`,
      {}
    );
  }

  // =========================
  // REACTIVATE CUSTOMER
  // ADMIN ONLY
  // PUT /api/customers/{id}/reactivate
  // =========================

  reactivateCustomer(
    customerId: number
  ): Observable<ServiceResult<object>> { 

    return this.http.put<
      ServiceResult<object>
    >(
      `${this.apiUrl}/${customerId}/reactivate`,
      {}
    );
  }
}