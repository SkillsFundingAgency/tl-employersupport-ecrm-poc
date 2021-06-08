# Employer Support Monitor POC

## Console app

For details on how to create a console app with hosted services see https://dfederm.com/building-a-console-app-with-.net-generic-host/

To set the environment (and make sure the development app settings are picked up), in the Debug tab add an environment variable called *DOTNET_ENVIRONMENT* with value *Development*


## Unit tests

Added NuGet packages and extensions for `ShouldNotAcceptNullConstructorArguments` and `ShouldNotAcceptNullOrBadConstructorArguments` 
- `AutoFixture`
- `AutoFixture.AutoNSubstitute`
- `AutoFixture.Idioms`


## What the POC won't do

- application insights logging


## Requirements on an Azure DevOps solution

- functions host
- storage - but probably just for functions host
- application insights
- build/release pipeline
- how about key vault for secrets?

