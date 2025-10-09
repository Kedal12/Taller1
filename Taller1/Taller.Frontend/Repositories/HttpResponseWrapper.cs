using System.Net;

namespace Taller.Frontend.Repositories;

public class HttpResponseWrapper<T>
{
    public HttpResponseWrapper(T? response, bool error, HttpResponseMessage httpResponseMessage)
    {
        Response = response;
        Error = error;
        HttpResponseMessage = httpResponseMessage;
    }

    public T? Response { get; }
    public bool Error { get; }
    public HttpResponseMessage HttpResponseMessage { get; }

    public async Task<string?> GetErrorMessageAsync()
    {
        if (!Error) return null;

        var status = HttpResponseMessage.StatusCode;
        var body = await HttpResponseMessage.Content.ReadAsStringAsync();

        return status switch
        {
            HttpStatusCode.NotFound => "Recurso no encontrado.",
            HttpStatusCode.BadRequest => string.IsNullOrWhiteSpace(body) ? "Solicitud inválida." : body,
            HttpStatusCode.Unauthorized => "Tienes que estar logueado para ejecutar esta operación.",
            HttpStatusCode.Forbidden => "No tienes permisos para hacer esta operación.",
            HttpStatusCode.RequestTimeout => "Tiempo de espera agotado.",
            HttpStatusCode.ServiceUnavailable => $"Servicio no disponible: {body}",
            _ => $"Error {(int)status} {status}. {(string.IsNullOrWhiteSpace(body) ? "" : body)}"
        };
    }
}
