import {Sku, FetchState} from "./types.js";

let sku: string = "BK-101";
let price: number = 29.99;
let inStosck: boolean = true;

let name = "jon";

let tags: string[] = ["architecture", "classic"];

console.log(sku, price, inStosck, tags, name);

let anything: any = "escape hatch";
anything = 42;
console.log(anything);

let incoming: unknown = JSON.parse('"?"');
if(typeof incoming === "string"){
    console.log(incoming.toUpperCase());
}

// incoming.toUpperCase();

type PriceChange = [min: number, max: number];

let state : FetchState = "loading";

const range: PriceChange = [0, 50];
const bkSku : Sku = "BK-101";

console.log(state, range[0], range[1], bkSku);