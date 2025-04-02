import { Component, inject, OnInit,AfterViewInit } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { initFlowbite } from 'flowbite';
import { IUser } from '../product/productModel';
import { NgIf } from '@angular/common';


@Component({
  selector: 'app-navbar',
  imports: [RouterLink,RouterOutlet,NgIf],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent implements OnInit,AfterViewInit {

  currentUser: IUser | null = null;

  ngOnInit(): void {
      const user = localStorage.getItem("angular19TokenData");
      if (user) {
        this.currentUser = JSON.parse(user);
      }
  }

  ngAfterViewInit(): void {
    initFlowbite();
  }

  router = inject(Router);


  onLogoff() {
    localStorage.removeItem("angular19Token");
    localStorage.removeItem("angular19User");
    localStorage.removeItem("angular19TokenData");
    this.router.navigateByUrl("login");
  }

}
