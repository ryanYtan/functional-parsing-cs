using System;
using System.Collections;

namespace FunctionalParser
{
	public class Option<T> : IEnumerable<T>
	{
		private readonly T[] _value;

		private Option(T value)
		{
			_value = new T[] { value };
        }

		private Option()
		{
			_value = Array.Empty<T>();
        }

		public static Option<T> Some(T value)
		{
			return new Option<T>(value);
        }

		public static Option<T> None()
		{
			return new Option<T>();
        }

		public bool IsSome()
		{
			return _value.Length > 0;
        }

		public bool IsNone()
		{
			return _value.Length == 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
			return _value.GetEnumerator();
        }

        public T Get()
		{
			if (IsSome())
			{
				return _value[0];
            }
			throw new Exception("Value does not exist");
		}

        public override string ToString()
        {
			if (IsSome())
			{ 
                return $"Option[{_value[0]}]";
            } else
			{
                return $"Option[None]";
            }
        }
    }
}

