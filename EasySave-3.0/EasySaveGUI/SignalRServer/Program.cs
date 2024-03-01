using SignalRServer.Hubs;

namespace SignalRServer
{
    class Program
    {
        static void Main(string[] args)
        {
            RunServer(args);
        }

        private static void RunServer(string[] pArgs)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(pArgs);

            builder.Services.AddSignalR();

            var app = builder.Build();

            app.UseRouting();

            //Map Hubs SignalR with endpoints
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<UserHub>("/UserHub");
            });

            app.Run();
        }
    }
}
