SalesforceMobileSDK-Windows
===========================

Windows SDK for Salesforce

You have arrived at the source repository for the Salesforce Mobile SDK for Windows. Welcome! Starting with our 4.0 release, there are now two ways you can choose to work with the Mobile SDK:

- If you'd like to work with the source code of the SDK itself, you've come to the right place! You can browse sample app source code and debug down through the layers to get a feel for how everything works under the covers. Read on for instructions on how to get started with the SDK in your development environment.
- If you're just eager to start developing your own application, the quickest way is using our Powershell script, called createapp.ps1, which uses the SDK Nuget package on Nuget.org to create your own application based on the application template. After you download the SDK using the instuction below, run createapp.ps1 (under SalesforceSDK/template) from PowerShell. You can do "createapp.ps1 version" to check the current SDK version. And to create an application do "createapp.ps1 create" and enter the appropriate information for each prompt.

INSTALLATION NOTES

You will need the following for the SDK,

1. Visual Studio 2015, preferably with Update 1
2. Visual Studio SDK Installed: http://msdn.microsoft.com/en-us/library/bb166441.aspx
3. Windows 10 SDK installed 
4. Sqlite installed for Visual Studio
  1. Universal App Platform: https://www.sqlite.org/2015/sqlite-uap-3090200.vsix

BUILDING

Build the project normally in Visual Studio; everything should build fine.  If you wish to create a new project and not use the NuGet versions of the core libraries, simply create a new project with the template, remove the NuGet reference and add references to Salesforce.SDK.Core, Salesforce.SDK.Universal (for Universal Windows Apps), Salesforce.SDK.SmartStore to use the SmarStore feature, and Salesforce.SDK.SmartSync to use the SmartSync feature.

It is recommended to stick with NuGet and get official builds, unless you absolutely need to make your own modifications or you're planning to contribute to the project.

Downloading the Salesforce SDK

To pull down the SDK from github, create a new directory and git clone the salesforce SDK repo.

git clone https://github.com/forcedotcom/SalesforceMobileSDK-Windows.git