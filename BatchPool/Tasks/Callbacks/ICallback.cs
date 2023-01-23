namespace BatchPool
{
    internal interface ICallback
    {
        Task RunCallbackIfPresent();
        Task WaitForCallbackIfRequired();
    }
}
