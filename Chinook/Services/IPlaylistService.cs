using Chinook.ClientModels;

namespace Chinook.Services
{
    public interface IPlaylistService
    {
        Task<List<Playlist>> GetCurrentUserPlaylistsAsync();
        Task<Playlist> GetPlayListAsyn(string currentUserId, long playListId);
        Task<bool> CreateNewPlaylistAsync(Models.Playlist playlist, string userId);
        Task<(bool, long)> AddTrackToPlaylistAsync(string newPlaylistName, long playlistId, long trackId, string userId);
    }
}