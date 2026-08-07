using SmartDiscount.Notification.API.Extensions;
using SmartDiscount.Notification.API.Services;
using SmartDiscount.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddApplicationServices();
builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();