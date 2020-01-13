﻿using System.Net;
using System.Web.Mvc;
using Sitecore.Analytics;
using Sitecore.Demo.Shared.Feature.Demo.Models;
using Sitecore.Demo.Shared.Feature.Demo.Services;
using Sitecore.Demo.Shared.Foundation.Alerts.Exceptions;
using Sitecore.Demo.Shared.Foundation.SitecoreExtensions.Attributes;
using Sitecore.ExperienceEditor.Utils;
using Sitecore.ExperienceExplorer.Core.State;
using Sitecore.Mvc.Controllers;
using Sitecore.Mvc.Presentation;
using Sitecore.Sites;

namespace Sitecore.Demo.Shared.Feature.Demo.Controllers
{
    [SkipAnalyticsTracking]
    public class DemoController : SitecoreController
    {
        private IDemoStateService DemoStateService { get; }
        private IExperienceDataFactory ExperienceDataFactory { get; }

        public DemoController(IDemoStateService demoStateService, IExperienceDataFactory experienceDataFactory)
        {
            this.DemoStateService = demoStateService;
            this.ExperienceDataFactory = experienceDataFactory;
        }

        public ActionResult ExperienceData()
        {
            if (Tracker.Current == null || Tracker.Current.Interaction == null || !this.DemoStateService.IsDemoEnabled)
            {
                return null;
            }
            var explorerContext = DependencyResolver.Current.GetService<IExplorerContext>();
            var isInExperienceExplorer = explorerContext?.IsExplorerMode() ?? false;
            if (Context.Site.DisplayMode != DisplayMode.Normal || WebEditUtility.IsDebugActive(Context.Site) || isInExperienceExplorer)
            {
                return new EmptyResult();
            }

            var experienceData = this.ExperienceDataFactory.Get();
            return this.View(experienceData);
        }

        public ActionResult ExperienceDataContent()
        {
            var experienceData = this.ExperienceDataFactory.Get();
            return this.View("_ExperienceDataContent", experienceData);
        }

        public ActionResult DemoContent()
        {
            var item = RenderingContext.Current?.Rendering?.Item ?? RenderingContext.Current?.ContextItem;
            if (item == null || !item.DescendsFrom(Templates.DemoContent.ID))
            {
                throw new InvalidDataSourceItemException($"Item should be not null and derived from {nameof(Templates.DemoContent)} {Templates.DemoContent.ID} template");
            }

            var demoContent = new DemoContent(item);
            return this.View("DemoContent", demoContent);
        }

        public ActionResult EndVisit()
        {
            this.Session.Abandon();
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}