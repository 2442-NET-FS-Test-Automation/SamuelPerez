import { useEffect, useState } from "react";
import { BookCard } from "./BookCard";
// import { catalog } from "../data/catalog";
import { SortDirection, type FetchState, type InventoryItem } from "../types";
import { getInventory } from "../api/inventory";
import { SearchBar } from "./SearchBar";

export function CatalogPage() {
    // const [items] = useState(catalog);
    const [compact, setCompact] = useState(false);
    const [items, setItems] = useState<InventoryItem[]>([]);
    const [fState, setFState] = useState<FetchState>("idle");
    const [userQuery, setUserQuery] = useState("");
    const [dir, setDir] = useState<SortDirection>(SortDirection.Ascending);
    useEffect(() => {
        let active = true;
        setFState("loading");
        getInventory()
            .then((data) => {
                if(!active) return;
                setItems(data);
                setFState("loaded");
            })
            .catch(() => {
                if (active) setFState("failed");
            });
        return () => {
            active = false;
        };

    }, []);

    const visibleBooks = [...items]
        .filter((i) => i.name.toLowerCase().includes(userQuery.toLowerCase()))
        .sort((a, b) => 
            dir === SortDirection.Ascending
                ? a.name.localeCompare(b.name)
                : b.name.localeCompare(a.name)
        );

    if (fState === "idle" || fState === "loading") return <p>Loading catalog...</p>

    if (fState === "failed")
        return <p>Could not reach the API. Is it running on :5223? Check CORS.</p>

    return (
        <section>
            <div className="toolbar">
                <h2>Catalog</h2>
                <SearchBar value={userQuery} onChange={setUserQuery}/>
                <button 
                    type="button"
                    onClick={() => 
                        setDir((d) => 
                            d === SortDirection.Ascending
                                ? SortDirection.Descending
                                : SortDirection.Ascending
                        )
                    }
                >
                    Sort {dir === SortDirection.Ascending ? "Z-A" : "A-Z"}
                </button>
            </div>

            {visibleBooks.length === 0 ? (
                <p>No books match "{userQuery}"</p>
            ) : (
                <div className="cards">
                    {visibleBooks.map((item) => (
                        <BookCard key={item.sku} item={item} />
                    ))}

                </div>
            )}
        </section>
    )
}