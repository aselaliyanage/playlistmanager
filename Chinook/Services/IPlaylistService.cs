using Chinook.Models;

namespace Chinook.Services
{
    public interface IPlaylistService
    {
        Task<List<ClientModels.Playlist>> GetCurrentUserPlaylistsAsync(string currentUserId);
        Task<ClientModels.Playlist> GetPlayListAsyn(string currentUserId, long playListId);
        Task<bool> CreateNewPlaylistAsync(Models.Playlist playlist, string userId);
        Task<(bool, long)> AddTrackToPlaylistAsync(string newPlaylistName, long playlistId, long trackId, string userId);
        Task<bool> RemoveTrackFromPlaylist(long playlistId, long trackId);
        Task<bool> RenamePlaylist(long playlistId, string newPlaylistname);
        Task<bool> RemovePlaylist(long playlistId);
        Task<ClientModels.Playlist> GetFavoritesPlaylistOfUser(string userId);
        Task<bool> CreateFavoritesPlaylistAndAddTrack(long trackId, string userId);
        Task<bool> AddTrackToFavoritesPlaylist(long favoritesPlaylistId, long trackId, string userId);
        Task<bool> RemoveTrackFromFavoritesPlaylist(long favoritesPlaylistId, long trackId, string userId);
    }
}