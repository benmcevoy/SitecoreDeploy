namespace SitecoreDeploy.Security
{
    public class SecurityState
    {
        public SecurityState(bool allowed)
        {
            IsAllowed = allowed;
        }

        public bool IsAllowed { get; }
    }
}