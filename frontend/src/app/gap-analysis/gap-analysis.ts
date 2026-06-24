import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { ProjectService } from '../services/project';
import { Project, ProjectEmployee, ProjectRequirement } from '../models/project.model';

@Component({
  selector: 'app-gap-analysis',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './gap-analysis.html',
  styleUrl: './gap-analysis.css',
})
export class GapAnalysisComponent implements OnInit {
  projects: Project[] = [];
  selectedProjectId: number | null = null;
  selectedProject: Project | null = null;

  requirements: ProjectRequirement[] = [];
  employees: ProjectEmployee[] = [];
  
  loading = false;
  detailsLoading = false;
  gapAnalysisLoading = false;
  gapAnalysisResult: any = null;
  gapAnalysisError: string | null = null;

  get groupedGaps() {
    if (!this.gapAnalysisResult || !this.gapAnalysisResult.gaps) return [];
    const groups: { [key: string]: any[] } = {};
    for (const gap of this.gapAnalysisResult.gaps) {
      const email = gap.EmployeeEmail || gap.employeeEmail || 'Unknown Employee';
      if (!groups[email]) {
        groups[email] = [];
      }
      groups[email].push(gap);
    }
    return Object.keys(groups).map((email) => ({
      employeeEmail: email,
      gaps: groups[email],
    }));
  }

  constructor(private projectService: ProjectService) {}

  ngOnInit(): void {
    this.loadProjects();
  }

  loadProjects(): void {
    this.loading = true;
    this.projectService.getProjects().subscribe({
      next: (data) => {
        this.projects = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  onProjectChange(projectId: number | null): void {
    this.selectedProject = this.projects.find((p) => p.id === projectId) || null;
    this.requirements = [];
    this.employees = [];
    this.gapAnalysisResult = null;
    this.gapAnalysisError = null;

    if (projectId != null) {
      this.loadProjectDetails(projectId);
    }
  }

  loadProjectDetails(projectId: number): void {
    this.detailsLoading = true;
    forkJoin({
      reqs: this.projectService.getProjectRequirements(projectId),
      emps: this.projectService.getProjectEmployees(projectId),
    }).subscribe({
      next: ({ reqs, emps }) => {
        this.requirements = reqs;
        this.employees = emps;
        this.detailsLoading = false;
      },
      error: () => {
        this.detailsLoading = false;
      },
    });
  }

  runGapAnalysis(): void {
    if (this.selectedProjectId == null) return;
    this.gapAnalysisLoading = true;
    this.gapAnalysisResult = null;
    this.gapAnalysisError = null;
    
    this.projectService.runGapAnalysis(this.selectedProjectId).subscribe({
      next: (res) => {
        this.gapAnalysisResult = res;
        this.gapAnalysisLoading = false;
      },
      error: (err) => {
        this.gapAnalysisError = err.error?.error || 'Failed to perform gap analysis. Make sure you have a valid Gemini ApiKey configured.';
        this.gapAnalysisLoading = false;
      },
    });
  }
}
