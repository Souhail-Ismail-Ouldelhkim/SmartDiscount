using SmartDiscount.Wishlist.API.Apis;
using SmartDiscount.Wishlist.API.Infrastructure;
using SmartDiscount.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<WishlistContext>("wishlistdb");

builder.Services.AddMigration<WishlistContext, WishlistSeed>();

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapWishlistApi();
app.Run();