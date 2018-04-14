using Sitecore.Diagnostics;

namespace SitecoreDeploy.Commands
{
    public class RebuildIndexes : SitecoreDeployCommand
    {
        public RebuildIndexes() : base("RebuildIndexes") { }

        public override SitecoreDeployCommandArguments Execute(SitecoreDeployCommandArguments args)
        {
            Assert.ArgumentNotNull(args, nameof(args));

            args.Result = "";

            if (args.Context.ContainsKey("arg"))
            {
                var indexName = args.Context["arg"];
                var index = Sitecore.ContentSearch.ContentSearchManager.GetIndex(indexName);

                index.Rebuild(Sitecore.ContentSearch.IndexingOptions.ForcedIndexing);

                args.Result = $"SitecoreDeploy: Rebuilt index with name '{indexName}'";

                return args;
            }

            foreach (var index in Sitecore.ContentSearch.ContentSearchManager.Indexes)
            {
                index.Rebuild(Sitecore.ContentSearch.IndexingOptions.ForcedIndexing);

                args.Result += $"SitecoreDeploy: Rebuilt index with name '{index.Name}'\r\n";
            }

            return args;
        }
    }
}