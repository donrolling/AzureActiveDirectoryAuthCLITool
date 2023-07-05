using AzureActiveDirectoryAuthCLITool.Models;
using Microsoft.Identity.Client;

namespace AzureActiveDirectoryAuthCLITool.Services
{
	public class MicrosoftIdentityService
	{
		private readonly AzureAd _azureAd;

		public MicrosoftIdentityService(AzureAd azureAd)
		{
			_azureAd = azureAd;
		}

		public async Task<AuthenticationResult> GetAuthenticationResultAsync()
		{
			var confidentialClientApplication = GetConfidentialClientApplication(_azureAd);
			var authenticationResult = await confidentialClientApplication
				.AcquireTokenForClient(_azureAd.Scopes)
				.WithForceRefresh(true)
				.ExecuteAsync();
			return authenticationResult;
		}

		private IConfidentialClientApplication GetConfidentialClientApplication(AzureAd azureAD)
		{
			var authority = $"{azureAD.Instance}{azureAD.TenantId}";
			return ConfidentialClientApplicationBuilder
				.Create(azureAD.ClientId)
				.WithClientSecret(azureAD.ClientSecret)
				.WithAuthority(new Uri(authority))
				.Build();
		}
	}
}