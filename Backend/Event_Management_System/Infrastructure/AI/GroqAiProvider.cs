using Application.Abstractions;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

public class GroqAiProvider : IAiProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GroqAiProvider> _logger;

    public GroqAiProvider(HttpClient httpClient, ILogger<GroqAiProvider> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> GetCompletionAsync(string prompt)
    {
        try
        {
            var request = new
            {
                model = "llama-3.1-8b-instant",
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = prompt
                    }
                },
                max_tokens = 1024,
                temperature = 0.7,
                top_p = 1,
                stream = false
            };

            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending request to Groq API");

            var response = await _httpClient.PostAsync("", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Groq API error: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                throw new HttpRequestException($"Groq API error: {response.StatusCode} - {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Groq API response received successfully");

            using var document = JsonDocument.Parse(responseContent);
            var choices = document.RootElement.GetProperty("choices");
            if (choices.GetArrayLength() > 0)
            {
                var message = choices[0].GetProperty("message");
                var contentText = message.GetProperty("content").GetString();
                return contentText ?? "No response content";
            }

            return "No response generated";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Groq API");
            throw;
        }
    }
}