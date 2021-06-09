# Employer Support Monitor POC


## What the POC won't do

- application insights logging


## Console app

For details on how to create a console app with hosted services see https://dfederm.com/building-a-console-app-with-.net-generic-host/

To set the environment (and make sure the development app settings are picked up), in the Debug tab add an environment variable called *DOTNET_ENVIRONMENT* with value *Development*


## Unit tests

Added NuGet packages and extensions for `ShouldNotAcceptNullConstructorArguments` and `ShouldNotAcceptNullOrBadConstructorArguments` 
- `AutoFixture`
- `AutoFixture.AutoNSubstitute`
- `AutoFixture.Idioms`


## SonarCloud Analysis

Based on https://pumpingco.de/blog/how-to-run-a-sonarcloud-scan-during-docker-builds-for-dotnet-core/


## Requirements for Azure DevOps

- functions host
- storage - but probably just for functions host
- application insights
- build/release pipeline
- key vault for secrets?
- logic app?


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


# HTTP compression

The setting `CompressApiResponse` turns on/off compression in API responses. For comparison a single ticket request used:

| Compression type | Message size |
| ---------------- | ------------ |
| compressed       | 1306 bytes   |
| uncompressed     | 7744 bytes   | 

