# Employer Support Monitor POC


## What the POC won't do

- application insights logging


## Console app

For details on how to create a console app with hosted services see https://dfederm.com/building-a-console-app-with-.net-generic-host/

To set the environment (and make sure the development app settings are picked up), in the Debug tab add an environment variable called *DOTNET_ENVIRONMENT* with value *Development*


### Settings 

Add a local file `appsettings.Development.json` (this is git ignored so won't be checked in)

**TODO:** Describe settings.


## Unit tests

Added NuGet packages and extensions for `ShouldNotAcceptNullConstructorArguments` and `ShouldNotAcceptNullOrBadConstructorArguments` 
- `AutoFixture`
- `AutoFixture.AutoNSubstitute`
- `AutoFixture.Idioms`


## Functions

TODO: describe the functions here


## SonarCloud Analysis

(Not implemented)

Based on https://pumpingco.de/blog/how-to-run-a-sonarcloud-scan-during-docker-builds-for-dotnet-core/


## Requirements for Azure 

- functions host
- storage - but probably just for functions host
- application insights
- build/release pipeline
- key vault for secrets?
- logic app?

The POC has a DevOps pipeline. 

This requires the following variable group:

    - **Group name** `tl-zd-ecrm-poc-vars`
    - **Values** as below

| Name                                | Sample Value |
| ----                                | ------------ |
| GovNotifyApiKey                     | ********     |
| ResourceGroupLocation               | UK South     |
| ResourceGroupName                   | tl-zd-ecrm-poc-rg |
| SupportEmailAddress                 | ********     |
| ZendeskApiBaseUri                   | https://tlevelsemployertest.zendesk.com/api/v2 |
| ZendeskApiToken                     | ********     |
| ZendeskAuthenticationMethod         | BasicWithApiToken |
| ZendeskPassword                     | NA           |
| ZendeskTicketCreatedEmailTemplateId | ********     |
| ZendeskUser                         | ********     |


### Connecting to Azure for the ARM subscription

This requires the following service connections

    `ARM Subscription` - Service connection for running ARM templates

The Azure deployment pipeline requires an Azure Container Registry, but
because in the first attempts it wasn't in the same the same 
Azure AD as the build pipeline, the following steps were needed:

(taken from https://stackoverflow.com/questions/55833711/azure-devops-add-azure-container-registry-in-build-pipeline-from-different-acco)

 - Create an app registration in the Azure AD where the ACR exists.
 - Give it a name like myregistry-app
 - Go to the myregistry-app Certificates and secrets page and create a new secret. Copy the value as you cannot retrieve it later.
 - Also copy the myregistry-app application id. You can find it on the overview screen.
 - Now go to the ACR Access Control (IAM) screen for your container registry.
 - Add a role assignment and assign the myregistry-app identity the Contributor role.
 - Back in your build pipeline create a Docker task and click on the New button under the Container Registry section.
 - In the popup dialog Add a Docker Registry service connection choose the Others radio button.
 - Put in the URL to your ACR which you can find on the container registry overview page.
 - Use the application id for myregistry-app as the Docker ID.
 - Use the myregistry-app secret for the password.



## Zendesk Contact Form

Data comes from the Zendesk ticket form `Form - T Levels - Employer Contact Form` abd has the following fields:

| Field name                           | Details |
| ----------                           | ------- |
| T Level Name                         | Required for end users, Editable for end users |
| T Levels Employers - Phone Number    | Required for end users, Editable for end users |
| T Levels Employers - Contact Method  | Required for end users, Editable for end users |
| T Levels Employers - Company Name    | Required for end users, Editable for end users |
| T Levels Employers - Total Employees | Editable for end users |
| Subject                              | System field, Required for end users |
| T Levels Employers - Contact reason  | Required for end users, Editable for end users |
| Description                          | System field, Required for end users |


## HTTP compression

The setting `CompressApiResponse` turns on/off compression in API responses. For comparison a single ticket request used:

| Compression type | Message size |
| ---------------- | ------------ |
| compressed       | 1306 bytes   |
| uncompressed     | 7744 bytes   | 

This is only implemented for Zendesk calls so far.



## Adding a .NET 5.0 functions project

If the project was created as .NET Core 3.1, it needs to be updated.
(Not needed if the project was created as .NET 5.0)

##### Change project:

```
  <PropertyGroup>
    <TargetFramework>net5.0</TargetFramework>
    <AzureFunctionsVersion>v3</AzureFunctionsVersion>
    <OutputType>Exe</OutputType>
    <_FunctionsSkipCleanOutput>true</_FunctionsSkipCleanOutput>
  </PropertyGroup>
```

##### Remove nuget package:
```
Microsoft.NET.Sdk.Functions
```

##### Add nuget packages:
```
Microsoft.Azure.Functions.Worker
Microsoft.Azure.Functions.Worker.Sdk OutputItemType="Analyzer" 
Microsoft.Azure.WebJobs.Extensions.Storage
System.Net.NameResolution

optional?
Microsoft.Azure.WebJobs.Extensions
Microsoft.Azure.WebJobs.Extensions.Http

?
Microsoft.Azure.WebJobs.Script.ExtensionsMetadataGenerator

```

Add an `OutputItemType="Analyzer"` attribute to `Microsoft.Azure.Functions.Worker.Sdk`

##### Add Program.cs
All the project startup will be done here.

##### Remove Startup.cs
This is no longer needed.



Update local.settings.json runtime version:
```
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
```

### Running functions from the command line

Navigate to the function project folder, e.g. 
```
cd C:\dev\repos\BlobStorageV12FunctionApp\BlobStorageV12FunctionApp\
```

If function tools are not installed, run
```
npm i -g azure-functions-core-tools@3 --unsafe-perm true
```

Then run the functions using
```
func host start --verbose
```
