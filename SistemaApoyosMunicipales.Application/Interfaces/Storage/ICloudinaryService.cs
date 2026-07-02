using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Interfaces.Storage
{
   
        public interface ICloudinaryService
        {
            Task<CloudinaryUploadResult> SubirImagenAsync(
                Stream stream,
                string nombreArchivo,
                string carpeta);

            Task EliminarImagenAsync(string publicId);
        }

        public class CloudinaryUploadResult
        {
            public string Url { get; set; } = string.Empty;
            public string PublicId { get; set; } = string.Empty;
        }
    
}
