import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { RegisterUser } from '../models/register-user';
import { Observable } from 'rxjs';
import { LoginUser } from '../models/login-user';

const API_BASE_URL: string = 'https://localhost:7094/api/v1/Accounts/';

@Injectable({
  providedIn: 'root',
})
export class AccountsService {
  public currentUsername: string | null = null;

  constructor(private httpClient: HttpClient) {}

  public postRegister(registerUser: RegisterUser): Observable<any> {
    return this.httpClient.post<RegisterUser>(
      `${API_BASE_URL}Register`,
      registerUser
    );
  }

  public postLogin(loginUser: LoginUser): Observable<any> {
    return this.httpClient.post<any>(`${API_BASE_URL}Login`, loginUser);
  }

  public getLogOut(): Observable<string> {
    return this.httpClient.get<string>(`${API_BASE_URL}LogOut`);
  }
}
