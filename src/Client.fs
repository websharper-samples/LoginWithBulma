namespace MyProject

open System
open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Notation
open WebSharper.UI.Templating

[<JavaScript>]
module Client =
    // The templates are loaded from the DOM, so you just can edit index.html
    // and refresh your browser, no need to recompile unless you add or remove holes.
    type MySPA = Template<"wwwroot/index.html", ClientLoad.FromDocument>

    [<SPAEntryPoint>]
    let Main () =
        let passwordValid = Var.Create true
        let emailValid = Var.Create true
        MySPA.LoginForm()
            .AttrEmail(Attr.DynamicClassPred "is-danger" (View.Map not emailValid.View))
            .AttrEmailMessage(Attr.DynamicClassPred "hidden" emailValid.View)
            .AttrPassword(Attr.DynamicClassPred "is-danger" (View.Map not passwordValid.View))
            .AttrPasswordMessage(Attr.DynamicClassPred "hidden" passwordValid.View)
            .Login(fun e ->
                passwordValid := not (String.IsNullOrWhiteSpace e.Vars.Password.Value)
                emailValid := not (String.IsNullOrWhiteSpace e.Vars.Email.Value)

                if passwordValid.Value && emailValid.Value then
                    JS.Alert (sprintf "Your email is %s" e.Vars.Email.Value)
                e.Event.PreventDefault()
            )
            .Doc()
        |> Doc.RunById "main"
