using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

public static class StringProcessorExtensions
{
	public static string DecodeString(this string input)
	{
		if (StringProcessor.TryDecode(input, out var decoded, out var encodingType))
		{
			return $"The string is encoded as {encodingType}.\n{decoded}";
		}
		else
		{
			return "The string is not encoded.";
		}
	}
}

public class StringProcessor
{
	private readonly DecoderClass _decoderClass;

	public StringProcessor()
	{
		_decoderClass = new DecoderClass();
	}

	internal static bool TryDecode(string input, out string decoded, out string encodingType)
	{
		decoded = null;
		encodingType = null;

		if (TryDecodeBase64(input, out decoded))
		{
			encodingType = "Base64";
			return true;
		}

		if (TryDecodeUrl(input, out decoded))
		{
			encodingType = "URL";
			return true;
		}

		if (TryDecodeHtml(input, out decoded))
		{
			encodingType = "HTML";
			return true;
		}

		if (TryDecodeUnicode(input, out decoded))
		{
			encodingType = "Unicode";
			return true;
		}

		return false;
	}

	private static bool TryDecodeBase64(string input, out string decoded)
	{
		try
		{
			decoded = DecoderClass.Base64Decoder(input);
			return true;
		}
		catch (FormatException)
		{
			decoded = null;
			return false;
		}
	}

	private static bool TryDecodeUrl(string input, out string decoded)
	{
		if (input.Contains("%"))
		{
			decoded = DecoderClass.URLDecoder(input);
			return true;
		}
		decoded = null;
		return false;
	}

	private static bool TryDecodeHtml(string input, out string decoded)
	{
		if (input.Contains("&lt;") || input.Contains("&gt;"))
		{
			decoded = DecoderClass.HTMLDecoder(input);
			return true;
		}
		decoded = null;
		return false;
	}

	private static bool TryDecodeUnicode(string input, out string decoded)
	{
		if (input.Contains("\\u"))
		{
			decoded = DecoderClass.UnicodeDecoder(input);
			return true;
		}
		decoded = null;
		return false;
	}
}

public class DecoderClass
{
	internal static string Base64Decoder(string input) => Encoding.UTF8.GetString(Convert.FromBase64String(input));

	internal static string URLDecoder(string input) => Uri.UnescapeDataString(input);

	internal static string HTMLDecoder(string input) => HttpUtility.HtmlDecode(input);

	internal static string UnicodeDecoder(string input) => Regex.Unescape(input);
}

class Program
{
	static void Main()
	{
		string base64EncodedString = "SGVsbG8gV29ybGQh";
		string urlEncodedString = "Hello%20World%21";
		string htmlEncodedString = "Hello&amp;World&lt;script&gt;alert('Hello')&lt;/script&gt;";
		string unicodeEncodedString = "\\u0048\\u0065\\u006C\\u006C\\u006F";


		var base64 = base64EncodedString.DecodeString();
		var url = urlEncodedString.DecodeString();
		var html = htmlEncodedString.DecodeString();
		var unicode = unicodeEncodedString.DecodeString();

		Console.WriteLine($"{base64}\n{url}\n{html}\n{unicode}");

	}
}
