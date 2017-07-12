using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Core.Debugging;

#if FACEBOOK
using Facebook.Unity;
#endif


namespace Core
{
	namespace Facebook
	{
		public class FacebookHelper
		{
			public const string GAMEREQUEST_APPREQUEST = "apprequest";
			public const string GAMEREQUEST_ASKFOR = "askfor";
			public const string GAMEREQUEST_SEND = "send";
			public const string GAMEREQUEST_TURN = "turn";
			private const string APP_LINK_ID = "https://fb.me/504893686374126";

			public static bool IsLoggedIn()
			{
				#if FACEBOOK
				return FB.IsLoggedIn;
				#else
				return false;
				#endif
			}

			public static bool IsInitialized()
			{
				#if FACEBOOK
				return FB.IsInitialized;
				#else
				return false;
				#endif
			}
				
			public static void LoginRead(Action failCallback, Action sucessCallback, List<string> permissions)
			{
				#if FACEBOOK
				if (!FacebookHelper.IsLoggedIn())
				{
					FB.LogInWithReadPermissions( permissions, 
						(result) => {

							Debugger.PrintDictionaryAsServerObject(result.ResultDictionary, "FacebookLogin", (int)SharedSystems.Systems.FACEBOOK_HELPER);

							if (result.Error == null && !result.Cancelled)
							{
								sucessCallback();
							}
							else
							{
								Debugger.Log("Login: FB.LogInWithReadPermissions " + Debugger.ServerObjectAsString(new Hashtable ((IDictionary)result.ResultDictionary)), Debugger.Severity.ERROR, (int)SharedSystems.Systems.FACEBOOK_HELPER);
								failCallback();
							}
						});

				}
				else
				{
					Debugger.Log("Login: already logged in", (int)SharedSystems.Systems.FACEBOOK_HELPER);
				}
				#endif
			}

			public static void LoginPublish(Action failCallback, Action sucessCallback)
			{
				#if FACEBOOK
				if (!FacebookHelper.IsLoggedIn())
				{
					FB.LogInWithPublishPermissions(new List<string> () {  "publish_actions" }, 
						(result) => {

							Debugger.PrintDictionaryAsServerObject(result.ResultDictionary, "FacebookLogin", (int)SharedSystems.Systems.FACEBOOK_HELPER);

							if (result.Error == null && !result.Cancelled)
							{
								sucessCallback();
							}
							else
							{
								Debugger.Log("Login: FB.LogInWithPublishPermissions " + Debugger.ServerObjectAsString(new Hashtable ((IDictionary)result.ResultDictionary)), Debugger.Severity.ERROR, (int)SharedSystems.Systems.FACEBOOK_HELPER);
								failCallback();
							}
						});

				}
				else
				{
					Debugger.Log("Login: already logged in", (int)SharedSystems.Systems.FACEBOOK_HELPER);
				}
				#endif
			}

			public static void GetLeaderboard(Action<string> gotLeaderboard, Action error)
			{
				#if FACEBOOK
				FB.API(FB.AppId + "/scores", HttpMethod.GET, (result) => {

					if (result.ResultDictionary.ContainsKey("error"))
					{
						Debugger.Log("get leaderboard error", (int)SharedSystems.Systems.FACEBOOK_HELPER);

						error();
					}
					else
					{
						Debugger.Log("get leaderboard success", (int)SharedSystems.Systems.FACEBOOK_HELPER);

						gotLeaderboard(result.RawResult);
					}
				});
				#endif
			}

			public static void PostLeaderboardScore(int score, Action success = null)
			{
				#if FACEBOOK
				Dictionary<string, string> dict = new Dictionary<string, string> ();

				dict.Add("score", score.ToString());

				FB.API("/me/scores", HttpMethod.POST, (result) => {

					if (result.ResultDictionary.ContainsKey("error"))
					{
						Debugger.Log("post leaderboard error", (int)SharedSystems.Systems.FACEBOOK_HELPER);
					}
					else
					{
						Debugger.Log("post leaderboard success", (int)SharedSystems.Systems.FACEBOOK_HELPER);

						if (success != null)
						{
							success();
						}
					}
				}, dict);
				#endif
			}

			#if FACEBOOK
			public static void RequestInvitableFriends(Action<IGraphResult> callback, Action<IGraphResult> failCallback)
			{
				RequestFriends("invitable_friends", callback, failCallback);
			}

			private static void RequestFriends(string friendsURI, Action<IGraphResult> callback, Action<IGraphResult> failCallback, int amount = 5000)
			{
				FB.API("/me/" + friendsURI + ("?limit=" + amount), HttpMethod.GET, (result) => {

					if (result.ResultDictionary.ContainsKey("error"))
					{
						failCallback(result);
					}
					else
					{
						callback(result);
					}
				});
			}
			#endif

			public static void RequestImageNormal(string id, Action<Texture2D> callback)
			{
				RequestImage(id, "normal", callback);
			}

			public static void RequestImageLarge(string id, Action<Texture2D> callback)
			{
				RequestImage(id, "large", callback);
			}

			private static void RequestImage(string id, string size, Action<Texture2D> callback)
			{
				#if FACEBOOK
				FB.API(id + "/picture?type=" + size, HttpMethod.GET, (result) => {
					callback(result.Texture);
				});
				#endif
			}

			public static void RequestCover(string id, Action<Texture2D> callback = null)
			{
				#if FACEBOOK
				FB.API(id + "/?fields=cover", HttpMethod.GET, (result) => {

					if (result.ResultDictionary.ContainsKey("cover"))
					{
						IDictionary<string,object> cover = result.ResultDictionary["cover"] as IDictionary<string,object>;

						string coverUrl = cover["source"] as string;

						Utils.WebImage.Request(coverUrl, ((Utils.WebImage.ServerImage obj) => {
							callback(obj.texture as Texture2D);
						}));
					}
				});
				#endif
			}

			public static string GetImageMediumUrl(string id)
			{
				return "https" + "://graph.facebook.com/" + id + "/picture?type=med";
			}

			public static string GetImageLargeUrl(string id)
			{
				return "https" + "://graph.facebook.com/" + id + "/picture?type=large";
			}



			#if FACEBOOK
			public static void AppRequestGameInvite(string recipienteMessage, string clientMessage, List<string> facebookIds, Action<IAppRequestResult> callback, Action errorCallback = null)
			{
				FB.AppRequest(
					recipienteMessage,
					facebookIds, null, null, null, GAMEREQUEST_APPREQUEST, clientMessage,
					delegate (IAppRequestResult result) {
						if (string.IsNullOrEmpty(result.Error))
						{
							Debugger.PrintDictionaryAsServerObject(result.ResultDictionary, "AppRequestGameInvite", (int)SharedSystems.Systems.FACEBOOK_HELPER);

							callback(result);
						}
						else
						{
							if (errorCallback != null)
							{
								errorCallback();
							}
						}
					}
				);
			}
			
			public static void GetAppRequests(Action<IGraphResult> callback, Action failcallback = null)
			{
				FB.API("me/apprequests", HttpMethod.GET, (result) => {
					Debugger.PrintDictionaryAsServerObject(result.ResultDictionary, "CheckkAppRequest", (int)SharedSystems.Systems.FACEBOOK_HELPER);

					if (!result.ResultDictionary.ContainsKey("error"))
					{
						callback(result);
					}
					else
					{
						if (failcallback != null)
						{
							failcallback();
						}
					}
				});
			}
			#endif


			public static void ShareLink(Uri contentURL, string contentTitle, string contentDescription, Uri photoURL, Action callback)
			{
				#if FACEBOOK
				FB.ShareLink(contentURL, contentTitle, contentDescription, photoURL, delegate (IShareResult result) {
					if (string.IsNullOrEmpty(result.Error))
					{
						Debugger.PrintDictionaryAsServerObject(result.ResultDictionary, "AppRequestGameInvite", (int)SharedSystems.Systems.FACEBOOK_HELPER);
						callback();
					}
					else
					{
						callback();
					}
				});
				#endif
			}

			public static void SendAppInvite(string imageURL, Action<bool> callback)
			{
				#if FACEBOOK
				FB.Mobile.AppInvite(new System.Uri (APP_LINK_ID), new System.Uri (imageURL), (result) => {
					if (string.IsNullOrEmpty(result.Error))
					{
						Debugger.PrintDictionaryAsServerObject(result.ResultDictionary, "SendAppInvite", (int)SharedSystems.Systems.FACEBOOK_HELPER);
					}
					callback(result.Cancelled);
				});
				#endif
			}

			public static void DeleteAppRequests(string appRequest)
			{
				#if FACEBOOK
				FB.API(appRequest, HttpMethod.DELETE, (result) => {
					Debugger.PrintDictionaryAsServerObject(result.ResultDictionary, "CheckkAppRequest", (int)SharedSystems.Systems.FACEBOOK_HELPER);

					if (result.ResultDictionary.ContainsKey("error"))
					{
						Debugger.PrintDictionaryAsServerObject(result.ResultDictionary, "Error deleting app request", (int)SharedSystems.Systems.FACEBOOK_HELPER);
					}

				});
				#endif
			}


			#if FACEBOOK
			public static void AppRequestGift(string openGraphId, string recipienteMessage, string clientMessage, List<string> facebookIds, Action<IAppRequestResult> callback, Action errorCallback = null)
			{
				FB.AppRequest(
					recipienteMessage, OGActionType.SEND, openGraphId, facebookIds, GAMEREQUEST_SEND, clientMessage,
					delegate (IAppRequestResult result) {
						if (string.IsNullOrEmpty(result.Error))
						{
							Debugger.PrintDictionaryAsServerObject(result.ResultDictionary, "AppRequestGameInvite", (int)SharedSystems.Systems.FACEBOOK_HELPER);

							callback(result);
						}
						else
						{
							if (errorCallback != null)
							{
								errorCallback();
							}
						}
					}
				);
			}

			public static void AppRequestAskFor(string openGraphId, string recipienteMessage, string clientMessage, List<string> facebookIds, Action<IAppRequestResult> callback, Action errorCallback = null)
			{
				FB.AppRequest(
					recipienteMessage, facebookIds, null, null, default(int?), GAMEREQUEST_ASKFOR, clientMessage,
					delegate (IAppRequestResult result) {
						if (string.IsNullOrEmpty(result.Error))
						{
							Debugger.PrintDictionaryAsServerObject(result.ResultDictionary, "AppRequestGameInvite", (int)SharedSystems.Systems.FACEBOOK_HELPER);

							callback(result);
						}
						else
						{
							if (errorCallback != null)
							{
								errorCallback();
							}
						}
					}
				);
			}

			public static void AppRequestYourTurn(string recipienteMessage, string clientMessage, List<string> facebookIds, Action<IAppRequestResult> callback, Action errorCallback = null)
			{
				FB.AppRequest(recipienteMessage, facebookIds, null, null, null, GAMEREQUEST_TURN, clientMessage,
					delegate (IAppRequestResult result) {
						if (string.IsNullOrEmpty(result.Error))
						{
							Debugger.PrintDictionaryAsServerObject(result.ResultDictionary, "AppRequestYourTurn", (int)SharedSystems.Systems.FACEBOOK_HELPER);

							if (callback != null)
							{
								callback(result);
							}
						}
						else
						{
							if (errorCallback != null)
							{
								errorCallback();
							}
						}
					}
				);
			}
			#endif
		}
	}
}