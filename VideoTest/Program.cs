using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using VideoTest.Components;
using VideoTest.Services;
using AuthenticationHeaderValue = System.Net.Http.Headers.AuthenticationHeaderValue;

namespace VideoTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorComponents();

            builder.Services.AddOptions<ApiOptions>()
                .BindConfiguration(ApiOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddHttpClient<IStreamApiService, StreamApiService>((sp, client) => 
            {
                var options = sp.GetRequiredService<IOptions<ApiOptions>>().Value;

                client.BaseAddress = new Uri(options.Url);

                var passwordBytes = Encoding.UTF8.GetBytes(options.Password);
                var passwordHash = MD5.HashData(passwordBytes);
                string md5PassHash = Convert.ToHexString(passwordHash).ToLower();

                var authBytes = Encoding.UTF8.GetBytes($"{options.Login}:{md5PassHash}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));

            });

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapGet("/stream", async (Guid channelId, HttpContext context, IStreamApiService streamService, ILogger<Program> logger) =>
            {
                if (channelId == Guid.Empty)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }

                try
                {
                    using var response = await streamService.OpenStreamAsync(channelId, context.RequestAborted);

                    if (!response.IsSuccessStatusCode)
                    {
                        logger.LogWarning("Сервер вернул статус {StatusCode}", (int)response.StatusCode);

                        context.Response.StatusCode = (int)response.StatusCode;
                        return;
                    }

                    context.Features.Get<IHttpResponseBodyFeature>()?.DisableBuffering();
                    context.Response.Headers.CacheControl = "no-store, no-cache";
                    context.Response.ContentType = response.Content.Headers.ContentType?.ToString() ?? "multipart/x-mixed-replace; boundary=myboundary";

                    using var stream = await response.Content.ReadAsStreamAsync(context.RequestAborted);
                    await stream.CopyToAsync(context.Response.Body, context.RequestAborted);
                }
                catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
                {
                    // Сюда код попадает при закрытии вкладки. Это не ошибка
                }
                catch(Exception ex)
                {
                    logger.LogWarning(ex, "Ошибка при передаче видеопотока");

                    if (!context.Response.HasStarted)
                    {
                        context.Response.StatusCode = StatusCodes.Status502BadGateway;
                    }
                }

            });

            app.MapRazorComponents<App>();

            app.Run();
        }
    }
}
