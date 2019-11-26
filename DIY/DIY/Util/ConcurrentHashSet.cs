using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DIY.Util
{
    /// <summary>
    /// Copied from https://stackoverflow.com/a/11034999
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConcurrentHashSet<T> : IDisposable
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private readonly HashSet<T> _hashSet = new HashSet<T>();

        #region Implementation of ICollection<T> ...ish
        public bool Add(T item)
        {
            try
            {
                _lock.EnterWriteLock();
                return _hashSet.Add(item);
            }
            finally
            {
                if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
            }
        }

        public void AddAll(ICollection<T> col)
        {
            try
            {
                _lock.EnterWriteLock();
                var en = col.GetEnumerator();
                while(en.MoveNext())
                {
                    _hashSet.Add(en.Current);
                }
            }
            finally
            {
                if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
            }
        }

        public void Clear()
        {
            try
            {
                _lock.EnterWriteLock();
                _hashSet.Clear();
            }
            finally
            {
                if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
            }
        }

        public bool Contains(T item)
        {
            try
            {
                _lock.EnterReadLock();
                return _hashSet.Contains(item);
            }
            finally
            {
                if (_lock.IsReadLockHeld) _lock.ExitReadLock();
            }
        }

        public bool Remove(T item)
        {
            try
            {
                _lock.EnterWriteLock();
                return _hashSet.Remove(item);
            }
            finally
            {
                if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
            }
        }

        public int Count
        {
            get
            {
                try
                {
                    _lock.EnterReadLock();
                    return _hashSet.Count;
                }
                finally
                {
                    if (_lock.IsReadLockHeld) _lock.ExitReadLock();
                }
            }
        }

        public void ForEach(Action<T> ac)
        {
            try
            {
                _lock.EnterReadLock();
                var en = _hashSet.GetEnumerator();
                while(en.MoveNext())
                {
                    ac(en.Current);
                }
            }
            finally
            {
                if (_lock.IsReadLockHeld) _lock.ExitReadLock();
            }
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            if (_lock != null) _lock.Dispose();
        }

        #endregion
    }
}
