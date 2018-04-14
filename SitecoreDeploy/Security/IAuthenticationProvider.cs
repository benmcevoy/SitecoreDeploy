using System.Net;
using System.Web;

namespace SitecoreDeploy.Security
{
	public interface IAuthenticationProvider
	{
		string GetChallengeToken();
		SecurityState ValidateRequest(HttpRequestBase request);
		WebClient CreateAuthenticatedWebClient(string remoteUnicornUrl);
	}
}