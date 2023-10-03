using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Net.Http;
using System.Threading.Tasks;

public class HttpServer
{
    public void Start()
    {
        var host = new WebHostBuilder()
            .UseKestrel()
            .Configure(app => app.Run(HandleRequest))
            .Build();
        host.Run();
    }

    private async Task HandleRequest(HttpContext context)
    {
        await context.Response.WriteAsync("Result: [OK]");
    }
}
