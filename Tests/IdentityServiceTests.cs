using AzureActiveDirectoryAuthCLITool.Models;
using AzureActiveDirectoryAuthCLITool.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Tests
{
	[TestClass]
	public class IdentityServiceTests
	{
		private AzureAd _azureAd;
		private MicrosoftIdentityService _microsoftIdentityService;
		private ILogger<IdentityServiceTests> _logger;

		public IdentityServiceTests()
		{
			var host = ConfigureServices();
			var scope = host.Services.CreateScope();
			var services = scope.ServiceProvider;
			_logger = services.GetRequiredService<ILogger<IdentityServiceTests>>();
			_microsoftIdentityService = new MicrosoftIdentityService(_azureAd);
		}

		private IHost ConfigureServices(string baseDirectory = "")
		{
			Log.Logger = new LoggerConfiguration().CreateLogger();

			return Host.CreateDefaultBuilder()
				.ConfigureAppConfiguration((context, builder) =>
				{
					var path = $"{baseDirectory}appsettings.json";
					builder.AddJsonFile(path, optional: false, reloadOnChange: true);
				})
				.ConfigureServices((hostContext, services) =>
				{
					services.Configure<AzureAd>(hostContext.Configuration.GetSection(nameof(AzureAd)));
					_azureAd = hostContext.Configuration.GetSection(nameof(AzureAd)).Get<AzureAd>();
				})
				.UseSerilog((hostContext, loggerConfiguration) =>
				{
					loggerConfiguration.ReadFrom.Configuration(hostContext.Configuration);
				})
			   .Build();
		}

		[TestMethod]
		public async Task GivenValidCreds_UserGetsValidToken()
		{
			var result = await _microsoftIdentityService.GetAuthenticationResultAsync();
			Assert.IsNotNull(result);
			Assert.IsFalse(string.IsNullOrWhiteSpace(result.AccessToken));
			Assert.IsTrue(result.ExpiresOn.UtcDateTime > DateTime.UtcNow);
		}
	}
}