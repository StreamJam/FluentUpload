using Microsoft.AspNetCore.Http;

namespace FluentUploads;

public class GenericHack<TValue, TResult>
{
    public TValue? Value { get; set; }

    public static implicit operator GenericHack<TValue, TResult>(TResult result)
    {
        if (result is IValueHttpResult<TValue> valueResult)
        {
            return new GenericHack<TValue, TResult> { Value = valueResult.Value };
        }

        return new GenericHack<TValue,TResult>();
    }
}