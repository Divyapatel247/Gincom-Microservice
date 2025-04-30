import { NgIf } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../shared/api.service';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../service/auth.service';
import { LoaderComponent } from "../../components/loader/loader.component";

@Component({
  selector: 'app-login',
  imports: [FormsModule, RouterLink, LoaderComponent],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  loading = false;
  loginObj: any = {
    client_id: 'client',
    client_secret: 'secret',
    grant_type: 'password',
    username: '',
    password: '',
  };

  constructor(private api: ApiService, private router: Router,private authService: AuthService) {}

  onLogin() {
    this.loading = true;
    const body = this.toUrlEncoded(this.loginObj);
    this.api.login(body).subscribe({
      next: (res) => {
        localStorage.setItem('access_token', res.access_token);
        localStorage.setItem('refresh_token', res.refresh_token);
        localStorage.setItem('scope', res.scope);
        const role = this.authService.getRole();
        console.log('Logged in user role:', role);
        this.loading = false;
        alert(`Welcome,You have successfully logged in.`);

        if (role === 'Admin') {
          this.router.navigateByUrl('/admin');
        } else if (role === 'User') {
          this.router.navigateByUrl('/customer');
        } else {
          this.router.navigateByUrl('/unauthorized');
        }
      },
      error: (err) => {
        console.error('Login error:', err);
        this.loading = false;
        alert('Login failed. Please try again.');
      },
    });
  }

  private toUrlEncoded(obj: any): string {
    return Object.keys(obj)
      .map(k => encodeURIComponent(k) + '=' + encodeURIComponent(obj[k]))
      .join('&');
  }

}
