using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hammerplay.Utils.Karaoke {

	/// <summary>
	/// SSA/ASS Parser. Parses a karoke subtitle file.
	/// </summary>
	public class ASSParser {

		private List<SubtitleKaraokeBlock> _subtitles;

		public ASSParser(TextAsset textAsset) {
			Load(textAsset);
		}

		private void Load(TextAsset textAsset) {
			if (textAsset == null) {
				Debug.LogError("Subtitle file in null");
				return;
			}

			// Parse the text block, we only need the Events section, and futher split them into lines.
			string[] dialogues = textAsset.text.Split(new[] { "[Events]", "Dialogue:" }, StringSplitOptions.None);

			_subtitles = new List<SubtitleKaraokeBlock>();

			double fromTimeInSeconds = 0, toTimeInSeconds = 0;
			string currentDialogue = "";

			for (int i = 0; i < dialogues.Length; i++) {
				if (i > 1) {

					// Each line format
					// Format: Layer, Start, End, Style, Name, MarginL, MarginR, MarginV, Effect, Text
					// We need only "Start" - 1, "End" - 2 and "Text" - 9
					string[] parsedDialogue = dialogues[i].Split(',');

					
					// Extract the from time and to time and convert them to seconds.
					TimeSpan fromTime;

					if (TimeSpan.TryParse(parsedDialogue[1], out fromTime)) {
						TimeSpan toTime;
						if (TimeSpan.TryParse(parsedDialogue[2], out toTime)) {
							fromTimeInSeconds = fromTime.TotalSeconds;
							toTimeInSeconds = toTime.TotalSeconds;
						}
					}


					// Ensures the rest of text is added after the 10th occurence of ',' so that commas after the 10th occurence don't get ommited.
					for (int j = 9; j < parsedDialogue.Length; j++) {
						currentDialogue += parsedDialogue[j];
					}

					// remove the \k tags from the text.
					currentDialogue = currentDialogue.Replace("\\k", "");

					// Dictionary stores the from time(in seconds) of karoke hightlighting for each string. And gives substring index of where it starts in the line.
					Dictionary<double, int> karaokeTimings = new Dictionary<double, int>();

					// Parse the tag of duration and add the From time of the line to duration
					string karaokeDurationText = "";
					bool open = false;
					int substringIndex = 0;
					string cleanText = "";

					double karaokeStartTime = fromTimeInSeconds;

					foreach (char c in currentDialogue) {
						if (c == '{') {
							open = true;
						}

						if (open) {
							karaokeDurationText += c;
						}

						if (!open) {
							substringIndex++;
							cleanText += c;
						}

						if (c == '}') {
							open = false;
							karaokeDurationText = karaokeDurationText.Replace("{", "");
							karaokeDurationText = karaokeDurationText.Replace("}", "");
							
							karaokeStartTime += (double.Parse(karaokeDurationText) / 100);
							
							karaokeTimings.Add(karaokeStartTime, substringIndex);
							karaokeDurationText = "";
						}
					}

					_subtitles.Add(new SubtitleKaraokeBlock(fromTimeInSeconds, toTimeInSeconds, cleanText, karaokeTimings));

					currentDialogue = "";
				}
			}
		}

		/// <summary>
		/// Gets the substring of where the karaoke text should be highlighted based on time.
		/// </summary>
		/// <param name="time"></param>
		/// <param name="substringIndex"></param>
		/// <returns></returns>
		public SubtitleKaraokeBlock GetForTime(float time, out int substringIndex) {
			substringIndex = 0;

			if (_subtitles.Count > 0) {
				var subtitle = _subtitles[0];

				if (time >= subtitle.To) {
					_subtitles.RemoveAt(0);

					if (_subtitles.Count == 0)
						return null;

					subtitle = _subtitles[0];
				}

				if (subtitle.From > time)
					return SubtitleKaraokeBlock.Blank;

				substringIndex = subtitle.GetKaraokeSubstring(time);

				return subtitle;
			}

			return null;
		}

		public class SubtitleKaraokeBlock {

			private static SubtitleKaraokeBlock _blank;

			public static SubtitleKaraokeBlock Blank {
				get { return _blank ?? (_blank = new SubtitleKaraokeBlock(0, 0, string.Empty, new Dictionary<double, int>())); }
			}

			public double From { get; private set; }
			public double To { get; private set; }
			public string Text { get; private set; }

			private Dictionary<double, int> karaokeTimings;
			private double[] karaokeTimingKeys;

			public SubtitleKaraokeBlock(double from, double to, string text, Dictionary<double, int> karaokeTimings) {
				this.From = from;
				this.To = to;
				this.Text = text;
				this.karaokeTimings = karaokeTimings;

				CalculateKaraokeTimings();
			}

			/// <summary>
			/// Extracts the keys from the dictionary and store in them in array for easy fetching.
			/// Also offset the values (substringindex) one key up, and the replace the last one with the text last substring.
			/// </summary>
			private void CalculateKaraokeTimings() {
				karaokeTimingKeys = new double[karaokeTimings.Count];
				int keyIndex = 0;

				foreach (KeyValuePair<double, int> item in karaokeTimings) {
					karaokeTimingKeys[keyIndex] = item.Key;
					keyIndex++;
				}

				for (int i = 0; i < karaokeTimingKeys.Length; i++) {
					if (i < karaokeTimingKeys.Length - 1)
						karaokeTimings[karaokeTimingKeys[i]] = karaokeTimings[karaokeTimingKeys[i + 1]];
					else
						karaokeTimings[karaokeTimingKeys[i]] = Text.Length;
				}
			}

			/// <summary>
			/// Gets the substring index to be highlighted based on time.
			/// </summary>
			/// <param name="time"></param>
			/// <returns></returns>
			public int GetKaraokeSubstring(float time) {

				for (int i = 0; i < karaokeTimingKeys.Length; i++) {
					if (karaokeTimingKeys[i] > (double)time) {
						return karaokeTimings[karaokeTimingKeys[i]];
					}
				}

				return 0;
			}

			public override string ToString() {
				string returnString = "From:{0} To:{1}, {2}";
				return string.Format(returnString, From.ToString(), To.ToString(), Text);
			}
		}
	}
}


