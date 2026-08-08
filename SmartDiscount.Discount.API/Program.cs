using SmartDiscount.Discount.API.Apis;
using SmartDiscount.Discount.API.Data;
using SmartDiscount.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Base de données PostgreSQL "discountdb"
builder.AddNpgsqlDbContext<DiscountContext>("discountdb");

// Migration + seed
builder.Services.AddMigration<DiscountContext, DiscountSeed>();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapDiscountApi();

app.Run();