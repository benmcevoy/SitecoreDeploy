using System;
using Sitecore.Data;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Diagnostics.PerformanceCounters;
using Sitecore.Publishing;
using Sitecore.Publishing.Diagnostics;

namespace SitecoreDeploy.Commands
{
    public class Publish : SitecoreDeployCommand
    {
        public Publish() : base("Publish") { }

        public override SitecoreDeployCommandArguments Execute(SitecoreDeployCommandArguments args)
        {
            Assert.ArgumentNotNull(args, nameof(args));

            args.Result = "";

            var master = Database.GetDatabase("master");
            var web = Database.GetDatabase("web");
            var targets = new[] { web };
            var mode = (args["arg"] ?? "smart").ToLowerInvariant();
            var languages = LanguageManager.GetLanguages(master).ToArray();

            switch (mode)
            {
                case "smart":
                    {

                        PublishManager.PublishSmart(master, targets, languages);
                        JobsCount.TasksPublishings.Increment();
                        args.Result = "SitecoreDeploy: Started Smart Publish";
                        break;
                    }
                case "full":
                    {
                        PublishingLog.Info("DeployService.PublishService Started Full Publish");
                        foreach (var l in languages)
                        {
                            PublishManager.Publish(new[]
                                {new PublishOptions(master, web, PublishMode.Full, l, DateTime.Now)});
                            JobsCount.TasksPublishings.Increment();
                            args.Result += $"SitecoreDeploy: Started Full Publish for '{l}'\r\n";
                        }

                        break;
                    }
                case "incremental":
                    {
                        PublishingLog.Info("DeployService.PublishService Started Incremental Publish");
                        PublishManager.PublishIncremental(master, targets, languages);
                        JobsCount.TasksPublishings.Increment();
                        args.Result = "SitecoreDeploy: Started Incremental Publish";
                        break;
                    }
                default: throw new ArgumentException($"unknown mode '{mode}'");
            }

            return args;
        }
    }
}