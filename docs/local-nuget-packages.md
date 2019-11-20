# Local NuGet Packages

When you are working on a project that consumes the public Sitecore.Demo.Shared NuGet packages, you might need to apply some changes to the shared code or items. In that case, you must generate local NuGet packages and use them.

## Generating Local NuGet Packages

### Building the Solutions

1. Adjust the values in the `cake-config.json` file if needed.
2. Open a PowerShell prompt as adminstrator.
3. Run the `.\build.ps1` command.

### Generating the Packages

1. Open a PowerShell prompt as adminstrator in the `\Build` folder.
2. Run the `.\generate-nuget-packages.ps1` command as is or with optional parameters:
    - `-version <version>`: Generated packages version. Should be a high number. Default value is using 999 for the last digit (e.g.: 9.3.0.999).
    - `-outputPath <path>`: Location where the packages are saved. Default value is `C:\sc_demo`

## Consuming Local NuGet Packages

### Adding a Local NuGet Package Repository

1. Edit your project `nuget.config` file.
2. Before any other source, add a new package source

    ```xml
    <configuration>
      <packageSources>
        <add key="sc_demo" value="C:\sc_demo" />
        ...
      </packageSources>
    </configuration>
    ```

3. Save the file.

### Deleting old NuGet Packages

1. In your solution folder, navigate to the `packages` folder.
2. Delete all folders that start with `sitecore.demo*`.
3. Navigate to your global NuGet cache (`C:\Users\[YourUsername]\.nuget\packages`).
4. Delete all folders that start with `sitecore.demo*`.

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

### Deleting the Local Packages

1. In your solution folder, navigate to the `packages` folder.
2. Delete all folders that start with `sitecore.demo*`.
3. Navigate to your global NuGet cache (`C:\Users\[YourUsername]\.nuget\packages`).
4. Delete all folders that start with `sitecore.demo*`.
