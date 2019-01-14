
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

namespace Osk42 {
	public class AppCore : MonoBehaviour {

		public Data data;

		void Start() {
			var rect = new Rect(-0.15f, -0.15f, 0.3f, 0.3f);
			var sizeMax = new Vector2(6, 6);
			data.btn.SetActive(true);
			for (int yi = 0; yi < sizeMax.y; yi++) {
				for (int xi = 0; xi < sizeMax.x; xi++) {
					var b = GameObject.Instantiate(data.btn, data.btn.transform.parent);

					b.transform.localPosition = new Vector3(
						rect.y + (rect.width / sizeMax.y) * (yi + 0.5f),
						0.0125f,
						rect.x + (rect.height / sizeMax.x) * (xi + 0.5f))
					;
					var btn = b.GetComponentInChildren<Button>();
					btn.OnPointerDownAsObservable().
						Subscribe(_ => {
							Debug.Log("click");
						}).
						AddTo(gameObject);
				}
			}
			data.btn.SetActive(false);
		}

		void FixedUpdate() {
			var pos = new Vector3();
			var angle = 90f;
			pos.x = Mathf.Sin(Time.fixedTime * angle * Mathf.Deg2Rad) * 4f;
			pos.z = Mathf.Cos(Time.fixedTime * angle * Mathf.Deg2Rad) * 4f;
			data.human.transform.position = pos;
		}

		void Update() {
		}

		[System.Serializable]
		public class Data {
			public GameObject btn;
			public GameObject human;
		}
	}
}
