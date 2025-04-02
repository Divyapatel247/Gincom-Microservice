import { Component, inject } from '@angular/core';
import { ApiService } from '../../shared/api.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-admin-login',
  imports: [FormsModule, NgIf],
  templateUrl: './admin-login.component.html',
  styleUrl: './admin-login.component.css'
})
export class AdminLoginComponent {

  loginObj: any = {
    email: '',
    password: ''
  }


  constructor(private api : ApiService, private router: Router) { }


  onLogin() {
    this.api.login(this.loginObj).subscribe({
      next: (res) => {
        localStorage.setItem("angular19User", res.username);
        localStorage.setItem("angular19Token", res.token);
        localStorage.setItem("angular19TokenData", JSON.stringify(res));
        alert(`Welcome, ${res.username}! You have successfully logged in.`);
        this.router.navigateByUrl("product");
      },
      error: (err) => {
        console.error("Login error:", err);
        alert(err.error || 'Login failed. Please try again.');
      }
    });
  }



}
