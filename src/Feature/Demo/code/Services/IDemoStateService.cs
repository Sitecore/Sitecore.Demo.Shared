using System;

namespace Sitecore.Demo.Feature.Demo.Services
{
    public interface IDemoStateService
    {
        bool IsDemoEnabled { get; }
    }
}
