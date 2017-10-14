using System;

namespace Zadatak1
{
    public class IntegerList : IIntegerList
    {
        private int[] _internalStorage;

        public IntegerList()
        {
            _internalStorage = new int[4];
            Count = 0;
        }

        public IntegerList(int initialSize)
        {
            if (initialSize < 0)
                throw new ArgumentOutOfRangeException();

            _internalStorage = new int[initialSize];
            Count = 0;
        }

        public void Add(int item)
        {
            if (Count == _internalStorage.Length)
                IncreaseInternalStorageSize();

            _internalStorage[Count++] = item;
        }

        public bool Remove(int item)
        {
            for (var i = 0; i < Count; i++)
                if (_internalStorage[i].Equals(item))
                    return RemoveAt(i);

            return false;
        }

        public bool RemoveAt(int index)
        {
            if (index >= Count || index < 0)
                return false;
                //throw new ArgumentOutOfRangeException();

            for (var i = index; i < Count - 1; i++)
                _internalStorage[i] = _internalStorage[i + 1];

            Count--;

            return true;
        }

        public int GetElement(int index)
        {
            if (index >= Count || index < 0)
                throw new ArgumentOutOfRangeException();

            return _internalStorage[index];
        }

        public int IndexOf(int item)
        {
            for (var i = 0; i < Count; i++)
                if (_internalStorage[i].Equals(item))
                    return i;

            return -1;
        }

        public int Count { get; private set; }

        public void Clear()
        {
            for (var i = 0; i < Count; i++)
                _internalStorage[i] = 0;

            Count = 0;
        }

        public bool Contains(int item)
        {
            return !IndexOf(item).Equals(-1);
        }

        private void IncreaseInternalStorageSize()
        {
            var oldInts = _internalStorage;
            _internalStorage = new int[_internalStorage.Length * 2];

            for (var i = 0; i < oldInts.Length; i++)
                _internalStorage[i] = oldInts[i];
        }
    }
}