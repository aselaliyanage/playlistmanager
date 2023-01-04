using Chinook.Models;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services
{
    public class ArtistService : IArtistService
    {
        private readonly IDbContextFactory<ChinookContext> _appDbContext;

        public ArtistService(IDbContextFactory<ChinookContext> appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Artist> GetArtistAsync(long id)
        {
            using var dbContext = await _appDbContext.CreateDbContextAsync();
            return dbContext.Artists.SingleOrDefault(a => a.ArtistId == id);
        }

        public async Task<List<Artist>> GetAllArtistsAsync()
        {
            using var dbContext = await _appDbContext.CreateDbContextAsync();
            return dbContext.Artists.ToList();
        }

        public async Task<List<Album>> GetAlbumsOfArtist(int artistId)
        {
            using var dbContext = await _appDbContext.CreateDbContextAsync();
            return dbContext.Albums.Where(a => a.ArtistId == artistId).ToList();
        }
    }
}
