using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIPeliculas.Models;

var builder = WebApplication.CreateBuilder(args);
var origenesPermitidos = builder.Configuration.GetValue<string>("origenesPermitidos")!;

//START: services area

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



app.MapGet("/generos", () =>
{
    var generos = new List<Genero>
    {
        new Genero
        {
            Id = 1,
            Nombre = "Drama"
        },
         new Genero
        {
            Id = 2,
            Nombre = "Acción"
        },
          new Genero
        {
            Id = 3,
            Nombre = "Comedia"
        },
    };

    return generos;
}).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(15)));

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
