import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-admin-sidebar',
  imports: [RouterLink],
  templateUrl: './admin-sidebar.component.html',
  styleUrl: './admin-sidebar.component.css'
})
export class AdminSidebarComponent {
 constructor( private router: Router){}
  onLogout()
  {
   localStorage.clear()
    this.router.navigateByUrl('login')
  }
}
