using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DIY.Project
{
    /// <summary>
    /// A Stack implementation that has a fixed size
    /// </summary>
    /// <typeparam name="T">The type of elements in the Stack</typeparam>
    public class FixedStack<T> : ICollection
    {
        public int Size { get; private set; }

        private List<T> Underlying { get; set; }

        public FixedStack(int size)
        {
            Size = size;
            Underlying = new List<T>();
        }

        public void Push(T t)
        {
            Underlying.Insert(0, t);
            if(Underlying.Count > Size)
            {
                Underlying.RemoveRange(Size - 1, Underlying.Count - Size + 1);
            }
        }

        public T Pop()
        {
            if(Underlying.Count < 1)
            {
                throw new IndexOutOfRangeException("No Element in Stack!");
            }
            T t = Peek();
            Underlying.RemoveAt(0);
            return t;
        }

        public T Peek()
        {
            if (Underlying.Count < 1)
            {
                throw new IndexOutOfRangeException("No Element in Stack!");
            }
            return Underlying[0];
        }

        #region Implementations of ICollection
        public int Count => Underlying.Count;

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            return Underlying.GetEnumerator();
        }
        #endregion
    }
}
