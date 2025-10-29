import { Component, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../data/services/auth.service';
import { CommonModule } from '@angular/common';
import { Subject, take } from 'rxjs';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  imports:[CommonModule, ReactiveFormsModule],
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnDestroy {
  private destroy$ = new Subject<void>();
  loginForm: FormGroup;
  errorMessage = '';
  isLoading = false;

  constructor(
    public fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    authService.checkAuth().pipe(take(1)).subscribe({
      next:()=>{ 
        if(authService.isLoggedIn())
        {
           this.setName();
           this.router.navigate(['/events']);}}
    });

    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }
  onSubmit(): void {
    if (this.loginForm.valid) {
      this.isLoading = true;
      this.errorMessage = '';
      
      this.authService.login(this.loginForm.value).subscribe({
        next: () => {
       
          this.isLoading = false;
          this.setName();
          this.router.navigate(['/events']);
        },
        error: (error) => {
          this.isLoading = false;
          this.errorMessage = error.error?.error || 'Login failed';
        }
      });
    }
  }
 ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

 setName() {
  this.authService.getName().subscribe({
    next: (name) => {
      localStorage.removeItem('username');
      localStorage.setItem('username', name.name);
    },
    error: (err) => {
      console.error('Error getting name:', err);
    }
  });
}

}