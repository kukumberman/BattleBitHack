using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityHack
{
	public class Pagination
	{
		private int currentIndex = 0;
		private int amount = 0;
		private int totalCount = 0;

		public int CurrentIndex => currentIndex;

		public Pagination(int _amount)
		{
			amount = _amount;
		}

		public void UpdateLength(int total)
		{
			totalCount = total;
		}

		public int TotalPages()
		{
			return Mathf.CeilToInt(totalCount / (float)amount);
		}

		public List<T> Paginate<T>(List<T> fullCollection, out int start)
		{
			start = currentIndex * amount;
			int count = Mathf.Min(fullCollection.Count - start, amount);
			List<T> _new = new List<T>();
			for (int i = 0; i < count; i++)
			{
				_new.Add(fullCollection[start + i]);
			}
			return _new;
			//return fullCollection.GetRange(start, count);
		}

		public void MoveToStart()
		{
			currentIndex = 0;
		}

		public void Next()
		{
			currentIndex = Mathf.Min(totalCount - 1, currentIndex + 1);
		}

		public void Back()
		{
			currentIndex = Mathf.Max(0, currentIndex - 1);
		}
	}
}
