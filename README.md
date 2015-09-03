SalesforceMobileSDK-Windows
===========================

Windows SDK for Salesforce

INSTALLATION NOTES

You will need the following for the SDK,

1. Visual Studio 2013 with Update 3 or newer
2. Visual Studio SDK Installed: http://msdn.microsoft.com/en-us/library/bb166441.aspx
3. Sqlite installed for Visual Studio
  1. Windows Phone 8.1: http://www.sqlite.org/2014/sqlite-wp81-winrt-3080704.vsix
  2. Windows 8.1: http://www.sqlite.org/2014/sqlite-winrt81-3080704.vsix
4. Pre-built Visual Studio template: [SalesforceUniversalApplicationTemplate.zip] (template/SalesforceUniversalApplicationTemplate.zip)
  * Copy this to {user}\Documents\Visual Studio 2013\Templates\ProjectTemplates

BUILDING

Build the project normally in Visual Studio; everything should build fine.  If you wish to create a new project and not use the NuGet versions of the core libraries, simply create a new project with the template, remove the NuGet reference and add references to Salesforce.SDK.Core, Salesforce.SDK.Store (for windows projects) or Salesforce.SDK.Phone for phone projects.

It is recommended to stick with NuGet and get official builds, unless you absolutely need to make your own modifications or you're planning to contribute to the project.

SMARTSTORE

SmartStore support is a work in progress. There are a few manual steps that are required after you create a project in order for things to function.

1. SQLite requires that your project be built for a specific target - your phone or store app will have to target x86, x64, or ARM specifically to use smart store.
2. You will need to add the SmartStore reference to your individual projects where you want to use SmartStore.
3. Add the MSOpenTech SqlitePCL NuGet to the project along with SmartStore.
4. Add SQLite for your specific platform to each individual project, for example "SQLite for Windows Runtime (Windows 8.1)" for a store application.

This work will hopefully be minimized in the future, and NuGet packages will be included. 
=======
# SalesforceMobileSDK-WindowsSamples
This repo contains sample apps built using the Salesforce Mobile SDK for Windows.

1. Container Sample:

This app shows how you can build an app that contains a webview.

2. AccountManager:

The AccountManager sample shows how to build an app using Cordova for Windows.