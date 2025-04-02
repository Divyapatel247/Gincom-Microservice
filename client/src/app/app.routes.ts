import { Routes } from '@angular/router';
import { NavbarComponent } from './components/navbar/navbar.component';
import { ProductComponent } from './components/product/product.component';
import { ProductDetailComponent } from './components/product-detail/product-detail.component';
import { ProductTableComponent } from './components/product-table/product-table.component';
import { AddProductComponent } from './components/add-product/add-product.component';
import { AdminLoginComponent } from './components/admin-login/admin-login.component';
import { authGuard } from './guard/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },
  {
    path: 'login',
    component: AdminLoginComponent
  },
  {
    path: '',
    component: NavbarComponent,
    canActivate: [authGuard],
    children: [
      {
        path: 'product',
        component: ProductComponent
      },
      {
        path: 'product-detail/:productid',
        component: ProductDetailComponent
      },
      {
        path: 'product/:category',
        component: ProductComponent
      },
      {
        path: 'product-table',
        component: ProductTableComponent
      },
      {
        path: 'add-product',
        component: AddProductComponent
      }
    ]
  }
]
