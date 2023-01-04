using Chinook.ClientModels;
using Chinook.Models;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services
{
    public class TracksService : ITracksService
    {
        private readonly IDbContextFactory<ChinookContext> _appDbContext;

        public TracksService(IDbContextFactory<ChinookContext> appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<ClientModels.PlaylistTrack>> GetTracksOfArtist(long artistId, string currentUserId)
        {
            using var dbContext = await _appDbContext.CreateDbContextAsync();

            return dbContext.Tracks.Where(a => a.Album.ArtistId == artistId)
            .Include(a => a.Album)
            .Select(t => new ClientModels.PlaylistTrack()
            {
                AlbumTitle = (t.Album == null ? "-" : t.Album.Title),
                TrackId = t.TrackId,
                TrackName = t.Name,
                IsFavorite = t.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == currentUserId && up.Playlist.Name == "Favorites")).Any()
            })
            .ToList();
        }
    }
}
