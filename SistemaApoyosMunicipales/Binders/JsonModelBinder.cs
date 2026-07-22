using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;

namespace SistemaApoyosMunicipales.API.Binders
{
    public sealed class JsonModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueResult == ValueProviderResult.None)
                return Task.CompletedTask;

            var valorCrudo = valueResult.FirstValue;

            if (string.IsNullOrWhiteSpace(valorCrudo))
                return Task.CompletedTask;

            try
            {
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var resultado = JsonSerializer.Deserialize(valorCrudo, bindingContext.ModelType, opciones);
                bindingContext.Result = ModelBindingResult.Success(resultado);
            }
            catch (JsonException)
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "El formato JSON enviado no es válido.");
            }

            return Task.CompletedTask;
        }
    }
}
