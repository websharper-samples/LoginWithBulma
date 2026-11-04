namespace LoginWithBulma

open System
open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Templating

[<JavaScript>]
module Client =
    // The page itself is the template: .Bind() fills its holes in place, so
    // there is no separate mount point (see also the Counter and 2048 samples).
    type IndexTemplate = Template<"wwwroot/index.html", ClientLoad.FromDocument>

    let private isBlank (s: string) = String.IsNullOrWhiteSpace s

    [<SPAEntryPoint>]
    let Main () =
        let email = Var.Create ""
        let password = Var.Create ""

        // Warnings only appear after the first sign-in attempt, then update live
        // as the user types, so they can correct the field.
        let submitted = Var.Create false

        let emailOk = email.View |> View.Map (isBlank >> not)
        let passwordOk = password.View |> View.Map (isBlank >> not)

        let toggled = View.Map2 (fun sent ok -> sent && ok)
        let flagged = View.Map2 (fun sent ok -> sent && not ok)

        IndexTemplate()
            .Email(email)
            .Password(password)
            // ws-attr holes accept a list, so each input can carry both states.
            .AttrEmail([
                Attr.DynamicClassPred "is-danger" (flagged submitted.View emailOk)
                Attr.DynamicClassPred "is-success" (toggled submitted.View emailOk)
            ])
            .AttrEmailMessage(Attr.DynamicClassPred "is-visible" (flagged submitted.View emailOk))
            .AttrPassword([
                Attr.DynamicClassPred "is-danger" (flagged submitted.View passwordOk)
                Attr.DynamicClassPred "is-success" (toggled submitted.View passwordOk)
            ])
            .AttrPasswordMessage(Attr.DynamicClassPred "is-visible" (flagged submitted.View passwordOk))
            .Login(fun e ->
                e.Event.PreventDefault()
                submitted.Value <- true
                if not (isBlank email.Value) && not (isBlank password.Value) then
                    JS.Alert (sprintf "Signed in as %s" email.Value))
            .Bind()
