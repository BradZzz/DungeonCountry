using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp
{


	public class Coroutines {
		public Coroutine coroutine { get; private set; }
		public object result;
		private IEnumerator target;
		public Coroutines(MonoBehaviour owner, IEnumerator target) {
			this.target = target;
			this.coroutine = owner.StartCoroutine(Run());
		}

		private IEnumerator Run() {
			while(target.MoveNext()) {
				result = target.Current;
				yield return result;
			}
		}

		//This moves a character from one position to the next
		public static IEnumerator smooth_move(Transform origin, Vector3 direction,float speed){
			float startime = Time.time;
			Vector3 start_pos = new Vector3(origin.position.x, origin.position.y, origin.position.z);
			Vector3 end_pos = direction;
			while (origin.position != end_pos) { 
				float move = Mathf.Lerp (0,1, (Time.time - startime) * speed);

				Vector3 position = origin.position;

				position.x += (end_pos.x - start_pos.x) * move;
				position.y += (end_pos.y - start_pos.y) * move;

				if (start_pos.x > end_pos.x && origin.position.x < end_pos.x) {
					position.x = end_pos.x;
				}

				if (start_pos.x < end_pos.x && origin.position.x > end_pos.x) {
					position.x = end_pos.x;
				}

				if (start_pos.y > end_pos.y && origin.position.y < end_pos.y) {
					position.y = end_pos.y;
				}

				if (start_pos.y < end_pos.y && origin.position.y > end_pos.y) {
					position.y = end_pos.y;
				}

				origin.position = position;
			}
			yield return null;
		}

		//This moves a character from one position to the next
		/*public static IEnumerator smooth_move(Transform origin, Vector3 direction, float speed, pAttackCallback callback, List<Transform> playerUnits){
			float startime = Time.time;
			Vector3 start_pos = new Vector3(origin.position.x, origin.position.y, origin.position.z);
			Vector3 end_pos = direction;
			while (origin.position != end_pos && ((Time.time - startime)*speed) < 1f) { 
				float move = Mathf.Lerp (0,1, (Time.time - startime) * speed);

				Vector3 position = origin.position;

				position.x += (end_pos.x - start_pos.x) * move;
				position.y += (end_pos.y - start_pos.y) * move;

				if (start_pos.x > end_pos.x && origin.position.x < end_pos.x) {
					position.x = end_pos.x;
				}

				if (start_pos.x < end_pos.x && origin.position.x > end_pos.x) {
					position.x = end_pos.x;
				}

				if (start_pos.y > end_pos.y && origin.position.y < end_pos.y) {
					position.y = end_pos.y;
				}

				if (start_pos.y < end_pos.y && origin.position.y > end_pos.y) {
					position.y = end_pos.y;
				}

				origin.position = position;
			}
			callback (origin, playerUnits);
			yield return null;

		}*/

		public static IEnumerator delay(float delay, endTurnCallback callback)
		{
			yield return new WaitForSeconds(delay);
			callback (true);
		}

		public delegate void endTurnCallback(bool pTurn);

		public delegate void pAttackCallback(Transform ai, List<Transform> playerUnits);

		public static bool checkRange(Vector2 pos, Vector2 sqr, int range){
			return Math.Abs(pos.x - sqr.x) + Math.Abs(pos.y - sqr.y) <= range;
		}

		public static bool hasParentVector3(Vector3 child){
			foreach (GameObject children in GameObject.FindGameObjectsWithTag("Unit")) {
				if (children.transform.position.x == child.x && children.transform.position.y == child.y) {
					return true;
				}
			}
			foreach (GameObject children in GameObject.FindGameObjectsWithTag("Obstacle")) {
				if (children.transform.position.x == child.x && children.transform.position.y == child.y) {
					return true;
				}
			}
			return false;
		}

		public static bool V3Equal(Vector3 a, Vector3 b){
			return Vector3.SqrMagnitude(a - b) < 0.0001;
		}

		public static bool hasParent(Transform child){
			return hasParentVector3(child.transform.position);
		}

		public static GameObject findUnitParent(Point3 child){
			foreach (GameObject children in GameObject.FindGameObjectsWithTag("Unit")) {
				Debug.Log ("Unit: " + children.name + " Position: " + children.transform.position.ToString());
				if (child.Equals(children.transform.position)) {
					return children;
				}
			}
			//This is only for units, not for obstacles
			foreach (GameObject children in GameObject.FindGameObjectsWithTag("Obstacle")) {
				if (child.Equals(children.transform.position)) {
					return children;
				}
			}
			return null;
		}

		public static void ShuffleArray<T>(T[] arr) {
			for (int i = arr.Length - 1; i > 0; i--) {
				int r = UnityEngine.Random.Range(0, i);
				T tmp = arr[i];
				arr[i] = arr[r];
				arr[r] = tmp;
			}
		}

		//Vector3 equals and contains both suck ass. need to write them myself
		public static bool containsPoint(List<Vector3> obs, Vector3 point){
			foreach (Vector3 item in obs) {
				if (V3Equal(item, point)) {
					return true;
				}
			}
			return false;
		}

		public static bool containsPoint(List<Point3> obs, Point3 point){
			foreach (Point3 item in obs) {
				if (item.Equals(point)) {
					return true;
				}
			}
			return false;
		}
	}
}

