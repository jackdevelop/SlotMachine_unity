using UnityEngine;
using System.Collections;

public class PayData {
    public static int totalSymbols = 10;
    public static int scatterIndex = 9;
    public static int wildIndex = 8;
    public static int totalFreeSpins = 8;

    public static int[,] table = new int[,]{
        {0, 0, 2, 10, 30}, 
        {0, 0, 3, 15, 45}, 
        {0, 0, 4, 20, 60}, 
        {0, 0, 5, 25, 75}, 
        {0, 0, 6, 30, 90}, 
        {0, 0, 7, 30, 90}, 
        {0, 0, 8, 40, 120}, 
        {0, 0, 10, 50, 150}, 
        {0, 0, 20, 100, 300}, 
        {0, 0, 0, 0, 0}
    };

    public static int[,] line = new int[,]{
        {1, 1, 1, 1, 1}, 
        {0, 0, 0, 0, 0}, 
        {2, 2, 2, 2, 2}, 
        {0, 1, 2, 1, 0}, 
        {2, 1, 0, 1, 2}, 
        {0, 0, 1, 2, 2}, 
        {2, 2, 1, 0, 0}, 
        {1, 1, 0, 1, 1}, 
        {1, 1, 2, 1, 1}, 
        {0, 1, 0, 1, 0}, 
        {2, 1, 2, 1, 2}, 
        {0, 1, 1, 1, 0}, 
        {2, 1, 1, 1, 2}, 
        {1, 0, 1, 2, 1}, 
        {1, 2, 1, 0, 1}, 
        {1, 0, 0, 0, 1}, 
        {1, 2, 2, 2, 1}, 
        {0, 0, 0, 1, 2}, 
        {2, 2, 2, 1, 0}, 
        {0, 1, 2, 2, 2}, 
        {2, 1, 0, 0, 0}, 
        {0, 0, 1, 0, 0}, 
        {2, 2, 1, 2, 2}, 
        {1, 1, 1, 0, 1}, 
        {1, 1, 1, 2, 1}
    };

    public static int[] betting = new int[] {
        25, 50, 100, 200, 500, 1000, 5000, 10000
    };
}
