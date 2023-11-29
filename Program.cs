using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

public class StringProcessor
{
	private readonly DecoderClass _decoderClass;

	public StringProcessor()
	{
		_decoderClass = new DecoderClass();
	}

	public void ProcessString(string input)
	{
		if (TryDecode(input, out var decoded, out var encodingType))
		{
			Console.WriteLine($"The string is encoded as {encodingType}.");
			Console.WriteLine(decoded);
		}
		else
		{
			Console.WriteLine("The string is not encoded.");
		}
	}

	private bool TryDecode(string input, out string decoded, out string encodingType)
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

	private bool TryDecodeBase64(string input, out string decoded)
	{
		try
		{
			decoded = _decoderClass.Base64Decoder(input);
			return true;
		}
		catch (FormatException)
		{
			decoded = null;
			return false;
		}
	}

	private bool TryDecodeUrl(string input, out string decoded)
	{
		if (input.Contains("%"))
		{
			decoded = _decoderClass.URLDecoder(input);
			return true;
		}
		decoded = null;
		return false;
	}

	private bool TryDecodeHtml(string input, out string decoded)
	{
		if (input.Contains("&lt;") || input.Contains("&gt;"))
		{
			decoded = _decoderClass.HTMLDecoder(input);
			return true;
		}
		decoded = null;
		return false;
	}

	private bool TryDecodeUnicode(string input, out string decoded)
	{
		if (input.Contains("\\u"))
		{
			decoded = _decoderClass.UnicodeDecoder(input);
			return true;
		}
		decoded = null;
		return false;
	}
}

public class DecoderClass
{
	public string Base64Decoder(string input) => Encoding.UTF8.GetString(Convert.FromBase64String(input));

	public string URLDecoder(string input) => Uri.UnescapeDataString(input);

	public string HTMLDecoder(string input) => HttpUtility.HtmlDecode(input);

	public string UnicodeDecoder(string input) => Regex.Unescape(input);
}

class Program
{
	static void Main()
	{
		string base64EncodedString = "SGVsbG8gV29ybGQh";
		string urlEncodedString = "Hello%20World%21";
		string htmlEncodedString = "Hello&amp;World&lt;script&gt;alert('Hello')&lt;/script&gt;";
		string unicodeEncodedString = "\\u0048\\u0065\\u006C\\u006C\\u006F";

		StringProcessor stringProcessor = new StringProcessor();
		stringProcessor.ProcessString(base64EncodedString);
		stringProcessor.ProcessString(urlEncodedString);
		stringProcessor.ProcessString(htmlEncodedString);
		stringProcessor.ProcessString(unicodeEncodedString);
	}
}
