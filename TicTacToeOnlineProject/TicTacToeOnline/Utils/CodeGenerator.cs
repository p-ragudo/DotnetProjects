namespace TicTacToeOnline.Utils;

public static class CodeGenerator
{
    private const string Chars = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ"; // Omitted 0, 1, O, I to avoid confusion

    public static string Generate(int length = 6)
    {
        var bytes = Guid.NewGuid().ToByteArray();
        var code = new char[length];

        for (int i = 0; i < length; i++)
        {
            code[i] = Chars[bytes[i] % Chars.Length];
        }

        return new string(code);
    }
}