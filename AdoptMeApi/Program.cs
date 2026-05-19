using AdoptMeApi.DBContext;
using AdoptMeApi.Endpoints;
using AdoptMeApi.Modelo;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Configurar el serializador JSON para que ignore las referencias cíclicas infinitas
builder.Services.ConfigureHttpJsonOptions(options => {
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// 1. Registrar OpenAPI nativo de .NET 10
builder.Services.AddOpenApi();

// 2. Conectar Entity Framework con MySQL usando el proveedor oficial
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<RefugioContext>(options =>
    options.UseMySQL(connectionString)); // <-- Cambiado a UseMySQL clásico de Oracle

var app = builder.Build();

// 3. Configurar la interfaz visual moderna (Scalar)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// ==========================================
// ENDPOINTS DE LA API (MINIMAL APIs)
// ==========================================

app.MapMascotasEndpoints();
app.MapCuidadoresEndpoints();

//// SACAR DATOS (GET)
//app.MapGet("/api/mascotas", async (RefugioContext db) =>
//{
//    // Usamos .Include para decirle a EF Core: "Tráeme la mascota Y ADEMÁS haz un JOIN con su Cuidador"
//    var listaMascotas = await db.Mascotas
//                                .Include(m => m.Cuidador)
//                                .ToListAsync();
//    return Results.Ok(listaMascotas);
//});

//app.MapGet("/api/cuidadores", async (RefugioContext db) =>
//{
//    var listaCuidadores = await db.Cuidadores
//                                  .Include(c => c.Mascotas) // Para traer también la lista de mascotas
//                                  .ToListAsync();
//    return Results.Ok(listaCuidadores);
//});

//// METER DATOS (POST)
//app.MapPost("/api/mascotas", async (Mascota nuevaMascota, RefugioContext db) =>
//{
//    db.Mascotas.Add(nuevaMascota);
//    await db.SaveChangesAsync();
//    return Results.Created($"/api/mascotas/{nuevaMascota.Id}", nuevaMascota);
//});

//// MODIFICAR DATOS (PUT)
//app.MapPut("/api/mascotas/{id}", async (int id, Mascota mascotaModificada, RefugioContext db) =>
//{
//    var mascotaOriginal = await db.Mascotas.FindAsync(id);
//    if (mascotaOriginal is null)
//    {
//        return Results.NotFound($"No se encontró ninguna mascota con el ID {id}");
//    }
//    mascotaOriginal.Nombre = mascotaModificada.Nombre;
//    mascotaOriginal.Especie = mascotaModificada.Especie;
//    mascotaOriginal.Edad = mascotaModificada.Edad;
//    mascotaOriginal.EstadoAdopcion = mascotaModificada.EstadoAdopcion;
//    await db.SaveChangesAsync();
//    return Results.Ok(mascotaOriginal);
//});

//// ELIMINAR DATOS (DELETE)
//app.MapDelete("/api/mascotas/{id}", async (int id, RefugioContext db) =>
//{
//    Mascota mascota = await db.Mascotas.FindAsync(id);
//    if (mascota is null)
//    {
//        return Results.NotFound($"No se encontró la mascota con ID {id}");
//    }
//    db.Mascotas.Remove(mascota);
//    await db.SaveChangesAsync();
//    return Results.Ok($"La mascota {mascota.Nombre} fue eliminada correctamente.");
//});

//// MODIFICAR ALGUNO DE LOS DATOS ÚNICAMENTE (PATCH)
//app.MapPatch("/api/mascotas/{id}", async (int id, PatchedMascota datosParciales, RefugioContext db) =>
//{
//    var mascotaOriginal = await db.Mascotas.FindAsync(id);
//    if (mascotaOriginal is null)
//    {
//        return Results.NotFound($"No se encontró la mascota con ID {id}");
//    }

//    if (datosParciales.Nombre is not null)
//    {
//        mascotaOriginal.Nombre = datosParciales.Nombre;
//    }

//    if (datosParciales.Especie is not null)
//    {
//        mascotaOriginal.Especie = datosParciales.Especie;
//    }

//    // Con los números (int?), verificamos que no sea nulo
//    if (datosParciales.Edad.HasValue)
//    {
//        mascotaOriginal.Edad = datosParciales.Edad.Value;
//    }

//    if (datosParciales.EstadoAdopcion is not null)
//    {
//        mascotaOriginal.EstadoAdopcion = datosParciales.EstadoAdopcion;
//    }

//    if (datosParciales.CuidadorId.HasValue)
//    {
//        mascotaOriginal.CuidadorId = datosParciales.CuidadorId;
//    }

//    await db.SaveChangesAsync();
//    return Results.Ok(mascotaOriginal);
//});

//app.MapPut("/api/mascotas/{mascotaId}/asignar-cuidador/{cuidadorId}", async (int mascotaId, int cuidadorId, RefugioContext db) =>
//{
//    // 1. Verificar si la mascota existe
//    var mascota = await db.Mascotas.FindAsync(mascotaId);
//    if (mascota is null) return Results.NotFound("No se encontró la mascota.");

//    // 2. Verificar si el cuidador existe en la base de datos
//    var cuidadorExiste = await db.Cuidadores.AnyAsync(c => c.Id == cuidadorId);
//    if (!cuidadorExiste) return Results.NotFound("El cuidador especificado no existe.");

//    // 3. Modificar la clave foránea directamente
//    mascota.CuidadorId = cuidadorId;

//    // 4. Guardar cambios en MySQL
//    await db.SaveChangesAsync();

//    return Results.Ok($"El cuidador con ID {cuidadorId} fue asignado con éxito a {mascota.Nombre}.");
//});

app.Run();
