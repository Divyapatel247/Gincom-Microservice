import { OAuthService } from 'angular-oauth2-oidc';
import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import 'flowbite';
import { HttpClient } from '@angular/common/http';


@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'Encom';


}
