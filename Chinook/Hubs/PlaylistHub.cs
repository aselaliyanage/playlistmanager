using Microsoft.AspNetCore.SignalR;

namespace Chinook.Hubs
{
    public class PlaylistHub : Hub
    {
        public async Task NotifyPlaylistsUpdated(long id, string playlistName)
        {
            await Clients.All.SendAsync("PlaylistAdded", id, playlistName);
        }
    }
}
