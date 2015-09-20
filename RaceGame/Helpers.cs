using System;
using System.Collections.Generic;
using SenseHatApi.Helpers;

namespace RaceGame
{
    public sealed class Helpers
    {
        public static Color Red = new Color(255, 0, 0);
        public static Color Black = new Color(0, 0, 0);
        public static Color Green = new Color(0, 255, 0);
        public static Color White = new Color(255, 255, 255);

        public static T[,] CreateRectangularArray<T>(IList<T[]> arrays)
        {
            var minorLength = arrays[0].Length;
            var ret = new T[arrays.Count, minorLength];
            for (var i = 0; i < arrays.Count; i++)
            {
                var array = arrays[i];
                if (array.Length != minorLength)
                {
                    throw new ArgumentException("All arrays must be the same length");
                }
                for (var j = 0; j < minorLength; j++)
                {
                    ret[i, j] = array[j];
                }
            }
            return ret;
        }
    }
}