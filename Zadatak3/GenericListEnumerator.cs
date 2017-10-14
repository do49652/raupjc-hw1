using System.Collections;
using System.Collections.Generic;

namespace Zadatak3
{
    public class GenericListEnumerator<T> : IEnumerator<T>
    {
        private int _currentIndex;
        private readonly GenericList<T> genericList;

        public GenericListEnumerator(GenericList<T> genericList)
        {
            _currentIndex = 0;
            this.genericList = genericList;
            Current = this.genericList.GetElement(_currentIndex);
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (_currentIndex + 1 >= genericList.Count)
                return false;

            Current = genericList.GetElement(++_currentIndex);
            return true;
        }

        public void Reset()
        {
            _currentIndex = 0;
            Current = genericList.GetElement(_currentIndex);
        }

        public T Current { get; private set; }

        object IEnumerator.Current => Current;
    }
}