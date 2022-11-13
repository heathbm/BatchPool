namespace BatchPool.Tasks.Callbacks
{
    internal interface ICallback
    {
        Task RunCallbackIfPresent();
        Task WaitForCallbackIfRequired();
    }
}
