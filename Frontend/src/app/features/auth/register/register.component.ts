import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../data/services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  imports:[ReactiveFormsModule, CommonModule],
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  registerForm: FormGroup;
  errorMessage = '';
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.registerForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });
  }

  private passwordMatchValidator(g: AbstractControl) {
    const password = g.get('password')?.value;
    const confirmPassword = g.get('confirmPassword')?.value;
    
    if (password !== confirmPassword) {
      g.get('confirmPassword')?.setErrors({ mismatch: true });
      return { mismatch: true };
    }
    return null;
  }

  onSubmit(): void {
    if (this.registerForm.valid) {
      this.isLoading = true;
      this.errorMessage = '';
      
      const { confirmPassword, ...userData } = this.registerForm.value;
      
      this.authService.register(userData).subscribe({
        next: () => {
          this.isLoading = false;
          this.router.navigate(['/login']);
        },
        error: (error) => {
          this.isLoading = false;
          this.errorMessage = error.error?.error || 'Registration failed. Please try again.';
        }
      });
    }
  }
getFormProgress(): number {
  const totalFields = 4;
  const completedFields = Object.keys(this.registerForm.controls).filter(
    key => this.registerForm.get(key)?.valid
  ).length;
  
  return Math.round((completedFields / totalFields) * 100);
}

togglePasswordVisibility(fieldName: string) {
  const field = document.getElementById(fieldName) as HTMLInputElement;
  if (field) {
    field.type = field.type === 'password' ? 'text' : 'password';
  }
}
}