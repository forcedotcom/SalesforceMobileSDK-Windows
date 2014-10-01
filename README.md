SalesforceMobileSDK-Windows
===========================

Windows SDK for Salesforce

INSTALLATION NOTES

You will need the following for the SDK,

1) Visual Studio 2013 with Update 3 or newer
2) Visual Studio SDK Installed: http://msdn.microsoft.com/en-us/library/bb166441.aspx
3) Sqlite installed for Visual Studio (http://www.sqlite.org/2014/sqlite-winrt81-3080600.vsix). This package is for Windows Phone and Store 8.1 apps.
4) Sqlite for Windows Phone 8.1 as well http://visualstudiogallery.msdn.microsoft.com/1d04f82f-2fe9-4727-a2f9-a2db127ddc9a

BUILDING

For template generation you need to run Visual Studio 2013 in admnistration mode.

Once the SDK has been built completely you should be able to see "Salesforce Application (Universal Apps)" as an option for a new project.

SMARTSTORE

SmartStore support is a work in progress. There are a few manual steps that are required after you create a project in order for things to function.

1) SQLite requires that your project be built for a specific target - your phone or store app will have to target x86, x64, or ARM specifically to use smart store.
2) You will need to add the SmartStore reference to your individual projects where you want to use SmartStore.
3) Add the MSOpenTech SqlitePCL NuGet to the project along with SmartStore.
4) Add SQLite for your specific platform to each individual project, for example "SQLite for Windows Runtime (Windows 8.1)" for a store application.

This work will hopefully be minimized in the future.