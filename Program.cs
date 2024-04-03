using FluentValidation;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalAPIPeliculas.Data;
using MinimalAPIPeliculas.Endpoints;
using MinimalAPIPeliculas.Models;
using MinimalAPIPeliculas.Repositories;
using MinimalAPIPeliculas.Servicios;
using MinimalAPIPeliculas.Swagger;
using MinimalAPIPeliculas.Utilities;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);
var origenesPermitidos = builder.Configuration.GetValue<string>("origenesPermitidos")!;

//START: services area

builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
    opciones.UseSqlServer("name=DefaultConnection"));

//Config para Identity:
builder.Services.AddIdentityCore<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<UserManager<IdentityUser>>();
builder.Services.AddScoped<SignInManager<IdentityUser>>();

var param = builder.Configuration.GetValue<string>("param");

builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(configuracion =>
    {
        configuracion.WithOrigins(origenesPermitidos).AllowAnyHeader().AllowAnyMethod();
    });
    opciones.AddPolicy("libre", configuracion =>
    {
        configuracion.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});
//builder.Services.AddOutputCache();

builder.Services.AddStackExchangeRedisOutputCache(opt =>
{
    opt.Configuration = builder.Configuration.GetConnectionString("redis");
});
builder.Services.AddEndpointsApiExplorer();

//Title and other information
builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Minimal API with .NET 8",
        Description = "A web API using the concept of the Minimal API",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Email = "",
            Name = "Jorge Rivadeneira",
            Url = new Uri("https://github.com/JorgeRivadeneira")
        },
        License = new Microsoft.OpenApi.Models.OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://opensource.org/license/mit")
        }
    });

    x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    x.OperationFilter<FiltroAutorizacion>();

    //Moved to the FiltroAutorizacion.cs
    //x.AddSecurityRequirement(new OpenApiSecurityRequirement
    //{
    //    {
    //        new OpenApiSecurityScheme
    //        {
    //            Reference = new OpenApiReference
    //            {
    //                Type = ReferenceType.SecurityScheme,
    //                Id = "Bearer"
    //            }
    //        }, new string[] {}
    //    }
    //});
});

builder.Services.AddScoped<IRepositorioGeneros, RepositorioGeneros>();
builder.Services.AddScoped<IRepositorioActores, RepositorioActores>();
builder.Services.AddScoped<IRepositorioPeliculas, RepositorioPeliculas>();
builder.Services.AddScoped<IRepositorioComentarios, RepositorioComentarios>();

builder.Services.AddScoped<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();
builder.Services.AddTransient<IServicioUsuario, ServicioUsuario>();

builder.Services.AddHttpContextAccessor(); //For storage locally

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddAuthentication().AddJwtBearer(opciones =>
{
    opciones.MapInboundClaims = false;

    opciones.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        //IssuerSigningKey = Llaves.ObtenerLlave(builder.Configuration).First(),
        IssuerSigningKeys = Llaves.ObtenerTodasLasLlave(builder.Configuration),
        ClockSkew = TimeSpan.Zero
    };

});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("isAdmin", politica => politica.RequireClaim("isAdmin"));
});

//END: services area
var app = builder.Build();

//START: area middlewares
if (builder.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles(); //For storage locally
app.UseCors();
app.UseOutputCache();
app.UseAuthorization();

app.MapGet("/", [EnableCors(policyName: "libre")] () => "Hello World");

app.MapGroup("/generos").MapGeneros();
app.MapGroup("/actores").MapActores();
app.MapGroup("/peliculas").MapPeliculas();
app.MapGroup("/pelicula/{peliculaId:int}/comentarios").MapComentarios();
app.MapGroup("/usuarios").MapUsuarios();

app.MapPost("/modelbinding", (string? nombre) =>
{
    if(nombre is null)
    {
        nombre = "Empty";
    }
    return TypedResults.Ok(nombre);
});

//END: area middlewares


app.Run();
