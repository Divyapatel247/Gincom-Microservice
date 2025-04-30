import { Component } from '@angular/core';
import { ApiService } from '../../shared/api.service';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { LoaderComponent } from "../../components/loader/loader.component";

@Component({
  selector: 'app-register',
  imports: [FormsModule, RouterLink, LoaderComponent],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  loading = false;
   registerObj: any = {
       email: '',
       username:'',
       password: '',
       role:''
     }


     constructor(private api : ApiService, private router: Router) { }


     onRegister() {
      this.loading = true
       this.api.register(this.registerObj).subscribe({
         next: (res) => {
           alert(`Welcome, You have successfully registered`);
           this.loading = false;
           this.router.navigateByUrl("admin");
         },
         error: (err) => {
           console.error("Register error:", err);
           this.loading = false;
           alert('Register failed. Please try again.');
         }
       });
     }
}
