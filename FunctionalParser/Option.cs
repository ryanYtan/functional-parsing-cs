using System;
namespace FunctionalParser
{
	public class Option<T>
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

		public static Option<T> Of(T value)
		{
			return new Option<T>(value);
        }

		public static Option<T> Empty()
		{
			return new Option<T>();
        }

		public bool IsPresent()
		{
			return _value.Length > 0;
        }

		public bool IsEmpty()
		{
			return _value.Length == 0;
        }

		public T Get()
		{
			if (IsPresent())
			{
				return _value[0];
            }
			throw new Exception("Value does not exist");
		}

        public override string ToString()
        {
			string s = IsPresent() ? Get().ToString() : "None";
			return $"Option[{s}]";
        }
    }
}

