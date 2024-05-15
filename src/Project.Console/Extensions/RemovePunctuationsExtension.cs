namespace University.ConsoleApp.Extensions;

public static class RemovePunctuationsExtension
{
    public static string RemovePunctuations(this string text) => string.Join("", text.Where(c => !char.IsPunctuation(c)));
}
