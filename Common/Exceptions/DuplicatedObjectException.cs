namespace Common.Exceptions;

[Serializable]
public class DuplicatedObjectException : Exception
{
    public DuplicatedObjectException() { }
    public DuplicatedObjectException(string message) : base(message) { }
    public DuplicatedObjectException(string message, Exception inner) : base(message, inner) { }
    protected DuplicatedObjectException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
