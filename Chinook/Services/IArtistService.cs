using Chinook.Models;

namespace Chinook.Services
{
    public interface IArtistService
    {
        Task<Artist> GetArtistAsync(long id);
        Task<List<Artist>> GetAllArtistsAsync();
        Task<List<Album>> GetAlbumsOfArtist(int artistId);
    }
}