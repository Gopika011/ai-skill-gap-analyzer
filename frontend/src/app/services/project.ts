import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Project, ProjectEmployee, ProjectRequirement } from '../models/project.model';

@Injectable({ providedIn: 'root' })
export class ProjectService {
  private readonly base = 'http://localhost:5213';
  private readonly http = inject(HttpClient);

  // Projects
  getProjects(): Observable<Project[]> {
    return this.http.get<Project[]>(`${this.base}/projects`);
  }

  createProject(data: {
    name: string;
    description: string;
    startDate: string;
    endDate: string;
  }): Observable<Project> {
    return this.http.post<Project>(`${this.base}/projects`, data);
  }

  deleteProject(projectId: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/projects/${projectId}`);
  }

  // Project Employees
  getProjectEmployees(projectId: number): Observable<ProjectEmployee[]> {
    return this.http.get<ProjectEmployee[]>(`${this.base}/projects/${projectId}/employees`);
  }

  assignEmployee(projectId: number, data: { userId: number }): Observable<void> {
    return this.http.post<void>(`${this.base}/projects/${projectId}/employees`, data);
  }

  removeEmployee(projectId: number, userId: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/projects/${projectId}/employees/${userId}`);
  }

  // Project Requirements
  getProjectRequirements(projectId: number): Observable<ProjectRequirement[]> {
    return this.http.get<ProjectRequirement[]>(`${this.base}/projects/${projectId}/requirements`);
  }

  addRequirement(projectId: number, data: { skillId: number; requiredLevel: string }): Observable<void> {
    return this.http.post<void>(`${this.base}/projects/${projectId}/requirements`, data);
  }

  removeRequirement(projectId: number, skillId: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/projects/${projectId}/requirements/${skillId}`);
  }

  runGapAnalysis(projectId: number): Observable<any> {
    return this.http.post<any>(`${this.base}/gap-analysis/project/${projectId}`, {});
  }
}