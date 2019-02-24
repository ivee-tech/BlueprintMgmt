# Azure Blueprints Managegment tool


## Introduction

Helps creating, viewing, publishing, assigning / unassigning, and deleting Azure Blueprints.

[Azure Blueprints](https://docs.microsoft.com/en-us/azure/governance/blueprints/overview) is an Azure feature which helps engineers and cloud architects define sets of repeatable resources which adhere to the organization standards, patterns, and requirements. A blueprint allows to configure role assignments, policy assignments, resource groups, and resource manager templates.

The **BlueprintMgmt** tool provides options to create, view, publish, assign / unassign, and delete Azure blueprints based on JSON files stored on client's computer.

The tool uses direct APIs calls. The Blueprint APIs documentation is avaiable here:
[Blueprints REST API reference](https://docs.microsoft.com/en-us/rest/api/blueprints/)

Although the **Microsoft.Azure.Management.Blueprint** Nuget package is available for blueprints management, it is still in preview and multiple attempts to use it caused versioning conflicts with **System.Net.Http**. For more information about the Nuget packages, visit the reference page:
[Blueprint Nuget package](https://www.nuget.org/packages/Microsoft.Azure.Management.Blueprint)


## Prerequisits

The application must be registered in Azure AD.
A script is provided to register the application

``` PowerShell
./scripts/create-AAD-app.ps1
```
You will need to provide the application name, and optionally App Uri, Home page Url, and reply Urls.
To execute the script, you will need to connect to Azure using

``` PowerShell
Connect-AzAccount
```

and to Azure AD using

``` PowerShell
Connect-AzureAD
```

For Azure AD operations, make sure the Azure AD module is installed. If not, install it using

``` PowerShell
Install-Module AzureAD
```

Particular to **Azure Blueprint**, you must give high-level permissions to both **BlueprintMgmt** and **Azure Blueprint** applications.
**Azure Blueprint** has a static Application Id (<code>f71766dc-90d9-4b7d-bd9d-4499c4331c3f</code>), but its Azure AD Object Id is different on different tenants.
**Azure Blueprint** is registered as Enterprise Application. In order to get information, run the following PowerShell script:

``` PowerShell
# Azure Blueprint
$azureBlueprintAppId = 'f71766dc-90d9-4b7d-bd9d-4499c4331c3f'
Get-AzADApplication -ApplicationId $azureBlueprintAppId # doesn't return anything
Get-AzADServicePrincipal -ApplicationId $azureBlueprintAppId
```

In order to Assign a blueprint, the **Azure Blueprint** application requires Owner permission on the subscription the assignment uses.

The **BlueprintMgmt** application requires Owner permissions at both subscription and management group level. It should be noted that Contributor role is not sufficient, as it does not have permissions to write and delete blueprint artifacts.
See NotActions section in the [Contributor role page](https://docs.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#contributor). 

The script *./scripts/assign-app-role.ps1* in the scripts solution folder performs the above actions. Make sure you set the management group ID, subscription ID, and the corresponding BlueprintMgmt Application ID as per your environment.

``` PowerShell
$roleDef = 'Owner'

$mgmtGroupId = ''
$subscriptionId = ''
$appId = ''
$subScope = '/subscriptions/' + $subscriptionId
$mgmtGroupScope = '/providers/Microsoft.Management/managementGroups/' + $mgmtGroupId

New-AzRoleAssignment -RoleDefinitionName $roleDef -ApplicationId $appId -Scope $subScope
New-AzRoleAssignment -RoleDefinitionName $roleDef -ApplicationId $appId -Scope $mgmtGroupScope

$azureBlueprintAppId = 'f71766dc-90d9-4b7d-bd9d-4499c4331c3f'  #Azure Blueprint application (static Application ID)
New-AzRoleAssignment -RoleDefinitionName $roleDef -ApplicationId $azureBlueprintAppId -Scope $subScope
```



## Usage
### Azure Blueprints management tool
Helps creating, viewing, publishing, assigning / unassigning, and deleting Azure Blueprints.

Usage:

<code>BlueprintMgmt.exe command [--arg1 value1[, --arg2 value2[...]]]</code>

command:
 - create - creates a blueprint specified by --name argument
 - get - returns the blueprint specified by --name argument
 - get-artifacts - returns the blueprint artifacts specified by --name argument
 - get-artifact - returns the blueprint artifact specified by --name and --artifact-name argument
 - publish - publihes a blueprint specified by --name and --version arguments
 - assign - assigns a blueprint specified by --name argument
 - unassign - unassigns a blueprint specified by --name and --assignment-name arguments
 - delete - deletes a blueprint specified by --name argument
 - help - displays this screen

Examples

 - creates Blueprint blueprint1234

<code>BlueprintMgmt.exe create --name blueprint1234</code>

 - returns Blueprint blueprint1234

<code>BlueprintMgmt.exe get --name blueprint1234</code>

 - returns Artifacts for Blueprint blueprint1234

<code>BlueprintMgmt.exe get-artifacts --name blueprint1234</code>

 - returns Artifact template-storage-account for Blueprint blueprint1234

<code>BlueprintMgmt.exe get-artifact --name blueprint1234 --atifact-name template-storage-account</code>

 - publishes version 1.0 for blueprint1234

<code>BlueprintMgmt.exe publish --name blueprint1234 --version v1.0</code>

 - assigns Blueprint blueprint1234, with assignment name Assignment-blueprint1234

<code>BlueprintMgmt.exe assign --name blueprint1234 --asignment-name Assignment-blueprint1234</code>

 - unassigns Blueprint blueprint1234, based on assignment name

<code>BlueprintMgmt.exe unassign --name blueprint1234 --asignment-name Assignment-blueprint1234</code>

 - deletes Blueprint blueprint1234

<code>BlueprintMgmt.exe delete --name blueprint1234</code>


## Dependencies
The application uses the following Nuget packages:
 - Microsoft.Azure.KeyVault v3.0.3
 - Microsoft.Azure.KeyVault.WebKey v3.0.3
 - Microsoft.Azure.Services.AppAuthentication v1.0.3
 - Microsoft.IdentityModel.Clients.ActiveDirectory v4.5.1
 - Microsoft.Rest.ClientRuntime v2.3.19
 - Microsoft.Rest.ClientRuntime.Azure v3.3.18
 - Newtonsoft.Json v10.0.3


## Configuration settings

The configuration settings are stored in *app.config*. For production applcations it is recommended to secure sensitive information like Tenant ID, Application ID and secret, Management Group ID, Subscription ID etc.
You will need to provide your own values for attibute values in <code>[...]</code>: 

``` xml
  <appSettings>
    <add key="ida:AuthorityFormat" value="https://login.windows.net/{0}" />
    <add key="MgmtResource" value="https://management.azure.com/" />

    <add key="ida:TenantId" value="[Azure AD Tenand ID]"/>
    <add key="ida:ClientId" value="[BlueprintMgmt Application ID]" />
    <add key="ida:ClientSecret" value="[BlueprintMgmt Application Secret]" />
    <add key="ida:ObjectId" value="[User or role Object ID]" />
    <add key="ManagementGroupId" value="[Management Group ID]" />
    <add key="SubscriptionId" value="[Subscription ID]" />

    <add key="ida:ReplyUrl" value="[BlueprintMgmt Reply URL]" />
    <add key="BlueprintsDir" value="blueprints"/>
    <add key="AssignmentsDir" value="assignments"/>
    <add key="ADGraphSPFormat" value="https://graph.windows.net/{0}/servicePrincipals?api-version=1.6&amp;&#x24;filter=appId eq 'f71766dc-90d9-4b7d-bd9d-4499c4331c3f'"/>

    <add key="GetUrlFormat" value="https://management.azure.com/providers/Microsoft.Management/managementGroups/{0}/providers/Microsoft.Blueprint/blueprints/{1}?api-version=2018-11-01-preview" />
    <add key="GetArtifactsUrlFormat" value="https://management.azure.com/providers/Microsoft.Management/managementGroups/{0}/providers/Microsoft.Blueprint/blueprints/{1}/artifacts?api-version=2018-11-01-preview"/>
    <add key="GetArtifactUrlFormat" value="https://management.azure.com/providers/Microsoft.Management/managementGroups/{0}/providers/Microsoft.Blueprint/blueprints/{1}/artifacts/{2}?api-version=2018-11-01-preview"/>
    <add key="CreateBlueprintUrlFormat" value="https://management.azure.com/providers/Microsoft.Management/managementGroups/{0}/providers/Microsoft.Blueprint/blueprints/{1}?api-version=2018-11-01-preview" />
    <add key="AddArtifactUrlFormat" value="https://management.azure.com/providers/Microsoft.Management/managementGroups/{0}/providers/Microsoft.Blueprint/blueprints/{1}/artifacts/{2}?api-version=2018-11-01-preview"/>
    <add key="PublishUrlFormat" value="https://management.azure.com/providers/Microsoft.Management/managementGroups/{0}/providers/Microsoft.Blueprint/blueprints/{1}/versions/{2}?api-version=2018-11-01-preview" />
    <add key="AssignUrlFormat" value="https://management.azure.com/subscriptions/{0}/providers/Microsoft.Blueprint/blueprintAssignments/{1}?api-version=2018-11-01-preview" />
    <add key="UnassignUrlFormat" value="https://management.azure.com/subscriptions/{0}/providers/Microsoft.Blueprint/blueprintAssignments/{1}?api-version=2018-11-01-preview" />
    <add key="DeleteUrlFormat" value="https://management.azure.com/providers/Microsoft.Management/managementGroups/{0}/providers/Microsoft.Blueprint/blueprints/{1}?api-version=2018-11-01-preview" />

    <add key="HelpDir" value="documentation"/>
  </appSettings>
``` 

## Sample Blueprints

Sample blueprints and assignments are provided, see the *blueprints* and *assignments* directories. Each blueprint should have its own directory, named by convention as the blueprint name. The JSON file *[blueprint name].json* is the blueprint definition whereas all the other JSON files represent artifacts (role & policy assignments and templates).

The *assignments* directory contains the parameter values passed to the blueprint assignment. For daemon services or CI/CD pipelines, this directory should contain JSON files generated at runtime with pre-defined blueprint parameter values. 

## Resources
 - [Azure Blueprints Overview](https://docs.microsoft.com/en-us/azure/governance/blueprints/overview)
 - [Quick-start create using Portal](https://docs.microsoft.com/en-us/azure/governance/blueprints/create-blueprint-portal)
 - [Quick-start create using API](https://docs.microsoft.com/en-us/azure/governance/blueprints/create-blueprint-rest-api)
 - [Blueprints deep dive](http://aka.ms/blueprintsdeepdive)
 - [Blueprints as code](http://aka.ms/blueprintsascode)
 - [API-list](https://docs.microsoft.com/en-us/rest/api/blueprints/blueprints/list)
 - [Blueprint Nuget package](https://www.nuget.org/packages/Microsoft.Azure.Management.Blueprint)
 - [Import/Export using PowerShell](https://www.powershellgallery.com/packages/Manage-AzureRMBlueprint/2.2)
 - [Import/Export of AzureBlueprints using Manage-AzureRMBlueprint PowerShell Script](https://www.youtube.com/watch?v=SMORUIPhKd8&feature=youtu.be)
 - [Assigning Azure Blueprints using Microsoft Graph](https://www.developmentsindigital.com/posts/2018-12-18-azure-blueprint-with-microsoft-graph/)
 - [Blueprint SDK](https://github.com/Azure/azure-sdk-for-net/tree/master/src/SDKs/Blueprint)
