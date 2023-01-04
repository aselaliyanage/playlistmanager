using Chinook.Models;

namespace Chinook.Services
{
    public interface IArtistService
    {
        Task<Artist> GetArtistAsync(long id);
    }
}