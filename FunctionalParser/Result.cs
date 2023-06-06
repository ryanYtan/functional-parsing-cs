﻿using System;

namespace FunctionalParser
{
	public class Result<T, R>
	{
		public Option<T> Value { get; set; }
		public R Remaining { get; set; }
		public bool IsSuccess { get; set; }

		private Result(Option<T> value, R remaining, bool isSuccess)
		{
			Value = value;
			Remaining = remaining;
			IsSuccess = isSuccess;
        }

		public static Result<T, R> Success(T value, R remaining)
		{
			return new Result<T, R>(Option<T>.Of(value), remaining, true);
        }

		public static Result<T, R> Fail(R remaining)
		{
			return new Result<T, R>(Option<T>.Empty(), remaining, false);
        }

		public T GetResult()
		{
			return Value.Get();
        }

        public void Deconstruct(out Option<T> value, out R remaining, out bool isSuccess)
        {
			value = Value;
            remaining = Remaining;
            isSuccess = IsSuccess;
        }
	}
}

