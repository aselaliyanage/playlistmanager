using Chinook.ClientModels;

namespace Chinook.Services
{
    public interface ITracksService
    {
        Task<List<PlaylistTrack>> GetTracksOfArtist(long artistId, string currentUserId);
    }
}