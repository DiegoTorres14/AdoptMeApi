namespace AdoptMeApi.Modelo
{
    public class PatchedCuidador
    {
        public string? Nombre { get; set; }
        public string? Telefono { get; set; }

        // Propiedad de navegación: Un cuidador tiene una lista de mascotas a su cargo
        public List<Mascota>? Mascotas { get; set; }
    }
}
