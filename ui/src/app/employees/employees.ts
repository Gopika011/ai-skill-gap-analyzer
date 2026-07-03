import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SkillService } from '../services/skill';
import { Employee } from '../models/employee.model';
import * as XLSX from 'xlsx';

@Component({
  selector: 'app-employees',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './employees.html',
  styleUrl: './employees.css'
})
export class EmployeesComponent implements OnInit {
  employees: Employee[] = [];
  loading = false;
  submitting = false;
  errorMessage = '';
  successMessage = '';

  // Form for single addition
  form = { id: '', name: '', email: '' };

  // Inline editing
  editingEmployeeId: string | null = null;
  editForm = { name: '', email: '' };

  // Excel / CSV Bulk Import
  importPreview: any[] = [];
  showImportPreview = false;

  constructor(private skillService: SkillService) {}

  ngOnInit(): void {
    this.loadEmployees();
  }

  loadEmployees(): void {
    this.loading = true;
    this.errorMessage = '';
    this.skillService.getEmployees().subscribe({
      next: (data) => {
        this.employees = data;
        this.loading = false;
      },
      error: (err) => {
        this.errorMessage = err.error?.error || 'Failed to load employees.';
        this.loading = false;
      }
    });
  }

  createEmployee(): void {
    if (!this.form.id.trim() || !this.form.name.trim() || !this.form.email.trim()) {
      this.errorMessage = 'Employee ID, Name and Email are all required.';
      return;
    }

    this.errorMessage = '';
    this.successMessage = '';
    this.submitting = true;

    const data = {
      id: this.form.id.trim(),
      name: this.form.name.trim(),
      email: this.form.email.trim()
    };

    this.skillService.createEmployee(data).subscribe({
      next: (emp) => {
        this.successMessage = `Employee ${emp.id} — ${emp.name} created successfully.`;
        this.form = { id: '', name: '', email: '' };
        this.loadEmployees();
        this.submitting = false;
      },
      error: (err) => {
        this.errorMessage = err.error || 'Failed to create employee. Email or ID might already be in use.';
        this.submitting = false;
      }
    });
  }

  startEdit(emp: Employee): void {
    this.editingEmployeeId = emp.id;
    this.editForm = {
      name: emp.name,
      email: emp.email
    };
  }

  cancelEdit(): void {
    this.editingEmployeeId = null;
    this.editForm = { name: '', email: '' };
  }

  updateEmployee(id: string): void {
    if (!this.editForm.name.trim() || !this.editForm.email.trim()) {
      this.errorMessage = 'Name and email are required.';
      return;
    }

    this.errorMessage = '';
    this.successMessage = '';
    this.submitting = true;

    this.skillService.updateEmployee(id, this.editForm).subscribe({
      next: () => {
        this.successMessage = `Employee ${id} updated successfully.`;
        this.editingEmployeeId = null;
        this.loadEmployees();
        this.submitting = false;
      },
      error: (err) => {
        this.errorMessage = err.error || 'Failed to update employee.';
        this.submitting = false;
      }
    });
  }

  deleteEmployee(id: string): void {
    if (!confirm(`Are you sure you want to delete employee ${id}? This will also remove all their skill records and project assignments.`)) {
      return;
    }

    this.errorMessage = '';
    this.successMessage = '';

    this.skillService.deleteEmployee(id).subscribe({
      next: () => {
        this.successMessage = `Employee ${id} deleted successfully.`;
        this.loadEmployees();
      },
      error: (err) => {
        this.errorMessage = err.error || 'Failed to delete employee.';
      }
    });
  }

  onFileChange(evt: any): void {
    const target: DataTransfer = <DataTransfer>(evt.target);
    if (target.files.length !== 1) return;

    const file = target.files[0];
    const reader: FileReader = new FileReader();
    reader.onload = (e: any) => {
      try {
        const bstr: string = e.target.result;
        const wb: XLSX.WorkBook = XLSX.read(bstr, { type: 'binary' });
        const wsname: string = wb.SheetNames[0];
        const ws: XLSX.WorkSheet = wb.Sheets[wsname];
        const data = XLSX.utils.sheet_to_json(ws, { header: 1 }) as any[];

        if (data.length <= 1) {
          this.errorMessage = 'The spreadsheet is empty or has no data rows.';
          return;
        }

        // Header mapping
        const headers = (data[0] as string[]).map(h => String(h || '').trim().toLowerCase());
        const idIdx = headers.findIndex(h => h.includes('id') || h.includes('code') || h.includes('key'));
        const nameIdx = headers.findIndex(h => h.includes('name') || h.includes('employee'));
        const emailIdx = headers.findIndex(h => h.includes('email') || h.includes('mail'));

        if (nameIdx === -1 || emailIdx === -1) {
          this.errorMessage = 'Could not find "Name" or "Email" columns in the sheet. Please make sure headers are present.';
          return;
        }

        const previewRows: any[] = [];
        for (let i = 1; i < data.length; i++) {
          const row = data[i];
          if (!row || row.length === 0) continue;

          const rawId = idIdx !== -1 && row[idIdx] !== undefined ? String(row[idIdx]).trim() : '';
          const name = nameIdx !== -1 && row[nameIdx] !== undefined ? String(row[nameIdx]).trim() : '';
          const email = emailIdx !== -1 && row[emailIdx] !== undefined ? String(row[emailIdx]).trim() : '';

          // Skip completely empty rows
          if (!name && !email) continue;

          // Simple email validation
          const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
          const isValidEmail = emailRegex.test(email);

          // Validation status
          let status = 'Valid';
          let warning = '';

          if (!rawId) {
            status = 'Invalid';
            warning = 'Employee ID is missing.';
          } else if (!name) {
            status = 'Invalid';
            warning = 'Name is missing.';
          } else if (!email) {
            status = 'Invalid';
            warning = 'Email is missing.';
          } else if (!isValidEmail) {
            status = 'Invalid';
            warning = 'Invalid email format.';
          } else {
            // Check if email already exists in preview list
            const dupInPreview = previewRows.some(r => r.email.toLowerCase() === email.toLowerCase());
            if (dupInPreview) {
              status = 'Invalid';
              warning = 'Duplicate email in spreadsheet.';
            } else {
              // Check if email exists in database
              const dupInDb = this.employees.some(emp => emp.email.toLowerCase() === email.toLowerCase());
              if (dupInDb) {
                status = 'Invalid';
                warning = 'Email already exists in database.';
              }
            }

            if (status === 'Valid') {
              // Check duplicate ID
              const idDupInPreview = previewRows.some(r => r.id && r.id.toUpperCase() === rawId.toUpperCase());
              const idDupInDb = this.employees.some(emp => emp.id.toUpperCase() === rawId.toUpperCase());
              if (idDupInPreview) {
                status = 'Invalid';
                warning = 'Duplicate ID in spreadsheet.';
              } else if (idDupInDb) {
                status = 'Invalid';
                warning = 'ID already exists in database.';
              }
            }
          }

          previewRows.push({
            id: rawId,
            name: name,
            email: email,
            status: status,
            warning: warning
          });
        }

        this.importPreview = previewRows;
        this.showImportPreview = true;
        this.errorMessage = '';
        // Clear file input value
        evt.target.value = '';
      } catch (err) {
        this.errorMessage = 'Failed to parse Excel file.';
        evt.target.value = '';
      }
    };
    reader.readAsBinaryString(file);
  }

  clearImport(): void {
    this.importPreview = [];
    this.showImportPreview = false;
  }

  get validImportCount(): number {
    return this.importPreview.filter(r => r.status === 'Valid').length;
  }

  submitImport(): void {
    const validRows = this.importPreview.filter(r => r.status === 'Valid');
    if (validRows.length === 0) {
      this.errorMessage = 'No valid rows to import.';
      return;
    }

    this.errorMessage = '';
    this.successMessage = '';
    this.submitting = true;

    const payload = validRows.map(r => ({
      id: r.id || undefined,
      name: r.name,
      email: r.email
    }));

    this.skillService.importEmployees(payload).subscribe({
      next: (res) => {
        this.successMessage = `Successfully imported ${res.count} employees.`;
        this.clearImport();
        this.loadEmployees();
        this.submitting = false;
      },
      error: (err) => {
        this.errorMessage = err.error || 'Failed to import employees.';
        this.submitting = false;
      }
    });
  }
}
