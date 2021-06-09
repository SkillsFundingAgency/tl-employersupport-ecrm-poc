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



## Requirements on an Azure DevOps solution

- functions host
- storage - but probably just for functions host
- application insights
- build/release pipeline
- how about key vault for secrets?

