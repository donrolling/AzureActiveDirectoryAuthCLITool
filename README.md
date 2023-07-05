# Azure Active Directory Auth CLI Tool

## Powershell Tool

Designed to make it easy to get an auth token via Powershell for using an API that is secured by AAD (Azure Active Directory). 

Assumptions: You have an API that is secured via Client Secret rather than a certificate.

## Usage

Pass values for each of these values to gain an valid bearer token. Instance will default to https://login.microsoftonline.com/.

`aadauth --TenantId xxx --ClientId xxx --Secret xxx --Scopes xxx`

`aadauth -t xxx --c xxx -s xxx -o xxx`

  -i, --Instance    The AAD Instance url. Defaults to https://login.microsoftonline.com/

  -t, --TenantId    Required. The AAD TenantId.

  -c, --ClientId    Required. The AAD ClientId.

  -s, --Secret      Required. The AAD Client Secret.

  -o, --Scopes      Required. A comma-separated list of AAD Client Scopes. It is ok to only have one.

## Installation

Prerequisites:

- Azure CLI installed. [Instructions](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli-windows?tabs=azure-cli)
- Logged into Azure with CaliberFS ADO credentials. From PowerShell: `az login`

From PowerShell:

`dotnet tool install --global AzureActiveDirectoryAuthCLITool`

## Project Setup

These three lines were needed in the csproj:
```
<PackAsTool>true</PackAsTool>
<ToolCommandName>aadauth</ToolCommandName>
```

## Building and deploying

Terminal command must be run in the terminal from the location of the csproj.
I think install works differently if using a non-local nupkg. More on that when I figure it out.

- build solution
- create a nupkg in the PackageOutputPath location
	> dotnet pack
- install the tool globally
	> dotnet tool install --global AzureActiveDirectoryAuthCLITool

**Update the tool**
> dotnet tool install --global AzureActiveDirectoryAuthCLITool --version [x.x.x]

**Uninstall the tool**
> dotnet tool uninstall AzureActiveDirectoryAuthCLITool -g