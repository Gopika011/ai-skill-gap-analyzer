import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { SkillService } from '../services/skill';
import { EmployeeSkill, Skill } from '../models/skill.model';
import { User } from '../models/user.model';

@Component({
  selector: 'app-employee-skills',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './employee-skills.html',
})
export class EmployeeSkillsComponent implements OnInit {
  employeeSkills: EmployeeSkill[] = [];
  allSkills: Skill[] = [];
  users: User[] = [];
  userId: number | null = null;
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
      users: this.skillService.getUsers(),
    }).subscribe(({ skills, users }) => {
      this.allSkills = skills;
      this.users = users;
    });
  }

  onUserChange(userId: number | null): void {
    this.userId = userId;
    this.employeeSkills = [];
    this.resetForm();
    if (userId != null) {
      this.loadEmployeeSkills(userId);
    }
  }

  private loadEmployeeSkills(userId: number): void {
    this.loading = true;
    this.skillService.getEmployeeSkills(userId).subscribe({
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
    if (this.userId == null || this.form.skillId == null || !this.form.proficiencyLevel) return;
    this.skillService
      .addEmployeeSkill(this.userId, {
        skillId: this.form.skillId,
        proficiencyLevel: this.form.proficiencyLevel,
        yearsOfExperience: this.form.yearsOfExperience,
      })
      .subscribe(() => {
        this.resetForm();
        this.loadEmployeeSkills(this.userId!);
      });
  }

  remove(skillId: number): void {
    if (this.userId == null || !confirm('Remove this skill?')) return;
    this.skillService.deleteEmployeeSkill(this.userId, skillId).subscribe(() => {
      this.loadEmployeeSkills(this.userId!);
    });
  }

  private resetForm(): void {
    this.form = { skillId: null, proficiencyLevel: '', yearsOfExperience: 0 };
  }
}
