import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { SkillService } from '../services/skill';
import { EmployeeSkill, Skill } from '../models/skill.model';
import { Employee } from '../models/employee.model';

@Component({
  selector: 'app-employee-skills',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './employee-skills.html',
})
export class EmployeeSkillsComponent implements OnInit {
  employeeSkills: EmployeeSkill[] = [];
  allSkills: Skill[] = [];
  employees: Employee[] = [];
  employeeId: string | null = null;
  loading = false;
  levels = ['Beginner', 'Intermediate', 'Advanced', 'Expert'] as const;
  form: { skillId: number | null; proficiencyLevel: string; yearsOfExperience: number } = {
    skillId: null,
    proficiencyLevel: '',
    yearsOfExperience: 0,
  };

  constructor(private skillService: SkillService) {}

  ngOnInit(): void {
    forkJoin({
      skills: this.skillService.getSkills(),
      employees: this.skillService.getEmployees(),
    }).subscribe(({ skills, employees }) => {
      this.allSkills = skills;
      this.employees = employees;
    });
  }

  onEmployeeChange(employeeId: string | null): void {
    this.employeeId = employeeId;
    this.employeeSkills = [];
    this.resetForm();
    if (employeeId != null) {
      this.loadEmployeeSkills(employeeId);
    }
  }

  private loadEmployeeSkills(employeeId: string): void {
    this.loading = true;
    this.skillService.getEmployeeSkills(employeeId).subscribe({
      next: (data) => {
        this.employeeSkills = data;
        this.loading = false;
      },
      error: () => {
        this.employeeSkills = [];
        this.loading = false;
      },
    });
  }

  add(): void {
    if (this.employeeId == null || this.form.skillId == null || !this.form.proficiencyLevel) return;
    this.skillService
      .addEmployeeSkill(this.employeeId, {
        skillId: this.form.skillId,
        proficiencyLevel: this.form.proficiencyLevel,
        yearsOfExperience: this.form.yearsOfExperience,
      })
      .subscribe(() => {
        this.resetForm();
        this.loadEmployeeSkills(this.employeeId!);
      });
  }

  remove(skillId: number): void {
    if (this.employeeId == null || !confirm('Remove this skill?')) return;
    this.skillService.deleteEmployeeSkill(this.employeeId, skillId).subscribe(() => {
      this.loadEmployeeSkills(this.employeeId!);
    });
  }

  private resetForm(): void {
    this.form = { skillId: null, proficiencyLevel: '', yearsOfExperience: 0 };
  }
}
