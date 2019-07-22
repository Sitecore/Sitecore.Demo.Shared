﻿namespace Sitecore.Demo.Feature.Demo.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Sitecore.Analytics;
    using Sitecore.Demo.Feature.Demo.Models;
    using Sitecore.Demo.Feature.Demo.Services;
    using Sitecore.Demo.Foundation.DependencyInjection;

    [Service]
    public class ProfileRepository
    {
        private readonly IProfileProvider profileProvider;

        public ProfileRepository(IProfileProvider profileProvider)
        {
            this.profileProvider = profileProvider;
        }

        public IEnumerable<Profile> GetProfiles(ProfilingTypes profiling)
        {
            if (!Tracker.IsActive)
            {
                return Enumerable.Empty<Profile>();
            }

            return this.profileProvider.GetSiteProfiles().Where(p => this.profileProvider.HasMatchingPattern(p, profiling)).Select(x => new Profile
            {
                Name = x.NameField,
                PatternMatches = this.profileProvider.GetPatternsWithGravityShare(x, profiling)
            });
        }
    }
}