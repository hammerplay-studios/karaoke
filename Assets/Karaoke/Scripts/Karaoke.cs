using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

namespace Hammerplay.Utils.Karaoke {
	public class Karaoke : MonoBehaviour {

		[SerializeField]
		private TextAsset subtitleFile;

		[SerializeField]
		private bool playOnAwake;

		[SerializeField]
		private string highlightStartTag = "<Color=Red>";

		[SerializeField]
		private string highlightEndTag = "</Color>";

		private Text text;

		private UnityEvent onComplete;

		private void Awake() {
			text = GetComponent<Text>();

			if (text == null) {
				Debug.LogError("Can't find Text component in Gameobject, Karaoke needs Text Component");
				this.enabled = false;
			}
		}

		private void Start() {
			if (playOnAwake)
				StartSubtitle();
		}

		/// <summary>
		/// Starts the karaoke subtitles.
		/// </summary>
		public void StartSubtitle() {
			StartSubtitle(null);
		}

		/// <summary>
		/// Starts the karaoke subtitles.
		/// </summary>
		/// <param name="onComplete"></param>
		public void StartSubtitle(UnityEvent onComplete) {
			if (subtitleFile == null) {
				Debug.LogError("Need subtitle file, use a SSA/ASS file in .txt format");
				return;
			}

			StartCoroutine(Begin());
			this.onComplete = onComplete;
		}

		private IEnumerator Begin() {
			var parser = new ASSParser(subtitleFile);
			var startTime = Time.time;

			while (true) {
				var elasped = Time.time - startTime;

				int substringIndex = 0;
				var subtitle = parser.GetForTime(elasped, out substringIndex);

				if (subtitle != null) {
					string modifiedString = subtitle.Text;
					modifiedString = modifiedString.Insert(substringIndex, highlightEndTag);
					modifiedString = modifiedString.Insert(0, highlightStartTag);
					text.text = modifiedString;
				}
				else {

					if (onComplete != null)
						onComplete.Invoke();

					yield break;
				}

				yield return null;
			}
		}
	}
}
