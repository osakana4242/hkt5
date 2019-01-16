
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

namespace Osk42 {
	public class AppCore : MonoBehaviour {

		public Data data;
		public GameObject World => gameObject;

		void Start() {
			var btnRect = new Rect(-0.15f, -0.15f, 0.3f, 0.3f);
			var blastRect = new Rect(-6f, -6f, 12f, 12f);
			var cellCount = new Vector2(6, 6);
			data.btn.SetActive(true);
			for (int yi = 0; yi < cellCount.y; yi++) {
				for (int xi = 0; xi < cellCount.x; xi++) {
					var b = GameObject.Instantiate(data.btn, data.btn.transform.parent);

					b.transform.localPosition = new Vector3(
						btnRect.y + (btnRect.width / cellCount.y) * (yi + 0.5f),
						0.02f,
						btnRect.x + (btnRect.height / cellCount.x) * (xi + 0.5f))
					;

					Vector3 blastPos = new Vector3(
						blastRect.x + (blastRect.width / cellCount.y) * (yi + 0.5f),
						0f,
						blastRect.y + (blastRect.height / cellCount.x) * (xi + 0.5f)
					);

					var btn = b.GetComponentInChildren<Button>();
					btn.OnPointerDownAsObservable().
						Subscribe(_ => {
							Debug.Log("click");
							var blast = GameObject.Instantiate(data.blast, blastPos, Quaternion.identity, World.transform);
							blast.FixedUpdateAsObservable().
								First().
								Subscribe(_2 => {
									var rb = data.human.GetComponent<Rigidbody>();
									var sqrMagnitude = Vector3.SqrMagnitude(blastPos - rb.position);
									if (sqrMagnitude < 5f * 5f) {
										rb.AddExplosionForce(1000f, blastPos + new Vector3(0f, -1f, 0f), 5f);
										data.progress.slowTime = MathHelper.toMsec(2f);
										if (data.humanData.earthTime <= 0f) {
											data.humanData.combo++;
										} else {
											data.humanData.combo = 1;
										}
										data.humanData.score += 100 * data.humanData.combo;
									}
								});

							GameObject.Destroy(blast, 1f);
						}).
						AddTo(gameObject);
				}
			}
			data.btn.SetActive(false);
		}

		void FixedUpdate() {
			var humanData = data.humanData;
			var human = data.human;
			var rb = human.GetComponent<Rigidbody>();

			var pos = rb.position;

			var isEarth = 0.1f <= humanData.earthTime;
			if (rb.position.y <= 0.01f) {
				humanData.earthTime += MathHelper.toMsec(Time.fixedDeltaTime);
			} else {
				humanData.earthTime = 0;
			}

			if (isEarth) {
				var angle = 90f;
				var tpos = pos;
				tpos.x = Mathf.Sin(Time.fixedTime * angle * Mathf.Deg2Rad) * 4f;
				tpos.z = Mathf.Cos(Time.fixedTime * angle * Mathf.Deg2Rad) * 4f;
				pos = Vector3.MoveTowards(pos, tpos, 5f * Time.fixedDeltaTime);

				rb.position = pos;
			}
			if (0 < data.progress.slowTime) {
				data.progress.slowTime = Mathf.Max(0, data.progress.slowTime - MathHelper.toMsec(Time.fixedUnscaledDeltaTime));
			}
			if (0.2f <= pos.y) {
				Time.timeScale = 0.25f;
				Time.fixedDeltaTime = (1 / 60f) * Time.timeScale;
			} else {
				Time.timeScale = 1f;
				Time.fixedDeltaTime = (1 / 60f) * Time.timeScale;
			}
		}

		void Update() {
		}

		void OnGUI() {
			using (new GUILayout.AreaScope(new Rect(0, 0,320, 320))) {
				GUILayout.Label(string.Format("SCORE {0}", data.humanData.score));
				GUILayout.Label(string.Format("COMBO {0}", data.humanData.combo));
			}
		}

	}

	[System.Serializable]
	public class Data {
		public Progress progress = new Progress();
		public HumanData humanData = new HumanData();
		public GameObject btn;
		public GameObject blast;
		public GameObject human;
	}

	[System.Serializable]
	public class Progress {
		public int slowTime;
	}

	[System.Serializable]
	public class HumanData {
		public int earthTime = 0;
		public int combo = 0;
		public int score = 0;
	}

	public static class MathHelper {
		public static int toMsec(float dt) {
			if (dt == 0f) return 0;
			int i = (int)(dt * 1000);
			if (i != 0) return i;
			if (dt < 0f) return -1;
			return 1;
		}
	}
}
