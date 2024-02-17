using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using MinimalAPIPeliculas.Data;
using MinimalAPIPeliculas.Endpoints;
using MinimalAPIPeliculas.Models;
using MinimalAPIPeliculas.Repositories;
using MinimalAPIPeliculas.Servicios;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);
var origenesPermitidos = builder.Configuration.GetValue<string>("origenesPermitidos")!;

//START: services area

builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
    opciones.UseSqlServer("name=DefaultConnection"));

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
builder.Services.AddOutputCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IRepositorioGeneros, RepositorioGeneros>();
builder.Services.AddScoped<IRepositorioActores, RepositorioActores>();
builder.Services.AddScoped<IRepositorioPeliculas, RepositorioPeliculas>();
builder.Services.AddScoped<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();
builder.Services.AddHttpContextAccessor(); //For storage locally

builder.Services.AddAutoMapper(typeof(Program));

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

app.MapGet("/", [EnableCors(policyName: "libre")]() =>  "Hello World");

app.MapGroup("/generos").MapGeneros();
app.MapGroup("/actores").MapActores();

//END: area middlewares


app.Run();
