﻿/*!
  Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
  Build: 2.0.1.WinJS.2014.7.22
  Version: WinJS.2.1
*/

// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

(function (global) {
    global.strings = global.strings || {};

    var appxVersion = "WinJS.2.1";
    var developerPrefix = "Developer.";
    if (appxVersion.indexOf(developerPrefix) === 0) {
        appxVersion = appxVersion.substring(developerPrefix.length);
    }

    function addStrings(keyPrefix,  strings) {
        Object.keys(strings).forEach(function (key) {
            global.strings[keyPrefix + key.replace("\\", "/")] = strings[key];
        });
    }
    
addStrings(
// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
"ms-resource://"+appxVersion+"/ui/",

{
    "appBarAriaLabel": "App Bar",
    "appBarCommandAriaLabel": "App Bar Item",
    "asynchronousRender" : "Top level items must render synchronously",
    "automaticallyLoadPagesIsDeprecated": "Invalid configuration: automaticallyLoadPages is deprecated. The control will default this property to false. Please refer to the 'ListView loading behaviors' SDK Sample for guidance on how to implement incremental load behavior.",
    "averageRating": "Average Rating",
    "backbuttonarialabel": "Back",
    "badAlignment": "Invalid argument: Flyout alignment should be 'center' (default), 'left', or 'right'.",
    "badAxis": "Invalid argument: orientation must be a string, either 'horizontal' or 'vertical'",
    "badButtonElement": "Invalid argument: For a button, toggle, or flyout command, the element must be null or a button element",
    "badClick": "Invalid argument: The onclick property for an {0} must be a function",
    "badCurrentPage": "Invalid argument: currentPage must be a number greater than or equal to zero and be within the bounds of the datasource",
    "badDivElement": "Invalid argument: For a content command, the element must be null or a div element",
    "badFlyout": "Invalid argument: The flyout property for an {0} must be a Flyout or String id of a Flyout",
    "badHrElement": "Invalid argument: For a separator, the element must be null or an hr element",
    "badItemSpacingAmount": "Invalid argument: itemSpacing must be a number greater than or equal to zero",
    "badLayout": "Invalid argument: The layout property must be 'custom' or 'commands'",
    "badPlacement": "Invalid argument: Flyout placement should be 'top' (default), 'bottom', 'left', 'right', or 'auto'.",
    "badReference": "Invalid argument: Invalid href to settings flyout fragment",
    "cannotChangeCommandsWhenVisible": "Invalid argument: You must call hide() before changing {0} commands",
    "cannotChangeHiddenProperty": "Unable to set hidden property while parent {0} is visible.",
    "cannotChangeLayoutWhenVisible": "Invalid argument: The layout property cannot be set when the AppBar is visible, call hide() first",
    "cannotChangePlacementWhenVisible": "Invalid argument: The placement property cannot be set when the AppBar is visible, call hide() first",
    "clearYourRating" : "Clear your rating",
    "closeOverlay" : "Close",
    "datePicker": "Date Picker",
    "disableBackdropIsDeprecated" : "Invalid configuration: disableBackdrop is deprecated. Style: .win-listview .win-container.win-backdrop { background-color:transparent; } instead.",
    "duplicateConstruction": "Invalid argument: Controls may only be instantiated one time for each DOM element",
    "duplicateSection": "Hub duplicate sections: Each HubSection must be unique",
    "backdropColorIsDeprecated" : "Invalid configuration: backdropColor is deprecated. Style: .win-listview .win-container.win-backdrop { rgba(155,155,155,0.23); } instead.",
    "flipViewNavigationDuringStateChange": "Error: After changing itemDataSource or itemTemplate, any navigation in the FlipView control should be delayed until the pageselected event is fired.",
    "flipViewPanningContainerAriaLabel": "Scrolling Container",
    "flyoutAriaLabel": "Flyout",
    "groupInfoIsDeprecated": "GridLayout.groupInfo may be altered or unavailable in future versions. Instead, use CellSpanningLayout.",
    "groupInfoResultIsInvalid": "Invalid result: groupInfo result for cell spanning groups must include the following numeric properties: cellWidth and cellHeight.",
    "hubViewportAriaLabel": "Scrolling Container",
    "indexIsInvalid": "Invalid argument: index must be a non-negative integer.",
    "invalidHubContent": "Invalid content: Hub content must be made up of HubSections.",
    "invalidCountReturned": "Error: data adapter should return undefined, null, CountResult.unknown, or a non-negative integer for the count.",
    "invalidIndexReturned": "Error: data adapter should return undefined, null or a non-negative integer for the index.",
    "invalidItemReturned": "Error: data adapter returned item that is not an object.",
    "invalidKeyReturned": "Error: data adapter returned item with undefined or null key.",
    "invalidPivotContent": "Invalid content: Pivot content must be made up of PivotItems.",
    "invalidSearchBoxSuggestionKind": "Error: Invalid search suggestion kind.",
    "invalidTemplate": "Invalid template: Templates must be created before being passed to the ListView, and must contain a valid tree of elements.",
    "invalidViewBoxChildren": "ViewBox expects to only have one child element",
    "invalidZoomFactor": "Invalid zoomFactor",
    "invalidRequestedCountReturned": "Error: data adapter should return CountResult.unknown, CountResult.failure, or a non-negative integer for the count.",
    "itemInfoIsDeprecated": "GridLayout.itemInfo may be altered or unavailable in future versions. Instead, use CellSpanningLayout.",
    "itemInfoIsInvalid": "Invalid argument: An itemInfo function must be provided which returns an object with numeric width and height properties.",
    "itemIsInvalid": "Invalid argument: item must be a DOM element that was returned by the Items Manager, and has not been replaced or released.",
    "itemRendererIsInvalid": "Invalid argument: itemRenderer must be a function.",
    "keyIsInvalid": "Invalid argument: key must be a string.",
    "listDataAdapterIsInvalid": "Invalid argument: listDataAdapter must be an object or an array.",
    "listDataSourceIsInvalid": "Invalid argument: dataSource must be an object.",
    "listViewInvalidItem": "Item must provide index, key or description of corresponding item.",
    "listViewViewportAriaLabel": "Scrolling Container",
    "loadingBehaviorIsDeprecated": "Invalid configuration: loadingBehavior is deprecated. The control will default this property to 'randomAccess'. Please refer to the 'ListView loading behaviors' SDK Sample for guidance on how to implement incremental load behavior.",
    "maxRowsIsDeprecated": "GridLayout.maxRows may be altered or unavailable in future versions. Instead, use the maximumRowsOrColumns property.",
    "menuCommandAriaLabel": "Menu Item",
    "menuAriaLabel": "Menu",
    "modeIsInvalid": "Invalid argument: mode must be one of following values: 'none', 'single' or 'multi'.",
    "mustContainCommands": "Invalid HTML: AppBars/Menus must contain only AppBarCommands/MenuCommands",
    "navBarContainerViewportAriaLabel": "Scrolling Container",
    "noAnchor": "Invalid argument: Showing flyout requires a DOM element as its parameter.",
    "noitemsManagerForCount": "Invalid operation: can't get count if no dataSource has been set",
    "notCompatibleWithSemanticZoom": "ListView can only be used with SemanticZoom if randomAccess loading behavior is specified.",
    "nullCommand": "Invalid argument: command must not be null",
    "off" : "Off",
    "on" : "On",
    "loadMorePagesIsDeprecated": "loadMorePages is deprecated. Invoking this function will not have any effect. Please refer to the 'ListView loading behaviors' SDK Sample for guidance on how to implement incremental load behavior.",
    "pagesToLoadIsDeprecated": "Invalid configuration: pagesToLoad is deprecated. The control will not use this property. Please refer to the 'ListView loading behaviors' SDK Sample for guidance on how to implement incremental load behavior.",
    "pagesToLoadThresholdIsDeprecated": "Invalid configuration: pagesToLoadThreshold is deprecated.  The control will not use this property. Please refer to the 'ListView loading behaviors' SDK Sample for guidance on how to implement incremental load behavior.",
    "pivotAriaLabel": "Pivot",
    "pivotViewportAriaLabel": "Scrolling Container",
    "refreshCycleIdentified": "refresh cycle found, likely data inconsistency",
    "requiresCommands": "Invalid argument: commands must not be empty",
    "repeaterReentrancy":"Cannot modify Repeater data until Repeater has commited previous modification.",
    "resetGroupHeaderIsDeprecated": "resetGroupHeader may be altered or unavailable in future versions. Instead, mark the header element as disposable using WinJS.Utilities.markDisposable.",
    "resetItemIsDeprecated": "resetItem may be altered or unavailable in future versions. Instead, mark the element as disposable using WinJS.Utilities.markDisposable.",
    "searchBoxAriaLabel": "Searchbox",
    "searchBoxAriaLabelInputNoPlaceHolder": "Searchbox, enter to submit query, esc to clear text",
    "searchBoxAriaLabelInputPlaceHolder": "Searchbox, {0}, enter to submit query, esc to clear text",
    "searchBoxAriaLabelButton": "Click to submit query",
    "searchBoxAriaLabelQuery": "Suggestion: {0}",
    "_searchBoxAriaLabelQuery.comment": "Suggestion: query text (example: Suggestion: contoso)",
    "searchBoxAriaLabelSeparator": "Separator: {0}",
    "_searchBoxAriaLabelSeparator.comment": "Separator: separator text (example: Separator: People or Separator: Apps)",
    "searchBoxAriaLabelResult": "Result: {0}, {1}",
    "_searchBoxAriaLabelResult.comment": "Result: text, detailed text (example: Result: contoso, www.contoso.com)",
    "selectAMPM": "Select A.M P.M",
    "selectDay": "Select Day",
    "selectHour": "Select Hour",
    "selectMinute": "Select Minute",
    "selectMonth": "Select Month",
    "selectYear": "Select Year",
    "settingsFlyoutAriaLabel": "Settings Flyout",
    "settingsFlyoutWidthIsDeprecated": "SettingsFlyout.width may be altered or unavailable in future versions. Instead, style the CSS width property on elements with the .win-settingsflyout class.",
    "tentativeRating": "Tentative Rating",
    "timePicker": "Time Picker",
    "tooltipStringsIsInvalid": "Invalid argument: tooltipStrings must be null or an array of strings.",
    "unrated": "Unrated",
    "userRating": "User Rating",
    // AppBar Icons follow, the format of the ui.js and ui.resjson differ for
    // the AppBarIcon namespace.  The remainder of the file therefore differs.
    // Code point comments are the icon glyphs in the 'Segoe UI Symbol' font.
    "appBarIcons\\previous":                            "\uE100", //  group:Media
    "_appBarIcons\\previous.comment":                   "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\next":                                "\uE101", //  group:Media
    "_appBarIcons\\next.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\play":                                "\uE102", //  group:Media
    "_appBarIcons\\play.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\pause":                               "\uE103", //  group:Media
    "_appBarIcons\\pause.comment":                      "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\edit":                                "\uE104", //  group:File
    "_appBarIcons\\edit.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\save":                                "\uE105", //  group:File
    "_appBarIcons\\save.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\clear":                               "\uE106", //  group:File
    "_appBarIcons\\clear.comment":                      "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\delete":                              "\uE107", //  group:File
    "_appBarIcons\\delete.comment":                     "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\remove":                              "\uE108", //  group:File
    "_appBarIcons\\remove.comment":                     "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\add":                                 "\uE109", //  group:File
    "_appBarIcons\\add.comment":                        "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\cancel":                              "\uE10A", //  group:Editing
    "_appBarIcons\\cancel.comment":                     "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\accept":                              "\uE10B", //  group:General
    "_appBarIcons\\accept.comment":                     "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\more":                                "\uE10C", //  group:General
    "_appBarIcons\\more.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\redo":                                "\uE10D", //  group:Editing
    "_appBarIcons\\redo.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\undo":                                "\uE10E", //  group:Editing
    "_appBarIcons\\undo.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\home":                                "\uE10F", //  group:General
    "_appBarIcons\\home.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\up":                                  "\uE110", //  group:General
    "_appBarIcons\\up.comment":                         "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\forward":                             "\uE111", //  group:General
    "_appBarIcons\\forward.comment":                    "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\right":                               "\uE111", //  group:General
    "_appBarIcons\\right.comment":                      "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\back":                                "\uE112", //  group:General
    "_appBarIcons\\back.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\left":                                "\uE112", //  group:General
    "_appBarIcons\\left.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\favorite":                            "\uE113", //  group:Media
    "_appBarIcons\\favorite.comment":                   "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\camera":                              "\uE114", //  group:System
    "_appBarIcons\\camera.comment":                     "{Locked:qps-ploc,qps-plocm}",    
    "appBarIcons\\settings":                            "\uE115", //  group:System
    "_appBarIcons\\settings.comment":                   "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\video":                               "\uE116", //  group:Media
    "_appBarIcons\\video.comment":                      "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\sync":                                "\uE117", //  group:Media
    "_appBarIcons\\sync.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\download":                            "\uE118", //  group:Media
    "_appBarIcons\\download.comment":                   "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\mail":                                "\uE119", //  group:Mail and calendar
    "_appBarIcons\\mail.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\find":                                "\uE11A", //  group:Data
    "_appBarIcons\\find.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\help":                                "\uE11B", //  group:General
    "_appBarIcons\\help.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\upload":                              "\uE11C", //  group:Media
    "_appBarIcons\\upload.comment":                     "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\emoji":                               "\uE11D", //  group:Communications
    "_appBarIcons\\emoji.comment":                      "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\twopage":                             "\uE11E", //  group:Layout
    "_appBarIcons\\twopage.comment":                    "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\leavechat":                           "\uE11F", //  group:Communications
    "_appBarIcons\\leavechat.comment":                  "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\mailforward":                         "\uE120", //  group:Mail and calendar
    "_appBarIcons\\mailforward.comment":                "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\clock":                               "\uE121", //  group:General
    "_appBarIcons\\clock.comment":                      "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\send":                                "\uE122", //  group:Mail and calendar
    "_appBarIcons\\send.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\crop":                                "\uE123", //  group:Editing
    "_appBarIcons\\crop.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\rotatecamera":                        "\uE124", //  group:System
    "_appBarIcons\\rotatecamera.comment":               "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\people":                              "\uE125", //  group:Communications
    "_appBarIcons\\people.comment":                     "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\closepane":                           "\uE126", //  group:Layout
    "_appBarIcons\\closepane.comment":                  "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\openpane":                            "\uE127", //  group:Layout
    "_appBarIcons\\openpane.comment":                   "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\world":                               "\uE128", //  group:General
    "_appBarIcons\\world.comment":                      "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\flag":                                "\uE129", //  group:Mail and calendar
    "_appBarIcons\\flag.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\previewlink":                         "\uE12A", //  group:General
    "_appBarIcons\\previewlink.comment":                "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\globe":                               "\uE12B", //  group:Communications
    "_appBarIcons\\globe.comment":                      "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\trim":                                "\uE12C", //  group:Editing
    "_appBarIcons\\trim.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\attachcamera":                        "\uE12D", //  group:System
    "_appBarIcons\\attachcamera.comment":               "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\zoomin":                              "\uE12E", //  group:Layout
    "_appBarIcons\\zoomin.comment":                     "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\bookmarks":                           "\uE12F", //  group:Editing
    "_appBarIcons\\bookmarks.comment":                  "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\document":                            "\uE130", //  group:File
    "_appBarIcons\\document.comment":                   "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\protecteddocument":                   "\uE131", //  group:File
    "_appBarIcons\\protecteddocument.comment":          "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\page":                                "\uE132", //  group:Layout
    "_appBarIcons\\page.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\bullets":                             "\uE133", //  group:Editing
    "_appBarIcons\\bullets.comment":                    "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\comment":                             "\uE134", //  group:Communications
    "_appBarIcons\\comment.comment":                    "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\mail2":                               "\uE135", //  group:Mail and calendar
    "_appBarIcons\\mail2.comment":                      "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\contactinfo":                         "\uE136", //  group:Communications
    "_appBarIcons\\contactinfo.comment":                "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\hangup":                              "\uE137", //  group:Communications
    "_appBarIcons\\hangup.comment":                     "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\viewall":                             "\uE138", //  group:Data
    "_appBarIcons\\viewall.comment":                    "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\mappin":                              "\uE139", //  group:General
    "_appBarIcons\\mappin.comment":                     "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\phone":                               "\uE13A", //  group:Communications
    "_appBarIcons\\phone.comment":                      "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\videochat":                           "\uE13B", //  group:Communications
    "_appBarIcons\\videochat.comment":                  "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\switch":                              "\uE13C", //  group:Communications
    "_appBarIcons\\switch.comment":                     "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\contact":                             "\uE13D", //  group:Communications
    "_appBarIcons\\contact.comment":                    "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\rename":                              "\uE13E", //  group:File
    "_appBarIcons\\rename.comment":                     "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\pin":                                 "\uE141", //  group:System
    "_appBarIcons\\pin.comment":                        "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\musicinfo":                           "\uE142", //  group:Media
    "_appBarIcons\\musicinfo.comment":                  "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\go":                                  "\uE143", //  group:General
    "_appBarIcons\\go.comment":                         "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\keyboard":                            "\uE144", //  group:System
    "_appBarIcons\\keyboard.comment":                   "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\dockleft":                            "\uE145", //  group:Layout
    "_appBarIcons\\dockleft.comment":                   "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\dockright":                           "\uE146", //  group:Layout
    "_appBarIcons\\dockright.comment":                  "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\dockbottom":                          "\uE147", //  group:Layout
    "_appBarIcons\\dockbottom.comment":                 "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\remote":                              "\uE148", //  group:System
    "_appBarIcons\\remote.comment":                     "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\refresh":                             "\uE149", //  group:Data
    "_appBarIcons\\refresh.comment":                    "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\rotate":                              "\uE14A", //  group:Layout
    "_appBarIcons\\rotate.comment":                     "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\shuffle":                             "\uE14B", //  group:Media
    "_appBarIcons\\shuffle.comment":                    "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\list":                                "\uE14C", //  group:Editing
    "_appBarIcons\\list.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\shop":                                "\uE14D", //  group:General
    "_appBarIcons\\shop.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\selectall":                           "\uE14E", //  group:Data
    "_appBarIcons\\selectall.comment":                  "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\orientation":                         "\uE14F", //  group:Layout
    "_appBarIcons\\orientation.comment":                "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\import":                              "\uE150", //  group:Data
    "_appBarIcons\\import.comment":                     "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\importall":                           "\uE151", //  group:Data
    "_appBarIcons\\importall.comment":                  "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\browsephotos":                        "\uE155", //  group:Media
    "_appBarIcons\\browsephotos.comment":               "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\webcam":                              "\uE156", //  group:System
    "_appBarIcons\\webcam.comment":                     "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\pictures":                            "\uE158", //  group:Media
    "_appBarIcons\\pictures.comment":                   "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\savelocal":                           "\uE159", //  group:File
    "_appBarIcons\\savelocal.comment":                  "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\caption":                             "\uE15A", //  group:Media
    "_appBarIcons\\caption.comment":                    "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\stop":                                "\uE15B", //  group:Media
    "_appBarIcons\\stop.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\showresults":                         "\uE15C", //  group:Data
    "_appBarIcons\\showresults.comment":                "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\volume":                              "\uE15D", //  group:Media
    "_appBarIcons\\volume.comment":                     "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\repair":                              "\uE15E", //  group:System
    "_appBarIcons\\repair.comment":                     "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\message":                             "\uE15F", //  group:Communications
    "_appBarIcons\\message.comment":                    "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\page2":                               "\uE160", //  group:Layout
    "_appBarIcons\\page2.comment":                      "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\calendarday":                         "\uE161", //  group:Mail and calendar
    "_appBarIcons\\calendarday.comment":                "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\calendarweek":                        "\uE162", //  group:Mail and calendar
    "_appBarIcons\\calendarweek.comment":               "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\calendar":                            "\uE163", //  group:Mail and calendar
    "_appBarIcons\\calendar.comment":                   "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\characters":                          "\uE164", //  group:Editing
    "_appBarIcons\\characters.comment":                 "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\mailreplyall":                        "\uE165", //  group:Mail and calendar
    "_appBarIcons\\mailreplyall.comment":               "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\read":                                "\uE166", //  group:Mail and calendar
    "_appBarIcons\\read.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\link":                                "\uE167", //  group:Communications
    "_appBarIcons\\link.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\accounts":                            "\uE168", //  group:Communications
    "_appBarIcons\\accounts.comment":                   "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\showbcc":                             "\uE169", //  group:Mail and calendar
    "_appBarIcons\\showbcc.comment":                    "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\hidebcc":                             "\uE16A", //  group:Mail and calendar
    "_appBarIcons\\hidebcc.comment":                    "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\cut":                                 "\uE16B", //  group:Editing
    "_appBarIcons\\cut.comment":                        "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\attach":                              "\uE16C", //  group:Mail and calendar
    "_appBarIcons\\attach.comment":                     "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\paste":                               "\uE16D", //  group:Editing
    "_appBarIcons\\paste.comment":                      "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\filter":                              "\uE16E", //  group:Data
    "_appBarIcons\\filter.comment":                     "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\copy":                                "\uE16F", //  group:Editing
    "_appBarIcons\\copy.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\emoji2":                              "\uE170", //  group:Mail and calendar
    "_appBarIcons\\emoji2.comment":                     "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\important":                           "\uE171", //  group:Mail and calendar
    "_appBarIcons\\important.comment":                  "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\mailreply":                           "\uE172", //  group:Mail and calendar
    "_appBarIcons\\mailreply.comment":                  "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\slideshow":                           "\uE173", //  group:Media
    "_appBarIcons\\slideshow.comment":                  "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\sort":                                "\uE174", //  group:Data
    "_appBarIcons\\sort.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\manage":                              "\uE178", //  group:System
    "_appBarIcons\\manage.comment":                     "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\allapps":                             "\uE179", //  group:System
    "_appBarIcons\\allapps.comment":                    "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\disconnectdrive":                     "\uE17A", //  group:System
    "_appBarIcons\\disconnectdrive.comment":            "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\mapdrive":                            "\uE17B", //  group:System
    "_appBarIcons\\mapdrive.comment":                   "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\newwindow":                           "\uE17C", //  group:System
    "_appBarIcons\\newwindow.comment":                  "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\openwith":                            "\uE17D", //  group:System
    "_appBarIcons\\openwith.comment":                   "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\contactpresence":                     "\uE181", //  group:Communications
    "_appBarIcons\\contactpresence.comment":            "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\priority":                            "\uE182", //  group:Mail and calendar
    "_appBarIcons\\priority.comment":                   "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\uploadskydrive":                      "\uE183", //  group:File
    "_appBarIcons\\uploadskydrive.comment":             "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\gototoday":                           "\uE184", //  group:Mail and calendar
    "_appBarIcons\\gototoday.comment":                  "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\font":                                "\uE185", //  group:Editing
    "_appBarIcons\\font.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\fontcolor":                           "\uE186", //  group:Editing
    "_appBarIcons\\fontcolor.comment":                  "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\contact2":                            "\uE187", //  group:Communications
    "_appBarIcons\\contact2.comment":                   "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\folder":                              "\uE188", //  group:File
    "_appBarIcons\\folder.comment":                     "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\audio":                               "\uE189", //  group:Media
    "_appBarIcons\\audio.comment":                      "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\placeholder":                         "\uE18A", //  group:General
    "_appBarIcons\\placeholder.comment":                "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\view":                                "\uE18B", //  group:Layout
    "_appBarIcons\\view.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\setlockscreen":                       "\uE18C", //  group:System
    "_appBarIcons\\setlockscreen.comment":              "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\settile":                             "\uE18D", //  group:System
    "_appBarIcons\\settile.comment":                    "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\cc":                                  "\uE190", //  group:Media
    "_appBarIcons\\cc.comment":                         "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\stopslideshow":                       "\uE191", //  group:Media
    "_appBarIcons\\stopslideshow.comment":              "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\permissions":                         "\uE192", //  group:System
    "_appBarIcons\\permissions.comment":                "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\highlight":                           "\uE193", //  group:Editing
    "_appBarIcons\\highlight.comment":                  "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\disableupdates":                      "\uE194", //  group:System
    "_appBarIcons\\disableupdates.comment":             "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\unfavorite":                          "\uE195", //  group:Media
    "_appBarIcons\\unfavorite.comment":                 "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\unpin":                               "\uE196", //  group:System
    "_appBarIcons\\unpin.comment":                      "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\openlocal":                           "\uE197", //  group:File
    "_appBarIcons\\openlocal.comment":                  "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\mute":                                "\uE198", //  group:Media
    "_appBarIcons\\mute.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\italic":                              "\uE199", //  group:Editing
    "_appBarIcons\\italic.comment":                     "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\underline":                           "\uE19A", //  group:Editing
    "_appBarIcons\\underline.comment":                  "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\bold":                                "\uE19B", //  group:Editing
    "_appBarIcons\\bold.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\movetofolder":                        "\uE19C", //  group:File
    "_appBarIcons\\movetofolder.comment":               "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\likedislike":                         "\uE19D", //  group:Data
    "_appBarIcons\\likedislike.comment":                "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\dislike":                             "\uE19E", //  group:Data
    "_appBarIcons\\dislike.comment":                    "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\like":                                "\uE19F", //  group:Data
    "_appBarIcons\\like.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\alignright":                          "\uE1A0", //  group:Editing
    "_appBarIcons\\alignright.comment":                 "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\aligncenter":                         "\uE1A1", //  group:Editing
    "_appBarIcons\\aligncenter.comment":                "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\alignleft":                           "\uE1A2", //  group:Editing
    "_appBarIcons\\alignleft.comment":                  "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\zoom":                                "\uE1A3", //  group:Layout
    "_appBarIcons\\zoom.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\zoomout":                             "\uE1A4", //  group:Layout
    "_appBarIcons\\zoomout.comment":                    "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\openfile":                            "\uE1A5", //  group:File
    "_appBarIcons\\openfile.comment":                   "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\otheruser":                           "\uE1A6", //  group:System
    "_appBarIcons\\otheruser.comment":                  "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\admin":                               "\uE1A7", //  group:System
    "_appBarIcons\\admin.comment":                      "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\street":                              "\uE1C3", //  group:General
    "_appBarIcons\\street.comment":                     "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\map":                                 "\uE1C4", //  group:General
    "_appBarIcons\\map.comment":                        "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\clearselection":                      "\uE1C5", //  group:Data
    "_appBarIcons\\clearselection.comment":             "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\fontdecrease":                        "\uE1C6", //  group:Editing
    "_appBarIcons\\fontdecrease.comment":               "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\fontincrease":                        "\uE1C7", //  group:Editing
    "_appBarIcons\\fontincrease.comment":               "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\fontsize":                            "\uE1C8", //  group:Editing
    "_appBarIcons\\fontsize.comment":                   "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\cellphone":                           "\uE1C9", //  group:Communications
    "_appBarIcons\\cellphone.comment":                  "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\reshare":                             "\uE1CA", //  group:Communications
    "_appBarIcons\\reshare.comment":                    "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\tag":                                 "\uE1CB", //  group:Data
    "_appBarIcons\\tag.comment":                        "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\repeatone":                           "\uE1CC", //  group:Media
    "_appBarIcons\\repeatone.comment":                  "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\repeatall":                           "\uE1CD", //  group:Media
    "_appBarIcons\\repeatall.comment":                  "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\outlinestar":                         "\uE1CE", //  group:Data
    "_appBarIcons\\outlinestar.comment":                "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\solidstar":                           "\uE1CF", //  group:Data
    "_appBarIcons\\solidstar.comment":                  "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\calculator":                          "\uE1D0", //  group:General
    "_appBarIcons\\calculator.comment":                 "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\directions":                          "\uE1D1", //  group:General
    "_appBarIcons\\directions.comment":                 "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\target":                              "\uE1D2", //  group:General
    "_appBarIcons\\target.comment":                     "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\library":                             "\uE1D3", //  group:Media
    "_appBarIcons\\library.comment":                    "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\phonebook":                           "\uE1D4", //  group:Communications
    "_appBarIcons\\phonebook.comment":                  "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\memo":                                "\uE1D5", //  group:Communications
    "_appBarIcons\\memo.comment":                       "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\microphone":                          "\uE1D6", //  group:System
    "_appBarIcons\\microphone.comment":                 "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\postupdate":                          "\uE1D7", //  group:Communications
    "_appBarIcons\\postupdate.comment":                 "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\backtowindow":                        "\uE1D8", //  group:Layout
    "_appBarIcons\\backtowindow.comment":               "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\fullscreen":                          "\uE1D9", //  group:Layout
    "_appBarIcons\\fullscreen.comment":                 "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\newfolder":                           "\uE1DA", //  group:File
    "_appBarIcons\\newfolder.comment":                  "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\calendarreply":                       "\uE1DB", //  group:Mail and calendar
    "_appBarIcons\\calendarreply.comment":              "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\unsyncfolder":                        "\uE1DD", //  group:File
    "_appBarIcons\\unsyncfolder.comment":               "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\reporthacked":                        "\uE1DE", //  group:Communications
    "_appBarIcons\\reporthacked.comment":               "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\syncfolder":                          "\uE1DF", //  group:File
    "_appBarIcons\\syncfolder.comment":                 "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\blockcontact":                        "\uE1E0", //  group:Communications
    "_appBarIcons\\blockcontact.comment":               "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\switchapps":                          "\uE1E1", //  group:System
    "_appBarIcons\\switchapps.comment":                 "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\addfriend":                           "\uE1E2", //  group:Communications
    "_appBarIcons\\addfriend.comment":                  "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\touchpointer":                        "\uE1E3", //  group:System
    "_appBarIcons\\touchpointer.comment":               "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\gotostart":                           "\uE1E4", //  group:System
    "_appBarIcons\\gotostart.comment":                  "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\zerobars":                            "\uE1E5", //  group:System
    "_appBarIcons\\zerobars.comment":                   "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\onebar":                              "\uE1E6", //  group:System
    "_appBarIcons\\onebar.comment":                     "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\twobars":                             "\uE1E7", //  group:System
    "_appBarIcons\\twobars.comment":                    "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\threebars":                           "\uE1E8", //  group:System
    "_appBarIcons\\threebars.comment":                  "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\fourbars":                            "\uE1E9", //  group:System
    "_appBarIcons\\fourbars.comment":                   "{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\scan":               			"\uE294", //  group:General
    "_appBarIcons\\scan.comment":                   	"{Locked:qps-ploc,qps-plocm}",
    "appBarIcons\\preview":            			"\uE295", //  group:General
    "_appBarIcons\\preview.comment":                   	"{Locked:qps-ploc,qps-plocm}"
}

);

}(this));