var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddApplicationServices();
builder.Services.AddProblemDetails();

var withApiVersioning = builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
});

builder.AddDefaultOpenApi(withApiVersioning);

var app = builder.Build();

app.MapDefaultEndpoints();

var orders = app.NewVersionedApi("Orders");

orders.MapOrdersApiV1()
      .RequireAuthorization();

// Endpoint INTERNE pour le Notification (non protégé, communication inter-services)
// obligé car Notification écoute un évenement et
// ce dernier nécessite un token donc c'est obliger de le mettre sans token
app.MapGet("/api/internal/orders/{orderId:int}",
    async (int orderId, IOrderQueries queries) =>
    {
        try
        {
            var order = await queries.GetOrderAsync(orderId);
            return Results.Ok(order);
        }
        catch
        {
            return Results.NotFound();
        }
    })
    .AllowAnonymous();

app.UseDefaultOpenApi();
app.Run();
