using System;

namespace tl.employersupport.ecrm.poc.application.Interfaces
{
    public interface IDateTimeService
    {
        DateTime Now { get; }
        DateTime UtcNow { get; }
        DateTime MinValue { get; }
        DateTime MaxValue { get; }
        DateTime Today { get; }
    }
}
