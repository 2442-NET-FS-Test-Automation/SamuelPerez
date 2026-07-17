export interface ApiError {
    status: number;
    message: string;
}

// export function isApiError(value: unknown): value is ApiError{
//     return typeof value === "object" && value !== null
//         && "satus" in value && "message" in value
// }

export function isApiError(value: unknown): value is ApiError{
    if(typeof value !== "object")
        return false;

    if (value === null)
        return false;

    if(!("status" in value))
        return false;

    if(!("message" in value))
        return false;

    return true;
}

export class ApiClient {
    constructor(private readonly baseUrl: string = "http://localhost:5223") {}

    async getJson<T>(path: string): Promise<T | ApiError> {
        try{
            const res = await fetch(`${this.baseUrl}${path}`);
            if (!res.ok) return { status: res.status, message: `API said: ${res.status}`};

            return await res.json() as T;
        } catch (err) {
            console.log(err instanceof Error ? err.message : "unknown error?");
            return { status: 0, message: "Cannot reach the API. Check if it's on, or CORS"};
        }
    }
}