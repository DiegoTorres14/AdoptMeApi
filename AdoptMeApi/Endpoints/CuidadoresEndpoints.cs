using AdoptMeApi.DBContext;
using AdoptMeApi.Modelo;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AdoptMeApi.Endpoints
{
    public static class CuidadoresEndpoints
    {
        public static void MapCuidadoresEndpoints(this IEndpointRouteBuilder app) 
        {
            // Agrupar todas las rutas bajo el prefijo "/api/cuidadores"
            var grupo = app.MapGroup("/api/cuidadores");

            // GET: Obtener todos los cuidadores
            grupo.MapGet("/", async (RefugioContext db) =>
            {
                var listaCuidadores = await db.Cuidadores.Include(c => c.Mascotas).ToListAsync();
                return listaCuidadores;
            });

            // POST: Registrar un nuevo cuidador
            grupo.MapPost("/", async (Cuidador cuidador, IValidator<Cuidador> validator, RefugioContext db) => 
            {
                // 1. Aplicar validaciones
                var resultado = await validator.ValidateAsync(cuidador);

                // 2. Si el resultado no es válido devolvemos el código correspondiente
                if (!resultado.IsValid)
                {
                    // ToDictionary agrupa los errores por el nombre del campo (ej. "Nombre": ["El nombre no puede ser..."])
                    return Results.BadRequest(resultado.ToDictionary());
                }

                // 3. Agregamos a la base de datos
                db.Cuidadores.Add(cuidador);
                await db.SaveChangesAsync();
                return Results.Created($"/api/cuidadores/{cuidador.Id}", cuidador);
            });

            // PUT: Reemplazar datos de un cuidador
            grupo.MapPut("/{id}", async (int id,Cuidador cuidadorModificado, IValidator<Cuidador> validator , RefugioContext db) =>
            {
                // 1. Validamos el cuidador que queremos modificar
                var resultado = await validator.ValidateAsync(cuidadorModificado);

                // 2. Si el resultado no es válido devolvemos el código correspondiente 
                if (!resultado.IsValid)
                {
                    // ToDictionary agrupa los errores por el nombre del campo (ej. "Nombre": ["El nombre no puede ser..."])
                    return Results.BadRequest(resultado.ToDictionary());
                }

                // 3. Si todo es correcto seguimos con el procedimiento
                var cuidadorOriginal = await db.Cuidadores.FindAsync(id);
                if (cuidadorOriginal is null) return Results.NotFound($"No se ha encontrado cuidador con el id {id}");

                //cuidadorOriginal.Id = cuidadorModificado.Id; // Modificar el Id puede ser un error. Las bases de datos controlan al crear que sea autoincrement. Podríamos modificar un ID por otro ya utilizado.
                cuidadorOriginal.Nombre = cuidadorModificado.Nombre;
                cuidadorOriginal.Telefono = cuidadorModificado.Telefono;

                await db.SaveChangesAsync();
                return Results.Ok(cuidadorOriginal);
            });

            // PUT: Asignar mascota específica
            grupo.MapPut("/{cuidadorId}/asignar-mascota/{mascotaID}", async (int cuidadorId, int mascotaId, RefugioContext db) =>
            {
                // Comprobar que ni el id de mascota ni el de cuidador son nulos
                if (cuidadorId <= 0 || mascotaId <= 0) 
                { 
                    return Results.BadRequest("Los IDs de mascota y cuidador deben ser números mayores a cero.");
                }
                
                // Primero obtener el cuidador
                var cuidador = await db.Cuidadores.FindAsync(cuidadorId);
                if (cuidador is null) return Results.NotFound($"No se encontró el cuidador que se buscaba con el id {cuidadorId}");

                // Si el cuidador no es nulo, buscamos la mascota
                var mascota = await db.Mascotas.FindAsync(mascotaId);
                if (mascota is null) return Results.NotFound($"La mascota con el id {mascotaId} no existe.");

                // Si la mascota existe agregamos la mascota a la lista de mascotas
                //cuidador.Mascotas.Add(mascota);
                // Como la relación es 1N debemos hacerlo como en la siguiente línea. En la anterior la lista puede ser nula y da error
                mascota.CuidadorId = cuidadorId;
                await db.SaveChangesAsync();

                // Retorno del método
                return Results.Ok($"La mascota con el id {mascotaId} ha sido asignado al cuidador {cuidador.Nombre}");
            });

            // PATCH: Modificación parcial de un cuidador
            grupo.MapPatch("/{id}", async (int id, PatchedCuidador datosParcheados, IValidator<PatchedCuidador> validator, RefugioContext db) =>
            {
                // 1. Validar los datos del cuidador que queremos modificar
                var resultado = await validator.ValidateAsync(datosParcheados);

                // 2. Si el resultado no es válido devolvemos el resultado correspondiente
                if (!resultado.IsValid) 
                {
                    return Results.BadRequest(resultado.ToDictionary());
                }

                // 3. Guardamos en la base de datos si ha salido bien
                var cuidadorOriginal = await db.Cuidadores.FindAsync(id);
                if (cuidadorOriginal is null) return Results.NotFound($"No se ha encontrado un cuidador con el id {id}");

                if (datosParcheados.Nombre is not null) cuidadorOriginal.Nombre = datosParcheados.Nombre;
                if (datosParcheados.Telefono is not null) cuidadorOriginal.Telefono = datosParcheados.Telefono;

                await db.SaveChangesAsync();
                return Results.Ok(cuidadorOriginal);
            });

            // DELETE: Eliminar cuidador
            grupo.MapDelete("/{id}", async (int id, RefugioContext db) => 
            {
                var cuidador = await db.Cuidadores.FindAsync(id);
                if (cuidador is null) return Results.NotFound($"No se encontró resultado para cuidador con el id: {id}");

                db.Cuidadores.Remove(cuidador);
                await db.SaveChangesAsync();
                return Results.Ok($"El cuidador {cuidador.Nombre} fue eliminado con exito.");
            });
        }
        
    }
}
