export interface BasketItem {
  id: number;
  productId: number;
  quantity: number;
  createdAt?: Date;
}

export interface Basket {
  id: number;
  userId: string;
  items: BasketItem[];
  createdAt?: Date;
  totalAmount?: number;
}

export interface BasketResponse {
  id: number;
  userId: string;
  items: BasketItem[];
  createdAt?: Date;
  totalAmount?: number;
}
