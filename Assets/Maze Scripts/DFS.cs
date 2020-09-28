using System.Collections.Generic;
using UnityEngine;

/*
 * This code helps you find the shortest path in the maze
 * This code is standard DFS, but I based my code on this: https://www.geeksforgeeks.org/print-the-path-between-any-two-nodes-of-a-tree-dfs/
 * There was only C++ and Python version of their code, so I after understanding what it did, I then proceeded to convert it to C#.
 */

public class DFS {
    private int rows, columns;

    public LinkedList<int[]>[,] adjList; // adjacency list
    private Stack<int[]> stack = new Stack<int[]>(); // dfs stack
    private bool[,] vis; // visited nodes
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

        Helper(vis, source, target, stack); // get ready for recursion
    }

    private void Helper(bool[,] vis, int[] currentPoint, int[] target, Stack<int[]> stack) {
        stack.Push(currentPoint); // for remembering the path

        if (currentPoint[0] == target[0] && currentPoint[1] == target[1]) { // path finally reaches target
            shortestPath = new Stack<int[]>(new Stack<int[]>(stack));
            return;
        }
        vis[currentPoint[0], currentPoint[1]] = true; // mark node as visited

        LinkedList<int[]> point = adjList[currentPoint[0], currentPoint[1]];
        if (point.Count > 0) {
            LinkedListNode<int[]> adjPoint = point.First;
            // where the DFS recursion takes place, applying the function to each unvisited adjacent node
            while (adjPoint != null) {
                if (vis[adjPoint.Value[0], adjPoint.Value[1]] == false) {
                    Helper(vis, adjPoint.Value, target, stack);
                }
                adjPoint = adjPoint.Next;
            }
        }

        stack.Pop();
    }

    public int[][] GetShortestPath() {
        return shortestPath.ToArray(); // originally a stack
    }

    // Debugging Purposes
    public void PrintShortestPath() {
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