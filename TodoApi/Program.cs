using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContextPool<ApplicationDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("ApplicationDb")));
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<Account>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddDataProtection()
    .PersistKeysToDbContext<ApplicationDbContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapIdentityApi<Account>();

app.MapControllers();

app.Run();
