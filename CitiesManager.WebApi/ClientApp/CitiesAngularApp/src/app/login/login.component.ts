import { Component } from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { AccountsService } from '../services/accounts.service';
import { LoginUser } from '../models/login-user';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  loginForm: FormGroup;
  isLoginFormSubmitted: boolean = false;

  constructor(
    private accountsService: AccountsService,
    private router: Router
  ) {
    this.loginForm = new FormGroup({
      email: new FormControl(null, [Validators.required, Validators.email]),
      password: new FormControl(null, [Validators.required]),
    });
  }

  get login_emailControl(): any {
    return this.loginForm.controls['email'];
  }
  get login_passwordControl(): any {
    return this.loginForm.controls['password'];
  }

  loginSubmitted(): void {
    this.isLoginFormSubmitted = true;

    if (!this.loginForm.valid) {
      return;
    }

    this.accountsService.postLogin(this.loginForm.value).subscribe({
      next: (response: any) => {
        console.log(response);

        this.isLoginFormSubmitted = false;
        this.accountsService.currentUsername = response.email;

        localStorage['token'] = response.token;

        this.router.navigate(['/cities']);
        this.loginForm.reset();
      },
      error: (error: any) => console.error(error),
      complete: () => {},
    });
  }
}
