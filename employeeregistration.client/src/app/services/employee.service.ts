import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Employee } from '../models/employee.model';
import { Observable } from 'rxjs/internal/Observable';
import { Country } from '../models/country.model';
import { State } from '../models/state.model';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  private apiUrl = 'https://localhost:44310/api/employee';

  constructor(private http: HttpClient) { }

  getEmployees(page: number, pageSize: number, name?: string, mobile?: string) {
    let params: any = {
      page: page.toString(),
      pageSize: pageSize.toString()
    };

    if (name) {
      params.name = name;
    }

    if (mobile) {
      params.mobile = mobile;
    }

    return this.http.get<{ $id: string; $values: Employee[] }>(this.apiUrl, {
      params,
      observe: 'response'
    });
  }

  getById(id: number) {
    return this.http.get<Employee>(`${this.apiUrl}/${id}`);
  }

  add(employee: Employee) {
    return this.http.post<Employee>(this.apiUrl, employee);
  }

  update(id: number, employee: Employee) {
    return this.http.put(`${this.apiUrl}/${id}`, employee);
  }

  deleteEmployee(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getCountries(): Observable<Country[]> {
    return this.http.get<any>(`https://localhost:44310/api/masterdata/countries`).pipe(
      map(res => res.$values || [])
    );
  }

  getStatesByCountry(countryId: number): Observable<State[]> {
    return this.http.get<any>(`https://localhost:44310/api/masterdata/states/${countryId}`).pipe(
      map(res => res.$values || [])
    );
  }


  checkMobile(mobile: string, excludeId?: number): Observable<boolean> {
    const url = excludeId
      ? `https://localhost:44310/api/Employee/check-mobile/${mobile}?excludeId=${excludeId}`
      : `https://localhost:44310/api/Employee/check-mobile/${mobile}`;

    return this.http.get(url, { responseType: 'text' }).pipe(
      map(response => response === 'true') 
    );
  }


  getNextId() {
    return this.http.get<number>(`${this.apiUrl}/next-id`);
  }
}

