﻿using Devdeb.Sets.Generic;
using Devdeb.Sets.Ratios;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Devdeb.Storage.Test
{
	class Program
	{
		public const string DatabaseDirectory = @"C:\Users\lehac\Desktop\data";
		public const long MaxHeapSize = 10000;

		static void Main(string[] args)
		{
			TestStorableHeap();
			TestRedBlackTreeSurjection();
		}
		static void TestSomeShit()
		{
			RedBlackTreeSurjection<int, ClassB> surjection = new RedBlackTreeSurjection<int, ClassB>(Comparer<int>.Default);
			ClassB b1 = new ClassB { Id = 2 };
			ClassB b2 = new ClassB { Id = 3 };
			surjection.TryAdd(b1.Id, b1);
			surjection.TryAdd(b2.Id, b2);
			var res = surjection.Remove(b2.Id);
		}

		static void TestRedBlackTreeSurjection()
		{
			RedBlackTreeSurjection<Guid, Class> surjection = new RedBlackTreeSurjection<Guid, Class>(Comparer<Guid>.Default);
			for (int i = 0; i < 10000000; i++)
			{
				Class temp = new Class();
				surjection.TryAdd(temp.Id, temp);
				if (i%3 ==0)
					Debug.Assert(surjection.Remove(temp.Id));
			}
			foreach (SurjectionRatio<Guid, Class> ratio in surjection)
				Console.WriteLine($"{ratio.Input} - {ratio.Output.IntValue}");
		}

		static void TestStorableHeap()
		{
			StorableHeap storableHeap = new StorableHeap(new DirectoryInfo(DatabaseDirectory), MaxHeapSize);
			Segment segment = storableHeap.AllocateMemory(10);
			Segment segment1 = storableHeap.AllocateMemory(10);
			Segment segment2 = storableHeap.AllocateMemory(10);
			Segment segment3 = storableHeap.AllocateMemory(10);
			Segment segment4 = storableHeap.AllocateMemory(10);

			storableHeap.FreeMemory(segment1);
			Segment segment5 = storableHeap.AllocateMemory(5000);
			storableHeap.Write(segment5, new byte[] { 1 }, 0, 1);

			storableHeap.FreeMemory(segment);
			storableHeap.FreeMemory(segment2);
			storableHeap.FreeMemory(segment3);
			storableHeap.FreeMemory(segment4);
			storableHeap.Defragment();
			storableHeap.FreeMemory(new Segment { Pointer = 0, Size = 5000 });
			storableHeap.Defragment();
		}

		static void TestFileStreamApi()
		{
			string filePath = Path.Combine(DatabaseDirectory, "_data");
			using FileStream fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
			byte[] buffer = new byte[5000000];
			fileStream.Write(buffer, 0, 4000);
			buffer[0] = 1;
			fileStream.Flush(true);
			fileStream.Seek(0, SeekOrigin.Begin);
			int readBytes = fileStream.Read(buffer, 0, 3);
		}
	}

	public class ClassB
	{
		static private Random _random = new Random();

		public int Id { get; set; }
		public int IntValue { get; } = _random.Next(int.MinValue, int.MaxValue);
	}
	public class Class 
	{
		static private Random _random = new Random();

		public Guid Id { get; set; } = Guid.NewGuid();
		public int IntValue { get; } = _random.Next(int.MinValue, int.MaxValue);
	}
}
