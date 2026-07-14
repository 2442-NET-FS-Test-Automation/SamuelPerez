using AutoMapper;
using Library.ControllerApi.DTOs;
using Library.ControllerApi.Services;
using Library.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;


[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _service;
    private readonly IMapper _mapper;

    private readonly IMemoryCache _cache;

    private readonly ISupplierClient _supplier;

    public InventoryController(IInventoryService service, IMapper mapper, IMemoryCache cache, ISupplierClient supplier)
    {
        _service = service;
        _mapper = mapper;
        _cache = cache;
        _supplier = supplier;
    }

    [HttpGet]
    [ResponseCache(Duration = 30)]
    public async Task<ActionResult<IEnumerable<InventoryDto>>> Get()
    {

        var dtos = await _cache.GetOrCreateAsync("inventory:all", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);

            var items = await _service.AllAsync();

            return _mapper.Map<List<InventoryDto>>(items);
        });

        return Ok(dtos);
        // var items = await _service.AllAsync();

        // var mappedItems =  _mapper.Map<List<InventoryDto>>(items);

        // return Ok(mappedItems);

        // // return Ok(await _repo.GetAllAsync());
        // var items = await _repo.GetAllAsync();

        // EntireInventoryDTO response = new();

        // foreach (var item in items)
        // {
        //     InventoryReturnDTO i = new InventoryReturnDTO
        //     {
        //         Name = item.Product.Name,
        //         Sku = item.Product.Sku,
        //         CurrentStock = item.CurrentStock
        //     };

        //     response.EntireInventory.Add(i);
        // }

        // return Ok(response.EntireInventory);

    }

    [HttpGet("{sku}")]
    public async Task<ActionResult<InventoryDto>> GetBySku(string sku)
    {
        var item = await _service.BySkuAsync(sku);

        if (item is null)
        {
            return NotFound();
        }
        else
        {
            var mappedItem = _mapper.Map<InventoryDto>(item);
            return Ok(mappedItem);
        }
        // var item = await _repo.GetInventoryItemBySkuAsync(sku);

        // if (item is null)
        //     return NotFound();

        // var response = new InventoryReturnDTO
        // {
        //     Name = item.Product.Name,
        //     Sku = item.Product.Sku,
        //     CurrentStock = item.CurrentStock
        // };

        // return Ok(response);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<InventoryDto>> Create(InventoryCreateDto newInv)
    {
        var created = await _service.AddAsync(newInv);
        var response = _mapper.Map<InventoryDto>(created);

        _cache.Remove("inventory:all");

        return CreatedAtAction(nameof(GetBySku), new { sku = response.Sku}, response);
    }

    [HttpDelete("{sku}")]
    public async Task<ActionResult> Delete(string sku)
    {
        bool isDeleted = await _service.RemoveAsync(sku);

        if (isDeleted)
        {
            _cache.Remove("inventory:all");
            return NoContent();
        }
        else
        {
            return NotFound();
        }
    }

    [HttpGet("{sku}/supplier-price")]
    [Authorize]
    public async Task<ActionResult> GetSupplierPrice(string sku)
    {
        var price = await _supplier.GetListPriceAsync(sku);

        if (price is null)
        {
            return NotFound();
        }

        return Ok( new { sku, SupplierPrice = price});
    }
}