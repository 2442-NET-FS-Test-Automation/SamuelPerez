import { useEffect, useState } from "react";
import { data, Link, useParams } from "react-router-dom";
import { getInventoryItem } from "../api/inventory";
import type { InventoryItem, FetchState } from "../types";

export function BookDetail() {
    const { sku } = useParams<{ sku: string}>();
    const [item, setItem] = useState<InventoryItem | null>(null);
    const [fState, setFState] = useState<FetchState>("idle");

    useEffect(() => {
        if (!sku) return;
        let active = true;
        setFState("loading");

        getInventoryItem(sku)
            .then((data) => {
                if (!active) return;
                setItem(data);
                setFState("loaded");
            })
            .catch(() => {
                if (active) setFState("failed");
            });
        
            return () => {
                active = false;
            };
    }, [sku]);

    if (fState === "idle" || fState === "loading") return <p>Loading...</p>
    if (fState === "failed" || !item)
        return (
            <p>
                Book {sku} not found. <Link to="/">Back to catalog</Link>
            </p>
        );
    
        return (
            <article>
                <p>
                    <Link to="/">&larr; Back to catalog</Link>
                </p>
                <h2>{item.name}</h2>
                <p>SKU: {item.name}</p>
                <p>In stock: {item.currentStock}</p>
            </article>
        )
}