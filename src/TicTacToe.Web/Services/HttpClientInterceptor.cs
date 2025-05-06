using System.Net.Http.Json;
using Microsoft.JSInterop;

namespace TicTacToe.Web.Services;

internal sealed class HttpClientInterceptorService : DelegatingHandler
{
    private readonly IJSRuntime _js;

    public HttpClientInterceptorService(IJSRuntime Js)
    {
        _js = Js;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        HttpResponseMessage response;

        try
        {
            response = await base.SendAsync(request, cancellationToken);
        }
        catch
        {
            Console.WriteLine("error catch");
            await _js.InvokeVoidAsync("showBootstrapAlert", cancellationToken, "error catch");
            throw;
        }

        if (!response.IsSuccessStatusCode)
        {
            try
            {
                var problem = await response.Content.ReadFromJsonAsync<ClientProblemDetails>(cancellationToken);

                if (problem is not null)
                {
                    Console.WriteLine(problem.Title);
                    await _js.InvokeVoidAsync("showBootstrapAlert", cancellationToken, problem.Title);
                }
            }
            catch (Exception e)
            {
                await _js.InvokeVoidAsync("showBootstrapAlert", cancellationToken, e.Message);
            }
        }

        return response;
    }
}
