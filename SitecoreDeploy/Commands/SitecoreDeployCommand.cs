namespace SitecoreDeploy.Commands
{
    public abstract class SitecoreDeployCommand 
    {
        protected SitecoreDeployCommand(string commandName)
        {
            CommandName = commandName;
        }

        public string CommandName { get; }

        public abstract SitecoreDeployCommandArguments Execute(SitecoreDeployCommandArguments args);
    }
}
