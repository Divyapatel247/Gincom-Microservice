
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
  IsNotifyDisabled?: boolean;
  soldCount : number;
  relatedProductIds : Number[]
}

export interface IProductWithRelatedProducts extends IProduct {
  relatedProducts : [{id:number,title:string,price:number,thumbnail:string,stock: number, categoryName?: string | null,rating?: number;}]
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
