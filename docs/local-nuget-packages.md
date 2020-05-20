# Local NuGet Packages

When you are working on a project that consumes the public Sitecore.Demo.Shared NuGet packages, you might need to apply some changes to the shared code or items. In that case, you must generate local NuGet packages and use them.

## Prerequisites

Install GitVersion by running `choco install GitVersion.Portable`

## Generating Local NuGet Packages

### Starting the Local NuGet Server

1. Open a PowerShell prompt as adminstrator in the `\Build\nuget-server` folder.
2. Run `docker-compose up -d`

### Building the Solutions and Generating Their Packages

1. Adjust the values in the `cake-config.json` file if needed.
2. Open a PowerShell prompt as adminstrator.
3. Run the `.\build.ps1 -PushLocalNuget` command.
4. Browse to your [local NuGet service](http://localhost:5555/v2/index.json) to see the published packages.
   - Packages are also saved in `C:\sc_demo`

## Consuming Local NuGet Packages

### Adding a Local NuGet Package Repository

1. Edit your project `nuget.config` file.
2. Before any other source, add a new package source

    ```xml
    <configuration>
      <packageSources>
        <add key="sc_demo" value="http://localhost:5555/v2/index.json" />
        ...
      </packageSources>
    </configuration>
    ```

3. Save the file.

### Restoring NuGet Packages

1. Open your project solution.
2. Restore NuGet packages.

### Building and Testing Your Solution

Do your work against your local NuGet Packages.

## Reverting to Public NuGet Packages

### Removing the Local NuGet Package Repository

1. Edit your project `nuget.config` file.
2. Remove the local package source.
3. Save the file.

### Deleting the Local Packages From a Solution

1. In your solution folder, navigate to the `packages` folder.
2. Delete all folders that start with `sitecore.demo*`.
3. Navigate to your global NuGet cache (`C:\Users\[YourUsername]\.nuget\packages`).
4. Delete all folders that start with `sitecore.demo*`.

## Deleting All Packages From the Local NuGet Server

1. Open a PowerShell prompt as adminstrator in the `\Build\nuget-server` folder.
2. Run `docker-compose down`
3. Ensure you have committed all pending changes in the repository.
4. Run `git clean -Xdf`
   - Uppercase `X` is important to avoid deleting new files tracked in Git.
5. Restart the Local NuGet server by running `docker-compose up -d`
