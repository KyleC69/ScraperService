namespace ScraperService
{


      using OpenQA.Selenium;
      using OpenQA.Selenium.Edge;



      public class BrowserControl
      {

            private EdgeDriver? _driver;

            public bool IsLoggedIn { get; set; }


            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public Task<EdgeDriver> InitializeWebDriver()
            {
                  try
                  {
                        EdgeOptions options = new();
                        // options.AddArgument("--headless");

                        options.AddArgument("--user-data-dir=/home/savaho/.config/microsoft-edge-dev/profiles");


                        _driver = new("./engine/", options, TimeSpan.FromMinutes(2));

                        _driver.Navigate().GoToUrl("https://www.newtumbl.com");

                        Task.Delay(TimeSpan.FromSeconds(10)).Wait();
                        // DoSiteLogin().Wait();
                  }
                  catch
                  {
                        Console.WriteLine("Error during Driver initialization");
                        _driver?.Quit();
                  }


                  return Task.FromResult(_driver);
            }

            public async Task DoSiteLogin()
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
                        this.IsLoggedIn = true;
                        return;

                  }
                  catch (Exception ex)
                  {
                        Console.WriteLine("Failed to login to site");
                        Console.WriteLine(ex.Message);
                        throw new FailedWebDriverLogin("Login Failed");
                  }
            }











      }

}


