namespace NetCasbin.Persist
{
    public interface IWatcher
    {
        void SetUpdateCallback();

        void Update();
    }
}
