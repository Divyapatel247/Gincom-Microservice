import { Router } from '@angular/router';
import { Component } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';

@Component({
  selector: 'app-admin-navbar',
  imports: [NgIf,NgFor],
  templateUrl: './admin-navbar.component.html',
  styleUrl: './admin-navbar.component.css'
})
export class AdminNavbarComponent {
  showDropdown = false;

  notifications = [
    { message: 'New order received!', timestamp: '2 minutes ago' },
    { message: 'New order received!', timestamp: '2 minutes ago' },
    { message: 'New order received!', timestamp: '2 minutes ago' },
    { message: 'New order received!', timestamp: '2 minutes ago' },
    { message: 'New order received!', timestamp: '2 minutes ago' },
    { message: 'New order received!', timestamp: '2 minutes ago' },
  ];

  toggleNotifications() {
    this.showDropdown = !this.showDropdown;
  }
}
