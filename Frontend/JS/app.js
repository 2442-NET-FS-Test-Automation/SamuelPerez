// let catalogItems = [
//     { sku: "BK-101", name: "Clean Code",                price: 29.99, currentStock: 12 },
//     { sku: "BK-102", name: "The Pragmatic Programmer",  price: 34.99, currentStock: 7 },
//     { sku: "BK-103", name: "Design Patterns",           price: 44.99, currentStock: 3 },
//     { sku: "BK-104", name: "Refactoring",               price: 39.99, currentStock: 0 },
// ];

let catalogItems = [];

async function LoadCatalog() {
    const container = document.querySelector("#catalog-cards");
    const loading = document.createElement("p");
    loading.className = "hint";
    loading.textContent = "loading...";

    container.innerHTML = "";
    container.appendChild(loading);

    try{
        const response = await fetch(`${API}/api/Inventory`);
        if (!response.ok){
            container.innerHTML = `<p class="hint">API said ${response.status}</p>`;
            return;
        }

        catalogItems = await response.json();
        renderCards(catalogItems);
    } catch (err) {
        console.error(err);
        container.innerHTML = `<p class="hint">cannot reach the API. Is it on?</p>`;
    }
}

function renderCards(items){
    const container = document.querySelector("#catalog-cards");

    if (items.length === 0){
        container.innerHTML = `<p class="hint">nothing matches</p>`;
        return;
    }

    container.innerHTML = items.map(item => `
                <article class="card" data-sku="${item.sku}">
                    <h3>${item.name}</h3>
                    <dl>
                        <dt>SKU</dt><dd>${item.sku}</dd>

                        <dt>In Stock</dt><dd>${item.currentStock}</dd>
                    </dl>
                    <button class="price-btn" data-sku="${item.sku}">Supplier price</button>
                    <p class="supplier-price"></p>
                </article>
        `
    ).join("");
}

document.querySelector("#catalog-cards").addEventListener("click", (e) => {
    if (e.target.matches(".price-btn")) {
        console.log("clicked", e.target.dataset.sku);
    }
});

document.querySelector("#search").addEventListener("input", (e) => {
    const search = e.target.value.trim().toLowerCase();
    renderCards(catalogItems.filter(item => 
        item.name.toLowerCase().includes(search) || item.sku.toLocaleLowerCase().includes(search)
    ));
});

document.addEventListener("DOMContentLoaded", () => {
    LoadCatalog();
    // fetch(`${API}/api/Inventory`)
    // .then(res => res.json())
    // .then(items => { catalogItems = items; items.map(item => item.price = 5)
    // renderCards(catalogItems); });
});

// const API = "http://localhost:5137";
// fetch(`${API}/api/inventory`)
//     .then(res => res.json())
//     .then(items => { catalogItems = items; renderCards(items)});