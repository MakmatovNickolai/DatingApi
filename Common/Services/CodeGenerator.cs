using System.Text;

namespace Common.Services;

public interface ICodeGenerator
{
    string Generate(bool useDigits, bool useLowerCase, bool useUppercase, bool useNonAlphanumeric, int length);
}
public class CodeGenerator : ICodeGenerator
{
    public string Generate(bool useDigits, bool useLowerCase, bool useUppercase, bool useNonAlphanumeric, int length)
    {
        if (!useDigits && !useLowerCase && !useUppercase && !useNonAlphanumeric)
            throw new ArgumentException("Invalid password generator parameters");
        if (length == 0)
            throw new ArgumentException("Invalid password generator length");

        var password = new StringBuilder();
        var random = new Random(DateTime.UtcNow.Millisecond);

        var allowedChars = Enumerable.Empty<int>();
        if (useNonAlphanumeric) allowedChars = allowedChars.Concat(Enumerable.Range(33, 47 - 33));
        if (useDigits) allowedChars = allowedChars.Concat(Enumerable.Range(48, 58 - 48));
        if (useLowerCase) allowedChars = allowedChars.Concat(Enumerable.Range(97, 123 - 97));
        if (useUppercase) allowedChars = allowedChars.Concat(Enumerable.Range(65, 91 - 65));

        var data = allowedChars.ToList();
        while (password.Length < length)
        {
            password.Append((char)data[random.Next(0, data.Count)]);
        }

        return password.ToString();
    }
}
