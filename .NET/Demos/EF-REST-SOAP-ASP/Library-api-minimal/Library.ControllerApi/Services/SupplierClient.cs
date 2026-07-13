namespace Library.ControllerApi.Services;

public class SupplierClient : ISupplierClient
{
    private readonly HttpClient _http;

    public SupplierClient(HttpClient http)
    {
        _http = http;
    }

    private record SupplierProduct(int id, string Title, decimal Price);

    public async Task<decimal?> GetListPriceAsync(string sku)
    {
        var digits = new string(sku.Where(char.IsDigit).ToArray());

        if(!int.TryParse(digits, out var id)) return null;

        var product = await _http.GetFromJsonAsync<SupplierProduct>($"products/{id}");

        return product?.Price;
    }
}