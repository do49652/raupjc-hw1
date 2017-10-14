using System;
using System.Collections;
using System.Collections.Generic;

namespace Zadatak3
{
    public class GenericList<T> : IGenericList<T>
    {
        private T[] _internalStorage;

        public GenericList()
        {
            _internalStorage = new T[4];
            Count = 0;
        }

        public GenericList(int initialSize)
        {
            if (initialSize < 0)
                throw new ArgumentOutOfRangeException();

            _internalStorage = new T[initialSize];
            Count = 0;
        }

        public int Count { get; private set; }

        public void Add(T item)
        {
            if (Count == _internalStorage.Length)
                IncreaseInternalStorageSize();

            _internalStorage[Count++] = item;
        }

        public bool Remove(T item)
        {
            for (var i = 0; i < Count; i++)
                if (_internalStorage[i].Equals(item))
                    return RemoveAt(i);

            return false;
        }

        public bool RemoveAt(int index)
        {
            if (index >= Count || index < 0)
                throw new IndexOutOfRangeException();

            for (var i = index; i < Count - 1; i++)
                _internalStorage[i] = _internalStorage[i + 1];

            Count--;

            return true;
        }

        public T GetElement(int index)
        {
            if (index >= Count || index < 0)
                throw new IndexOutOfRangeException();

            return _internalStorage[index];
        }

        public int IndexOf(T item)
        {
            for (var i = 0; i < Count; i++)
                if (_internalStorage[i].Equals(item))
                    return i;

            return -1;
        }

        public void Clear()
        {
            _internalStorage = new T[_internalStorage.Length];
            Count = 0;
        }

        public bool Contains(T item)
        {
            return !IndexOf(item).Equals(-1);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new GenericListEnumerator<T>(this);
        }

        private void IncreaseInternalStorageSize()
        {
            var oldInts = _internalStorage;
            _internalStorage = new T[_internalStorage.Length * 2];

            for (var i = 0; i < oldInts.Length; i++)
                _internalStorage[i] = oldInts[i];
        }
    }
}