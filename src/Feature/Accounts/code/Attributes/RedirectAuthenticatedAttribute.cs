﻿using System.Web.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Demo.Shared.Feature.Accounts.Services;
using Sitecore.DependencyInjection;

namespace Sitecore.Demo.Shared.Feature.Accounts.Attributes
{
    public class RedirectAuthenticatedAttribute : ActionFilterAttribute
    {
        private readonly IGetRedirectUrlService getRedirectUrlService;

        public RedirectAuthenticatedAttribute()
        {
            this.getRedirectUrlService = ServiceLocator.ServiceProvider.GetService<IGetRedirectUrlService>();
        }


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!Context.PageMode.IsNormal)
                return;
            if (!Context.User.IsAuthenticated)
                return;
            var link = this.getRedirectUrlService.GetRedirectUrl(AuthenticationStatus.Authenticated);
            filterContext.Result = new RedirectResult(link);
        }
    }
}