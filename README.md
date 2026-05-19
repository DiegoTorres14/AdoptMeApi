# 🐾 AdoptMe API - Sistema de Gestión para Refugio de Mascotas

¡Bienvenido a **AdoptMe API**! Este proyecto es una API REST robusta y moderna diseñada para gestionar el flujo de trabajo de un refugio de animales (ingreso, adopción, asignación de personal y control de datos). 

Desarrollada de principio a fin utilizando las últimas tecnologías del ecosistema de Microsoft, esta aplicación implementa una arquitectura limpia, relaciones de bases de datos de uno a muchos (1:N) y estrictos escudos de validación de datos.

---

## 🛠️ Tecnologías y Herramientas Utilizadas

* **Framework principal:** .NET 10.0 (C# moderno)
* **Arquitectura:** Minimal APIs de ASP.NET Core (Enfoque ligero de alto rendimiento)
* **Base de Datos:** MySQL Server
* **ORM (Mapeo de Datos):** Entity Framework Core 10 (Enfoque Database-First / Fluent API)
* **Validación de Datos:** FluentValidation (Dependency Injection Extensions)
* **Documentación:** OpenAPI (Soporte nativo de .NET 10)
* **Pruebas de Integración:** Insomnia / Postman

---

## 🏗️ Arquitectura del Proyecto

El código fuente se encuentra completamente modularizado bajo principios de código limpio (*Clean Code*), separando las responsabilidades en carpetas independientes para garantizar la escalabilidad:

```text
AdoptMeApi/
│
├── 📂 DBContext/
│   └── RefugioContext.cs        # Configuración de EF Core y mapeo de DbSets
│
├── 📂 Endpoints/
│   ├── MascotasEndpoints.cs     # Rutas lógicas para la entidad Mascota
│   └── CuidadoresEndpoints.cs   # Rutas lógicas para la entidad Cuidador
│
├── 📂 Modelo/
│   ├── Mascota.cs               # Entidad relacional Mascota
│   ├── Cuidador.cs              # Entidad relacional Cuidador
│   ├── PatchedMascota.cs        # DTO para modificaciones parciales (Mascotas)
│   └── PatchedCuidador.cs       # DTO para modificaciones parciales (Cuidadores)
│
└── 📂 Validaciones/
    ├── MascotaValidator.cs      # Escudo de reglas de negocio para Mascotas
    └── CuidadorValidator.cs     # Escudo de reglas de negocio para Cuidadores
```

---

## 🛡️ Características Clave Implementadas

### 1. Relaciones de Base de Datos (1 a N)
El sistema gestiona la relación donde **Un Cuidador** tiene a su cargo **N Mascotas**. Se implementó `.Include(c => c.Mascotas)` mediante la carga previa de Entity Framework (*Eager Loading*), optimizando el comportamiento del serializador JSON para romper bucles infinitos con `ReferenceHandler.IgnoreCycles`.

### 2. Capa de Validación Robusta
Se utiliza **FluentValidation** para blindar la base de datos contra registros corruptos o ilógicos. El sistema rechaza automáticamente peticiones inválidas con respuestas estructuradas **HTTP 400 Bad Request**:
* Longitud estricta de cadenas (Nombres de máximo 50/100 caracteres).
* Validación tipada y lógica de rangos (Edades entre 0 y 30 años).
* Control estricto de formato mediante Expresiones Regulares (Teléfonos exactamente de 9 dígitos numéricos `^[0-9]+$`).

### 3. Operaciones REST Avanzadas (PUT vs PATCH)
* **`PUT`**: Reemplazo total del recurso garantizando la idempotencia del endpoint.
* **`PATCH`**: Modificación parcial optimizada mediante tipos anulables (`?`), permitiendo alterar campos individuales sin peligro de sobreescribir el resto del registro con datos vacíos.

---

## 🚀 Endpoints de la API

### 🐕 Mascotas (`/api/mascotas`)
* `GET /api/mascotas` - Obtiene la lista completa de mascotas junto con el objeto anidado de su cuidador asignado.
* `POST /api/mascotas` - Registra un nuevo animal (Requiere validación completa).
* `PUT /api/mascotas/{id}` - Reemplaza los datos de un animal existente.
* `PATCH /api/mascotas/{id}` - Actualiza únicamente los campos enviados (ej. cambiar edad o estado de adopción).
* `DELETE /api/mascotas/{id}` - Elimina físicamente el registro de la base de datos.
* `PUT /api/mascotas/{mascotaId}/asignar-cuidador/{cuidadorId}` - Endpoint de negocio dedicado a enlazar de forma segura personal con animales a través de la URL.

### 🧑‍⚕️ Cuidadores (`/api/cuidadores`)
* `GET /api/cuidadores` - Devuelve todos los cuidadores y la lista de animales reales que tienen asignados.
* `POST /api/cuidadores` - Registra personal de refugio válido.
* `PUT /api/cuidadores/{id}` - Actualiza de forma completa la información del cuidador.
* `PATCH /api/cuidadores/{id}` - Modifica parcialmente el teléfono o nombre del cuidador.
* `DELETE /api/cuidadores/{id}` - Remueve un cuidador del sistema.

---

## ⚙️ Instrucciones de Configuración Local

### Prerrequisitos
* SDK de .NET 10.0 o superior
* Instancia local de MySQL Server activa

### Configuración del Entorno Seguro
Este repositorio cuenta con protección de credenciales sensibles. El archivo `appsettings.json` contiene cadenas de conexión genéricas. Para usar en local:

1. Clona este repositorio en tu máquina.
2. Cree la base de datos en MySQL y las tablas -> cuidadores y mascotas. El script está en el repositorio.
3. Crea el archivo local `appsettings.Development.json` dentro del directorio del proyecto o usa el `appsettings.json`.
4. Configura tu cadena de conexión privada con tu puerto y contraseña reales:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=3306;database=RefugioDB;user=root;password=TU_CONTRASEÑA_REAL"
  }
}
```

5. Compila e inicia el servidor
6. Utiliza mediante Postman, Insomnia, o cualquier herramienta similar para probar los endpoints correctamente.
