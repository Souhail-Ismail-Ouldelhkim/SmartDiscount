using SmartDiscount.WebAppComponents.Catalog;

namespace SmartDiscount.WebAppComponents.Item;
public static class ItemHelper
{
    public static string Url(CatalogItem item)
        => $"item/{item.Id}";
}