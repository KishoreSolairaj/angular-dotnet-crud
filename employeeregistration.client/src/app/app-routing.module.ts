import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EmployeeFormComponent } from './components/employee-form/employee-form.component';
import { EmployeeListComponent } from './components/employee-list/employee-list.component';

const routes: Routes = [
  { path: 'add', component: EmployeeFormComponent },
  {path: 'add/:id', component: EmployeeFormComponent},
  { path: 'list', component: EmployeeListComponent },
  { path: '', redirectTo: 'list', pathMatch: 'full' }, 
  { path: '**', redirectTo: 'list' } 
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
