
export interface IProduct {
  id: number;
  title: string;
  description: string;
  price: number;
  discountPercentage: number;
  rating?: number;
  stock: number;
  tags: string[];
  brand?: string;
  sku?: string;
  weight?: number;
  dimensions?: { width?: number; height?: number; depth?: number };
  warrantyInformation?: string;
  shippingInformation?: string;
  availabilityStatus?: string;
  reviews?: IReview[];
  returnPolicy?: string;
  minimumOrderQuantity?: number;
  images?: string[];
  thumbnail?: string;
  categoryId: number;
  categoryName?: string | null;
<<<<<<< HEAD
  IsNotifyDisabled?: boolean;
  
=======
  relatedProductIds : Number[]
>>>>>>> 31137c9a6ec154a4a96d465194914a83b49c0b4a
}

export interface IProductWithRelatedProducts extends IProduct {
  relatedProducts : [{title:string,price:number}]
}

export interface IReview {
  id:number
  productId:number;
  userId:number;
  rating: number;
  description: string;
createdAt: string;
}

export interface IUser {
  username: string;
  email: string;
  profilePictureUrl: string;
}
