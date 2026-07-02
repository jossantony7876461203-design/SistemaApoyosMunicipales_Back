using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using SistemaApoyosMunicipales.Application.Interfaces.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Infrastructure.Storage.Services
{
    public sealed class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            var account = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]);

            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
        }

        public async Task<CloudinaryUploadResult> SubirImagenAsync(
            Stream stream,
            string nombreArchivo,
            string carpeta)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(nombreArchivo, stream),
                Folder = carpeta,
                PublicId = $"{carpeta}/{Guid.NewGuid()}",
                Overwrite = false,
                Transformation = new Transformation()
                    .Quality("auto")
                    .FetchFormat("auto")
            };

            var resultado = await _cloudinary.UploadAsync(uploadParams);

            if (resultado.Error is not null)
                throw new Exception(
                    $"Error al subir imagen: {resultado.Error.Message}");

            return new CloudinaryUploadResult
            {
                Url = resultado.SecureUrl.ToString(),
                PublicId = resultado.PublicId
            };
        }

        public async Task EliminarImagenAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            await _cloudinary.DestroyAsync(deleteParams);
        }
    }
}
