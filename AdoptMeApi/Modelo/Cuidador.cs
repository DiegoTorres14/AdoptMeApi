namespace AdoptMeApi.Modelo
{
    public class Cuidador
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;

        // Propiedad de navegación: Un cuidador tiene una lista de mascotas a su cargo
        public List<Mascota> Mascotas { get; set; } = new();
    }
}
