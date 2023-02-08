using csharp_webapi.Config;
using csharp_webapi.Helpers;
using csharp_webapi.Middlewares;
using Microsoft.EntityFrameworkCore;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy
                    // .SetIsOriginAllowed(origin => true)
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
        builder.Services.AddControllers(c => c.Filters.Add<AuthorizeAttribute>());

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Add custom configurations.
        app.UseCors();

        app.UseVerifyAccessToken();
        // app.UseVerifyRefreshToken();

        // app.UseHttpsRedirection();
        // app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}