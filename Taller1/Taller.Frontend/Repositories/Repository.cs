using System.Net;
using System.Text;
using System.Text.Json;

namespace Taller.Frontend.Repositories;

public class Repository : IRepository
{
    private readonly HttpClient _httpClient;

    private JsonSerializerOptions _jsonDefaultOptions => new()
    {
        PropertyNameCaseInsensitive = true
    };

    public Repository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseWrapper<T>> GetAsync<T>(string url)
    {
        try
        {
            var responseHttp = await _httpClient.GetAsync(url);
            if (responseHttp.IsSuccessStatusCode)
            {
                var response = await DeserializeAsync<T>(responseHttp);
                return new HttpResponseWrapper<T>(response, false, responseHttp);
            }

            return new HttpResponseWrapper<T>(default, true, responseHttp);
        }
        catch (HttpRequestException ex)
        {
            return FallbackError<T>(HttpStatusCode.ServiceUnavailable, ex.Message);
        }
        catch (TaskCanceledException ex)
        {
            return FallbackError<T>(HttpStatusCode.RequestTimeout, ex.Message);
        }
        catch (Exception ex)
        {
            return FallbackError<T>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<HttpResponseWrapper<object>> PostAsync<T>(string url, T model)
    {
        try
        {
            using var content = CreateJsonContent(model);
            var responseHttp = await _httpClient.PostAsync(url, content);
            return new HttpResponseWrapper<object>(null, !responseHttp.IsSuccessStatusCode, responseHttp);
        }
        catch (HttpRequestException ex)
        {
            return FallbackError<object>(HttpStatusCode.ServiceUnavailable, ex.Message);
        }
        catch (TaskCanceledException ex)
        {
            return FallbackError<object>(HttpStatusCode.RequestTimeout, ex.Message);
        }
        catch (Exception ex)
        {
            return FallbackError<object>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<HttpResponseWrapper<TActionResponse>> PostAsync<T, TActionResponse>(string url, T model)
    {
        try
        {
            using var content = CreateJsonContent(model);
            var responseHttp = await _httpClient.PostAsync(url, content);

            if (responseHttp.IsSuccessStatusCode)
            {
                var response = await DeserializeAsync<TActionResponse>(responseHttp);
                return new HttpResponseWrapper<TActionResponse>(response, false, responseHttp);
            }

            return new HttpResponseWrapper<TActionResponse>(default, true, responseHttp);
        }
        catch (HttpRequestException ex)
        {
            return FallbackError<TActionResponse>(HttpStatusCode.ServiceUnavailable, ex.Message);
        }
        catch (TaskCanceledException ex)
        {
            return FallbackError<TActionResponse>(HttpStatusCode.RequestTimeout, ex.Message);
        }
        catch (Exception ex)
        {
            return FallbackError<TActionResponse>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<HttpResponseWrapper<object>> DeleteAsync(string url)
    {
        try
        {
            var responseHttp = await _httpClient.DeleteAsync(url);
            return new HttpResponseWrapper<object>(null, !responseHttp.IsSuccessStatusCode, responseHttp);
        }
        catch (HttpRequestException ex)
        {
            return FallbackError<object>(HttpStatusCode.ServiceUnavailable, ex.Message);
        }
        catch (TaskCanceledException ex)
        {
            return FallbackError<object>(HttpStatusCode.RequestTimeout, ex.Message);
        }
        catch (Exception ex)
        {
            return FallbackError<object>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<HttpResponseWrapper<object>> PutAsync<T>(string url, T model)
    {
        try
        {
            using var content = CreateJsonContent(model);
            var responseHttp = await _httpClient.PutAsync(url, content);
            return new HttpResponseWrapper<object>(null, !responseHttp.IsSuccessStatusCode, responseHttp);
        }
        catch (HttpRequestException ex)
        {
            return FallbackError<object>(HttpStatusCode.ServiceUnavailable, ex.Message);
        }
        catch (TaskCanceledException ex)
        {
            return FallbackError<object>(HttpStatusCode.RequestTimeout, ex.Message);
        }
        catch (Exception ex)
        {
            return FallbackError<object>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<HttpResponseWrapper<TActionResponse>> PutAsync<T, TActionResponse>(string url, T model)
    {
        try
        {
            using var content = CreateJsonContent(model);
            var responseHttp = await _httpClient.PutAsync(url, content);

            if (responseHttp.IsSuccessStatusCode)
            {
                var response = await DeserializeAsync<TActionResponse>(responseHttp);
                return new HttpResponseWrapper<TActionResponse>(response, false, responseHttp);
            }

            return new HttpResponseWrapper<TActionResponse>(default, true, responseHttp);
        }
        catch (HttpRequestException ex)
        {
            return FallbackError<TActionResponse>(HttpStatusCode.ServiceUnavailable, ex.Message);
        }
        catch (TaskCanceledException ex)
        {
            return FallbackError<TActionResponse>(HttpStatusCode.RequestTimeout, ex.Message);
        }
        catch (Exception ex)
        {
            return FallbackError<TActionResponse>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    // ----------------- Helpers -----------------

    private static StringContent CreateJsonContent<T>(T model)
    {
        var json = JsonSerializer.Serialize(model);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private async Task<T> DeserializeAsync<T>(HttpResponseMessage responseHttp)
    {
        var response = await responseHttp.Content.ReadAsStringAsync();

        // Si el server no devolvió contenido (204/empty), evitamos JsonException
        if (string.IsNullOrWhiteSpace(response))
            return default!;

        return JsonSerializer.Deserialize<T>(response, _jsonDefaultOptions)!;
    }

    private static HttpResponseWrapper<T> FallbackError<T>(HttpStatusCode code, string message)
    {
        var fake = new HttpResponseMessage(code)
        {
            ReasonPhrase = "Client-side fallback",
            Content = new StringContent(message)
        };
        return new HttpResponseWrapper<T>(default, true, fake);
    }
}
