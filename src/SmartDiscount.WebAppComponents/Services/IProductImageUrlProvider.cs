using SmartDiscount.WebAppComponents.Catalog;

namespace SmartDiscount.WebAppComponents.Services;

public interface IProductImageUrlProvider
{
    string GetProductImageUrl(CatalogItem item)
        => GetProductImageUrl(item.Id);

    string GetProductImageUrl(int productId);
}