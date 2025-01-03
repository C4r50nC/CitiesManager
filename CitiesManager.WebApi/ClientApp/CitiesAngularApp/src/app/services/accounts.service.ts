import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { RegisterUser } from '../models/register-user';
import { Observable } from 'rxjs';

const API_BASE_URL: string = 'https://localhost:7094/api/v1/Accounts/';

@Injectable({
  providedIn: 'root',
})
export class AccountsService {
  constructor(private httpClient: HttpClient) {}

  public postRegister(registerUser: RegisterUser): Observable<RegisterUser> {
    return this.httpClient.post<RegisterUser>(
      `${API_BASE_URL}/Register`,
      registerUser
    );
  }
}
