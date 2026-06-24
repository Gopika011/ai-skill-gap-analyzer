import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { SkillService } from '../services/skill';
import { Skill, Category } from '../models/skill.model';

@Component({
  selector: 'app-skills',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './skills.html',
})
export class SkillsComponent implements OnInit {
  skills: Skill[] = [];
  categories: Category[] = [];
  editingId: number | null = null;
  loading = false;
  errorMessage = '';
  form: { skillName: string; description: string; categoryId: number | null } = {
    skillName: '',
    description: '',
    categoryId: null,
  };

  constructor(private skillService: SkillService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.errorMessage = '';
    forkJoin({
      skills: this.skillService.getSkills(),
      categories: this.skillService.getCategories(),
    }).subscribe({
      next: ({ skills, categories }) => {
        this.skills = skills;
        this.categories = categories;
        this.loading = false;
      },
      error: (err) => {
        this.errorMessage = err.error?.error || 'Failed to load skills and categories.';
        this.loading = false;
      },
    });
  }

  create(): void {
    if (!this.form.skillName.trim() || this.form.categoryId == null) return;
    this.errorMessage = '';
    this.skillService.createSkill({ ...this.form, categoryId: this.form.categoryId }).subscribe({
      next: () => {
        this.resetForm();
        this.load();
      },
      error: (err) => {
        this.errorMessage = err.error?.error || 'Failed to create skill.';
      }
    });
  }

  startEdit(s: Skill): void {
    this.editingId = s.id;
    this.errorMessage = '';
    this.form = {
      skillName: s.skillName,
      description: s.description || '',
      categoryId: s.categoryId,
    };
  }

  update(): void {
    if (!this.editingId || this.form.categoryId == null) return;
    this.errorMessage = '';
    this.skillService.updateSkill(this.editingId, { ...this.form, categoryId: this.form.categoryId }).subscribe({
      next: () => {
        this.resetForm();
        this.load();
      },
      error: (err) => {
        this.errorMessage = err.error?.error || 'Failed to update skill.';
      }
    });
  }

  delete(id: number): void {
    if (!confirm('Delete this skill?')) return;
    this.errorMessage = '';
    this.skillService.deleteSkill(id).subscribe({
      next: () => this.load(),
      error: (err) => {
        this.errorMessage = err.error?.error || 'Failed to delete skill. It might be used by an employee or project.';
      }
    });
  }

  cancelEdit(): void {
    this.resetForm();
  }

  private resetForm(): void {
    this.editingId = null;
    this.form = { skillName: '', description: '', categoryId: null };
  }
}
