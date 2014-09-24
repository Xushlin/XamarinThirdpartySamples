using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.IO;
using AzurePortable;

namespace iOSTodo
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main (args, null, "AppDelegate");
		}
	}

	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		public static AppDelegate Current { get; private set; }
		public TodoItemManager TaskMgr { get; set; }
		
		public override UIWindow Window {
			get;
			set;
		}

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			Current = this;

			// SQL
//			var sqliteFilename = "TaskDB.db3";
//			string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
//			string libraryPath = Path.Combine (documentsPath, "..", "Library"); // Library folder
//			var path = Path.Combine(libraryPath, sqliteFilename);
//			var conn = new Connection(path);
//			TaskMgr = new TodoItemManager(conn);

			// AZURE
			TaskMgr = new TodoItemManager(AzureStorageImplementation.DefaultService);


			// PUSH
			UIRemoteNotificationType notificationTypes = 
				 UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
			UIApplication.SharedApplication
				.RegisterForRemoteNotificationTypes(notificationTypes); 

			return true;
		}


		public string DeviceToken { get; set; }
		public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
		{
			string trimmedDeviceToken = deviceToken.Description;
			if (!string.IsNullOrWhiteSpace(trimmedDeviceToken))
			{
				trimmedDeviceToken = trimmedDeviceToken.Trim('<');
				trimmedDeviceToken = trimmedDeviceToken.Trim('>');
			}
			DeviceToken = trimmedDeviceToken;
		}
		public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
		{
			Console.WriteLine(userInfo.ToString());
			NSObject inAppMessage;

			bool success = userInfo.TryGetValue(new NSString("inAppMessage"), out inAppMessage);

			if (success)
			{
				var alert = new UIAlertView("Got push notification", inAppMessage.ToString(), null, "OK", null);
				alert.Show();
			}
		}
	}
}

