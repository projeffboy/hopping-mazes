using System;
using System.Collections.Generic;
using UnityEngine;

public class DFS {
    private int rows, columns;

    public LinkedList<int[]>[,] adjList;
    private Stack<int[]> stack = new Stack<int[]>();
    private bool[,] vis;
    private Stack<int[]> shortestPath;

    public DFS(int rows, int columns) {
        this.rows = rows;
        this.columns = columns;

        adjList = new LinkedList<int[]>[rows, columns];
        for (int r = 0; r < rows; r++) {
            for (int c = 0; c < columns; c++) {
                adjList[r, c] = new LinkedList<int[]>();
            }
        }
        vis = new bool[rows, columns];
        shortestPath = new Stack<int[]>();
    }

    public void addEdge(int[] p1, int[] p2) {
        adjList[p1[0], p1[1]].AddLast(p2);
        adjList[p2[0], p2[1]].AddLast(p1);
    }

    public void findShortestPath(int[] source, int[] target) {
        bool[,] vis = new bool[rows, columns];

        stack = new Stack<int[]>();

        helper(vis, source, target, stack);
    }

    private void helper(bool[,] vis, int[] currentPoint, int[] target, Stack<int[]> stack) {
        stack.Push(currentPoint);

        if (currentPoint[0] == target[0] && currentPoint[1] == target[1]) {
            shortestPath = new Stack<int[]>(new Stack<int[]>(stack));
            return;
        }
        vis[currentPoint[0], currentPoint[1]] = true;

        LinkedList<int[]> point = adjList[currentPoint[0], currentPoint[1]];
        if (point.Count > 0) {
            LinkedListNode<int[]> adjPoint = point.First;
            while (adjPoint != null) {
                if (vis[adjPoint.Value[0], adjPoint.Value[1]] == false) {
                    helper(vis, adjPoint.Value, target, stack);
                }
                adjPoint = adjPoint.Next;
            }
        }

        stack.Pop();
    }

    public int[][] getShortestPath() {
        return shortestPath.ToArray();
    }

    public void printShortestPath() {
        var shortestPathStack = new Stack<int[]>(new Stack<int[]>(shortestPath));

        if (shortestPathStack.Count == 0) {
            Debug.Log("Empty");
        } else {
            while (shortestPathStack.Count != 0) {
                int[] point = shortestPathStack.Pop();
                Debug.Log("(" + point[0] + "," + point[1] + ")");
            }
        }
    }
}