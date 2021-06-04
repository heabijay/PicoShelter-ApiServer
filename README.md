# PicoShelter-ApiServer

<p align="center">
    <img src="https://www.picoshelter.tk/assets/icons/picoShelter/Black%20Icon%20%2B%20Text.svg" height="64px">
</p>

The server part of the PicoShelter Project.

## About PicoShelter Project

_"PicoShelter is a free image hosting service with user profiles, shared albums, and direct links. Also, we provide the official desktop application for more comfortable experience."_

PicoShelter's [ApiServer](https://github.com/heabijay/PicoShelter-ApiServer), [WebApp](https://github.com/heabijay/PicoShelter-WebApp), [DesktopApp](https://github.com/heabijay/DesktopApp) were created by [heabijay](https://github.com/heabijay) as the diploma project.

## Demo

This project demo currently serves on Azure App Service (Free Plan) since 05.05.2021.

**Server endpoint:** [picoshelter-apiserver.azurewebsites.net](https://picoshelter-apiserver.azurewebsites.net)

**Swagger UI:** [picoshelter-apiserver.azurewebsites.net/apidocs](https://picoshelter-apiserver.azurewebsites.net/apidocs)

_Due to the free plan on Azure, the server isn't alive all time so the first request could take some time._

## Configuration

You need to rename two configuration files and fill your own configuration:

1. `PicoShelter-ApiServer/appsettings.Example.json` => `PicoShelter-ApiServer/appsettings.json`

    _This file presents the global server configuration._

    **Main fields:**
    - `ConnectionStrings.DefaultConnection` — The connection string to your database. Currently, we guarantee MySQL and MS SQL support.
    - `WebApp.Default.HomeUrl` — The URL path to the client part of the project. This is using in the email composing process, in redirect on the home route `./`.
    - `WebApp.Default.ConfirmEndpoint` — Such as previous — the URL path, which is using in the email composing process to activate confirmation keys.
    - `SmtpServers.DefaultServer.Host` — SMTP host address. SMTP using for email sending.
    - `SmtpServers.DefaultServer.Port` — SMTP host port.
    - `SmtpServers.DefaultServer.UseSsl` — Is SMTP connection needs SSL.
    - `SmtpServers.DefaultServer.Authorization.Username` — SMTP username.
    - `SmtpServers.DefaultServer.Authorization.Password` — SMTP password.
    - `SmtpServers.DefaultServer.FromAddress` — Address, which sends emails. Could be the same as the SMTP username.

2. `PicoShelter-ApiServer/AuthOptions.Example.cs` => `PicoShelter-ApiServer/AuthOptions.cs`

    _This file presents the JWT composer settings._

    **Main fields:**
    - `AuthOptions.Issuer` — Issuer field of JWT.
    - `AuthOptions.Audience` — Audience field of JWT.
    - `AuthOptions.Key` — Master password for JWT hasher.
    - `AuthOptions.Lifetime` — The timespan for JWT valid until.

By default, File Repository with user's images using the "FileRepository" folder of execution assembly path. You could change it in `Startup.cs` _(~73 line with IFileUnitOfWork)_.