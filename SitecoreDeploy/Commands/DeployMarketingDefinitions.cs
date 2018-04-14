using Sitecore.Diagnostics;

namespace SitecoreDeploy.Commands
{
    public class DeployMarketingDefinitions : SitecoreDeployCommand
    {
        public DeployMarketingDefinitions() : base("DeployMarketingDefinitions") { }

        public override SitecoreDeployCommandArguments Execute(SitecoreDeployCommandArguments args)
        {
            Assert.ArgumentNotNull(args, nameof(args));

            args.Result = "SitecoreDeploy: DeployMarketingDefinitions is not implemented yet!!";
            // TODO:

            return args;
        }
    }
}