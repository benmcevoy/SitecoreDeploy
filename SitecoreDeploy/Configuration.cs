using System;
using System.Collections.Generic;
using System.Xml;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Xml;
using SitecoreDeploy.Commands;
using SitecoreDeploy.Security;

namespace SitecoreDeploy
{
    public static class Configuration
    {
        private static readonly CommandList Instance;

        static Configuration()
        {
            Instance = (CommandList)Factory.CreateObject("/sitecore/sitecoreDeploy/commandList", true);
            AuthenticationProvider = (IAuthenticationProvider)Factory.CreateObject("/sitecore/sitecoreDeploy/authenticationProvider", true);
        }

        public static List<SitecoreDeployCommand> Commands => Instance.Commands;

        public static IAuthenticationProvider AuthenticationProvider { get; set; }
    }

    public class CommandList
    {
        public void AddCommand(XmlNode configNode)
        {
            Assert.ArgumentNotNull(configNode, nameof(configNode));

            var typeName = XmlUtil.GetAttribute("type", configNode, true);

            if (string.IsNullOrWhiteSpace(typeName)) throw new InvalidOperationException("type attribute is missing");

            Commands.Add((SitecoreDeployCommand)Activator.CreateInstance(Type.GetType(typeName)));
        }

        public List<SitecoreDeployCommand> Commands { get; set; } = new List<SitecoreDeployCommand>();
    }
}