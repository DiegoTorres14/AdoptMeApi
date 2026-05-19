namespace AdoptMeApi.Modelo
{
    public class Mascota
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Especie { get; set; } = string.Empty;
        public int Edad { get; set; }
        public string EstadoAdopcion { get; set; } = "Disponible";
        public DateTime FechaIngreso { get; set; } = DateTime.Now;

        // Nuevos campos para la relación:
        public int? CuidadorId { get; set; } // Permitimos nulo por si una mascota no tiene cuidador aún
        // Propiedad de navegación: La mascota conoce a su cuidador
        public Cuidador? Cuidador { get; set; }
    }
}
