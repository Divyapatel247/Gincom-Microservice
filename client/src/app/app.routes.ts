import { Routes } from '@angular/router';
import { NavbarComponent } from './components/navbar/navbar.component';
import { ProductComponent } from './components/product/product.component';
import { ProductDetailComponent } from './components/product-detail/product-detail.component';
import { ProductTableComponent } from './components/product-table/product-table.component';
import { AddProductComponent } from './components/add-product/add-product.component';
import { AdminLoginComponent } from './components/admin-login/admin-login.component';
import { authGuard } from './guard/auth.guard';
import { AdminLayoutComponent } from './layouts/admin-layout/admin-layout.component';
import { CustomerLayoutComponent } from './layouts/customer-layout/customer-layout.component';
import { LoginComponent } from './components/login/login.component';

// export const routes: Routes = [
//   {
//     path: '',
//     redirectTo: 'login',
//     pathMatch: 'full'
//   },
//   {
//     path: 'login',
//     component: AdminLoginComponent
//   },
//   {
//     path: '',
//     component: NavbarComponent,
//     canActivate: [authGuard],
//     children: [
//       {
//         path: 'product',
//         component: ProductComponent
//       },
//       {
//         path: 'product-detail/:productid',
//         component: ProductDetailComponent
//       },
//       {
//         path: 'product/:category',
//         component: ProductComponent
//       },
//       {
//         path: 'product-table',
//         component: ProductTableComponent
//       },
//       {
//         path: 'add-product',
//         component: AddProductComponent
//       }
//     ]
//   }
// ]

export const routes: Routes = [
  {
    path: 'admin',
    component: AdminLayoutComponent,
    // canActivate: [AuthGuard],
    data: { role: 'admin' },
    children: [
      { path: '', component: ProductTableComponent },
      // { path: 'users', component: UserManagementComponent },
      // other admin routes
    ]
  },
  {
    path: 'customer',
    component: CustomerLayoutComponent,
    // canActivate: [AuthGuard],
    data: { role: 'customer' },
    children: [
      // { path: '', component: CustomerDashboardComponent },
      // { path: 'orders', component: CustomerOrdersComponent },
      // other customer routes
    ]
  },
  { path: 'login', component: LoginComponent },
  { path: '**', redirectTo: '/login' }
];

