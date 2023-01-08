using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace FirebirdGames.Utilities
{
    public enum Direction
    {
        UP,
        LEFT,
        DOWN,
        RIGHT,
        NONE
    }

    public static class Utils
    {

        public static void QuitApplication()
        {
#if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
        }

        public static bool IsPaused { get; private set; }
        /// <summary>
        /// To have animations continue playing, change the Update Mode of the Animator component to use Unscaled Time.
        /// For audio that should still play, set AudioSource.ignoreListenerPause=true
        /// </summary>
        public static void PauseApplication(bool pause, bool flagOnly=false)
        {
            IsPaused = pause;

            if (!flagOnly)
            {
                Time.timeScale = pause ? 0 : 1; //**This also prevents tweens from working**
                AudioListener.pause = pause;
            }
        }

        public static T GetRandom<T>(this List<T> collection)
        {
            int index = UnityEngine.Random.Range(0, collection.Count);
            return collection[index];
        }
        
        public static T PopRandom<T>(this List<T> collection)
        {
            int index = UnityEngine.Random.Range(0, collection.Count);
            T item = collection[index];
            collection.Remove(item);
            return item;
        }

        public static T GetWeightedRandom<T>(this List<T> collection, List<float> weights)
        {
            if (weights.Count != collection.Count)
            {
                Debug.LogError("GetWeightedRandom() - weights list does not match collection length");
                return default(T);
            }

            float totalWeight = weights.Sum();
            var roll = UnityEngine.Random.Range(0, totalWeight);

            for (int i = 0; i < collection.Count; i++)
            {
                var item = collection[i];
                var weight = weights[i];


                if (roll < weight)
                    return item;

                roll -= weight;
            }

            Debug.LogError("GetWeightedRandom() - something went wrong, did not return anything from the passed in collection");
            return default(T);
        }

        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n); 
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }

        public static Vector2 GetDirectionVector(Direction facingDir)
        {
            if (facingDir == Direction.DOWN) return Vector2.down;
            if (facingDir == Direction.UP) return Vector2.up;
            if (facingDir == Direction.RIGHT) return Vector2.right;
            if (facingDir == Direction.LEFT) return Vector2.left;

            return Vector2.zero;
        }

        public static Vector2 GetUISpaceSize(Camera cam, RectTransform elem)
        {
            Vector3[] e_wcorners = new Vector3[4];
            elem.GetWorldCorners(e_wcorners);

            Vector2 elem_minCorner = (Vector2)cam.WorldToScreenPoint(e_wcorners[0]);
            Vector2 elem_maxCorner = (Vector2)cam.WorldToScreenPoint(e_wcorners[2]);

            return new Vector2(elem_maxCorner.x - elem_minCorner.x, elem_maxCorner.y - elem_minCorner.y);
        }

        public static Vector2 GetWorldSpaceSize(Camera cam, RectTransform elem)
        {
            Vector3[] e_wcorners = new Vector3[4];
            elem.GetWorldCorners(e_wcorners);
            
            Vector2 elem_minCorner = (Vector2)(e_wcorners[0]);
            Vector2 elem_maxCorner = (Vector2)(e_wcorners[2]);
            return new Vector2(Mathf.Abs(elem_maxCorner.x - elem_minCorner.x), Mathf.Abs(elem_maxCorner.y - elem_minCorner.y));
        }

        public static Vector3 PositionInCanvasSpace(Canvas canvas, Vector3 position)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, (Vector2)position, canvas.worldCamera, out pos);
            return canvas.transform.TransformPoint((Vector3)pos);
        }
        
        public static RectTransform RectTransform(this GameObject c)
        {
            if (c == null)
                return null;
            return c.transform as RectTransform;
        }

        public static bool RectOverlaps(RectTransform rt1, RectTransform rt2)
        {
            return WorldRect(rt1).Overlaps(WorldRect(rt2), true);
        }

        public static Rect WorldRect(RectTransform rt)
        {
            Vector2 sizeDelta = rt.sizeDelta;
            float recTransWidth = sizeDelta.x * rt.lossyScale.x;
            float rectTransHeight = sizeDelta.y * rt.lossyScale.y;
            Vector3 pos = rt.position;
            return new Rect(pos.x - recTransWidth / 2f, pos.y - rectTransHeight / 2f, recTransWidth, rectTransHeight);
        }

        public enum TimeFormat
        {
            MIN_SEC,
            HR_MIN_SEC
        }
        public static string FormatAsTime(this float seconds, TimeFormat format = TimeFormat.MIN_SEC)
        {
            int timeInSec = Mathf.FloorToInt(seconds);
            int mins = Mathf.FloorToInt((float)timeInSec / 60f);
            int sec = timeInSec - (mins * 60);
            string minStr = "";
            if (mins < 10) minStr += "0";
            minStr += mins.ToString();
            string secStr = "";
            if (sec < 10) secStr += "0";
            secStr += sec.ToString();
            return $"{minStr}:{secStr}";
        }
        
        public static bool Approximately(this float f, float other)
        {
            return Mathf.Approximately(f, other);
        }
        
        public static int GetKeyboardHeight(bool includeInput)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
				using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					var unityPlayer = unityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer");
					var view = unityPlayer.Call<AndroidJavaObject>("getView");
			#if UNITY_2019_3_OR_NEWER
					var dialog = unityPlayer.Get<AndroidJavaObject>("mSoftInputDialog");
			#else
					var dialog = unityPlayer.Get<AndroidJavaObject>("b");
			#endif

					if (view == null || dialog == null)
						return 0;

					var decorHeight = 0;

					if (includeInput)
					{
						var decorView = dialog.Call<AndroidJavaObject>("getWindow").Call<AndroidJavaObject>("getDecorView");

						if (decorView != null)
							decorHeight = decorView.Call<int>("getHeight");
					}

					using (var rect = new AndroidJavaObject("android.graphics.Rect"))
					{
						view.Call("getWindowVisibleDisplayFrame", rect);
						return Display.main.systemHeight - rect.Call<int>("height") + decorHeight;
					}
				}
#else
            var height = Mathf.RoundToInt(TouchScreenKeyboard.area.height);
            return height >= Display.main.systemHeight ? 0 : height;
#endif
        }
        
        public static IEnumerable<FieldInfo> GetAllFields(Type t)
        {
            if (t == null)
                return Enumerable.Empty<FieldInfo>();

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | 
                                 BindingFlags.Static | BindingFlags.Instance | 
                                 BindingFlags.DeclaredOnly;
            return t.GetFields(flags).Concat(GetAllFields(t.BaseType));
        }
    }
}
