namespace AspExam.Helpers;

public static class Base62Converter
{
    private const string ALPHABET = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    public static string Encode(long input)
    {
        string result = string.Empty;
        int targetBase = ALPHABET.Length;

        do
        {
            result = ALPHABET[(int)(input % targetBase)] + result;
            input /= targetBase;
        } while (input > 0);

        return result;
    }

    public static long Decode(string input)
    {
        long id = 0;
        int srcBase = ALPHABET.Length;

        for (int i = 0; i < input.Length; i++)
        {
            int charIndex = ALPHABET.IndexOf(input[i]);
            id += charIndex * (long)Math.Pow(srcBase, input.Length-1-i);
        }

        return id;
    }
}