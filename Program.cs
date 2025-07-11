using GeorgianRailwayApi.Data;
using GeorgianRailwayApi.Repositories.TicketRepositoryFile;
using GeorgianRailwayApi.Repositories.TrainRepositoryFile;
using GeorgianRailwayApi.Services.TicketService;
using GeorgianRailwayApi.Services.Token;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using GeorgianRailwayApi.Middleware;
using AutoMapper;
using GeorgianRailwayApi.Mapping;
using Microsoft.Extensions.Caching.Memory;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<RegisterRequestDtoValidator>());
;


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ITrainRepository, TrainRepository>();
builder.Services.AddScoped<GeorgianRailwayApi.Services.Email.IEmailService, GeorgianRailwayApi.Services.Email.EmailService>();



builder.Services.AddMediatR(typeof(Program).Assembly);


builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddMemoryCache();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "1.0.0", // Valid OpenAPI version format
        Title = "Georgian Railway API",
        Description = "API for booking and management of Georgian Railway"
    });
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateAudience = false,
        ValidateIssuer = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!))
    };
}); 
var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    GeorgianRailwayApi.Data.SeedData.SeedAdminUserAsync(dbContext).Wait();
}


app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger(c =>
{
    c.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0;
});

app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseCors(police => police.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.Run();


