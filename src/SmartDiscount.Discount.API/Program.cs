using SmartDiscount.Discount.API.Apis;
using SmartDiscount.Discount.API.Data;
using SmartDiscount.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<DiscountContext>("discountdb");

builder.Services.AddMigration<DiscountContext, DiscountSeed>();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapDiscountApi();

app.Run();