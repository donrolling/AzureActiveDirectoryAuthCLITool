namespace AzureActiveDirectoryAuthCLITool.Models
{
	public class AzureAd
	{
		public string Instance { get; set; }
		public string TenantId { get; set; }
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
		public List<string> Scopes { get; set; }
	}
}