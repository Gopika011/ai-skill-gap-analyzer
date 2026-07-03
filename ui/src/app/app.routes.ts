import { Routes } from '@angular/router';
import { Login } from './login/login';
import { Register } from './register/register';
import { CategoriesComponent } from './categories/categories';
import { SkillsComponent } from './skills/skills';
import { EmployeesComponent } from './employees/employees';
import { EmployeeSkillsComponent } from './employee-skills/employee-skills';
import { ProjectsComponent } from './project/project';
import { GapAnalysisComponent } from './gap-analysis/gap-analysis';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from './services/auth';

const authGuard = () => {
  const authService = inject(AuthService);
  const router = inject(Router);
  if (authService.isLoggedIn()) {
    return true;
  }
  return router.parseUrl('/login');
};

const loginGuard = () => {
  const authService = inject(AuthService);
  const router = inject(Router);
  if (!authService.isLoggedIn()) {
    return true;
  }
  return router.parseUrl('/projects');
};

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },

  { path: 'login', component: Login, canActivate: [loginGuard] },
  { path: 'register', component: Register, canActivate: [loginGuard] },

  { path: 'categories', component: CategoriesComponent, canActivate: [authGuard] },
  { path: 'skills', component: SkillsComponent, canActivate: [authGuard] },
  { path: 'employees', component: EmployeesComponent, canActivate: [authGuard] },
  { path: 'employee-skills', component: EmployeeSkillsComponent, canActivate: [authGuard] },
  { path: 'projects', component: ProjectsComponent, canActivate: [authGuard] },
  { path: 'gap-analysis', component: GapAnalysisComponent, canActivate: [authGuard] },
];
