import { Component } from '@angular/core';
import { CustomerNavbarComponent } from "../../components/customer-navbar/customer-navbar.component";
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from "../../components/navbar/navbar.component";

@Component({
  selector: 'app-customer-layout',
  imports: [CustomerNavbarComponent, RouterOutlet, NavbarComponent],
  templateUrl: './customer-layout.component.html',
  styleUrl: './customer-layout.component.css'
})
export class CustomerLayoutComponent {

}
