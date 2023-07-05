using AzureActiveDirectoryAuthCLITool.Models;
using AzureActiveDirectoryAuthCLITool.Services;
using CommandLine;
using Enterprise.Operations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

var host = ConfigureServices(AppDomain.CurrentDomain.BaseDirectory);
using (var scope = host.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var logger = services.GetRequiredService<ILogger<Program>>();
	logger.LogInformation("Azure Active Directory Auth CLI Tool");
	logger.LogInformation("------------------------------------");

	var azureAdResult = GetOptions(args, logger);

	if (azureAdResult.Failed)
	{
		logger.LogError(azureAdResult.Message);
		return;
	}

	var azureAd = azureAdResult.Result;
	try
	{
		var microsoftIdentityServiceLogger = services.GetRequiredService<ILogger<MicrosoftIdentityService>>();
		var microsoftIdentityService = new MicrosoftIdentityService(azureAd);
		var result = await microsoftIdentityService.GetAuthenticationResultAsync();
		LogTokenDetails(logger, result);
	}
	catch (Exception ex)
	{
		logger.LogError(ex.Message);
	}

	// this is required because serilog doesn't flush messages quite fast enough if you don't do this.
	Thread.Sleep(TimeSpan.FromSeconds(1));
}

static void LogTokenDetails(ILogger<Program> logger, AuthenticationResult authenticationResult)
{
	logger.LogInformation("Access Token:");
	logger.LogInformation($"bearer {authenticationResult.AccessToken}");
	logger.LogInformation("------------------------------------------");
	logger.LogInformation($"Token details.\r\nUTC: {DateTimeOffset.UtcNow} Local: {DateTimeOffset.UtcNow.LocalDateTime}\r\nUTC Expiration: {authenticationResult.ExpiresOn.UtcDateTime} Local Expiration: {authenticationResult.ExpiresOn.LocalDateTime}");
}

static OperationResult<AzureAd> GetOptions(string[] args, ILogger logger)
{
	var options = Parser.Default.ParseArguments<AzureActiveDirectoryAuthCLITool.Models.Options>(args);
	if (options.Errors.Any())
	{
		var message = string.Join(Environment.NewLine, options.Errors);
		return OperationResult.Fail<AzureAd>(message);
	}
	var commandLineArgs = Environment.GetCommandLineArgs();
	if (commandLineArgs == null)
	{
		return OperationResult.Fail<AzureAd>("Command Line Arguments are missing.");
	}
	var azureAd = new AzureAd();
	if (!string.IsNullOrWhiteSpace(options.Value.Instance))
	{
		azureAd.Instance = options.Value.Instance;
	}
	else
	{
		azureAd.Instance = "https://login.microsoftonline.com/";
	}
	azureAd.TenantId = options.Value.TenantId;
	azureAd.ClientId = options.Value.ClientId;
	azureAd.ClientSecret = options.Value.Secret;
	azureAd.Scopes = options.Value.Scopes.Split(",").ToList();
	return OperationResult.Ok(azureAd);
}

static IHost ConfigureServices(string baseDirectory = "")
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
		})
		.UseSerilog((hostContext, loggerConfiguration) =>
		{
			loggerConfiguration.WriteTo.Console();
		})
	   .Build();
}