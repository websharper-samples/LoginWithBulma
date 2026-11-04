open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open WebSharper.AspNetCore

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)

    builder.Services.AddWebSharper() |> ignore

    let app = builder.Build()

    if not (app.Environment.IsDevelopment()) then
        app.UseExceptionHandler("/Error").UseHsts() |> ignore

    app.UseHttpsRedirection()
       .UseDefaultFiles()
       .UseStaticFiles()
    |> ignore

    app.Run()

    0 // Exit code
