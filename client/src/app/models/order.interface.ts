import { IProduct } from "../components/product/productModel";

export interface OrderItem {
  id?:number;
  productId: number;
  quantity: number;
  product? : IProduct;
  createdAt? : Date;
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
