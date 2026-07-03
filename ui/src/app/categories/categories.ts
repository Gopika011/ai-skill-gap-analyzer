import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SkillService } from '../services/skill';
import { Category } from '../models/skill.model';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './categories.html',
})
export class CategoriesComponent implements OnInit {
  categories: Category[] = [];
  newName = '';
  newDesc = '';
  loading = false;
  errorMessage = '';

  constructor(private skillService: SkillService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.errorMessage = '';
    this.skillService.getCategories().subscribe({
      next: (data) => {
        this.categories = data;
        this.loading = false;
      },
      error: (err) => {
        this.errorMessage = err.error?.error || 'Failed to load categories.';
        this.loading = false;
      },
    });
  }

  create(): void {
    if (!this.newName.trim()) return;
    this.errorMessage = '';
    this.skillService.createCategory({ categoryName: this.newName, description: this.newDesc }).subscribe({
      next: () => {
        this.newName = '';
        this.newDesc = '';
        this.load();
      },
      error: (err) => {
        this.errorMessage = err.error?.error || 'Failed to create category. It might already exist.';
      }
    });
  }
}
