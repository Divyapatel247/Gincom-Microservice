import { Component } from '@angular/core';
import { ApiService } from '../../shared/api.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-register',
  imports: [FormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
   registerObj: any = {
       email: '',
       username:'',
       password: '',
       role:''
     }


     constructor(private api : ApiService, private router: Router) { }


     onRegister() {
       this.api.register(this.registerObj).subscribe({
         next: (res) => {
          console.log(res)
          //  alert(`Welcome, You have successfully registered`);
           this.router.navigateByUrl("admin");
         },
         error: (err) => {
           console.error("Login error:", err);
           alert(err.error || 'Login failed. Please try again.');
         }
       });
     }
}
