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
    }
}
