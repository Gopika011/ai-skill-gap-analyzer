import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Category, Skill, EmployeeSkill } from '../models/skill.model';
import { User } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class SkillService {
  private readonly base = 'http://localhost:5213';
  private readonly http = inject(HttpClient);

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.base}/users`);
  }

  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.base}/categories`);
  }

  createCategory(data: { categoryName: string; description?: string }): Observable<Category> {
    return this.http.post<Category>(`${this.base}/categories`, data);
  }

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

  getEmployeeSkills(userId: number): Observable<EmployeeSkill[]> {
    return this.http.get<EmployeeSkill[]>(`${this.base}/employees/${userId}/skills`);
  }

  addEmployeeSkill(
    userId: number,
    data: { skillId: number; proficiencyLevel: string; yearsOfExperience: number },
  ): Observable<EmployeeSkill> {
    return this.http.post<EmployeeSkill>(`${this.base}/employees/${userId}/skills`, data);
  }

  deleteEmployeeSkill(userId: number, skillId: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/employees/${userId}/skills/${skillId}`);
  }
}
