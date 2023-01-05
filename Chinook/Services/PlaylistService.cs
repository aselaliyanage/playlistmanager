using Chinook.ClientModels;
using Chinook.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Internal;

namespace Chinook.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IDbContextFactory<ChinookContext> _appDbContext;

        public PlaylistService(IDbContextFactory<ChinookContext> appDbContextFactory)
        {
            _appDbContext = appDbContextFactory;
        }

        public async Task<List<ClientModels.Playlist>> GetCurrentUserPlaylistsAsync(string currentUserId)
        {
            using var dbContext = await _appDbContext.CreateDbContextAsync();
            
            return dbContext.UserPlaylists
                .Include(x => x.Playlist)
                .Where(x => x.UserId == currentUserId && x.Playlist.Name != "Favorites")
                .Select(x => new ClientModels.Playlist
                {
                    Id = x.PlaylistId,
                    Name = x.Playlist.Name!
                }).ToList();
        }

        public async Task<ClientModels.Playlist> GetFavoritesPlaylistOfUser(string userId)
        {
            using var dbContext = await _appDbContext.CreateDbContextAsync();
            var favoritesPlaylist = dbContext.UserPlaylists
                .Include(x => x.Playlist)
                .Where(x => x.UserId == userId && x.Playlist.Name == "Favorites")
                .Select(x => new ClientModels.Playlist
                {
                    Id = x.PlaylistId,
                    Name = x.Playlist.Name!,
                    Tracks = x.Playlist.Tracks.Select(pt => new PlaylistTrack
                    {
                        AlbumTitle = pt.Album.Title,
                        ArtistName = pt.Album.Artist.Name,
                        TrackId = pt.TrackId,
                        TrackName = pt.Name,
                        IsFavorite = true
                    }).ToList()
                }).FirstOrDefault();
            return favoritesPlaylist;
        }

        public async Task<ClientModels.Playlist> GetPlayListAsyn(string currentUserId, long playListId)
        {
            using var dbContext = await _appDbContext.CreateDbContextAsync();
            var playlist = dbContext.Playlists
            .Include(a => a.Tracks).ThenInclude(a => a.Album).ThenInclude(a => a.Artist)
            .Where(p => p.PlaylistId == playListId)
            .Select(p => new ClientModels.Playlist()
            {
                Name = p.Name,
                Tracks = p.Tracks.Select(t => new ClientModels.PlaylistTrack()
                {
                    AlbumTitle = t.Album.Title,
                    ArtistName = t.Album.Artist.Name,
                    TrackId = t.TrackId,
                    TrackName = t.Name,
                    IsFavorite = t.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == currentUserId && up.Playlist.Name == "Favorites")).Any()
                }).ToList()
            })
            .FirstOrDefault();

            return playlist;
        }        

        public async Task<bool> CreateNewPlaylistAsync(Models.Playlist playlist, string userId)
        {
            try
            {
                using var dbContext = await _appDbContext.CreateDbContextAsync();
                await CreateNewPlaylistAsync(playlist, userId, dbContext);
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private async Task<bool> CreateNewPlaylistAsync(Models.Playlist playlist, string userId, ChinookContext dbContext)
        {
            await CreatePlaylistAsync(playlist, dbContext);
            await dbContext.UserPlaylists.AddAsync(new UserPlaylist() { PlaylistId = playlist.PlaylistId, UserId = userId });
            return true;
        }
        private static async Task<long> CreatePlaylistAsync(Models.Playlist playlist, ChinookContext dbContext)
        {
            var maxId = dbContext.Playlists.OrderByDescending(x => x.PlaylistId).FirstOrDefault()?.PlaylistId;
            playlist.PlaylistId = maxId == null ? 1 : (long)++maxId;

            await dbContext.Playlists.AddAsync(playlist);
            return playlist.PlaylistId;
        }

        public async Task<bool> CreateFavoritesPlaylistAndAddTrack(long trackId, string userId)
        {
            using var dbContext = await _appDbContext.CreateDbContextAsync();
            var track = dbContext.Tracks.Where(x => x.TrackId == trackId).First();
            var favoritePlaylist = new Models.Playlist()
            { 
                Name = "Favorites", 
                Tracks = new List<Track>() { track }
            };
            await CreateNewPlaylistAsync(favoritePlaylist, userId, dbContext);
            dbContext.SaveChanges();
            return true;
        }

        public async Task<bool> AddTrackToFavoritesPlaylist(long favoritesPlaylistId, long trackId, string userId)
        {
            using var dbContext = await _appDbContext.CreateDbContextAsync();
            var track = dbContext.Tracks.Where(x => x.TrackId == trackId).First();
            var playListInDb = dbContext.Playlists.Include(x => x.Tracks).AsTracking().Where(x => x.PlaylistId == favoritesPlaylistId).FirstOrDefault();
            if (playListInDb?.Tracks != null)
            {
                playListInDb.Tracks.Add(track);
            }
            else
            {
                playListInDb!.Tracks = new List<Track> { track };
            }
            dbContext.SaveChanges();
            return true;
        }

        public async Task<(bool, long)> AddTrackToPlaylistAsync(string newPlaylistName, long playlistId, long trackId, string userId)
        {
            var isNewPlaylist = !string.IsNullOrEmpty(newPlaylistName);
            var playList = new Models.Playlist() { Name = newPlaylistName,  };
            long newPlaylistId = 0;
            bool isTrackAdded = false;

            try
            {
                using var dbContext = await _appDbContext.CreateDbContextAsync();
                var track = dbContext.Tracks.Where(x => x.TrackId == trackId).First();

                if (isNewPlaylist) // If the track is added to a new playlist
                {
                    var newPlayList = new Models.Playlist()
                    {
                        Name = newPlaylistName, 
                        Tracks = new List<Track>() { track },
                    };
                    await CreateNewPlaylistAsync(newPlayList, userId, dbContext);
                    newPlaylistId = newPlayList.PlaylistId;
                }
                else // If the track is added to an existing playlist
                {
                    var playListInDb = dbContext.Playlists.Include(x => x.Tracks).AsTracking().Where(x => x.PlaylistId == playlistId).FirstOrDefault();
                    if (playListInDb?.Tracks != null)
                    {
                        playListInDb.Tracks.Add(track);
                    }
                    else
                    {
                        playListInDb!.Tracks = new List<Track> { track };
                    }
                }

                dbContext.SaveChanges();
                isTrackAdded = true;
            }
            catch (Exception e)
            {
                return (isTrackAdded, 0);
            }

            return (isTrackAdded, newPlaylistId);
        }

        public async Task<bool> RemoveTrackFromPlaylist(long playlistId, long trackId)
        {
            try
            {
                using var dbContext = await _appDbContext.CreateDbContextAsync();
                var playListInDb = dbContext.Playlists.Include(x => x.Tracks).AsTracking().Where(x => x.PlaylistId == playlistId).FirstOrDefault();
                if(playListInDb != null)
                {
                    var track = playListInDb?.Tracks?.First(x => x.TrackId == trackId);
                    playListInDb!.Tracks.Remove(track!);
                    dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> RenamePlaylist(long playlistId, string newPlaylistname)
        {
            using var dbContext = await _appDbContext.CreateDbContextAsync();
            var playListInDb = dbContext.Playlists.Include(x => x.Tracks).AsTracking().Where(x => x.PlaylistId == playlistId).FirstOrDefault();
            if(playListInDb != null)
            {
                playListInDb.Name = newPlaylistname;
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<bool> RemovePlaylist(long playlistId)
        {
            using var dbContext = await _appDbContext.CreateDbContextAsync();
            var playListInDb = dbContext.Playlists.Include(x => x.Tracks).AsTracking().Where(x => x.PlaylistId == playlistId).FirstOrDefault();
            if (playListInDb != null)
            {
                dbContext.Playlists.Remove(playListInDb);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }
    }


}
