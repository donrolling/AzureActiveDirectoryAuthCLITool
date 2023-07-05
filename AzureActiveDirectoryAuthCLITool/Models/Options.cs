using CommandLine;
using CommandLine.Text;

namespace AzureActiveDirectoryAuthCLITool.Models
{
	public class Options
	{
		[Option('i', "Instance", Required = false, HelpText = "The AAD Instance url. Defaults to https://login.microsoftonline.com/")]
		public string Instance { get; set; } = "https://login.microsoftonline.com/";

		[Option('t', "TenantId", Required = true, HelpText = "The AAD TenantId.")]
		public string TenantId { get; set; }

		[Option('c', "ClientId", Required = true, HelpText = "The AAD ClientId.")]
		public string ClientId { get; set; }

		[Option('s', "Secret", Required = true, HelpText = "The AAD Client Secret.")]
		public string Secret { get; set; }

		[Option('o', "Scopes", Required = true, HelpText = "A comma-separated list of AAD Client Scopes. It is ok to only have one.")]
		public string Scopes { get; set; }

		[Usage(ApplicationAlias = "aadauth")]
		public static IEnumerable<Example> Examples
		{
			get
			{
				return new List<Example>() {
					new Example(
						"Pass values for each of these values to gain an valid bearer token. Instance will default to https://login.microsoftonline.com/",
						new Options {
							Instance = "https://login.microsoftonline.com/",
							TenantId = "xxx",
							ClientId = "xxx",
							Secret = "xxx",
							Scopes = "xxx,xxx"
						}
					)
				};
			}
		}
	}
}