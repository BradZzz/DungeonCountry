using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using System;

public class Astar {

	private Queue<List<Point3>> paths;

	private Queue<Point3> stepQueue;
	private Queue<Point3> nextQueue;

	private List<Point3> foundVal;

	private Point3 destination;
	private int columns, rows;
	private int[,] map;
	private int[,] generatedMap;

	public IEnumerator generateOverflowMapv1(Transform ai, BattleMeta meta, Transform enemy, int move, int rows, int columns, 
		List<Point3> obs, Action<List<Point3>,BattleMeta,Transform, Transform> pathCallback){

		foundVal = overflowAlgorithm (new Point3(ai.position), move, rows, columns, obs);
		pathCallback (foundVal, meta, enemy, ai);
		yield return foundVal;
	}

	public List<Point3> baseAlgorithm(Point3 startingPos, Point3 destination, int rows, int columns, List<Point3> obs, bool deleteFirst){
		this.destination = destination;
		this.columns = columns;
		this.rows = rows;
		//thisPath.Clear ();
		map = new int[columns,rows];
		generatedMap = new int[columns,rows];
		for (int y = 0; y < rows; y++){
			for (int x = 0; x < columns; x++){
				Point3 check = new Point3 ((float)x, (float)y, 0);
				if ((check.Equals(startingPos) || Coroutines.containsPoint (obs, check)) && !destination.Equals(check)) {
					map [x,y] = 1;
				} else {
					map [x,y] = 0;
				}
				generatedMap [x, y] = 0;
			}
		}
		foundVal = null;
		nextQueue = new Queue<Point3>();
		stepQueue = new Queue<Point3>();
		stepQueue.Enqueue (startingPos);
		generatedMap[startingPos.x,startingPos.y] = 1;
		int count = 2;
		while (stepQueue.Count > 0 && foundVal == null){
			while (stepQueue.Count > 0) {
				stepv2 (stepQueue.Dequeue(), count);
				if (foundVal != null) {
					break;
				}
			}
			stepQueue = new Queue<Point3>(nextQueue);
			nextQueue.Clear ();
			count++;
		}
		if (deleteFirst && foundVal != null) {
			foundVal.Reverse ();
			foundVal.RemoveAt (0);
		}

		return foundVal;
	}

	public List<Point3> overflowAlgorithm(Point3 startingPos, int move, int rows, int columns, List<Point3> obs){
		this.destination = null;
		this.columns = columns;
		this.rows = rows;
		map = new int[columns,rows];
		generatedMap = new int[columns,rows];
		for (int y = 0; y < rows; y++){
			for (int x = 0; x < columns; x++){
				Point3 check = new Point3 ((float)x, (float)y, 0);
				if ((check.Equals(startingPos) || Coroutines.containsPoint (obs, check))) {
					map [x,y] = 1;
				} else {
					map [x,y] = 0;
				}
				generatedMap [x, y] = 0;
			}
		}
		foundVal = new List<Point3>();
		nextQueue = new Queue<Point3>();
		stepQueue = new Queue<Point3>();
		stepQueue.Enqueue (startingPos);
		generatedMap[startingPos.x,startingPos.y] = 1;
		int count = 2;
		while (stepQueue.Count > 0){
			while (stepQueue.Count > 0) {
				stepv2 (stepQueue.Dequeue(), count);
			}
			stepQueue = new Queue<Point3>(nextQueue);
			nextQueue.Clear ();
			count++;
			//need to compile the unique list of points into a new list here
			foreach (Point3 point in stepQueue.ToArray()) {
				if (!foundVal.Contains(point)) {
					Debug.Log ("Adding: " + point.ToString());
					foundVal.Add (point);
				}
			}
			if (count > move + 1) {
				break;
			}
		}

		return foundVal;
	}

	private void stepv2(Point3 step, int iteration){
		Point3 edge = step;
		//Debug.Log ("Working on: " +  edge.ToString() + " : " + step.Count);

		int[] directions = new int[]{ 1, 2, 3, 4 };
		Coroutines.ShuffleArray (directions);

		Point3 nextStep;

		foreach (int way in directions) {
			switch(way){
			case 1:
				if (edge.x - 1 >= 0 && map[step.x - 1, step.y] == 0 && generatedMap[step.x - 1, step.y] == 0) {
					nextStep = new Point3 (step.x - 1, step.y, 0);
					generatedMap [step.x - 1, step.y] = iteration;
					nextQueue.Enqueue (nextStep);
					checkDestination (nextStep, iteration);
				}
				break;
			case 2:
				if (edge.y - 1 >= 0 && map[step.x, step.y - 1] == 0 && generatedMap[step.x, step.y - 1] == 0) {
					nextStep = new Point3(step.x, step.y - 1, 0);
					generatedMap [step.x, step.y - 1] = iteration;
					nextQueue.Enqueue (nextStep);
					checkDestination (nextStep, iteration);
				}
				break;
			case 3:
				if (edge.x + 1 < columns && map[step.x + 1, step.y] == 0 && generatedMap[step.x + 1, step.y] == 0) {
					nextStep = new Point3(step.x + 1, step.y, 0);
					generatedMap [step.x + 1, step.y] = iteration;
					nextQueue.Enqueue (nextStep);
					checkDestination (nextStep, iteration);
				}
				break;
			case 4:
				if (edge.y + 1 < rows && map[step.x, step.y + 1] == 0 && generatedMap[step.x, step.y + 1] == 0) {
					nextStep = new Point3(step.x, step.y + 1, 0);
					generatedMap [step.x, step.y + 1] = iteration;
					nextQueue.Enqueue (nextStep);
					checkDestination (nextStep, iteration);
				}
				break;
			}
		}
	}

	private Point3 retraceSteps(Point3 thisStep, int looking){
		if (thisStep.x - 1 >= 0 && generatedMap[thisStep.x - 1,thisStep.y] == looking) {
			return new Point3 (thisStep.x - 1, thisStep.y, thisStep.z);
		}
		if (thisStep.y - 1 >= 0 && generatedMap[thisStep.x,thisStep.y - 1] == looking) {
			return new Point3 (thisStep.x, thisStep.y - 1, thisStep.z);
		}
		if (thisStep.x + 1 < columns && generatedMap[thisStep.x + 1,thisStep.y] == looking) {
			return new Point3 (thisStep.x + 1, thisStep.y, thisStep.z);
		}
		if (thisStep.y + 1 < rows && generatedMap[thisStep.x,thisStep.y + 1] == looking) {
			return new Point3 (thisStep.x, thisStep.y + 1, thisStep.z);
		}

		return new Point3 ();
	}

	private void checkDestination(Point3 nextStep, int iteration){
		if (nextStep.Equals(destination)) {

			foundVal = new List<Point3> ();
			foundVal.Add (nextStep);

			while(iteration > 1){
				foundVal.Add (retraceSteps (foundVal[foundVal.Count - 1], --iteration));
			}
		}
	}
}
