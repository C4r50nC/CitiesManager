import { Component } from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { AccountsService } from '../services/accounts.service';
import { Router } from '@angular/router';
import { RegisterUser } from '../models/register-user';
import { CompareValidation } from '../validators/custom-validator';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  registerForm: FormGroup;
  isRegisterFormSubmitted: boolean = false;

  constructor(
    private accountsService: AccountsService,
    private router: Router
  ) {
    this.registerForm = new FormGroup(
      {
        personName: new FormControl(null, [Validators.required]),
        email: new FormControl(null, [Validators.required, Validators.email]),
        phoneNumber: new FormControl(null, [Validators.required]),
        password: new FormControl(null, [Validators.required]),
        confirmPassword: new FormControl(null, [Validators.required]),
      },
      { validators: [CompareValidation('password', 'confirmPassword')] }
    );
  }

  get register_personNameControl(): any {
    return this.registerForm.controls['personName'];
  }
  get register_emailControl(): any {
    return this.registerForm.controls['email'];
  }
  get register_phoneNumberControl(): any {
    return this.registerForm.controls['phoneNumber'];
  }
  get register_passwordControl(): any {
    return this.registerForm.controls['password'];
  }
  get register_confirmPasswordControl(): any {
    return this.registerForm.controls['confirmPassword'];
  }

  registerSubmitted(): void {
    this.isRegisterFormSubmitted = true;

    if (!this.registerForm.valid) {
      return;
    }

    this.accountsService.postRegister(this.registerForm.value).subscribe({
      next: (response: any) => {
        console.log(response);

        this.isRegisterFormSubmitted = false;

        localStorage['token'] = response.token;

        this.router.navigate(['/cities']);
        this.registerForm.reset();
      },
      error: (error: any) => console.error(error),
      complete: () => {},
    });
  }
}
