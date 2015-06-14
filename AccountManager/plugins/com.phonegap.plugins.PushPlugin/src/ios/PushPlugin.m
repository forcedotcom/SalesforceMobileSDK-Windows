/*
 Copyright 2009-2011 Urban Airship Inc. All rights reserved.

 Redistribution and use in source and binary forms, with or without
 modification, are permitted provided that the following conditions are met:

 1. Redistributions of source code must retain the above copyright notice, this
 list of conditions and the following disclaimer.

 2. Redistributions in binaryform must reproduce the above copyright notice,
 this list of conditions and the following disclaimer in the documentation
 and/or other materials provided withthe distribution.

 THIS SOFTWARE IS PROVIDED BY THE URBAN AIRSHIP INC``AS IS'' AND ANY EXPRESS OR
 IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
 MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO
 EVENT SHALL URBAN AIRSHIP INC OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
 BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE
 OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

#import "PushPlugin.h"

@implementation PushPlugin

@synthesize notificationMessage;
@synthesize isInline;

@synthesize callbackId;
@synthesize notificationCallbackId;
@synthesize callback;


- (void)unregister:(CDVInvokedUrlCommand*)command;
{
	self.callbackId = command.callbackId;

    [[UIApplication sharedApplication] unregisterForRemoteNotifications];
    [self successWithMessage:@"unregistered"];
}

- (void)register:(CDVInvokedUrlCommand*)command;
{
	self.callbackId = command.callbackId;

    NSMutableDictionary* options = [command.arguments objectAtIndex:0];
    
    // [UIApplication registerUserNotificationSettings:] is how iOS 8 and above registers for notifications.
    if ([[UIApplication sharedApplication] respondsToSelector:@selector(registerUserNotificationSettings:)]) {
        [self iOS8Register:options];
    } else {
        [self preiOS8Register:options];
    }
	
	if (notificationMessage)			// if there is a pending startup notification
		[self notificationReceived];	// go ahead and process it
}

- (void)preiOS8Register:(NSDictionary *)options {
    UIRemoteNotificationType notificationTypes = UIRemoteNotificationTypeNone;
    id badgeArg = [options objectForKey:@"badge"];
    id soundArg = [options objectForKey:@"sound"];
    id alertArg = [options objectForKey:@"alert"];
    
    if ([self notificationTypeConfigured:badgeArg])
        notificationTypes |= UIRemoteNotificationTypeBadge;
    if ([self notificationTypeConfigured:soundArg])
        notificationTypes |= UIRemoteNotificationTypeSound;
    if ([self notificationTypeConfigured:alertArg])
        notificationTypes |= UIRemoteNotificationTypeAlert;
    
    self.callback = [options objectForKey:@"ecb"];
    
    if (notificationTypes == UIRemoteNotificationTypeNone)
        NSLog(@"PushPlugin.register: Push notification type is set to none");
    
    isInline = NO;
    
    [[UIApplication sharedApplication] registerForRemoteNotificationTypes:notificationTypes];
}

- (void)iOS8Register:(NSDictionary *)options {
    // This is necessary to build libraries with the iOS 7 runtime, that can execute iOS 8 methods.  When
    // we switch to building libraries with Xcode 6, this can go away.
    //
    // >= iOS 8 notification types have to be NSUInteger, for backward compatibility with < iOS 8 build environments.
    //
    // UIUserNotificationTypes:
    //   UIUserNotificationTypeNone    = 0,      // the application may not present any UI upon a notification being received
    //   UIUserNotificationTypeBadge   = 1 << 0, // the application may badge its icon upon a notification being received
    //   UIUserNotificationTypeSound   = 1 << 1, // the application may play a sound upon a notification being received
    //   UIUserNotificationTypeAlert   = 1 << 2, // the application may display an alert upon a notification being received
    NSSet *categories = nil;
    NSUInteger notificationTypes = 0;
    
    id badgeArg = [options objectForKey:@"badge"];
    id soundArg = [options objectForKey:@"sound"];
    id alertArg = [options objectForKey:@"alert"];
    if ([self notificationTypeConfigured:badgeArg])
        notificationTypes |= (1 << 0);
    if ([self notificationTypeConfigured:soundArg])
        notificationTypes |= (1 << 1);
    if ([self notificationTypeConfigured:alertArg])
        notificationTypes |= (1 << 2);
    
    self.callback = [options objectForKey:@"ecb"];
    
    if (notificationTypes == 0)
        NSLog(@"PushPlugin.register: Push notification type is set to none");
    
    isInline = NO;
    
    Class userNotificationSettings = NSClassFromString(@"UIUserNotificationSettings");
    NSMethodSignature *settingsForTypesSig = [userNotificationSettings methodSignatureForSelector:@selector(settingsForTypes:categories:)];
    NSInvocation *settingsForTypesInv = [NSInvocation invocationWithMethodSignature:settingsForTypesSig];
    [settingsForTypesInv setTarget:userNotificationSettings];
    [settingsForTypesInv setSelector:@selector(settingsForTypes:categories:)];
    [settingsForTypesInv setArgument:&notificationTypes atIndex:2];
    [settingsForTypesInv setArgument:&categories atIndex:3];
    [settingsForTypesInv invoke];
    
    CFTypeRef settingsForTypesRetVal;
    [settingsForTypesInv getReturnValue:&settingsForTypesRetVal];
    if (settingsForTypesRetVal)
        CFRetain(settingsForTypesRetVal);
    
    [[UIApplication sharedApplication] performSelector:@selector(registerUserNotificationSettings:) withObject:(__bridge_transfer id)settingsForTypesRetVal];
    [[UIApplication sharedApplication] performSelector:@selector(registerForRemoteNotifications)];
}

- (BOOL)notificationTypeConfigured:(id)notificationOption {
    if ([notificationOption isKindOfClass:[NSString class]]) {
        if ([notificationOption isEqualToString:@"true"]) {
            return YES;
        }
    }
    
    if ([notificationOption boolValue]) {
        return YES;
    }
    
    return NO;
}

/*
- (void)isEnabled:(NSMutableArray *)arguments withDict:(NSMutableDictionary *)options {
    UIRemoteNotificationType type = [[UIApplication sharedApplication] enabledRemoteNotificationTypes];
    NSString *jsStatement = [NSString stringWithFormat:@"navigator.PushPlugin.isEnabled = %d;", type != UIRemoteNotificationTypeNone];
    NSLog(@"JSStatement %@",jsStatement);
}
*/

- (void)didRegisterForRemoteNotificationsWithDeviceToken:(NSData *)deviceToken {

    NSMutableDictionary *results = [NSMutableDictionary dictionary];
    NSString *token = [[[[deviceToken description] stringByReplacingOccurrencesOfString:@"<"withString:@""]
                        stringByReplacingOccurrencesOfString:@">" withString:@""]
                       stringByReplacingOccurrencesOfString: @" " withString: @""];
    [results setValue:token forKey:@"deviceToken"];
    
    #if !TARGET_IPHONE_SIMULATOR
    
    #if __IPHONE_OS_VERSION_MAX_ALLOWED < 80000
    
    // TODO: Ideally, you'd want to have a scheme where you override
    // [[UIApplication sharedApplication].delegate application:didRegisterUserNotificationSettings:] for iOS 8 and
    // above, and get the equivalent UIRemoteNotificationType values there (UIUserNotificationType).
    // However, given that these values aren't currently utilized from this method, that's an exercise for
    // another day.
    
        // Get Bundle Info for Remote Registration (handy if you have more than one app)
        [results setValue:[[[NSBundle mainBundle] infoDictionary] objectForKey:@"CFBundleDisplayName"] forKey:@"appName"];
        [results setValue:[[[NSBundle mainBundle] infoDictionary] objectForKey:@"CFBundleVersion"] forKey:@"appVersion"];
        
        // Check what Notifications the user has turned on.  We registered for all three, but they may have manually disabled some or all of them.
        NSUInteger rntypes = [[UIApplication sharedApplication] enabledRemoteNotificationTypes];

        // Set the defaults to disabled unless we find otherwise...
        NSString *pushBadge = @"disabled";
        NSString *pushAlert = @"disabled";
        NSString *pushSound = @"disabled";

        // Check what Registered Types are turned on. This is a bit tricky since if two are enabled, and one is off, it will return a number 2... not telling you which
        // one is actually disabled. So we are literally checking to see if rnTypes matches what is turned on, instead of by number. The "tricky" part is that the
        // single notification types will only match if they are the ONLY one enabled.  Likewise, when we are checking for a pair of notifications, it will only be
        // true if those two notifications are on.  This is why the code is written this way
        if(rntypes & UIRemoteNotificationTypeBadge){
            pushBadge = @"enabled";
        }
        if(rntypes & UIRemoteNotificationTypeAlert) {
            pushAlert = @"enabled";
        }
        if(rntypes & UIRemoteNotificationTypeSound) {
            pushSound = @"enabled";
        }

        [results setValue:pushBadge forKey:@"pushBadge"];
        [results setValue:pushAlert forKey:@"pushAlert"];
        [results setValue:pushSound forKey:@"pushSound"];

        // Get the users Device Model, Display Name, Token & Version Number
        UIDevice *dev = [UIDevice currentDevice];
        [results setValue:dev.name forKey:@"deviceName"];
        [results setValue:dev.model forKey:@"deviceModel"];
        [results setValue:dev.systemVersion forKey:@"deviceSystemVersion"];
    
    #endif

		[self successWithMessage:[NSString stringWithFormat:@"%@", token]];
    #endif
}

- (void)didFailToRegisterForRemoteNotificationsWithError:(NSError *)error
{
	[self failWithMessage:@"" withError:error];
}

- (void)notificationReceived {
    NSLog(@"Notification received");

    if (notificationMessage && self.callback)
    {
        NSMutableDictionary *dict = [[NSMutableDictionary alloc] init];
        dict[@"event"] = @"message";
        dict[@"payload"] = notificationMessage;
        dict[@"foreground"] = [NSNumber numberWithBool:isInline];
        if (isInline) {
            isInline = NO;
        }
        NSString * jsCallBack = [NSString stringWithFormat:@"%@(%@);", self.callback, [PushPlugin JSONRepresentation:dict]];
        [self.webView stringByEvaluatingJavaScriptFromString:jsCallBack];
        
        self.notificationMessage = nil;
    }
}

+ (NSString*)JSONRepresentation:(id)obj {
    NSString *result = nil;
    
    NSData *jsonData = [self JSONDataRepresentation:obj];
    if (nil != jsonData) {
        result = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    }
    
    return result;
}

+(NSData*)JSONDataRepresentation:(id)obj {
    NSError *err = nil;
    NSData *jsonData = nil;
    
    if (nil != obj) {
        NSJSONWritingOptions options = 0;
        jsonData = [NSJSONSerialization dataWithJSONObject:obj
                                                   options:options
                                                     error:&err
                    ];
        
        
    }
    return  jsonData;
}

- (void)setApplicationIconBadgeNumber:(CDVInvokedUrlCommand *)command {

    self.callbackId = command.callbackId;

    NSMutableDictionary* options = [command.arguments objectAtIndex:0];
    int badge = [[options objectForKey:@"badge"] intValue] ?: 0;

    [[UIApplication sharedApplication] setApplicationIconBadgeNumber:badge];

    [self successWithMessage:[NSString stringWithFormat:@"app badge count set to %d", badge]];
}
-(void)successWithMessage:(NSString *)message
{
    if (self.callbackId != nil)
    {
        CDVPluginResult *commandResult = [CDVPluginResult resultWithStatus:CDVCommandStatus_OK messageAsString:message];
        [self.commandDelegate sendPluginResult:commandResult callbackId:self.callbackId];
    }
}

-(void)failWithMessage:(NSString *)message withError:(NSError *)error
{
    NSString        *errorMessage = (error) ? [NSString stringWithFormat:@"%@ - %@", message, [error localizedDescription]] : message;
    CDVPluginResult *commandResult = [CDVPluginResult resultWithStatus:CDVCommandStatus_ERROR messageAsString:errorMessage];
    
    [self.commandDelegate sendPluginResult:commandResult callbackId:self.callbackId];
}

@end
