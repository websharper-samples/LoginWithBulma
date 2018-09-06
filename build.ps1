param ([switch] $update)

if ($update) {
  dotnet add src package WebSharper
  dotnet add src package WebSharper.FSharp
  dotnet add src package WebSharper.UI
}

dotnet build src
