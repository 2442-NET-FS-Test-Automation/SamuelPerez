export interface InventoryItem {
    sku: Sku;
    name: string;
    currentStock: number;
}

export interface SupplierPrice {
    sku: Sku;
    supplierPrice: number;

}

export type Sku = string;

export type FetchState = "idle" | "loading" | "loaded" | "failed";

export const HttpStatus = {
    Ok : 200,
    Created : 201,
    NoContent : 204,
    BadRequest : 400,
    Unauthorized : 401,
    Forbidden : 403,
    NotFound : 404
} as const;
export type HttpStatus = typeof HttpStatus[keyof typeof HttpStatus];

export const SortDirection =  {
    Ascending : "asc",
    Descending : "desc"
} as const;
export type SortDirection = typeof SortDirection[keyof typeof SortDirection];

export type InventoryPatch = Partial<InventoryItem>;
export type NewInventoryItem = Omit<InventoryItem, "sku">;