import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { ProjectService } from '../services/project';
import { SkillService } from '../services/skill';
import { Project, ProjectEmployee, ProjectRequirement } from '../models/project.model';
import { Skill } from '../models/skill.model';
import { User } from '../models/user.model';

@Component({
  selector: 'app-projects',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './project.html',
  styleUrl: './project.css'
})
export class ProjectsComponent implements OnInit {
  projects: Project[] = [];
  skills: Skill[] = [];
  users: User[] = [];
  loading = false;
  detailsLoading = false;
  errorMessage = '';

  selectedProject: Project | null = null;
  requirements: ProjectRequirement[] = [];
  employees: ProjectEmployee[] = [];

  form = { name: '', description: '', startDate: '', endDate: '' };
  requirementForm = { skillId: null as number | null, requiredLevel: 'Intermediate' };
  employeeForm = { userId: null as number | null };

  levels = ['Beginner', 'Intermediate', 'Advanced', 'Expert'] as const;

  constructor(
    private projectService: ProjectService,
    private skillService: SkillService
  ) {}

  ngOnInit(): void {
    this.load();
    this.loadSkillsAndUsers();
  }

  load(): void {
    this.loading = true;
    this.errorMessage = '';
    this.projectService.getProjects().subscribe({
      next: (data) => {
        this.projects = data;
        this.loading = false;
      },
      error: (err) => {
        this.errorMessage = err.error?.error || 'Failed to load projects.';
        this.loading = false;
      },
    });
  }

  loadSkillsAndUsers(): void {
    this.errorMessage = '';
    forkJoin({
      skills: this.skillService.getSkills(),
      users: this.skillService.getUsers()
    }).subscribe({
      next: ({ skills, users }) => {
        this.skills = skills;
        this.users = users;
      },
      error: (err) => {
        this.errorMessage = err.error?.error || 'Failed to load skills and employees.';
      }
    });
  }

  create(): void {
    if (!this.form.name.trim() || !this.form.startDate || !this.form.endDate) return;
    this.errorMessage = '';
    this.projectService.createProject(this.form).subscribe({
      next: () => {
        this.form = { name: '', description: '', startDate: '', endDate: '' };
        this.load();
      },
      error: (err) => {
        this.errorMessage = err.error?.error || 'Failed to create project.';
      }
    });
  }

  deleteProject(projectId: number): void {
    if (!confirm('Are you sure you want to delete this project?')) return;
    this.errorMessage = '';
    this.projectService.deleteProject(projectId).subscribe({
      next: () => {
        if (this.selectedProject?.id === projectId) {
          this.selectedProject = null;
        }
        this.load();
      },
      error: (err) => {
        this.errorMessage = err.error?.error || 'Failed to delete project.';
      }
    });
  }

  selectProject(project: Project): void {
    this.selectedProject = project;
    this.loadProjectDetails(project.id);
  }

  loadProjectDetails(projectId: number): void {
    this.detailsLoading = true;
    this.errorMessage = '';
    forkJoin({
      reqs: this.projectService.getProjectRequirements(projectId),
      emps: this.projectService.getProjectEmployees(projectId)
    }).subscribe({
      next: ({ reqs, emps }) => {
        this.requirements = reqs;
        this.employees = emps;
        this.detailsLoading = false;
        this.requirementForm.skillId = null;
        this.employeeForm.userId = null;
      },
      error: (err) => {
        this.errorMessage = err.error?.error || 'Failed to load project details.';
        this.detailsLoading = false;
      }
    });
  }

  addRequirement(): void {
    if (!this.selectedProject || this.requirementForm.skillId == null) return;
    this.errorMessage = '';
    this.projectService.addRequirement(this.selectedProject.id, {
      skillId: this.requirementForm.skillId,
      requiredLevel: this.requirementForm.requiredLevel
    }).subscribe({
      next: () => {
        this.loadProjectDetails(this.selectedProject!.id);
      },
      error: (err) => {
        this.errorMessage = err.error?.error || 'Failed to add requirement. Skill may already be required.';
      }
    });
  }

  removeRequirement(skillId: number): void {
    if (!this.selectedProject || !confirm('Remove this skill requirement?')) return;
    this.errorMessage = '';
    this.projectService.removeRequirement(this.selectedProject.id, skillId).subscribe({
      next: () => {
        this.loadProjectDetails(this.selectedProject!.id);
      },
      error: (err) => {
        this.errorMessage = err.error?.error || 'Failed to remove requirement.';
      }
    });
  }

  assignEmployee(): void {
    if (!this.selectedProject || this.employeeForm.userId == null) return;
    this.errorMessage = '';
    this.projectService.assignEmployee(this.selectedProject.id, {
      userId: this.employeeForm.userId
    }).subscribe({
      next: () => {
        this.loadProjectDetails(this.selectedProject!.id);
      },
      error: (err) => {
        this.errorMessage = err.error?.error || 'Failed to assign employee. Employee may already be assigned.';
      }
    });
  }

  removeEmployee(userId: number): void {
    if (!this.selectedProject || !confirm('Remove this employee assignment?')) return;
    this.errorMessage = '';
    this.projectService.removeEmployee(this.selectedProject.id, userId).subscribe({
      next: () => {
        this.loadProjectDetails(this.selectedProject!.id);
      },
      error: (err) => {
        this.errorMessage = err.error?.error || 'Failed to remove employee assignment.';
      }
    });
  }
}