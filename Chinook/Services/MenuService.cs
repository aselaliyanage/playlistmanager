namespace Chinook.Services
{
    public class NavMenuStateService
    {
        public Func<long, string, Task> OnChange;

        public async Task MenuChanged(long newPlaylistId, string newPlaylistName) => await OnChange!.Invoke(newPlaylistId, newPlaylistName);
    }
}
