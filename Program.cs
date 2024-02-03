using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using MinimalAPIPeliculas.Data;
using MinimalAPIPeliculas.Models;
using MinimalAPIPeliculas.Repositories;
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

//END: services area
var app = builder.Build();

//START: area middlewares
if (builder.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseOutputCache();

app.MapGet("/", [EnableCors(policyName: "libre")]() =>  "Hello World");



app.MapGet("/generos", async(IRepositorioGeneros repositorio) =>
{
    return await repositorio.ObtenerTodos();
}).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("generos-get"));

app.MapPost("/generos", async (Genero genero, IRepositorioGeneros repositorio, IOutputCacheStore outputCacheStore) =>
{
    var id = await repositorio.Crear(genero);
    await outputCacheStore.EvictByTagAsync("generos-get", default);
    return Results.Created($"/generos/{id}", genero);
});

app.MapGet("/generos/{id:int}", async(IRepositorioGeneros repositorio, int id) => {
    var genero = await repositorio.ObtenerPorId(id);
    if(genero is null)
    {
        return Results.NotFound();
    }
    return Results.Ok(genero);
});

app.MapPut("/generos/{id:int}", async (int id, Genero genero, IRepositorioGeneros repositorioGeneros,
    IOutputCacheStore outputCacheStore) =>
{
    var existe = await repositorioGeneros.Existe(id);
    if(!existe)
    {
        return Results.NotFound();
    }
    await repositorioGeneros.Actualizar(genero);
    await outputCacheStore.EvictByTagAsync("generos-get", default);
    return Results.NoContent();

});

app.MapDelete("/generos/", async (int id, IRepositorioGeneros repositorioGeneros,
    IOutputCacheStore outputCacheStore) =>
{
    var existe = await repositorioGeneros.Existe(id);
    if (!existe)
    {
        return Results.NotFound();
    }
    await repositorioGeneros.Borrar(id);
    await outputCacheStore.EvictByTagAsync("generos-get", default);
    return Results.NoContent();
});

//Cache para endpoints con controladores:
//[HttpGet]
//[OutputCache(Duration = 15)]
//public IEnumerable<Genero> Get()
//{
//    List<Genero> generos =
//    [
//        new Genero() { Id = 1, Nombre = "Drama" },
//        new Genero() { Id = 2, Nombre = "Accion" },
//        new Genero() { Id = 3, Nombre = "Comedia" },
//    ];

//    return generos;
//}

//END: area middlewares


app.Run();
