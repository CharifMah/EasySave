using Microsoft.AspNetCore.SignalR;

namespace SignalRServer.Hubs
{
    public class UserHub : Hub
    {
        public async Task UpdateJobs(string pJobsJson)
        {
            await Clients.All.SendAsync("UpdateJobs", pJobsJson);
            Console.WriteLine("JobUpdated");
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine("User Connected");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine("User Disconnected " + exception);
            return base.OnDisconnectedAsync(exception);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
