namespace AdoptMeApi.Modelo
{
    /// <summary>
    /// Esta clase se usa para el uso del verbo PATCH con el cual simplemente modificamos algunos de los datos de la mascota.
    /// </summary>
    public class PatchedMascota
    {
        public string? Nombre { get; set; }
        public string? Especie { get; set; }
        public int? Edad { get; set; }
        public string? EstadoAdopcion { get; set; }
        public int? CuidadorId { get; set; }
    }
}
