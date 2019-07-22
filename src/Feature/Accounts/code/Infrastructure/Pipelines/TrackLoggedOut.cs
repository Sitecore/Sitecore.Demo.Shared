using Sitecore.Demo.Feature.Accounts.Services;
using Sitecore.Demo.Foundation.Accounts.Pipelines;

namespace Sitecore.Demo.Feature.Accounts.Infrastructure.Pipelines
{                                                                   
    public class TrackLoggedOut
    {
        private readonly IAccountTrackerService _accountTrackerService;

        public TrackLoggedOut(IAccountTrackerService accountTrackerService)
        {
            _accountTrackerService = accountTrackerService;
        }

        public void Process(AccountsPipelineArgs args)
        {
            _accountTrackerService.TrackLogout(args.UserName);
        }
    }
}