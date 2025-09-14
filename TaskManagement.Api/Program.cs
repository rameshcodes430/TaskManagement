using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TaskManagement.Api.Data;
using TaskManagement.Api.Models;
using TaskManagement.Api.Services;
using TaskManagement.Api.Middleware;


var builder = WebApplication.CreateBuilder(args);

Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// using Microsoft.AspNetCore.Identity;
// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

#region Configure JWT Authentication
// Configure JWT Authentication
// Fix for CS8604: Ensure the configuration value is not null before using it
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT Key is not configured in the application settings.");
}
var key = Encoding.ASCII.GetBytes(jwtKey);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});
#endregion

// Add services
builder.Services.AddScoped<ITaskService, TaskService>();

// Swashbuckle.AspNetCore
// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Task Management API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

//// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();


/*
 We have 2 ways to do automatic migrations:
1. Use a Migration Runner Service (as implemented in MigrationService.cs)
   Add Hosted Service for automatic migrations like below
   builder.Services.AddHostedService<MigrationService>();
2. Use the code block below in Program.cs (commented out for now)
 
 */
#region Custom Middleware Registration (if needed) for automatic migrations

builder.Services.AddHostedService<MigrationService>();

#endregion

var app = builder.Build();

#region Automatic Migrations using code block in Program.cs (alternative to MigrationService)
//// Apply migrations automatically
//if (app.Environment.IsDevelopment())
//{
//    using (var scope = app.Services.CreateScope())
//    {
//        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
//        logger.LogInformation("Applying database migrations...");
//        dbContext.Database.Migrate();
//        logger.LogInformation("Database migrations applied successfully.");
//    }
//}

#endregion

# region Swagger Configuration for Dev environments
//// Configure the HTTP request pipeline
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
#endregion

# region Swagger Configuration for all environments
// Configure Swagger for all environments
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Management API V1");
    c.RoutePrefix = "swagger"; // so it's accessible at /swagger
});
#endregion

/*
We have 3 ways to do this redirect from root to Swagger:
1. Using Middleware (as implemented in RootRedirectMiddleware.cs)
   app.UseMiddleware<RootRedirectMiddleware>();
2. Using app.MapGet (as shown below)
   app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
3. launchSettings.json add "launchUrl": "swagger", which is simpler but only works in development when launched from Visual Studio or via dotnet run command.
 
 */
//app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

// Add custom middleware
//app.UseMiddleware<RootRedirectMiddleware>(); // Add this before other middleware for redirect from root to Swagger
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
