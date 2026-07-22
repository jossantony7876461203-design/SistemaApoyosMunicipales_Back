using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using SistemaApoyosMunicipales.Application.DTOs.RegistroApoyo;

namespace SistemaApoyosMunicipales.API.Binders
{
    public sealed class JsonModelBinderProvider : IModelBinderProvider
    {
        // Aquí registras QUÉ tipos deben tratarse como JSON crudo en multipart/form-data.
        private static readonly HashSet<Type> TiposConBindingJson = new()
    {
        typeof(List<PagoRegistroApoyoDto>)
    };

        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (TiposConBindingJson.Contains(context.Metadata.ModelType))
            {
                return new BinderTypeModelBinder(typeof(JsonModelBinder));
            }

            return null; // deja que ASP.NET Core siga con su lógica normal para todo lo demás
        }
    }
}
