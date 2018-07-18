using System.Globalization;
using System.Threading.Tasks;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Marketing.Definitions.AutomationPlans.Model;
using Sitecore.Marketing.Definitions.Campaigns;
using Sitecore.Marketing.Definitions.Events;
using Sitecore.Marketing.Definitions.Funnels;
using Sitecore.Marketing.Definitions.Goals;
using Sitecore.Marketing.Definitions.MarketingAssets;
using Sitecore.Marketing.Definitions.Outcomes.Model;
using Sitecore.Marketing.Definitions.PageEvents;
using Sitecore.Marketing.Definitions.Profiles;
using Sitecore.Marketing.Definitions.Segments;
using Sitecore.Marketing.xMgmt.Pipelines.DeployDefinition;

namespace SitecoreDeploy.Commands
{
    public class DeployMarketingDefinitions : SitecoreDeployCommand
    {
        public DeployMarketingDefinitions() : base("DeployMarketingDefinitions") { }

        public override SitecoreDeployCommandArguments Execute(SitecoreDeployCommandArguments args)
        {
            Assert.ArgumentNotNull(args, nameof(args));

            args.Result = "";

            DeployAll().Wait();

            args.Result = "SitecoreDeploy: DeployMarketingDefinitions completed";

            return args;
        }

        private async Task DeployAll()
        {
            var deploymentManager = (DeploymentManager)ServiceLocator.ServiceProvider.GetService(typeof(DeploymentManager));
            var culture = CultureInfo.InvariantCulture;

            Log.Info("Deploying definition type 'automationplans'", this);
            await deploymentManager.DeployAllAsync<IAutomationPlanDefinition>(culture);

            Log.Info("Deploying definition type 'campaigns'", this);
            await deploymentManager.DeployAllAsync<ICampaignActivityDefinition>(culture);

            Log.Info("Deploying definition type 'events'", this);
            await deploymentManager.DeployAllAsync<IEventDefinition>(culture);

            Log.Info("Deploying definition type 'funnels'", this);
            await deploymentManager.DeployAllAsync<IFunnelDefinition>(culture);

            Log.Info("Deploying definition type 'goals'", this);
            await deploymentManager.DeployAllAsync<IGoalDefinition>(culture);

            Log.Info("Deploying definition type 'marketingassets'", this);
            await deploymentManager.DeployAllAsync<IMarketingAssetDefinition>(culture);

            Log.Info("Deploying definition type 'outcomes'", this);
            await deploymentManager.DeployAllAsync<IOutcomeDefinition>(culture);

            Log.Info("Deploying definition type 'pageevents'", this);
            await deploymentManager.DeployAllAsync<IPageEventDefinition>(culture);

            Log.Info("Deploying definition type 'profiles'", this);
            await deploymentManager.DeployAllAsync<IProfileDefinition>(culture);

            Log.Info("Deploying definition type 'segments'", this);
            await deploymentManager.DeployAllAsync<ISegmentDefinition>(culture);
        }
    }
}