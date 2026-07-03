import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Category, Skill, EmployeeSkill } from '../models/skill.model';
import { User } from '../models/user.model';
import { Employee } from '../models/employee.model';

@Injectable({ providedIn: 'root' })
export class SkillService {
  private readonly base = 'http://localhost:5213';
  private readonly http = inject(HttpClient);

  // Users (for auth/admins)
  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.base}/users`);
  }

  // Employees Management
  getEmployees(): Observable<Employee[]> {
    return this.http.get<Employee[]>(`${this.base}/employees`);
  }

  createEmployee(data: { id?: string; name: string; email: string }): Observable<Employee> {
    return this.http.post<Employee>(`${this.base}/employees`, data);
  }

  updateEmployee(id: string, data: { name: string; email: string }): Observable<void> {
    return this.http.put<void>(`${this.base}/employees/${id}`, data);
  }

  deleteEmployee(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/employees/${id}`);
  }

  importEmployees(data: { id?: string; name: string; email: string }[]): Observable<{ count: number }> {
    return this.http.post<{ count: number }>(`${this.base}/employees/import`, data);
  }

  // Categories
  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.base}/categories`);
  }

  createCategory(data: { categoryName: string; description?: string }): Observable<Category> {
    return this.http.post<Category>(`${this.base}/categories`, data);
  }

  // Skills
  getSkills(): Observable<Skill[]> {
    return this.http.get<Skill[]>(`${this.base}/skills`);
  }

  createSkill(data: { skillName: string; description?: string; categoryId: number }): Observable<Skill> {
    return this.http.post<Skill>(`${this.base}/skills`, data);
  }

  updateSkill(skillId: number, data: { skillName: string; description?: string; categoryId: number }): Observable<void> {
    return this.http.put<void>(`${this.base}/skills/${skillId}`, data);
  }

  deleteSkill(skillId: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/skills/${skillId}`);
  }

  // Employee Skills (using string employeeId)
  getEmployeeSkills(employeeId: string): Observable<EmployeeSkill[]> {
    return this.http.get<EmployeeSkill[]>(`${this.base}/employees/${employeeId}/skills`);
  }

  addEmployeeSkill(
    employeeId: string,
    data: { skillId: number; proficiencyLevel: string; yearsOfExperience: number },
  ): Observable<EmployeeSkill> {
    return this.http.post<EmployeeSkill>(`${this.base}/employees/${employeeId}/skills`, data);
  }

  deleteEmployeeSkill(employeeId: string, skillId: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/employees/${employeeId}/skills/${skillId}`);
  }
}
