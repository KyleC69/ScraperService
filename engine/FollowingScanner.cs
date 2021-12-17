





using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;
using OpenQA.Selenium;
using ScraperService;
using ScraperService.Data;
using ScraperService.Model;

public class FollowingScanner : BrowserControl
{


    public FollowingScanner(ILogger logger)
    {
        base.Logger = logger;
    }

    internal Task BeginFollowingScan()
    {
        TaskCompletionSource tcs = new();


        _ = Task.Run(async () =>
        {
            await LoadFollowingPageForScanningAsync();
            tcs.SetResult();

        });
        return tcs.Task;
    }







    internal async Task LoadFollowingPageForScanningAsync()
    {
        HtmlDocument document = new();
        try
        {
            if (TheDriver.Equals(null))
            { Logger.WebDriverInitializationFault(); }
            if (!base.IsLoggedIn) { await DoSiteLogin(); }

            // WebDriverWait wait = new WebDriverWait(this.TheDriver, TimeSpan.FromSeconds(30)).Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a")));
            TheDriver.Navigate().GoToUrl("https://newtumbl.com/follow");
            await Task.Delay(5000);
        }
        catch (OpenQA.Selenium.WebDriverException wde)
        {
            Logger.PageScraperNavigationFault(wde);
        }

        try
        {
            document.DisableServerSideCode = true;

            bool NextPageExists = true;
            Logger.LogInformation("starting to scrape blog list");
            while (NextPageExists)
            {
                document.LoadHtml(this.TheDriver.PageSource);
                ScrapeFollowingPage(document);
                Console.WriteLine("Switch pages and hit spacebar to continue or Q to exit");

                Debugger.Break();

                Logger.LogInformation("Proceeding to next page..");
            }
            // :Cleanup


        }
        catch
        {
            Logger.PageScraperSiteCaptureFault();
        }

    }

    private async void ScrapeFollowingPage(HtmlDocument document)
    {
        // document.Save("savedwebsite.html");
        HtmlNodeCollection blognodes = document.DocumentNode.SelectNodes("//div[@blog_src]");
        Console.WriteLine(blognodes.Count);


        foreach (HtmlNode nod in blognodes)
        {
            ParseVideoNode(nod);
        }
    }


    private async void ParseVideoNode(HtmlNode node)
    {
        Console.WriteLine(node.Line.ToString());
        HtmlNode icon = node.SelectSingleNode("descendant::div[@class='blog_icon']").FirstChild;
        if (!Equals(icon,null))
        {
            Uri path = new(icon.GetAttributeValue("src", ""));

            var id = Convert.ToInt32(path.Segments[2].Remove(path.Segments[2].Length - 1));
            var name = node.SelectSingleNode("descendant::div[@class='blog_name']").InnerText;

            NewTumblBlog tumBlog = new();
            tumBlog.BlogId = id;
            tumBlog.BlogName = name;
            tumBlog.BlogUrl = $"https://{name}.newtumbl.com";

            using DataController dc = new(Logger);
            await dc.AddNewTumblBlog(tumBlog);

        }
    }



}





[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public struct NewBlog
{
    public NewBlog(int blogId, string blogName)
    {
        BlogId = blogId;
        BlogName = blogName;
        BlogUrl = $"https://{blogName}.newtumbl.com";
        BlogEnabled = true;
        InsertedDate = DateTime.Now;
    }

    public NewBlog(int blogId, string blogName, string blogUrl, bool blogEnabled, DateTime insertedDate) : this(blogId, blogName)
    {
        BlogUrl = blogUrl;
        BlogEnabled = blogEnabled;
        InsertedDate = insertedDate;
    }

    public int BlogId { get; set; }
    public string BlogName { get; set; }
    public string BlogUrl { get; set; }
    public bool BlogEnabled { get; set; }
    public DateTime InsertedDate { get; set; }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string? ToString()
    {
        return base.ToString();
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}



