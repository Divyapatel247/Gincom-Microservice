import { AfterViewInit, Component, OnInit } from '@angular/core';
import { Basket, BasketResponse } from '../../../models/cart.interface';
import { ApiService } from '../../../shared/api.service';
import { CartItemComponent } from "../../../components/cart-item/cart-item/cart-item.component";
import { CommonModule, NgForOf, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../service/auth.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-add-to-cart',
  imports: [CartItemComponent,NgIf,NgForOf,CommonModule,FormsModule,RouterLink],
  templateUrl: './add-to-cart.component.html',
  styleUrl: './add-to-cart.component.css'
})
export class AddToCartComponent implements OnInit, AfterViewInit {
  basket: BasketResponse | null = null;
  totalQuantity: number = 0;
  totalAmount: number = 0;
  userId: string | null = null;
  private razorpayScriptLoaded: boolean = false;

  constructor(private apiService: ApiService, private authService: AuthService) {}

  ngOnInit() {
    this.userId = this.authService.getUserId();
    if (this.userId) {
      this.apiService.getCart(this.userId).subscribe({
        next: (data) => {
          console.log('Cart data:', data);
          this.basket = data;
          this.calculateTotals();
        },
        error: (err) => console.error('Error fetching cart:', err),
      });
    } else {
      console.error('No user logged in');
    }
  }

  ngAfterViewInit() {
    this.loadRazorpayScript().then(() => {
      this.razorpayScriptLoaded = true;
      console.log('Razorpay script loaded successfully');
    }).catch(err => {
      console.error('Error loading Razorpay script:', err);
    });
  }

  calculateTotals() {
    if (this.basket) {
      this.totalQuantity = this.basket.items.reduce((sum, item) => sum + item.quantity, 0);
      this.totalAmount = this.basket.totalAmount || 0;
      if (!this.totalAmount && this.basket.items.length > 0) {
        this.basket.items.forEach(item => {
          this.apiService.getProduct(item.productId).subscribe({
            next: (product) => {
              this.totalAmount += (product?.price || 0) * item.quantity;
            },
            error: (err) => console.error('Error fetching product price:', err),
          });
        });
      }
    }
  }

  updateQuantity(event: { id: number; quantity: number }) {
    if (this.basket && this.userId) {
      const item = this.basket.items.find(i => i.id === event.id);
      if (item) {
        item.quantity = event.quantity;
        this.apiService.updateCartItem(this.userId, event.id, item.quantity).subscribe({
          next: (data) => {
            this.basket = data;
            this.calculateTotals();
          },
          error: (err) => console.error('Error updating quantity:', err),
        });
      }
    }
  }

  removeItem(itemId: number) {
    if (this.basket && this.userId) {
      this.apiService.removeCartItem(this.userId, itemId).subscribe({
        next: (data) => {
          this.basket = data;
          this.calculateTotals();
        },
        error: (err) => console.error('Error removing item:', err),
      });
    }
  }

  deleteCart() {
    if (this.userId) {
      this.apiService.deleteCart(this.userId).subscribe({
        next: () => {
          this.basket = null;
          this.totalQuantity = 0;
          this.totalAmount = 0;
        },
        error: (err) => console.error('Error deleting cart:', err),
      });
    }
  }

  makeOrder() {
    if (this.userId && this.basket) {
      this.initiateRazorpayPayment();
    }
  }

  initiateRazorpayPayment() {
    if (this.userId && this.basket && !this.razorpayScriptLoaded) {
      console.error('Razorpay script not loaded yet. Please wait or refresh the page.');
      return;
    }

    if (this.userId && this.basket) {
      let calculatedTotal = 0;
      const productPromises = this.basket.items.map(item =>
        this.apiService.getProduct(item.productId).toPromise()
      );
      Promise.all(productPromises).then(products => {
        calculatedTotal = products.reduce((sum, product, index) => {
          return sum + (product?.price || 0) * this.basket!.items[index].quantity;
        }, 0);

        if (typeof (window as any).Razorpay === 'undefined') {
          console.error('Razorpay is not available. Script may have failed to load.');
          return;
        }

        const options = {
          key: 'rzp_test_xH3hHR8WECV8q3',
          amount: calculatedTotal * 100,
          currency: 'INR',
          name: 'Encom',
          description: 'Test Transaction',
          handler: (response: any) => {
            console.log('Payment successful:', response);
            this.createOrderAfterPayment(response.razorpay_payment_id);
            alert('Payment successful! Order ID: ' + response.razorpay_payment_id);
          },
          prefill: {
            name: 'Khushi Bansal',
            email: 'customer@example.com',
            contact: '9999999999',
          },
          notes: {
            address: 'Customer Address',
          },
          theme: {
            color: '#3399cc',
          },
        };

        const rzp = new (window as any).Razorpay(options);
        rzp.open();
        rzp.on('payment.failed', (response: any) => {
          console.error('Payment failed:', response.error);
          alert('Payment failed: ' + response.error.description);
        });
      }).catch(err => console.error('Error fetching products for payment:', err));
    }
  }

  createOrderAfterPayment(paymentId: string) {
    if (this.userId) {
      const userEmail : string = this.authService.getEmail() ?? " ";
      this.apiService.createOrder(this.userId,userEmail).subscribe({
        next: (response) => {
          console.log('Order created after payment:', response);
          this.basket = null; // Clear cart after successful order
          this.totalQuantity = 0;
          this.totalAmount = 0;
        },
        error: (err) => console.error('Error creating order:', err),
      });
    }
  }

  private loadRazorpayScript(): Promise<void> {
    return new Promise((resolve, reject) => {
      if ((window as any).Razorpay) {
        resolve();
        return;
      }

      const script = document.createElement('script');
      script.src = 'https://checkout.razorpay.com/v1/checkout.js';
      script.async = true;
      script.onload = () => resolve();
      script.onerror = () => reject(new Error('Failed to load Razorpay script'));
      document.body.appendChild(script);
    });
  }
}
