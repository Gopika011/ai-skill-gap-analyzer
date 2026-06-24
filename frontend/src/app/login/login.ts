import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../services/auth';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, RouterLink, CommonModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  email = '';
  pass = '';
  message = '';
  isError = false;
  loading = false;

  constructor(private authService: AuthService, private router: Router) {}

  login() {
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

    this.authService.login(data)
      .subscribe({
        next: (response: any) => {
          this.loading = false;
          this.authService.setCurrentUser(response);
          this.router.navigate(['/projects']);
        },
        error: (error) => {
          this.loading = false;
          this.isError = true;
          this.message = error.error?.error || 'Invalid credentials';
          console.log(error);
        }
      });
  }
}
