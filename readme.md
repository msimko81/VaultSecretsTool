# Vault Secrets Tool

## Overview

The tool can be used to write secrets from a local file to the dev, staging and production HC Vault store.

Secrets will be stored in a lastpass entry as properly formatted text and will be copied manually from lastpass to the local file.

## Usage

Run the tool by executing the following command from the base directory:
```
dotnet run --project .\src\VaultSecretsTool
```

## Prerequisites

Access to the selected HC Vault server is required by the tool. This needs to be provided by the person running the tool.

The secrets is meant to be a json containing one top level object with arbitrary number of string properties.
This object is internally mapped to a <i>Dictionary<string,string></i> to ensure consistency.

## Configuration

Application is configured using global [appsettings.json](src/VaultSecretsTool/appsettings.json) and optional personalized `appsettings.Local.json`, 
which can created out of the template [appsettings.Local.json.template](src/VaultSecretsTool/appsettings.Local.json.template).

Configurable custom options: 
* used environment
* username
* password
* local secrets json file path

## Scenario

The tool authenticates the user. If the personalized configuration is not available, 
user is asked to input required configuration, i.e. used HC Vault environment (Development, Staging or Production)
and LDAP user credentials (username and password). 

In the next step, the tool loads json file containing credentials to be written. 
The file to be loaded is selected using the following procedure:
* if the secrets local file path is configured, load the file defined by the configured location
* otherwise list all json files from the working directory and let the user choose which one to select, or
* optionally let the user enter the full file path to the json file with secrets

Secrets are then written to the HC Vault.

Finally, the used configuration options are saved to the copy of `appsettings.Local.json`, 
which is used the next time the program is started. Prior saving the config, the user is asked 
whether to save password along with another configuration options.

If some custom option is configured when executing the tool, its value is printed on input request 
and the user can confirm the value by hitting Enter without re-entering the value.

## Architecture

* .NET 5 CLI application
* connecting to the HC Vault REST API
* defaults for all inputs can be configured in the local application config (i.e. `appsettings.Local.json`)

