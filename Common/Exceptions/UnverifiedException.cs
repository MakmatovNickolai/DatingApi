namespace Common.Exceptions;


[Serializable]
public class UnverifiedException : Exception
{
    public UnverifiedException() { }
    public UnverifiedException(string message) : base(message) { }
    public UnverifiedException(string message, Exception inner) : base(message, inner) { }
    protected UnverifiedException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
