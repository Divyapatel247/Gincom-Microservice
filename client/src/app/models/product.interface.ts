export interface Product {
  id: number;
  title: string;
  description: string;
  price: number;
  discountPercentage?: number;
  rating?: number;
  stock: number;
  tags: string[];
  brand?: string;
  sku?: string;
  weight?: number;
  dimensions?: {
    width: number;
    height: number;
    depth: number;
  };
  warrantyInformation?: string;
  shippingInformation?: string;
  availabilityStatus?: string;
  reviews?: any[];
  returnPolicy?: string;
  minimumOrderQuantity?: number;
  images?: string[];
  thumbnail?: string;
  categoryId?: number;
  categoryName?: string;
  relatedProducts?: any[];
}
