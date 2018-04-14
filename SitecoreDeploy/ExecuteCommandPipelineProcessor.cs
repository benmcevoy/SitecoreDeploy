using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security;
using System.Web;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.SecurityModel;
using SitecoreDeploy.Commands;

namespace SitecoreDeploy
{
    public class ExecuteCommandPipelineProcessor : HttpRequestProcessor
    {
        private readonly string _route;

        public ExecuteCommandPipelineProcessor(string route)
        {
            _route = route;
        }

        public override void Process(HttpRequestArgs args)
        {
            if (string.IsNullOrWhiteSpace(_route)) return;

            if (!args.HttpContext.Request.RawUrl.StartsWith(_route, StringComparison.OrdinalIgnoreCase)) return;

            ProcessRequest(args.HttpContext);

            args.HttpContext.Response.End();
        }

        private void ProcessRequest(HttpContextBase context)
        {
            if (string.IsNullOrWhiteSpace(context.Request.QueryString["command"]))
                throw new InvalidOperationException("command was missing");

            context.Server.ScriptTimeout = 86400;

            context.Response.Clear();
            context.Response.ContentType = "plain/text";

            // workaround to allow streaming output without an exception in Sitecore 8.1 Update-3 and later
            context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";

            // this securitydisabler allows the control panel to execute unfettered when debug compilation is enabled but you are not signed into Sitecore
            using (new SecurityDisabler())
            {
                var args = new SitecoreDeployCommandArguments(ToDictionary(context.Request.QueryString));
                
                if (WasChallenge(context, args)) return;

                var securityState = Configuration.AuthenticationProvider.ValidateRequest(new HttpRequestWrapper(HttpContext.Current.Request));

                if (!securityState.IsAllowed) throw new SecurityException();

                DispatchCommand(context, args);
            }
        }

        private void DispatchCommand(HttpContextBase context, SitecoreDeployCommandArguments args)
        {
            var command = Configuration.Commands.FirstOrDefault(deployCommand =>
                deployCommand.CommandName.Equals(args.Command, StringComparison.OrdinalIgnoreCase));

            if (command == null) throw new InvalidOperationException($"unknown command '{args.Command}'");

            Log.Info($"SitecoreDeploy: Begin executing command '{args}'", this);

            args = command.Execute(args);

            Log.Info($"SitecoreDeploy: Finished executing command '{args}'", this);

            Write(context, args.Result);
        }

        private static bool WasChallenge(HttpContextBase context, SitecoreDeployCommandArguments args)
        {
            if (!args.Command.Equals("challenge", StringComparison.OrdinalIgnoreCase)) return false;

            Write(context, Configuration.AuthenticationProvider.GetChallengeToken());

            return true;
        }

        private static void Write(HttpContextBase context, string text)
        {
            context.Response.Write(text);
        }

        private static IDictionary<string, string> ToDictionary(NameValueCollection queryString)
        {
            var source = queryString.AllKeys.Where(s => !string.IsNullOrWhiteSpace(s));

            return source.ToDictionary(key => key, key => queryString[key]);
        }
    }
}