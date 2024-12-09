namespace LighthouseAutomation
{
	public static class ParserExtensions
	{
		public static string GetNumbers(this string value)
		{
			string number = "";
			foreach (var character in value)
			{
				if (char.IsNumber(character))
				{
					number += character;
				}
			}

			return number;
		}

		public static string GetNumbersAndCommas(this string value)
		{
			string number = "";
			foreach (var character in value)
			{
				if (char.IsNumber(character) || character == ',')
				{
					number += character;
				}
			}

			return number;
		}

		/// <summary>
		/// Only getting number characters and parsing it
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static ushort? GetUShortFromString(this string value)
		{
			if(ushort.TryParse(value.GetNumbers(), out var res))
			{
				return res;
			}
			return null;
		}


		public static double? GetCommaDoubleFromString(this string value)
		{
			if (double.TryParse(value.GetNumbersAndCommas(), out var res))
			{
				return res;
			}
			return null;
		}
	}
}
