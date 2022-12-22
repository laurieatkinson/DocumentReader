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

# Logic App
## Demo uses Blob storage to save the updated file.
* Create a Logic App in your subscription
  * Enable Managed Identity on the Logic App
    * Settings -> Identity -> System assigned -> Status = On
* Create the Blob Storage account
  * Create Storage account
  * Add blob container to storage account
    * Data storage -> Containers
  * Give managed identity access to blob storage
    * IAM -> Role assignments -> Add -> Role = Storage Blob Data Contributor, Assign access to = Logic App, select = your Logic App
* Verify permission on Logic App
  * Settings -> Identity -> Azure role assignments
    * Scope = Storage
    * Role = Storage Blob Data Contributor
    * Resource Name = your-storage-account-name
* Select Logic app code view, paste the contents with LogicApp/SPSyncLogicApp.json, and replace the following placeholders
  * {SHAREPOINT_URL} -> your-tenant
  * {SUBSCRIPTION_ID} -> your-subscription-guid
  * {LIST_ID} -> guid for SharePoint List, get this by going to Document Settings of library and looking at the url
    * e.g. https://your-tenant.sharepoint.com/sites/your-sitename/_layouts/15/listedit.aspx?List=list-id-guid
  * {SITE_NAME} -> SharePoint site name, which is DemoSite in the MVC app
  * {SHAREPOINT_FOLDER_NAME} -> TemporaryEditing is the name of the folder in the MVC app
  * {RESOURCE_GROUP_NAME} -> the resource group containing the Logic App
  * {STORAGE_ACCOUNT_NAME} -> your-storage-account-name
  * {LOCATION} -> location of your logic app, e.g. eastus, westus

## Instructions to replace Blob storage with on-premises file system

https://learn.microsoft.com/en-us/azure/logic-apps/logic-apps-using-file-connector