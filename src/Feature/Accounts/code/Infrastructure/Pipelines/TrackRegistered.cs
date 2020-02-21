using Sitecore.Demo.Shared.Feature.Accounts.Services;
using Sitecore.Demo.Shared.Foundation.Accounts.Pipelines;

namespace Sitecore.Demo.Shared.Feature.Accounts.Infrastructure.Pipelines
{            
    public class TrackRegistered
    {
        private readonly IAccountTrackerService _accountTrackerService;
        private readonly IUserProfileService _userProfileService;

        public TrackRegistered(IAccountTrackerService accountTrackerService, IUserProfileService userProfileService)
        {
            _accountTrackerService = accountTrackerService;
            _userProfileService = userProfileService;
        }

        public void Process(AccountsPipelineArgs args)
        {
            _userProfileService.UpdateContactFacetData(args.User.Profile);
            _accountTrackerService.TrackRegistration();
        }             
    }
}