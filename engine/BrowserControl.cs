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
            _driver = InitializeWebDriver().Result;
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
        public Task<EdgeDriver> InitializeWebDriver()
        {
            TaskCompletionSource<EdgeDriver> tcs = new();

            _ = Task.Run(async () =>
              {
                  try
                  {
                      EdgeOptions options = new();
                      // options.AddArgument("--headless");
                      // options.AddArgument("--user-data-dir=/home/savaho/.config/microsoft-edge-dev/profiles");
                      _driver = new("./engine/", options, TimeSpan.FromMinutes(2));
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
                    await Task.Delay(300);
                  
                  //div[@title='Sign In']

                  
                    // _driver.FindElement(By.XPath("//div[@title='Sign In']")).Submit();
                  _driver.FindElement(By.XPath("/html[1]/body[1]/div[2]/div[1]/div[1]/div[2]/div[1]/div[1]/div[4]/div[2]/form_input[1]/div[1]/button[1]/div[1]")).Click();
                  
                  
                  
                  
                    // _driver.FindElement(By.XPath("body > div.NTFW___sled > div > div > div.NTHOME___loginSignup > div.NTHOME___loginSignup__formHolder.NTHOME___centerGroup.NTHOME___loggedout > div.NTHOME___loginSignup__formGroups.state_login > div.NTHOME___loginSignup__formGroup.NTHOME___loginSignup__formGroup--buttons > div.NTHOME___loginSignup__formButton.NTHOME___loginSignup__formButton--login.input_login > form_input > div > button")
                    // _driver.FindElement(By.XPath("//button[contains(@class, 'NTHOME___formInputButton__button')]")).Click();

                    await Task.Delay(5000);
                    IsLoggedIn = true;
                    return tcs.TrySetResult(true);

                }
                catch (Exception ex)
                {
                    _logger.PageScraperSiteLoginFault(ex);
                    IsLoggedIn = false;
                    return tcs.TrySetResult(false);

                }
            });
            return tcs.Task;
        }

    }
}


