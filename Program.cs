using System.Text;
using System.Text.RegularExpressions;
using System.Web;
public class StringProcessor
{
	private readonly AutoDetector _autoDetector;
	private readonly WhichEncoded _whichEncoded;
	private readonly DecoderClass _decoderClass;

	public StringProcessor()
	{
		_autoDetector = new AutoDetector();
		_whichEncoded = new WhichEncoded();
		_decoderClass = new DecoderClass();
	}

	public void ProcessString(string input)
	{
		if (_autoDetector.IsEncoded(input, out var encodingType))
		{
			Console.WriteLine($"The string is encoded as {encodingType}.");
			DecodeAndPrint(input, encodingType);
		}
		else
		{
			Console.WriteLine("The string is not encoded.");
		}
	}

	private void DecodeAndPrint(string input, string encodingType)
	{
		switch (encodingType)
		{
			case "Base64":
				Console.WriteLine(_decoderClass.Base64Decoder(input));
				break;
			case "URL":
				Console.WriteLine(_decoderClass.URLDecoder(input));
				break;
			case "HTML":
				Console.WriteLine(_decoderClass.HTMLDecoder(input));
				break;
			case "Unicode":
				Console.WriteLine(_decoderClass.UnicodeDecoder(input));
				break;
		}
	}
}

public class AutoDetector
{
	private readonly WhichEncoded _whichEncoded;

	public AutoDetector()
	{
		_whichEncoded= new WhichEncoded();
	}
	public bool IsEncoded(string input, out string encodingType)
	{
		encodingType = null;
		if (_whichEncoded.IsBase64(input))
		{
			encodingType = "Base64";
			return true;
		}
		if (_whichEncoded.IsUrlEncoded(input))
		{
			encodingType = "URL";
			return true;
		}
		if (_whichEncoded.IsHtmlEncoded(input))
		{
			encodingType = "HTML";
			return true;
		}
		if (_whichEncoded.IsUnicodeEncoded(input))
		{
			encodingType = "Unicode";
			return true;
		}
		return false;
	}
}

public class WhichEncoded
{
	public bool IsBase64(string input)
	{
		try
		{
			Convert.FromBase64String(input);
			return true;
		}
		catch (FormatException)
		{
			return false;
		}
	}

	public bool IsUrlEncoded(string input)
	{
		return input.Contains("%");
	}

	public bool IsHtmlEncoded(string input)
	{
		return input.Contains("&lt;") || input.Contains("&gt;");
	}

	public bool IsUnicodeEncoded(string input)
	{
		return input.Contains("\\u");
	}
}

public class DecoderClass
{
	public string Base64Decoder(string input)
	{
		byte[] data = Convert.FromBase64String(input);
		return Encoding.UTF8.GetString(data);
	}

	public string URLDecoder(string input)
	{
		return Uri.UnescapeDataString(input);
	}

	public string URLPathDecoder(string input)
	{
		return Uri.UnescapeDataString(input);
	}

	public string HTMLDecoder(string input)
	{
		return HttpUtility.HtmlDecode(input);
	}

	public string UnicodeDecoder(string input)
	{
		return Regex.Unescape(input);
	}
}

class Program
{
	static void Main()
	{
		string base64EncodedString = "SGVsbG8gV29ybGQh";
		string urlEncodedString = "Hello%20World%21";
		string htmlEncodedString = "Hello&amp;World&lt;script&gt;alert('Hello')&lt;/script&gt;";
		string urlPathEncodedString = "Hello%2FWorld%2F";
		string unicodeEncodedString = "\\u0048\\u0065\\u006C\\u006C\\u006F";

		StringProcessor stringProcessor = new StringProcessor();
		stringProcessor.ProcessString(base64EncodedString);
		stringProcessor.ProcessString(urlEncodedString);
		stringProcessor.ProcessString(htmlEncodedString);
		stringProcessor.ProcessString(urlPathEncodedString);
		stringProcessor.ProcessString(unicodeEncodedString);
	}
}
