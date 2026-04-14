using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Todo.Api;
using Todo.Api.Accounts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddDbContextPool<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AppDbConnection")));
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<Account>()
    .AddEntityFrameworkStores<AppDbContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();
app.UseStatusCodePages();
app.MapIdentityApi<Account>();
app.MapControllers();

app.Run();
