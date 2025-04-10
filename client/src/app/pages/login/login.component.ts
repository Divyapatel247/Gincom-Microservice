import { NgIf } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../shared/api.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
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
         this.router.navigateByUrl("admin");
       },
       error: (err) => {
         console.error("Login error:", err);
         alert(err.error || 'Login failed. Please try again.');
       }
     });
   }
}
