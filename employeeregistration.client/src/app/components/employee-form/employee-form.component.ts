import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { EmployeeService } from '../../services/employee.service';
import { ActivatedRoute, Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Employee } from '../../models/employee.model';
import { Country } from '../../models/country.model';
import { State } from '../../models/state.model';
@Component({
  selector: 'app-employee-form',
  standalone: false,
  templateUrl: './employee-form.component.html',
  styleUrl: './employee-form.component.css'
})
export class EmployeeFormComponent implements OnInit {
  employeeForm!: FormGroup;
  isEditMode = false;
  maxDate = new Date().toISOString().split('T')[0];
  countries: Country[] = [];
  states: State[] = [];
  filteredStates: State[] = [];
  employeeIdFromRoute: number | null = null;

  constructor(
    private fb: FormBuilder,
    private employeeService: EmployeeService,
    private route: ActivatedRoute,
    private router: Router,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.employeeForm = this.fb.group({
      employeeId: [{ value: 0, disabled: true }],
      employeeName: ['', [Validators.required, Validators.pattern(/^[a-zA-Z ]+$/)]],
      age: [{ value: '', disabled: true }, [Validators.required, Validators.min(0), Validators.max(999)]],
      mobileNum: ['', [Validators.required, Validators.pattern(/^\d{10}$/)]],
      dob: [''],
      addressLine1: ['', [Validators.required, Validators.pattern(/^[^$%!+]*$/)]],
      addressLine2: ['', [Validators.pattern(/^[^$%!+]*$/)]],
      pincode: ['', [Validators.required, Validators.pattern(/^\d{6}$/)]],
      countryId: ['', Validators.required],
      stateId: [{ value: '', disabled: true }, Validators.required]
    });

    this.employeeService.getCountries().subscribe(data => (this.countries = data));

    const routeId = this.route.snapshot.paramMap.get('id');
    if (routeId) {
      this.isEditMode = true;
      this.employeeIdFromRoute = +routeId;
      this.employeeService.getById(+routeId).subscribe(emp => {
        this.filteredStates = this.states.filter(s => s.countryId === emp.countryId);
        if (emp.dob) {
          emp.dob = emp.dob.split('T')[0]; 
        }
        this.employeeForm.patchValue(emp);
        this.employeeForm.get('employeeId')?.setValue(emp.employeeId);
        this.employeeService.getStatesByCountry(emp.countryId).subscribe(states => {
          this.filteredStates = states;
          this.employeeForm.get('stateId')?.enable(); // Enable if states exist
        });
      });
    } else {
      this.employeeService.getNextId().subscribe(nextId => {
        this.employeeForm.get('employeeId')?.setValue(nextId);
      });
    }
  }

  onCountryChange(): void {
    const countryId = this.employeeForm.get('countryId')?.value;

    if (countryId) {
      this.employeeService.getStatesByCountry(countryId).subscribe(states => {
        this.filteredStates = states;

        if (states.length > 0) {
          this.employeeForm.get('stateId')?.enable();
        } else {
          this.employeeForm.get('stateId')?.disable();
        }

        this.employeeForm.get('stateId')?.setValue('');
      });
    } else {
      this.filteredStates = [];
      this.employeeForm.get('stateId')?.disable();
      this.employeeForm.get('stateId')?.setValue('');
    }
  }
  calculateAge(): void {
    const dob = this.employeeForm.get('dob')?.value;
    if (dob) {
      const birthDate = new Date(dob);
      const today = new Date();
      let age = today.getFullYear() - birthDate.getFullYear();
      const m = today.getMonth() - birthDate.getMonth();
      if (m < 0 || (m === 0 && today.getDate() < birthDate.getDate())) {
        age--;
      }
      this.employeeForm.get('age')?.setValue(age);
    }
  }

  onSubmit(): void {
    const formData = this.employeeForm.getRawValue();
    const currentId = this.isEditMode ? this.employeeIdFromRoute : undefined;
    const idToCheck = this.isEditMode ? Number(currentId) : undefined;
    

    this.employeeService.checkMobile(formData.mobileNum, idToCheck).subscribe(isDuplicate => {
      if (isDuplicate) {
        this.snackBar.open('Already registered with this mobile number. Please enter a new one.', 'Close', { duration: 3000 });
        return;
      }

      if (this.isEditMode) {
        this.employeeService.update(currentId!, formData).subscribe(() => {
          this.snackBar.open('Employee updated successfully.', 'Close', { duration: 3000 });
          this.router.navigate(['/employees']);
        });
      } else {
        this.employeeService.add(formData).subscribe(() => {
          this.snackBar.open('Employee added successfully.', 'Close', { duration: 3000 });
          this.router.navigate(['/employees']);
        });
      }
    });
  }
}
