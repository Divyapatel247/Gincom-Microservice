import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-customer-navbar',
  imports: [],
  templateUrl: './customer-navbar.component.html',
  styleUrl: './customer-navbar.component.css'
})
export class CustomerNavbarComponent {
  constructor(private router: Router) {}

  logout() {
    localStorage.clear();
    this.router.navigate(['/login']);
  }
}
