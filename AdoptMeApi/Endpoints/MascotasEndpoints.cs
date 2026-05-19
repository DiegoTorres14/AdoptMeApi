using AdoptMeApi.DBContext;
using AdoptMeApi.Modelo;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AdoptMeApi.Endpoints
{
    public static class MascotasEndpoints // Clase estática porque no se van a crear instancias de la misma
    {
        public static void MapMascotasEndpoints(this IEndpointRouteBuilder app)
        {
            // Agrupamos todas las rutas bajo el prefijo "/api/mascotas" para no repetir código
            var grupo = app.MapGroup("/api/mascotas");

            // GET: Obtener todas las mascotas
            grupo.MapGet("/", async (RefugioContext db) =>
            {
                var listaMascotas = await db.Mascotas.Include(m => m.Cuidador).ToListAsync();
                return Results.Ok(listaMascotas);
            });

            // POST: Registrar una nueva mascota
            grupo.MapPost("/", async (Mascota nuevaMascota, IValidator<Mascota> validator, RefugioContext db) =>
            {
                // 1. Ejecutar las reglas de validación sobre el objeto recibido
                var resultado = await validator.ValidateAsync(nuevaMascota);

                // 2. Si no es válido, frenar la operación y devolver los errores al cliente
                if (!resultado.IsValid)
                {
                    // ToDictionary agrupa los errores por el nombre del campo (ej. "Edad": ["La edad no puede ser..."])
                    return Results.BadRequest(resultado.ToDictionary());
                }

                // 3. Si todo está bien se guarda en la base de datos
                db.Mascotas.Add(nuevaMascota);
                await db.SaveChangesAsync();
                return Results.Created($"/api/mascotas/{nuevaMascota.Id}", nuevaMascota);
            });

            // PUT: Reemplazar datos de una mascota
            grupo.MapPut("/{id}", async (int id, Mascota mascotaModificada, IValidator <Mascota> validator, RefugioContext db) =>
            {
                // 1. Ejecutar las reglas de validación sobre el objeto recibido
                var resultado = await validator.ValidateAsync(mascotaModificada);

                // 2. Si no es válido, frenar la operación y devolver los errores al cliente
                if (!resultado.IsValid)
                {
                    // ToDictionary agrupa los errores por el nombre del campo (ej. "Edad": ["La edad no puede ser..."])
                    return Results.BadRequest(resultado.ToDictionary());
                }

                // 3. Si todo está bien se procede a actualizar la base de datos
                var mascotaOriginal = await db.Mascotas.FindAsync(id);
                if (mascotaOriginal is null) return Results.NotFound($"No se encontró la mascota con ID {id}");

                mascotaOriginal.Nombre = mascotaModificada.Nombre;
                mascotaOriginal.Especie = mascotaModificada.Especie;
                mascotaOriginal.Edad = mascotaModificada.Edad;
                mascotaOriginal.EstadoAdopcion = mascotaModificada.EstadoAdopcion;

                await db.SaveChangesAsync();
                return Results.Ok(mascotaOriginal);
            });

            // PUT: Asignar cuidador específico
            grupo.MapPut("/{mascotaId}/asignar-cuidador/{cuidadorId}", async (int mascotaId, int cuidadorId, RefugioContext db) =>
            {
                // Validar que existan ambos IDs
                if (mascotaId <= 0 || cuidadorId <= 0)
                {
                    return Results.BadRequest("Los IDs de mascota y cuidador deben ser números mayores a cero.");
                }

                var mascota = await db.Mascotas.FindAsync(mascotaId);
                if (mascota is null) return Results.NotFound("No se encontró la mascota.");

                var cuidadorExiste = await db.Cuidadores.AnyAsync(c => c.Id == cuidadorId);
                if (!cuidadorExiste) return Results.NotFound("El cuidador especificado no existe.");

                mascota.CuidadorId = cuidadorId;
                await db.SaveChangesAsync();

                return Results.Ok($"El cuidador con ID {cuidadorId} fue asignado con éxito a {mascota.Nombre}.");
            });

            // PATCH: Modificación parcial
            grupo.MapPatch("/{id}", async (int id, PatchedMascota datosParciales, IValidator <PatchedMascota> validator, RefugioContext db) =>
            {
                var resultado = await validator.ValidateAsync(datosParciales);
                if (!resultado.IsValid)
                {
                    return Results.BadRequest(resultado.ToDictionary());
                }

                var mascotaOriginal = await db.Mascotas.FindAsync(id);
                if (mascotaOriginal is null) return Results.NotFound($"No se encontró la mascota con ID {id}");

                if (datosParciales.Nombre is not null) mascotaOriginal.Nombre = datosParciales.Nombre;
                if (datosParciales.Especie is not null) mascotaOriginal.Especie = datosParciales.Especie;
                if (datosParciales.Edad.HasValue) mascotaOriginal.Edad = datosParciales.Edad.Value;
                if (datosParciales.EstadoAdopcion is not null) mascotaOriginal.EstadoAdopcion = datosParciales.EstadoAdopcion;
                if (datosParciales.CuidadorId.HasValue) mascotaOriginal.CuidadorId = datosParciales.CuidadorId.Value;

                await db.SaveChangesAsync();
                return Results.Ok(mascotaOriginal);
            });

            // DELETE: Eliminar una mascota
            grupo.MapDelete("/{id}", async (int id, RefugioContext db) =>
            {
                var mascota = await db.Mascotas.FindAsync(id);
                if (mascota is null) return Results.NotFound($"No se encontró la mascota con ID {id}");

                db.Mascotas.Remove(mascota);
                await db.SaveChangesAsync();
                return Results.Ok($"La mascota {mascota.Nombre} fue eliminada correctamente.");
            });
        }
    }
}
