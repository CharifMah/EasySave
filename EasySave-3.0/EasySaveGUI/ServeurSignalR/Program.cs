using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ServeurSignalR
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers(
                options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true).AddNewtonsoftJson();

            builder.Services.AddSignalR();

            var app = builder.Build();

            //// Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}

            app.UseRouting();
            app.MapControllers();

            //Map Hubs SignalR with endpoints
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<LobbyHub>("/JurrasicRisk/LobbyHub");
                endpoints.MapHub<PartieHub>("/JurrasicRisk/PartieHub");
            });

            app.Run();
        }       
    }
}