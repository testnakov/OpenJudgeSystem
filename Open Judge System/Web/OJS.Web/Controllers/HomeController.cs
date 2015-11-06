﻿namespace OJS.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;

    using OJS.Data;
    using OJS.Web.Common.Extensions;
    using OJS.Web.ViewModels.Home.Index;

    public class HomeController : BaseController
    {
        public HomeController(IOjsData data)
            : base(data)
        {
        }

        public ActionResult Index()
        {
            var isAdmin = this.User.IsAdmin();
            var userId = this.UserProfile?.Id;

            var indexViewModel = new IndexViewModel();
            indexViewModel.ActiveContests =
                this.Data.Contests.AllActive()
                    .OrderByDescending(x => x.StartTime)
                    .Select(HomeContestViewModel.FromContest)
                    .ToList();

            indexViewModel.FutureContests =
                this.Data.Contests.All()
                    .Where(x => x.StartTime > DateTime.Now  &&
                        (x.IsVisible ||
                            (x.Lecturers.Any(l => l.LecturerId == userId) || isAdmin)))
                    .OrderBy(x => x.StartTime)
                    .Select(HomeContestViewModel.FromContest)
                    .ToList();

            indexViewModel.PastContests =
                this.Data.Contests.AllPast()
                    .OrderByDescending(x => x.StartTime)
                    .Select(HomeContestViewModel.FromContest)
                    .Take(5)
                    .ToList();

            return this.View(indexViewModel);
        }

        /// <summary>
        /// Gets the robots.txt file.
        /// </summary>
        /// <returns>Returns a robots.txt file.</returns>
        [HttpGet]
        [OutputCache(Duration = 3600)]
        public FileResult RobotsTxt()
        {
            var robotsTxtContent = new StringBuilder();
            robotsTxtContent.AppendLine("User-Agent: *");
            robotsTxtContent.AppendLine("Allow: /");

            return this.File(Encoding.ASCII.GetBytes(robotsTxtContent.ToString()), "text/plain");
        }
    }
}