import { Routes } from '@angular/router';
import { NavbarComponent } from './components/navbar/navbar.component';
import { ProductComponent } from './components/product/product.component';
import { ProductDetailComponent } from './components/product-detail/product-detail.component';
import { ProductTableComponent } from './components/product-table/product-table.component';
import { AddProductComponent } from './components/add-product/add-product.component';
import { AdminLoginComponent } from './components/admin-login/admin-login.component';
import { AdminLayoutComponent } from './layouts/admin-layout/admin-layout.component';
import { CustomerLayoutComponent } from './layouts/customer-layout/customer-layout.component';
import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/register/register.component';
import { AuthGuard } from './guard/auth.guard';
import { OrdersComponent } from './pages/orders/orders.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { ShowProductDetailComponent } from './pages/show-product-detail/show-product-detail.component';
import { AddToCartComponent } from './pages/add-to-cart/add-to-cart/add-to-cart.component';
import { MyOrdersComponent } from './pages/my-orders/my-orders.component';
import { AppComponent } from './app.component';
import { AdminOrderDetailComponent } from './pages/admin-order-detail/admin-order-detail.component';

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
    canActivate: [AuthGuard],
    data: { role: 'Admin' },
    children: [
      { path: '', component: DashboardComponent },
      { path: 'dashboard', component: DashboardComponent },
      { path: 'products', component: ProductTableComponent },
      { path: 'product/:id', component: ShowProductDetailComponent },
      { path: 'orders', component: OrdersComponent },
      {path: 'orders/:id', component : AdminOrderDetailComponent},
      {path: 'add-product', component: AddProductComponent}
      // { path: 'users', component: UserManagementComponent },
      // other admin routes
    ]
  },
  {
    path: 'customer',
    component: CustomerLayoutComponent,
    canActivate: [AuthGuard],
    data: { role: 'User' },
    children: [
      // { path: '', component: CustomerDashboardComponent },
      // { path: 'orders', component: CustomerOrdersComponent },
      { path: 'addToCart', component: AddToCartComponent },
      { path: 'myOrders', component: MyOrdersComponent },
      // other customer routes
      { path: '', redirectTo: 'product', pathMatch: 'full' }, // Default to products
      { path: 'product', component: ProductComponent },
      { path: 'product-detail/:productid', component: ProductDetailComponent },
      {
        path: 'product/:category',
        component: ProductComponent
      }
    ]
  },
  { path: 'login', component: LoginComponent  },
  { path: 'register', component: RegisterComponent  },
  { path: '**', redirectTo: '/login' },
  { path: '', component: AppComponent }
];

