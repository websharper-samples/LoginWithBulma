# First steps: Using HTML templates, accessing form values, and wiring events
> Written by Adam Granicz, IntelliFactory.

Congratulations on taking the first step to learn WebSharper! We have carefully put together this hands-on tutorial with the aim to help you get started with WebSharper and on your way to learn functional, reactive web development, putting you on a fast track to unleash the real web developer in you. The skills you pick up with WebSharper will make you a better web developer, and the concepts you learn will remain valid and useful with other functional, reactive web frameworks and libraries as well. You can find links to further material below and the sources of this tutorial in the bottom of this page.

## What you will learn and where you can find out more

1. **Using HTML templates** (these work on the client and server alike) instead of inline HTML combinators. *Further reading*: the "HTML Templates" section of the [Reactive HTML](https://developers.websharper.com/docs/v4.x/fs/ui) page of the main documentation.
2. **Reading and writing the values of input controls** (text boxes, text areas, checkboxes, etc.) in your HTML page through your template's data model. *Further reading*: the "Accessing the template’s model" subsection of the [Reactive HTML](https://developers.websharper.com/docs/v4.x/fs/ui) page of the main documentation.
3. **Wiring events** such as button clicks. *Further reading*: the bottom of the "Holes" subsection of the [Reactive HTML](https://developers.websharper.com/docs/v4.x/fs/ui) page of the main documentation.

## Our application: Login page for an SPA

In this tutorial, you will learn how to work with external HTML files (aka. templates) and how to implement your application logic into them. For this, we will take a [login page](https://dansup.github.io/bulma-templates/templates/login.html) from [dansup](https://github.com/dansup)'s wonderful free [Bulma template collection](https://dansup.github.io/bulma-templates/). Our app will look pretty much the same and also implement basic form validation:

![](https://i.imgur.com/E8Uv7oNl.png)

> The main takeaway of this tutorial is that you should use HTML templates as much as possible instead of inlining HTML code into your application logic. While it's certainly easy to construct HTML in C# or F# (by using the HTML combinators defined in `WebSharper.UI.Html`), it's our recommendation that you avoid it as much as you can for **better logic vs presentation separation**.

## Prerequisites

To get the most out of this tutorial, please make sure you have installed:

 * .NET Core 2.0+ and ASP.NET Core
 * the latest WebSharper templates
 * Visual Studio Code and/or Visual Studio 2017
 
## 1. Create your first SPA with WebSharper

Grab a command prompt, `cd` into the folder you want to use for your new project, and do:
```text
dotnet new websharper-spa -lang f# -n MyProject
```
This will create a new WebSharper SPA project for you. We will use F# for this tutorial, but you can also choose to create a C# project (just leave off the `-lang f#` part) and adapt the sources we discuss here. Go ahead and open this project with your favorite editor.

## 2. Get to know your files

![](https://i.imgur.com/nhnYqYBm.png)

* `wwwroot\index.html` - Your main SPA - this is the file you open to run your app
* `Client.fs` - The logic for your SPA - this is where your F# code will be
* `MyProject.fsproj` - The .NET Core project file for your SPA
* `Program.fs` / `Startup.fs` - The minimal boilerplate to run/host your app with the default ASP.NET Core web server
* `wsconfig.json` - Your WebSharper configuration file - normally, it's all set up for you

## 3. Bringing in your HTML code

The SPA project you just created consists of a sample template that you can simply replace with the new markup from the login template. To see what's going on underneath, follow these steps:

1. Replace `wwwroot\index.html` with the source of the [login page](https://dansup.github.io/bulma-templates/templates/login.html)
2. Download `login.css` and `bulma.js` from the login template as `wwwroot\login.css` and `wwwroot\bulma.js`, respectively, and update the references to them in `wwwroot\index.html` accordingly:
    ```html
      ..
      <link rel="stylesheet" type="text/css" href="css/login.css">
    </head>
    ```
    ```html
      ..
      <script async type="text/javascript" src="js/bulma.js"></script>
   </body>
    ```
3. Re-add the following block to the bottom of the `<head>` section:
    ```html
    <style>
      [ws-template], [ws-children-template] { display: none; }
     .hidden { display: none; }
   </style>

   <script type="text/javascript" src="Content/MyProject.head.js"></script>
    ```
    This will give us a `.hidden` CSS class to hide things (always comes handy), and also make sure that **any dependencies are correctly brought into the page** when needed.

4. Re-add the following block to the bottom of the `<body>` section:
    ```html
    <script type="text/javascript" src="Content/MyProject.min.js"></script>
    ```
    This will **load the JavaScript code WebSharper generates** for our SPA.

5. Feel free to update the `<title>...</title>` with the title you prefer to give to your app.

6. Change the links below the login form as you see fit (we won't deal with those in this tutorial.)

## 4. Implementing login with validation

Now that we have the skeleton of our login page, we can quickly wire in the necessary logic. First, we need to be able to access the email and password values the user types in. The WebSharper way of doing that is going into the template and marking the input controls that supply values to the F# layer with a `ws-var` attribute. We also want to implement some basic validation and use Bulma's `is-danger` class to visually indicate when an input control is not giving what we are expecting. So we add in `ws-attr` attributes as well. So change these lines to:

```html
...
    <input ws-var="Email" ws-attr="AttrEmail" class="input is-large" type="email" placeholder="Your Email" autofocus="">
...
    <input ws-var="Password" ws-attr="AttrPassword" class="input is-large" type="password" placeholder="Your Password">
...
    <input ws-var="RememberMe" type="checkbox">
...
```

Also, add a `ws-onclick` attribute to the Login button so we can wire a click event handler to it:

```html
...
   <button ws-onclick="Login" class="button is-block is-info is-large is-fullwidth">Login</button>
...
```
Now, we are ready to write our F# logic and switch over to `Client.fs`. If we didn't have to worry about validation, things will be super simple, but in our case we want the full enchilada so we also introduce a couple reactive variables to stand for whether the email and password fields are valid.

![](https://i.imgur.com/9AHo0srl.png)

At this point, we can see that the WebSharper UI templating type provider conveniently feeds back the reactive variables and attributes we defined above inside the `LoginForm` template, and we can simply set these up as follows:

1. Show a visual validation error for the Email input box if `emailValid` is false by applying Bulma's `is-danger` class:
    ```fsharp
    MySPA.LoginForm()
        .AttrEmail(Attr.DynamicClassPred "is-danger" (View.Map not emailValid.View))
    ```

2. Similary, show a validation error for the password field is `passwordValid` is false:
    ```fsharp
        .AttrPassword(Attr.DynamicClassPred "is-danger" (View.Map not passwordValid.View))
    ```

3. Now, let's handle the Login button click - this is what decides what constitutes a valid input (no empty fields) and simply putting up a popup if login is successful (this is where you would do a server call to authenticate the user and to log them in by creating a user session):
    ```fsharp
        .Login(fun e ->
            passwordValid := not (String.IsNullOrWhiteSpace e.Vars.Password.Value)
            emailValid := not (String.IsNullOrWhiteSpace e.Vars.Email.Value)

            if passwordValid.Value && emailValid.Value then
                JS.Alert (sprintf "Your email is %s" e.Vars.Email.Value)
            e.Event.PreventDefault()
        )
    ```
    > Note how `e` in the login handler enables you to **access all the input values** through `e.Vars`. You can also use these to **set their values on the UI** - two-way binding in WebSharper UI is super easy.

4. And last, we need to seal these  and inject our login form to the `#main` template placeholder:
    ```fsharp
        .Doc()
    |> Doc.RunById "main"
    ```

## 5. Other considerations

Our login app is 30 lines of F# code, and even for such a short tutorial like this one, we can pull off one more trick. Say, we wanted to add **error messages** below the input boxes failing validation. Bulma's way is to add these to the markup after each input control - and all we need as extra is another `ws-attr` attribute:

```html
<div class="field">
  <div class="control">
    <input ...>
  </div>
  <p ws-attr="AttrEmailMessage" class="is-danger help">Please enter an email address</p>
</div>
```

The last bit is handling the showing/hiding of this error message in `Client.fs` by applying the `hidden` class we added to our template earlier:

```fsharp
    .AttrEmailMessage(Attr.DynamicClassPred "hidden" emailValid.View)
```

We leave it to you as an exercise to show a similar error message for the password box; just a small hint: it looks exactly like above.

## Source code and try the app

You can fork [this SPA project](https://github.com/websharper-samples/LoginWithBulma) with all of its 35 lines of F# code and the adapted HTML template via GitHub. You can also [try out a slightly adapted version](http://try.websharper.com/snippet/adam.granicz/0000Jn) live on Try WebSharper.

Happy coding!
