using System;

namespace Anomaleye
{
    public interface IClock
    {
        DateTime UtcNow { get; }
    }
}