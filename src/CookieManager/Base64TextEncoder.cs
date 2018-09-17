using System;
using System.Collections.Generic;
using System.Text;

namespace CookieManager
{
    internal static class Base64TextEncoder
    {
		public static string Decode(string text)
		{
			byte[] data = Convert.FromBase64String(text);
			return Encoding.UTF8.GetString(data);
		}


		public static string Encode(string data)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(data);
			return Convert.ToBase64String(bytes);
		}

		public static bool TryDecode(string encodedText,out string plainText)
		{
			plainText = string.Empty;
			try
			{
				plainText = Decode(encodedText);
				return true;
			}
			catch (Exception ex)
			{
			}

			return false;
		}

	}
}
