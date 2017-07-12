using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Core.Debugging;

namespace Core
{
	namespace Utils
	{
		public struct ProgressResponse
		{
			public float progress;
			public bool isDone;
		}

		public class WebImage
		{
			private const float DEFAULT_CACHE_TIMEOUT = 4320f;
			private const int ANALYTICS_TRIGGER_SIZE_DEFAULT = 204800;

			public struct ServerImage
			{
				public Texture2D texture;
				public bool isCached;
			}

			private struct CacheImage
			{
				public WeakReference weakReference;
				public bool isCached;
				public System.DateTime timeStamp;
			}

			private static Dictionary<ulong, CacheImage> s_cachedImages = new Dictionary<ulong, CacheImage> ();
			private static Dictionary<string, Dictionary<ulong, UnityWebRequest>> s_onGoingRequests = new Dictionary<string, Dictionary<ulong, UnityWebRequest>> ();

			public const string FB_IMAGE_URL = "http://graph.facebook.com/";
			public const string FB_IMAGE_SIZE_LARGE = "/picture?type=large";
			public const string DEFAULT_URL_CANCEL_KEY = "default_url";
			public const string DEFAULT_DISK_CANCEL_KEY = "default_disk";
			private const string ORIENTATION_SUBSTRING = "?ori";

			private const int MAX_SIZE_BYTES = 1048576;
			// 1MB in bytes
			private const int MAX_IO_OPERATIONS_PER_FRAME = 1;
			private static int s_IO_OperationsCounter = 0;

			private static string m_prefixURL = null;

			public static void SetPrefixURL(string prefix)
			{
				m_prefixURL = prefix;
			}

			public static void Request(string url, Action<ServerImage> successCallback, Action<string> failedCallback = null, Action<ProgressResponse> progressCallback = null, string cancelKey = DEFAULT_URL_CANCEL_KEY, float cacheTimeOutInSecs = DEFAULT_CACHE_TIMEOUT, float requestTimeout = -1)
			{
				if (m_prefixURL != null && !url.Contains("http"))
				{
					url = m_prefixURL + url;
				}

				CacheImage cacheImage;
				bool useMemoryCache = false;
				bool imageOnDisk = false;
				ulong codedURL = GetHashCodeInt64(url);

				Debugger.Log("WebImage::Request key: " + cancelKey + " url: " + url, (int)SharedSystems.Systems.WEB_IMAGE);
				Debugger.Assert(!string.IsNullOrEmpty(cancelKey), "WebImage:;Request() called with no cancel key, ignoring request");
				if (string.IsNullOrEmpty(cancelKey))
				{
					return;
				}

				if (s_cachedImages.TryGetValue(codedURL, out cacheImage))
				{
					useMemoryCache = ((cacheImage.weakReference.Target as Texture2D) != null);
				}

				Debugger.Log("WebImage::Request useMemoryCache: " + useMemoryCache, (int)SharedSystems.Systems.WEB_IMAGE);
				if (useMemoryCache)
				{	
					cacheImage.isCached = true;
					cacheImage.timeStamp = System.DateTime.UtcNow;

					if (progressCallback != null)
					{
						ProgressResponse progress = new ProgressResponse ();
						progress.isDone = true;
						progress.progress = (float)1.0;
						progressCallback(progress);
					}

					if (successCallback != null)
					{	
						ServerImage image = new ServerImage ();

						image.texture = cacheImage.weakReference.Target as Texture2D;
						image.isCached = true;

						Debugger.Log("WebImage::Request successCallback key: " + cancelKey + " url: " + url, (int)SharedSystems.Systems.WEB_IMAGE);
						successCallback(image);
					}
				}
				else
				{
					bool doRequest = true;

					string filePath = Application.temporaryCachePath + "/CACHE" + codedURL.ToString();

					if (System.IO.File.Exists(filePath))
					{
						//check how old
						System.DateTime written = File.GetLastWriteTimeUtc(filePath);
						System.DateTime now = System.DateTime.UtcNow;
						double totalSeconds = now.Subtract(written).TotalSeconds;

						if (totalSeconds < cacheTimeOutInSecs)
						{
							doRequest = false;

							CoroutineHelper.Instance.Run(LoadImageFromDiskInternal(filePath, codedURL, successCallback, cancelKey));
						}
					}

					if (doRequest)
					{
						CoroutineHelper.Instance.Run(GetImage(url, codedURL, successCallback, progressCallback, failedCallback, cancelKey, imageOnDisk, requestTimeout));
					}
				}
			}

			public static void LoadImageFromDisk(string filePath, Action<ServerImage> successCallback, Action<string> failedCallback = null, Action<ProgressResponse> progressCallback = null, string cancelKey = DEFAULT_DISK_CANCEL_KEY)
			{
				Debugger.Log("LoadImageFromDisk filePath: " + filePath, (int)SharedSystems.Systems.WEB_IMAGE);
				ulong codedURL = GetHashCodeInt64(filePath);

				#if !UNITY_EDITOR
				#if UNITY_IPHONE
			int foundSuffix = filePath.IndexOf(ORIENTATION_SUBSTRING);
			if (foundSuffix != -1)
			{
			filePath = filePath.Substring(0, foundSuffix);
			}
				#elif UNITY_ANDROID
			filePath = filePath.Replace("file://","");
				#endif
				#endif


				CoroutineHelper.Instance.Run(LoadImageFromDiskInternal(filePath, codedURL, successCallback, cancelKey));
			}

			private static IEnumerator LoadImageFromDiskInternal(string filePath, ulong codedURL, Action<ServerImage> successCallback, string cancelKey)
			{
				Debugger.Log("LoadImageFromDiskInternal filePath: " + filePath, (int)SharedSystems.Systems.WEB_IMAGE);
				while (s_IO_OperationsCounter >= MAX_IO_OPERATIONS_PER_FRAME)
				{
					yield return new WaitForEndOfFrame ();
				}

				AddRequest(cancelKey, codedURL, null);

				s_IO_OperationsCounter++;
				byte[] bytes = File.ReadAllBytes(filePath);
				Texture2D tex = new Texture2D (2, 2, TextureFormat.ARGB32, false);
				tex.LoadImage(bytes);

				FinaliseTexture(filePath, codedURL, tex, successCallback, cancelKey);

				yield return new WaitForEndOfFrame ();

				s_IO_OperationsCounter--;
			}

			private static void FinaliseTexture(string url, ulong codedURL, Texture2D tex, Action<ServerImage> successCallback, string cancelKey)
			{
				Debugger.Log("FinaliseTexture key: " + cancelKey + " url: " + url, (int)SharedSystems.Systems.WEB_IMAGE);
				CacheImage newCacheImage = new CacheImage ();
				WeakReference weakReference = new WeakReference (tex);

				newCacheImage.weakReference = weakReference;
				newCacheImage.isCached = false;
				newCacheImage.timeStamp = System.DateTime.UtcNow;
				if (RemoveRequest(cancelKey, codedURL))
				{
					try
					{
						ServerImage image = new ServerImage ();

						image.texture = newCacheImage.weakReference.Target as Texture2D;
						image.isCached = false;

						Debugger.Log("FinaliseTexture successCallback key: " + cancelKey + " url: " + url, (int)SharedSystems.Systems.WEB_IMAGE);
						successCallback(image);
					}
					catch (Exception ex)
					{
						Debugger.Error("FinaliseTexture Error in success callback (" + cancelKey + "): " + ex.Message, (int)SharedSystems.Systems.WEB_IMAGE);
					}
				}
				else
				{
					Debugger.Log("FinaliseTexture RemoveRequestFailed url: " + url, (int)SharedSystems.Systems.WEB_IMAGE);
				}

				AddImageToCache(newCacheImage, codedURL);
			}


			private static IEnumerator GetImage(string url, ulong codedURL, Action<ServerImage> successCallback, Action<ProgressResponse> progressCallback, Action<string> failedCallback, string cancelKey, bool imageOnDisk, float requestTimeout)
			{
				Debugger.Log("GetImage() key: " + cancelKey + " url: " + url, (int)SharedSystems.Systems.WEB_IMAGE);

				WWW www = new WWW (url);

				AddRequest(cancelKey, codedURL, null);
				Texture2D texture = null;
				float timer = 0f;

				if (www.isDone || texture != null)
				{
					Debugger.Error("GetImage() www.isDone is true before we get to the while. Forcing it through.");
				}
					
				bool forceEnterWhile = true; // Force into the while at least once
				while (forceEnterWhile || (!www.isDone && texture == null))
				{
					forceEnterWhile = false;

					yield return new WaitForFixedUpdate ();
					timer += Time.deltaTime;

					if (progressCallback != null)
					{
						ProgressResponse progress = new ProgressResponse ();
						progress.isDone = www.isDone;
						progress.progress = www.progress;
						progressCallback(progress);
					}

					if (!string.IsNullOrEmpty(www.error))
					{
						Debugger.Log(www.error + "\nurl: " + url, (int)SharedSystems.Systems.WEB_IMAGE);
						RemoveRequest(cancelKey, codedURL);
						if (failedCallback != null)
						{
							failedCallback(www.error);
						}

						yield break;
					}

					if (www.isDone)
					{
						Debugger.Log("www.isDone: " + url, (int)SharedSystems.Systems.WEB_IMAGE);
						texture = www.texture;
						FinaliseTexture(url, codedURL, texture, successCallback, cancelKey);

						if (!imageOnDisk)
						{
							yield return SaveImage(www.bytes, codedURL, cancelKey);
						}
						
						RemoveRequest(cancelKey, codedURL);
						yield break;
					}
					
					if (requestTimeout != -1 && timer > requestTimeout)
					{
						Debugger.Log("Image download timed out", (int)SharedSystems.Systems.WEB_IMAGE);

						RemoveRequest(cancelKey, codedURL);
						if (failedCallback != null)
						{
							failedCallback("timeout");
						}

						yield break;
					}
				}
			}

			private static IEnumerator SaveImage(Byte[] data, ulong codedURL, string cancelKey)
			{
				while (s_IO_OperationsCounter >= MAX_IO_OPERATIONS_PER_FRAME)
				{
					yield return new WaitForEndOfFrame ();
				}

				s_IO_OperationsCounter++;

				string filePath = Application.temporaryCachePath + "/CACHE" + codedURL.ToString();

				File.WriteAllBytes(filePath, data);

				yield return new WaitForEndOfFrame ();

				s_IO_OperationsCounter--;
			}

			private static IEnumerator UpdateImageProgress(UnityWebRequest webRequest, Action<ProgressResponse> progressCallback, Action<string> failCallback, bool ignoreFileSize)
			{
				bool eventSent = false;

				while (!webRequest.isDone)
				{
					yield return new WaitForFixedUpdate ();

					int contentLength = 0;

					int.TryParse(webRequest.GetResponseHeader("Content-Length"), out contentLength);

					if (!eventSent && (contentLength > ANALYTICS_TRIGGER_SIZE_DEFAULT || (int)webRequest.downloadedBytes > ANALYTICS_TRIGGER_SIZE_DEFAULT))
					{
						eventSent = true;

						Core.Analytics.Dispatcher.Param urlParam = new Core.Analytics.Dispatcher.Param () {
							name = "url",
							value = webRequest.url
						};

						Core.Analytics.Dispatcher.Param contentLengthParam = new Core.Analytics.Dispatcher.Param () {
							name = "contentLength",
							value = contentLength
						};

						Core.Analytics.Dispatcher.Param bytesDownloadedParam = new Core.Analytics.Dispatcher.Param () {
							name = "downloadedBytes",
							value = webRequest.downloadedBytes
						};

						Core.Analytics.Dispatcher.Param[] paramsArray = new Core.Analytics.Dispatcher.Param[3] {
							urlParam,
							contentLengthParam,
							bytesDownloadedParam
						};

						Core.Analytics.Dispatcher.SendEvent(Core.Analytics.Events.IMAGE_OVER_SIZE_LIMIT, paramsArray);
					}

					if (!ignoreFileSize && (contentLength > MAX_SIZE_BYTES || webRequest.downloadedBytes > MAX_SIZE_BYTES))
					{
						Debugger.Log("UpdateImageProgress abort file size webRequest.url: " + webRequest.url, (int)SharedSystems.Systems.WEB_IMAGE);
						webRequest.Abort();

						yield break;
					}
					else
					{
						if (progressCallback != null)
						{
							ProgressResponse progress = new ProgressResponse ();
							progress.isDone = webRequest.isDone;
							progress.progress = webRequest.downloadProgress;
							progressCallback(progress);
						}
					}
				}
			}

			private static void AddImageToCache(CacheImage image, ulong codedURL)
			{
				if (!s_cachedImages.ContainsKey(codedURL))
				{
					s_cachedImages.Add(codedURL, image);
				}
				else if (s_cachedImages[codedURL].weakReference.Target == null)
				{
					s_cachedImages[codedURL] = image;
				}
			}

			public static void CancelRequest(string key)
			{
				Debugger.Log("CancelRequest key: " + key, (int)SharedSystems.Systems.WEB_IMAGE);
				Dictionary<ulong, UnityWebRequest> values;
				if (s_onGoingRequests.TryGetValue(key, out values))
				{
					if (values != null)
					{
						foreach (UnityWebRequest request in values.Values)
						{
							if (request != null && !request.isDone)
							{
								request.Abort();
							}
						}
					}

					s_onGoingRequests.Remove(key);
				}
			}

			private static void AddRequest(string key, ulong codedURL, UnityWebRequest request)
			{
				Debugger.Log("AddRequest key: " + key, (int)SharedSystems.Systems.WEB_IMAGE);
				Dictionary<ulong, UnityWebRequest> values;
				if (!s_onGoingRequests.TryGetValue(key, out values))
				{
					values = new Dictionary<ulong, UnityWebRequest> ();
					s_onGoingRequests[key] = values;
				}

				if (values.ContainsKey(codedURL))
				{
					Debugger.Error("repeat request key: " + key + ", codedURL: " + codedURL);
				}

				values[codedURL] = request;
			}

			private static bool RemoveRequest(string key, ulong codedURL)
			{
				Debugger.Log("RemoveRequest key: " + key, (int)SharedSystems.Systems.WEB_IMAGE);
				Dictionary<ulong, UnityWebRequest> values;
				if (s_onGoingRequests.TryGetValue(key, out values))
				{
					if (values.ContainsKey(codedURL))
					{
						values.Remove(codedURL);

						if (values.Count == 0)
						{
							s_onGoingRequests.Remove(key);
						}

						return true;
					}
				}
				return false;	
			}

			public static void ClearCache()
			{
				s_cachedImages.Clear();
			}

			public static ulong GetHashCodeInt64(string input)
			{
				Debugger.Assert(!string.IsNullOrEmpty(input), "GetHashCodeInt64() - empty input passed in");

				var s1 = input.Substring(0, (int)(input.Length * 0.5f));
				var s2 = input.Substring((int)(input.Length * 0.5f));

				return (((ulong)s1.GetHashCode()) << 0x20 | (ulong)s2.GetHashCode());
			}
		}
	}
}