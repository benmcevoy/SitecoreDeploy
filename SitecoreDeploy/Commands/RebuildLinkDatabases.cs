using Sitecore.Diagnostics;

namespace SitecoreDeploy.Commands
{
    public class RebuildLinkDatabases : SitecoreDeployCommand
    {
        public RebuildLinkDatabases() : base("RebuildLinkDatabases") { }

        public override SitecoreDeployCommandArguments Execute(SitecoreDeployCommandArguments args)
        {
            Assert.ArgumentNotNull(args, nameof(args));

            args.Result = "";

            if (args.Context.ContainsKey("arg"))
            {
                var databaseName = args.Context["arg"];
                Sitecore.Globals.LinkDatabase.Rebuild(Sitecore.Configuration.Factory.GetDatabase(databaseName));

                args.Result = $"SitecoreDeploy: Rebuilt link database for '{databaseName}'";

                return args;
            }

            Sitecore.Globals.LinkDatabase.Rebuild(Sitecore.Configuration.Factory.GetDatabase("core"));
            args.Result += "SitecoreDeploy: Rebuilt link database for 'core'";

            Sitecore.Globals.LinkDatabase.Rebuild(Sitecore.Configuration.Factory.GetDatabase("master"));
            args.Result += "SitecoreDeploy: Rebuilt link database for 'master'";

            Sitecore.Globals.LinkDatabase.Rebuild(Sitecore.Configuration.Factory.GetDatabase("web"));
            args.Result += "SitecoreDeploy: Rebuilt link database for 'web'";

            return args;
        }
    }
}