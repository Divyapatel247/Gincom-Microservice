import { NgFor } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-add-cross-products',
  imports: [NgFor],
  templateUrl: './add-cross-products.component.html',
  styleUrl: './add-cross-products.component.css'
})
export class AddCrossProductsComponent {
  crossProducts = [1, 2, 3];
}
