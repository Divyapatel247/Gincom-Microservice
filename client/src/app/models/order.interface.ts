import { Product } from "./product.interface";

export interface OrderItem {
  productId: number;
  quantity: number;
  product? : Product
}

export interface Order {
  id: number;
  userId: string;
  status: string;
  items: OrderItem[];
  razorpayOrderId?: string;
  createdAt?: Date;
}

export interface OrderResponse {
  id: number;
  userId: string;
  status: string;
  items: OrderItem[];
  createdAt?: Date;
  razorpayOrderId?: string;
}
