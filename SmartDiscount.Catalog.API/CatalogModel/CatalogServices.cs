using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartDiscount.Catalog.API.CatalogIntegrationEvents;
using SmartDiscount.Catalog.API.Infrastructure;
using SmartDiscount.Catalog.API.SmartDiscountServices;

namespace SmartDiscount.Catalog.API.CatalogModel;

public class CatalogServices(
    CatalogContext context,
    [FromServices] ICatalogAI catalogAI,
    IOptions<CatalogOptions> options,
    ILogger<CatalogServices> logger,
    [FromServices] ICatalogIntegrationEventService eventService)
{
    public CatalogContext Context { get; } = context;
    public ICatalogAI CatalogAI { get; } = catalogAI;
    public IOptions<CatalogOptions> Options { get; } = options;
    public ILogger<CatalogServices> Logger { get; } = logger;
    public ICatalogIntegrationEventService EventService { get; } = eventService;
};