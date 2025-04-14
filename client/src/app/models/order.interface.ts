export interface OrderItem {
  productId: number;
  quantity: number;
}

export interface Order {
  id: number;
  userId: string;
  status: string;
  items: OrderItem[];
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
