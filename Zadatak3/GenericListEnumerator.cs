using System.Collections;
using System.Collections.Generic;

namespace Zadatak3
{
    public class GenericListEnumerator<T> : IEnumerator<T>
    {
        private int _currentIndex;
        private readonly GenericList<T> _genericList;

        public GenericListEnumerator(GenericList<T> genericList)
        {
            _currentIndex = -1;
            this._genericList = genericList;
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (_currentIndex + 1 >= _genericList.Count)
                return false;
            _currentIndex++;
            return true;
        }

        public void Reset()
        {
            _currentIndex = -1;
        }

        public T Current => _genericList.GetElement(_currentIndex);

        object IEnumerator.Current => Current;
    }
}