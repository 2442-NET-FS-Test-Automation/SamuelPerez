using Library.ControllerApi.DTOs;
using Library.Data;
using Library.Data.Entities;

namespace Library.ControllerApi.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryReposiory _repo;

    public InventoryService(IInventoryReposiory repo)
    {
        _repo = repo;
    }

    public Task<IReadOnlyList<InventoryItem>> AllAsync()
    {
        return _repo.GetAllAsync();
    }

    public Task<InventoryItem?> BySkuAsync(string sku)
    {
        return _repo.GetInventoryItemBySkuAsync(sku);
    }

    public Task<InventoryItem> AddAsync(InventoryCreateDto dto)
    {
        return _repo.AddInventoryItemAsync(dto.Sku, dto.Name, dto.Price, dto.CurrentStock);
    }

    public Task<bool> RemoveAsync(string sku)
    {
        return _repo.RemoveBySkuAsync(sku);
    }
}