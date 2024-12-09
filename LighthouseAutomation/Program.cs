#if DEBUG
#define LogSteps
#define ScreenshotForResult
#endif

using LighthouseAutomation;
using PuppeteerSharp;
using PuppeteerSharp.Input;
using System;

bool ScreenshotForResult = false;
#if ScreenshotForResult
ScreenshotForResult = true;
#endif

Console.WriteLine("Lighthouse Automation Started");

string? GetInput(string question)
{
	Console.Write(question);
	return Console.ReadLine();
}

var target = GetInput("Please Enter Target Website Url: ");

var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync(); //Download Required Files


Task<string> GetString(IElementHandle elementHandle)
{
	return elementHandle.EvaluateFunctionAsync<string>("element => element.textContent");
}

await using (var browser = await Puppeteer.LaunchAsync(
	new LaunchOptions { Headless = true })) //Launch browser
{
	await using (var page = await browser.NewPageAsync()) //Create new page in browser
	{
		Console.WriteLine("Going To Target Website...");

		await page.GoToAsync("https://pagespeed.web.dev/?hl=tr"); //go to pagespeed website | ?hl=tr query is specifying language


		Console.WriteLine("Typing Url...");

		var urlBox = await page.QuerySelectorAsync("#i4");

		await urlBox.TypeAsync(target, new TypeOptions() { Delay = 40 }); //Write target url to urlbox with 40ms per character delay

		var spans = await page.QuerySelectorAllAsync("span");

		foreach(var span in spans)
		{
			var textContent = await GetString(span);

			if(textContent == "Analiz et")
			{
				Console.WriteLine("Clicking Button...");

				await span.EvaluateFunctionAsync("span => span.parentElement.click()");
				break;
			}
		}

		await page.WaitForNavigationAsync(new NavigationOptions()
		{
			WaitUntil = new WaitUntilNavigation[]
			{
				WaitUntilNavigation.Networkidle0
			}
		});

		var metricInspector = new MetricInspector(page);
		var hMetricInspector = new HMetricInspector(page);

		Console.WriteLine("Collecting Datas...");

		async ValueTask<string?> GetMetric(Metrics metric)
		{
			var val = await metricInspector.GetMetricAsync(metric);
			if(val == null)
			{
				return "No Value Found";
			}
			return val.ToString();
		}

		var perf = await GetMetric(Metrics.Performance);
		var acces = await GetMetric(Metrics.Accessibility);
		var bestpractices = await GetMetric(Metrics.BestPractices);
		var seo = await GetMetric(Metrics.SEO);


		Console.WriteLine($"Metrics | Performance => {perf} | Accessibility => {acces} | Best Practices => {bestpractices} | SEO => {seo}");

		async ValueTask<string?> GetHMetric(HMetrics metric)
		{
			var val = await hMetricInspector.GetMetricAsync(metric);
			if (!val.HasValue)
			{
				return "No Value Found";
			}
			return val.Value.TotalSeconds.ToString();
		}

		var fcp = await GetHMetric(HMetrics.FCP);
		var lcp = await GetHMetric(HMetrics.LCP);
		var cls = await GetHMetric(HMetrics.CLS);
		var tbt = await GetHMetric(HMetrics.TBT);
		var speedIndex = await GetHMetric(HMetrics.SpeedIndex);

		Console.WriteLine($"HMetrics | FCP => {fcp} | LCP => {lcp} | CLS => {cls} | TBT => {tbt} | Speed Index => {speedIndex}");
	}
}

Console.ReadLine();

