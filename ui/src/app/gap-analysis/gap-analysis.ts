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
  showPreviewModal = false;

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

  getEmployeeDetails(email: string): { id: string; name: string } {
    const emp = this.employees.find(e => e.email?.toLowerCase() === email.toLowerCase());
    return emp ? { id: emp.id, name: emp.name } : { id: 'N/A', name: email };
  }

  calculateEmployeeScore(email: string): number {
    if (this.gapAnalysisResult && this.gapAnalysisResult.scores) {
      const aiScoreObj = this.gapAnalysisResult.scores.find((s: any) => 
        (s.EmployeeEmail || s.employeeEmail || '').toLowerCase() === email.toLowerCase()
      );
      if (aiScoreObj && aiScoreObj.Score !== undefined) {
        return Number(aiScoreObj.Score);
      }
      if (aiScoreObj && aiScoreObj.score !== undefined) {
        return Number(aiScoreObj.score);
      }
    }

    const employeeGaps = this.gapAnalysisResult?.gaps?.filter((g: any) => 
      (g.EmployeeEmail || g.employeeEmail || '').toLowerCase() === email.toLowerCase()
    ) || [];

    if (this.requirements.length === 0) return 100;

    let totalScore = 0;
    for (const req of this.requirements) {
      const gap = employeeGaps.find((g: any) => 
        (g.SkillName || g.skillName || '').toLowerCase() === req.skillName.toLowerCase()
      );

      let skillScore = 0;
      if (gap) {
        const severity = (gap.GapSeverity || gap.gapSeverity || '').toLowerCase();
        if (severity.includes('no gap') || severity === 'none') {
          skillScore = 100;
        } else if (severity.includes('minor')) {
          skillScore = 75;
        } else if (severity.includes('moderate')) {
          skillScore = 50;
        } else {
          skillScore = 0;
        }
      } else {
        skillScore = 0;
      }
      totalScore += skillScore;
    }

    return Math.round(totalScore / this.requirements.length);
  }

  escapeCsvValue(val: any): string {
    if (val === null || val === undefined) return '';
    const str = String(val);
    if (str.includes(',') || str.includes('"') || str.includes('\n') || str.includes('\r')) {
      return `"${str.replace(/"/g, '""')}"`;
    }
    return str;
  }

  reportDate = new Date();

  exportReport(): void {
    if (!this.gapAnalysisResult) return;
    this.reportDate = new Date();
    this.showPreviewModal = true;
  }

  getSkillGapLevel(gaps: any[], skillName: string): string {
    const gap = gaps.find(g => 
      (g.SkillName || g.skillName || '').toLowerCase() === skillName.toLowerCase()
    );
    return gap ? (gap.GapSeverity || gap.gapSeverity || 'Critical Gap') : 'Critical Gap';
  }

  printPdf(): void {
    window.print();
  }

  closePreview(): void {
    this.showPreviewModal = false;
  }

  downloadCsv(): void {
    if (!this.gapAnalysisResult) return;

    const skillNames = this.requirements.map(r => r.skillName);
    const headers = ['employee', ...skillNames, 'score'];
    
    const rows: string[][] = [headers];

    for (const group of this.groupedGaps) {
      const empDetails = this.getEmployeeDetails(group.employeeEmail);
      const employeeLabel = `${empDetails.id} - ${empDetails.name}`;

      const rowValues: string[] = [this.escapeCsvValue(employeeLabel)];

      for (const req of this.requirements) {
        const gap = group.gaps.find(g => 
          (g.SkillName || g.skillName || '').toLowerCase() === req.skillName.toLowerCase()
        );
        const gapLevel = gap ? (gap.GapSeverity || gap.gapSeverity || 'Critical Gap') : 'Critical Gap';
        rowValues.push(this.escapeCsvValue(gapLevel));
      }

      const score = this.calculateEmployeeScore(group.employeeEmail);
      rowValues.push(this.escapeCsvValue(score));

      rows.push(rowValues);
    }

    const csvContent = rows.map(r => r.join(',')).join('\n');
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.setAttribute('href', url);
    
    const projectName = this.selectedProject?.name || 'Project';
    const safeProjectName = projectName.replace(/[^a-z0-9]/gi, '_').toLowerCase();
    link.setAttribute('download', `${safeProjectName}_gap_analysis_report.csv`);
    
    link.style.visibility = 'hidden';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }
}
