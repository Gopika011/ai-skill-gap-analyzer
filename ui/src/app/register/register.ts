import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../services/auth';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterLink, CommonModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  email = '';
  pass = '';
  message = '';
  isError = false;
  loading = false;

  constructor(private authService: AuthService, private router: Router) {}

  register() {
    if (!this.email || !this.pass) {
      this.message = 'Please enter both email and password';
      this.isError = true;
      return;
    }

    this.loading = true;
    this.message = '';
    this.isError = false;

    const data = {
      email: this.email,
      pass: this.pass
    };

    this.authService.register(data)
      .subscribe({
        next: (response) => {
          this.loading = false;
          this.isError = false;
          this.message = 'Registration successful! Redirecting to login...';
          setTimeout(() => {
            this.router.navigate(['/login']);
          }, 1500);
        },
        error: (error) => {
          this.loading = false;
          this.isError = true;
          this.message = error.error || 'Registration failed. User may already exist.';
          console.log(error);
        }
      });
  }
}
