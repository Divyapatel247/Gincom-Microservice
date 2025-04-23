import { IProduct } from "../components/product/productModel";

export interface OrderItem {
  productId: number;
  quantity: number;
  product? : IProduct
}

export interface Order {
  id: number;
  userId: string;
  status: string;
  items: OrderItem[];
  totalAmount?: number;
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
