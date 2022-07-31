using System;

public static class UnityTools
{
	public static T GetRandomValue<T>(T[,] array, Random random)
	{
		int values = array.GetLength(0) * array.GetLength(1);
		int index = random.Next(values);
		return array[index / array.GetLength(0), index % array.GetLength(0)];
	}
	public static bool IsPowerOfTwo(int n)
	{
		if(n==0) return false;
		return (int)(Math.Ceiling((Math.Log(n) /
		                           Math.Log(2)))) ==
		       (int)(Math.Floor(((Math.Log(n) /
		                          Math.Log(2)))));
	}
	public static T RandomEnumValue<T> ()
	{
		Random _R = new Random();
		var v = Enum.GetValues (typeof (T));
		return (T) v.GetValue (_R.Next(v.Length));
	}
}