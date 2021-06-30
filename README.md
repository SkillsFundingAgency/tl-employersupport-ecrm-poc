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

### Functions local settings

Add a file `local.settings.json` to the functions project. This file is gitignored so secrets will not be saved to git.

```
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "ServiceBusConnectionString": "<service bus connection string",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "EmailConfiguration:SupportEmailAddress": "<your-email>",
    "EmailConfiguration:GovNotifyApiKey": "<key>",
    "EmailConfiguration:ZendeskTicketCreatedEmailTemplateId": "<template-id>",
    "ZendeskConfiguration:AuthenticationMethod": "BasicWithApiToken",
    "ZendeskConfiguration:ApiBaseUri": "<api-url>",
    "ZendeskConfiguration:User": "<user-email>",
    "ZendeskConfiguration:ApiToken": "<api-token>",
    "EcrmConfiguration:ApiBaseUri": "<api-url>",
    "EcrmConfiguration:ApiKey": "<api-key>"
  }
}
```

### Running functions from the command line

Navigate to the function project folder, e.g. 
```
cd C:\dev\esfa\tl-employersupport-ecrm-poc\src\tl.employersupport.ecrm.poc.functions
```

If function tools are not installed, run
```
npm i -g azure-functions-core-tools@3 --unsafe-perm true
```

Then run the functions using
```
func host start --verbose
```


## Benchmarks

To run benchmarks in a release build use
```
dotnet run -p tl.employersupport.ecrm.poc.benchmarking.csproj -c Release
```


## Automated Tests

If not already installed, install the Specflow for Visual Studio 2019. 
See (https://marketplace.visualstudio.com/items?itemName=TechTalkSpecFlowTeam.SpecFlowForVisualStudio for details.

TODO: Add integration/automation tests project.


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

The POC has a DevOps pipeline in `azure-pipelines.yml` with a build and a single release stage. 
The pipeline uses YAML multi-line strings to make it more readable. See https://yaml-multiline.info/

The pipeline requires the following variable groups to be set up in the Library:

    - **Group name** `tl-zd-ecrm-poc-deployment`
    - **Values** as below
        - note no hyphens in resource identifier, to avoid invalid resource group name

        | Name                                | Sample Value   |
        | ----                                | ------------   |
        | ResourceLocation                    | UK South       |
        | ResourceIdentifier                  | tlzdecrmpoc    |
        | ResourceEnvironmentName             | dev            |

    - **Group name** `tl-zd-ecrm-poc-arm`
    - **Values** as below

        | Name                                | Sample Value |
        | ----                                | ------------ |
        | GovNotifyApiKey                     | ********     |
        | SupportEmailAddress                 | ********     |
        | EcrmApiBaseUri                      | https://dev.api.crm.org.uk/directory/apiname/ |
        | EcrmApiKey                          | ********     |
        | ZendeskApiBaseUri                   | https://tlevelsemployertest.zendesk.com/api/v2 |
        | ZendeskApiToken                     | ********     |
        | ZendeskAuthenticationMethod         | BasicWithApiToken |
        | ZendeskPassword                     | NA           |
        | ZendeskTicketCreatedEmailTemplateId | ********     |
        | ZendeskUser                         | ********     |

### Connecting to Azure for the ARM subscription

This requires the following service connections

    `ARM Subscription` - Service connection for running ARM templates

Set up guide - https://docs.microsoft.com/en-us/azure/active-directory/develop/howto-create-service-principal-portal

To set this up, go to the Azure portal and create a new app registration called (for example) `tl-zd-ecrm-app` and choose "multitenant" - this probably needs access to all clients.
Go to the Certificates and secrets tab for the new app registration 
and create a new secret. Copy the value as you cannot retrieve it later. 
Also copy the application id AND tenant id from the overview tab.

*(Optional step - may work without this)* On the API Permissions tab add a permission - choose Azure DevOps then select Delegated Permissions - user_impersonation.

To get the subscription details, either check in the portal or run
```
az account show
```

Assign a role to the app registration within the subscription.
 - go to Subscriptions in the Azure portal
 - select the subscription
 - select Access control (IAM).
 - set the role to Contributer, assign access to service principal and select the new app registration
 - save

Go to the Project Settings in the Azure DevOps project and create a service connection.
 - select Azure resource management
 - type is service principal (manual) 
 - name should be "ARM Subscription" to match the name used in the pipeline yaml
 - Choose "Subscription" as the scope (if this is one of the options)
 - Subscription id should be your Azure subscription id
 - Subscription name should be your Azure subscription name, e.g. "Visual Studio Professional Subscription"
 - Principal Id is the application id of the app registration
 - Service principal key is the value from the secret created above 
 - tenant id is the tenant id from the app registration 


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

Comparison of a single Zendesk with and without compression in an API response:

| Compression type | Message size |
| ---------------- | ------------ |
| compressed       | 1306 bytes   |
| uncompressed     | 7744 bytes   | 

This is only implemented for Zendesk calls so far.



