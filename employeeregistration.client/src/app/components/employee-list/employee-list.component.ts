import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { EmployeeService } from '../../services/employee.service';
import { Employee } from '../../models/employee.model';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialogComponent } from '../shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-employee-list',
  standalone: false,
  templateUrl: './employee-list.component.html',
  styleUrl: './employee-list.component.css'
})
export class EmployeeListComponent implements OnInit {
  displayedColumns: string[] = ['employeeId', 'employeeName', 'mobileNum', 'age', 'actions'];
  dataSource = new MatTableDataSource<Employee>();

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  filterName: string = '';
  filterMobile: string = '';
  totalCount = 0;
  pageSize = 5;
  currentPage = 0;
  totalPages = 0;
  constructor(private employeeService: EmployeeService, private router: Router, private dialog: MatDialog) { }

  ngOnInit(): void {

    this.loadEmployees();
  }

  loadEmployees(): void {
    this.employeeService.getEmployees(
      this.currentPage + 1,
      this.pageSize,
      this.filterName,
      this.filterMobile
    ).subscribe(response => {

      const employees = response.body?.$values || [];
      this.dataSource.data = employees;
      const totalCountHeader = response.headers.get('X-Total-Count');
      const totalPagesHeader = response.headers.get('X-Total-Pages');

      this.totalCount = totalCountHeader ? +totalCountHeader : 0;
      this.totalPages = totalPagesHeader ? +totalPagesHeader : 0;
      console.log(totalCountHeader, "wertyu")
      console.log(this.totalCount, "wertyu")

    });
  }

  edit(id: number): void {
    this.router.navigate(['/add', id]);
  }

  confirmDelete(id: number): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent);

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.employeeService.deleteEmployee(id).subscribe(() => {
          this.loadEmployees();
        });
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.loadEmployees();
  }

  onFilterChange(): void {
    this.currentPage = 0; // reset to first page
    this.loadEmployees();
  }

}
