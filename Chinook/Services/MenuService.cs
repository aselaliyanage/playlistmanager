namespace Chinook.Services
{
    public class NavMenuStateService
    {
        public event Action<long, string> OnChange;

        public void MenuChanged(long newPlaylistId, string newPlaylistName) => OnChange?.Invoke(newPlaylistId, newPlaylistName);
    }
}
