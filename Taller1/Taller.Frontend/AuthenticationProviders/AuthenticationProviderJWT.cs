using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Taller.Frontend.Services;
using Taller.Frontend.Helpers;

namespace Taller.Frontend.AuthenticationProviders;

public class AuthenticationProviderJWT : AuthenticationStateProvider, ILoginService
{
    private readonly IJSRuntime _jSRuntime;
    private readonly HttpClient _httpClient;
    private readonly string _tokenKey;
    private readonly AuthenticationState _anonimous;

    public AuthenticationProviderJWT(IJSRuntime jSRuntime, HttpClient httpClient)
    {
        _jSRuntime = jSRuntime;
        _httpClient = httpClient;
        _tokenKey = "TOKEN_KEY";
        _anonimous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public async Task LoginAsync(string token)
    {
        await _jSRuntime.SetLocalStorage(_tokenKey, token);
        var authState = BuildAuthenticationState(token);
        NotifyAuthenticationStateChanged(Task.FromResult(authState));
    }

    public async Task LogoutAsync()
    {
        await _jSRuntime.RemoveLocalStorage(_tokenKey);
        _httpClient.DefaultRequestHeaders.Authorization = null;
        NotifyAuthenticationStateChanged(Task.FromResult(_anonimous));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await _jSRuntime.GetLocalStorage(_tokenKey);

            if (token is null)
            {
                return _anonimous;
            }

            // CAMBIO IMPORTANTE: Limpiar comillas y espacios extra
            string tokenString = token.ToString()!.Trim('"').Trim();

            if (string.IsNullOrEmpty(tokenString))
            {
                return _anonimous;
            }

            return BuildAuthenticationState(tokenString);
        }
        catch
        {
            // Si ocurre CUALQUIER error (token corrupto, error de lectura, etc.)
            // Devolvemos anónimo para no romper la app y limpiamos el token malo.
            try
            {
                await _jSRuntime.RemoveLocalStorage(_tokenKey);
            }
            catch { /* Ignorar si falla la limpieza */ }

            return _anonimous;
        }
    }

    private AuthenticationState BuildAuthenticationState(string token)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            var claims = ParseClaimsFromJWT(token);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt")));
        }
        catch
        {
            return _anonimous;
        }
    }

    private IEnumerable<Claim> ParseClaimsFromJWT(string token)
    {
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var unserializedToken = jwtSecurityTokenHandler.ReadJwtToken(token);
        return unserializedToken.Claims;
    }
}