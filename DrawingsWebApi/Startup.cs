using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using DI;
using DIContracts.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DrawingsWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dlls");
            var resolver = new Resolver(path, services);

            //TODO:change to dlls
            services.AddSingleton<SocketManager>();
            services.AddSingleton<DrawingShareHandler>();

            services.AddSingleton<IResolver>(sp => resolver);
            services.AddControllers();
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWebSockets(new WebSocketOptions {KeepAliveInterval = TimeSpan.FromSeconds(120)});
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        var docId = context.Request.Query["docId"];
                        var userId = context.Request.Query["userId"];
                        var manager = app.ApplicationServices.GetService<DrawingShareHandler>();
                        await manager.OnConnected(docId,userId,webSocket);

                        await Receive(webSocket, async(result, buffer) =>
                        {
                            if (result.MessageType == WebSocketMessageType.Text)
                            {
                                await manager.ReceiveAsync(webSocket, result, buffer,docId);
                                return;
                            }

                            if (result.MessageType == WebSocketMessageType.Close)
                            {
                                await manager.OnDisconnected(docId, userId);
                                return;
                            }

                        });
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();


            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 2];

            while(socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
                    cancellationToken: CancellationToken.None);

                handleMessage(result, buffer);
            }
        }
    }
}
