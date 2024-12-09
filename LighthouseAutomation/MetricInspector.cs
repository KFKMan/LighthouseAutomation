using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace LighthouseAutomation
{

	public class MetricInspector : IMetricInspector
	{
		private IPage Page;

		public MetricInspector(IPage page)
		{
			Page = page;
		}

		public async ValueTask<ushort?> GetMetricAsync(Metrics metric)
		{
			var metricElement = await GetMetricHandle(metric);
			var metricContent = await GetMetricContent(metricElement);
			return metricContent?.GetUShortFromString();
		}

		private Task<string?> GetMetricContent(IElementHandle? metricElement)
		{
			if(metricElement == null)
			{
				return Task.FromResult<string?>(null);
			}
			return metricElement.GetTextContentAsync();
		}

		private string GetMetricHandleHref(Metrics metric)
		{
			switch (metric)
			{
				case Metrics.Performance:
					return "performance";
				case Metrics.Accessibility:
					return "accessibility";
				case Metrics.BestPractices:
					return "best-practices";
				case Metrics.SEO:
					return "seo";
				default:
					throw new NotImplementedException();
			}
		}

		private async ValueTask<IElementHandle?> GetMetricHandle(Metrics metric)
		{
			return await Page.WaitForSelectorAsync($"a[href='#{GetMetricHandleHref(metric)}']",new WaitForSelectorOptions()
			{
				Timeout = TimeSpan.FromMinutes(5).Milliseconds
			});
		}


	}
}
