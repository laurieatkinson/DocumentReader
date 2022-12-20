# DocumentReader

* Most of the demo code is in HomeController.cs and Program.cs

* Modify the appsettings.json with your Azure tenant settings
* Through Visual Studio, you can Manage User Secrets and add something like this:
```json
{
  "AzureAD:ClientSecret": "<client-secret-in-your-azure-ad-app-registration>"
}
```
## Azure AD App Registration
* Redirect URI: https://localhost:7185/signin-oidc
* Add a client secret and copy it into the app for development
* API permissions:
  * Microsoft Graph
    - Files.ReadWrite.All
    - User.Read
  * SharePoint
    - AllSites.Read