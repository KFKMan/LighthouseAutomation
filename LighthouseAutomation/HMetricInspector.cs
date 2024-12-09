using PuppeteerSharp;

namespace LighthouseAutomation
{
	public class HMetricInspector : IHMetricInspector
	{
		private IPage Page;

		public HMetricInspector(IPage page)
		{
			Page = page;
		}

		public async ValueTask<TimeSpan?> GetMetricAsync(HMetrics metric)
		{
			var metricElement = await GetMetricHandle(metric);
			var metricRawValue = await GetMetricRawValue(metricElement);
			return GetMetricValue(metricRawValue);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rawValue">Returns 1 for finded type or returning null</param>
		/// <returns></returns>
		public TimeSpan? GetMetricType(string? rawValue)
		{
			if(rawValue == null) //it's not needed actually
			{
				return null;
			}

			bool Contains(string value)
			{
				return rawValue.Contains(value);
			}

			//Values For Turkish Localization
			if (Contains("sn"))
			{
				return TimeSpan.FromSeconds(1);
			}
			else if (Contains("dk"))
			{
				return TimeSpan.FromMinutes(1);
			} //I'm think more of them that not needed
			//Using else if's not needed actually

			return null;
		}

		public TimeSpan? GetMetricValue(string? rawValue)
		{
			if(rawValue == null)
			{
				return null;
			}
			var value = rawValue.GetCommaDoubleFromString();
			var valueType = GetMetricType(rawValue);

			if(valueType == null || value == null)
			{
				return null;
			}
			return value * valueType;
		}

		public async ValueTask<string?> GetMetricRawValue(IElementHandle? metricElement)
		{
			if (metricElement == null)
			{
				return null;
			}
			return await metricElement.EvaluateFunctionAsync<string?>("handle => handle.parentElement ? handle.parentElement.getElementsByClassName(\"lh-metric__value\")[0].textContent : null");
		}

		public string GetMetricHandleText(HMetrics metric)
		{
			switch (metric)
			{
				case HMetrics.FCP:
					return "First Contentful Paint";
				case HMetrics.LCP:
					return "Largest Contentful Paint";
				case HMetrics.TBT:
					return "Total Blocking Time";
				case HMetrics.CLS:
					return "Cumulative Layout Shift";
				case HMetrics.SpeedIndex:
					return "Speed Index";
				default:
					throw new NotImplementedException();
			}
		}

		public async ValueTask<IElementHandle?> GetMetricHandle(HMetrics metric)
		{
			var targetText = GetMetricHandleText(metric);
			var wraps = await Page.QuerySelectorAllAsync(".lh-metric__title");

			foreach (var wrap in wraps)
			{
				var content = await wrap.GetTextContentAsync();

				if (content == targetText)
				{
					return wrap;
				}
			}

			return null;
		}
	}
}
