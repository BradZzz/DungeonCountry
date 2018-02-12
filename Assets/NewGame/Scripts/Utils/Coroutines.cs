﻿using System;
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

		public static IEnumerator smooth_move(Transform origin, Vector3 direction,float speed){
			float startime = Time.time;
			Vector3 start_pos = new Vector3(origin.position.x, origin.position.y, origin.position.z);
			Vector3 end_pos = direction;
			while (!origin.position.Equals(end_pos)) { 
				float move = .25f;

				Vector3 position = origin.position;

				position.x += ((end_pos.x - start_pos.x) * move);
				position.y += ((end_pos.y - start_pos.y) * move);

				if ((start_pos.x > end_pos.x && origin.position.x < end_pos.x) || (start_pos.x < end_pos.x && origin.position.x > end_pos.x)) {
					position.x = end_pos.x;
				}
				if ((start_pos.y > end_pos.y && origin.position.y < end_pos.y)||(start_pos.y < end_pos.y && origin.position.y > end_pos.y)) {
					position.y = end_pos.y;
				}

				origin.position = position;

				if (((Time.time - startime) * speed) >= .75f) {
					origin.position = end_pos;
				}

				yield return null;
			}
		}

		public static Component CopyComponent(Component original, GameObject destination)
		{
			System.Type type = original.GetType();
			Component copy = destination.AddComponent(type);
			// Copied fields can be restricted with BindingFlags
			System.Reflection.FieldInfo[] fields = type.GetFields(); 
			foreach (System.Reflection.FieldInfo field in fields)
			{
				field.SetValue(copy, field.GetValue(original));
			}
			return copy;
		}

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
			foreach (GameObject children in GameObject.FindGameObjectsWithTag("ObsBattle")) {
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
			foreach (GameObject children in GameObject.FindGameObjectsWithTag("ObsBattle")) {
				if (child.Equals(children.transform.position)) {
					return children;
				}
			}
			foreach (GameObject children in GameObject.FindGameObjectsWithTag("Hazard")) {
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

		public static void ShuffleArray<T>(List<T> list) {
			for (int i = list.Count - 1; i > 0; i--) {
				int r = UnityEngine.Random.Range(0, i);
				T tmp = list[i];
				list[i] = list[r];
				list[r] = tmp;
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

		public static bool containsPoint(List<Transform> obs, Point3 point){
			foreach (Transform item in obs) {
				if (new Point3(item.position).Equals(point)) {
					return true;
				}
			}
			return false;
		}

		public static int[,] RotateMatrixCounterClockwise(int[,] oldMatrix)
		{
			int[,] newMatrix = new int[oldMatrix.GetLength(1), oldMatrix.GetLength(0)];
			int newColumn, newRow = 0;
			for (int oldColumn = oldMatrix.GetLength(1) - 1; oldColumn >= 0; oldColumn--)
			{
				newColumn = 0;
				for (int oldRow = 0; oldRow < oldMatrix.GetLength(0); oldRow++)
				{
					newMatrix[newRow, newColumn] = oldMatrix[oldRow, oldColumn];
					newColumn++;
				}
				newRow++;
			}
			return newMatrix;
		}

		public static void toggleVisibilityTransform(Transform parent, bool shown)
		{
			foreach (Transform child in parent) {
				child.GetComponent<SpriteRenderer> ().enabled = shown;
				//child.GetComponent<BoxCollider2D> ().enabled = shown;
				//child.GetComponent<Collider2D> ().enabled = shown;
			}
		}
	}
}

