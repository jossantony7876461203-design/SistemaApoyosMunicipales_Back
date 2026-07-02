namespace SistemaApoyosMunicipales.Application.Common.Models
{
    public class ImagenTarea
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string NombreArchivo { get; set; } = string.Empty;
        public byte[] Bytes { get; set; } = [];    
        public string Carpeta { get; set; } = string.Empty;
        public Guid EntidadId { get; set; }          
        public EstadoTarea Estado { get; set; } = EstadoTarea.Pendiente;
        public string? UrlResultado { get; set; }
        public string? PublicIdResultado { get; set; }
        public string? Error { get; set; }
        public DateTimeOffset CreadaAt { get; set; } = DateTimeOffset.UtcNow;
    }

    public enum EstadoTarea
    {
        Pendiente,
        Procesando,
        Completada,
        Fallida
    }
}