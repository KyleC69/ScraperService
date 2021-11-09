namespace ScraperService
{


      using OpenQA.Selenium;
      using OpenQA.Selenium.Edge;



      public class BrowserControl
      {

            private EdgeDriver? _driver;
            private ILogger _logger;

            public BrowserControl()
            {

            }

            public bool IsLoggedIn { get; set; }
            public EdgeDriver TheDriver { get => _driver; }
            public ILogger Logger
            {
                  get => _logger;
                  set => _logger = value;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public Task InitializeWebDriver()
            {
                  TaskCompletionSource<EdgeDriver> tcs = new();

                  _ = Task.Run(async () =>
                    {
                          try
                          {
                                EdgeOptions options = new();
                                // options.AddArgument("--headless");
                                options.AddArgument("--user-data-dir=/home/savaho/.config/microsoft-edge-dev/profiles");
                                _driver = new("./engine/", options, TimeSpan.FromMinutes(2));
                                await Task.Delay(TimeSpan.FromSeconds(10));
                                _driver.Navigate().GoToUrl("https://www.newtumbl.com");
                                await Task.Delay(TimeSpan.FromSeconds(10));

                                tcs.SetResult(_driver);

                          }
                          catch
                          {
                                _logger.WebDriverInitializationFault();

                                tcs.TrySetException(new WebDriverException());
                          }
                    });

                  return tcs.Task;


            }

            public Task DoSiteLogin()
            {
                  TaskCompletionSource<bool> tcs = new();

                  _ = Task.Run(async () =>
                  {

                        try
                        {

                              _driver.Navigate().GoToUrl("https://www.newtumbl.com/sign?in");
                              IWebElement? ele = _driver.FindElement(By.XPath("//div[@class='NTHOME___loginSignup__inputObject input_field input_email']//input[@class='NTHOME___formInputText__input deprecate form-input_control']"));
                              ele.SendKeys("fetishmaster1969@gmail.com");

                              IWebElement? ele2 = _driver.FindElement(By.XPath("//div[@class='NTHOME___formInputText__group deprecate form-input formwrap has-label form-input_password']//input[@class='NTHOME___formInputText__input deprecate form-input_control']"));
                              ele2.SendKeys("Angel1031");

                              _driver.FindElement(By.XPath("(//form_input[@class='NTHOME___formInputButton deprecate button'])[1]")).Click();

                              await Task.Delay(5000);

                              return tcs.TrySetResult(true);

                        }
                        catch (Exception ex)
                        {
                              _logger.PageScraperSiteLoginFault(ex);
                              return tcs.TrySetResult(false);

                        }
                  });
                  return tcs.Task;
            }

      }
}


