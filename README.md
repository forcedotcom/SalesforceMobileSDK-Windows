SalesforceMobileSDK-Windows
===========================

Windows SDK for Salesforce

INSTALLATION NOTES

You will need the following for the SDK,

1. Visual Studio 2015, preferably with Update 1
2. Visual Studio SDK Installed: http://msdn.microsoft.com/en-us/library/bb166441.aspx
3. Windows 10 SDK installed 
4. Sqlite installed for Visual Studio
  1. Universal App Platform: https://www.sqlite.org/2015/sqlite-uap-3090200.vsix
4. Pre-built Visual Studio template: [SalesforceUniversalApplicationTemplate.zip] (template/SalesforceUniversalApplicationTemplate.zip)
  * Copy this to {user}\Documents\Visual Studio 2013\Templates\ProjectTemplates

BUILDING

Build the project normally in Visual Studio; everything should build fine.  If you wish to create a new project and not use the NuGet versions of the core libraries, simply create a new project with the template, remove the NuGet reference and add references to Salesforce.SDK.Core, Salesforce.SDK.Universal (for Universal Windows Apps), Salesforce.SDK.SmartStore to use the SmarStore feature, and Salesforce.SDK.SmartSync to use the SmartSync feature.

It is recommended to stick with NuGet and get official builds, unless you absolutely need to make your own modifications or you're planning to contribute to the project.
