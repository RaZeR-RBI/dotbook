# DotBook - cross-platform documentation generator for .NET and .NET Core
This project is being worked on. Stay tuned.

# About the project
DotBook is a command-line tool to generate human-friendly documentation from source files.

Currently supported formats are:
* Markdown (can be uploaded to Github Pages directly)
* Html (suitable for offline browsing or iframe integration)

# Command-line options
Option format: -option, --option
* ```o, output``` - Output directory for the generated documentation. If not specified, defaults to 'doc'.
* ```s, src``` - Directory for C# code search
* ```v, visibility``` - Include types and members with the specified visibilities. Defaults to 'public'.
* ```h, use-hash``` - Use hashing for documentation filenames to allow deep hierarchies. If false, uses escaped type/member name. Defaults to 'false'.
* ```f, format``` - Sets the output format. Default is Markdown. Available formats: Markdown, Html
* ```?, help``` - Displays the help message.

## Examples


