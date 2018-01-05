[![Build status](https://ci.appveyor.com/api/projects/status/47l1253qq6m7h7iw/branch/master?svg=true)](https://ci.appveyor.com/project/RaZeR-RawByte/dotbook/branch/master) [![GitHub license](https://img.shields.io/github/license/RaZeR-RBI/dotbook.svg)](https://github.com/RaZeR-RBI/dotbook/blob/master/LICENSE) [![NuGet Version](https://img.shields.io/nuget/v/DotBook.svg)](https://www.nuget.org/packages/DotBook) [![NuGet](https://img.shields.io/nuget/dt/DotBook.svg)]()

---

# About the project
DotBook is a .NET CLI tool for human-friendly documentation generation from C# source files.

Currently supported formats are:
* **Markdown** (can be uploaded to Github Pages directly)
* **HTML** (suitable for offline browsing or iframe integration)

[Live demo](https://razer-rbi.github.io/diffstore)

# How to use
1. Add the following item to your .csproj (replace the version number with [an actual one](https://www.nuget.org/packages/DotBook)):
```
  <ItemGroup>
     <DotNetCliToolReference Include="DotBook" Version="(insert-version-here)"/>
  </ItemGroup>
```

2. Run the ```dotnet restore``` command
3. Use it! Run ```dotnet doc``` in the project folder to use default settings or ```dotnet doc --help``` for help.

**Tip:** If you have a README.md or README.html (depending on the format) at the source root, it will be included at the index page before the API Documentation.

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

