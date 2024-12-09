using PuppeteerSharp;

namespace LighthouseAutomation
{
	public static class ElementExtensions
	{
		public static Task<string?> GetTextContentAsync(this IElementHandle elementHandle)
		{
			return elementHandle.EvaluateFunctionAsync<string?>("element => element.textContent");
		}
	}
}
