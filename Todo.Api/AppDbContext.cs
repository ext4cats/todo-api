using Microsoft.EntityFrameworkCore;

namespace Todo.Api;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options);
