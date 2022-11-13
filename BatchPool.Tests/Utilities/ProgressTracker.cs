using System.Threading;
using System.Threading.Tasks;

namespace BatchPool.UnitTests.Utilities
{
    internal class ProgressTracker
    {
        internal int Progress;

        internal int IncrementProgress()
        {
            return Interlocked.Increment(ref Progress);
        }

        internal async Task IncrementProgressAsync()
        {
            await Task.Delay(0);
            IncrementProgress();
        }
    }
}
