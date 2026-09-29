using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace AvaloniaClient;

/// <summary>Тонкая обёртка над HttpClient для C# API (Docker: порт 5000).</summary>
public class ApiClient
{
    private const string BaseUrl = "http://localhost:5000/api/";

    private readonly HttpClient _http = new() { BaseAddress = new Uri(BaseUrl) };

    public async Task<List<Dictionary<string, string>>> ListAsync(string path)
    {
        var response = await _http.GetAsync(path);
        await EnsureSuccess(response);

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return doc.RootElement.EnumerateArray().Select(ToRow).ToList();
    }

    public async Task CreateAsync(string path, Dictionary<string, object?> body) =>
        await EnsureSuccess(await _http.PostAsJsonAsync(path, body));

    public async Task UpdateAsync(string path, int id, Dictionary<string, object?> body) =>
        await EnsureSuccess(await _http.PutAsJsonAsync($"{path}/{id}", body));

    public async Task DeleteAsync(string path, int id) =>
        await EnsureSuccess(await _http.DeleteAsync($"{path}/{id}"));

    // JSON-объект -> словарь "имя поля -> текст"
    private static Dictionary<string, string> ToRow(JsonElement element) =>
        element.EnumerateObject().ToDictionary(
            p => p.Name,
            p => p.Value.ValueKind == JsonValueKind.Null ? "" : p.Value.ToString());

    private static async Task EnsureSuccess(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return;
        var text = await response.Content.ReadAsStringAsync();
        if (text.Length > 300) text = text[..300];
        throw new HttpRequestException($"{(int)response.StatusCode} {response.ReasonPhrase}\n{text}");
    }
}
