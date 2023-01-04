namespace Chinook.Services
{
    public class NavMenuStateService
    {
        public Func<Task> OnChange;

        public async Task MenuChanged() => await OnChange!.Invoke();
    }
}
