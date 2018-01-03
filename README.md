# About the project
DotBook is a .NET CLI tool for human-friendly documentation generation from C# source files.

Currently supported formats are:
* **Markdown** (can be uploaded to Github Pages directly)
* **HTML** (suitable for offline browsing or iframe integration)

# How to use
1. Add the following item to your .csproj:
```
  <ItemGroup>
     <DotNetCliToolReference Include="DotBook" Version="0.5.3-dev"/>
  </ItemGroup>
```

2. Run the ```dotnet restore``` command
3. Use it! Run ```dotnet doc``` in the project folder to use default settings or ```dotnet doc --help``` for help.

# Command-line options
Option format: -option, --option
* ```o, output``` - Output directory for the generated documentation. If not specified, defaults to 'docs'.
* ```s, src``` - Directory for C# code search
* ```v, visibility``` - Include types and members with the specified visibilities. Defaults to 'public'.
* ```h, use-hash``` - Use hashing for documentation filenames to allow deep hierarchies. If false, uses escaped type/member name. Defaults to 'false'.
* ```f, format``` - Sets the output format. Default is Markdown. Available formats: Markdown, Html
* ```?, help``` - Displays the help message.

# Examples
```dotnet doc```

```dotnet doc --format html```

```dotnet doc --src /my-module --output /my-module-docs```

