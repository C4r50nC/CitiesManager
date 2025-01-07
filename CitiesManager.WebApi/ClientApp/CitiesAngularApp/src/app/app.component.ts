import { Component } from '@angular/core';
import { Router, RouterModule, RouterOutlet } from '@angular/router';
import { AccountsService } from './services/accounts.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
  imports: [RouterModule, RouterOutlet],
})
export class AppComponent {
  constructor(
    public accountsService: AccountsService,
    private router: Router
  ) {}

  onLogOutClicked() {
    this.accountsService.getLogOut().subscribe({
      next: (response: string) => {
        console.log(response);

        this.accountsService.currentUsername = null;
        localStorage.removeItem('token');
        this.router.navigate(['/login']);
      },
      error: (error: any) => console.error(error),
      complete: () => {},
    });
  }
}
