using System;
using tl.employersupport.ecrm.poc.application.Interfaces;

namespace tl.employersupport.ecrm.poc.application.Services
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
        public DateTime MinValue => DateTime.MinValue;
        public DateTime MaxValue => DateTime.MaxValue;
        public DateTime Today => DateTime.Today;
    }
}
