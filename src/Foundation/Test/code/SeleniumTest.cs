﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Newtonsoft.Json;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace Sitecore.Demo.Shared.Foundation.Test
{
    public class SeleniumTest
    {
        private static string _codeBasePath;
        private string _bitmapsPath;
        private IWebDriver _driver;


        public string BackstopPath { get; set; } = FindBackstopPath();


        public string BitmapsPath =>
            _bitmapsPath ??
            (_bitmapsPath = Path.Combine(BackstopPath, "backstop_data", "bitmaps_test",
                DateTime.Now.ToString("yyyyMMdd-HHmmss")));


        protected static string CodeBasePath
        {
            get
            {
                if (_codeBasePath != null)
                    return _codeBasePath;

                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                _codeBasePath = Path.GetDirectoryName(path);
                return CodeBasePath;
            }
        }


        protected IWebDriver Driver
        {
            get
            {
                if (_driver != null)
                    return _driver;

                _driver = new ChromeDriver();
                _driver.Manage().Window.Size = new Size(1024, 768);
                return _driver;
            }
        }


        public void AcceptAlert()
        {
            var alertBox = Driver.SwitchTo().Alert();
            Console.WriteLine($"accepting alert \"{alertBox.Text}\"");
            alertBox.Accept();
        }


        protected void ApplyConfig()
        {
            try
            {
                var jsonFilename = Path.Combine(CodeBasePath, GetType().Name + ".json");
                var json = File.ReadAllText(jsonFilename);
                JsonConvert.PopulateObject(json, this);
            }
            catch (Exception ex)
            {
                // Just log the exception and rely on defaults.
                // If it's catastrophic, the tests will fail.
                Console.WriteLine(ex.ToString());
            }
        }


        protected void CenterOn(IWebElement element)
        {
            const string centerOnElement = "var viewPortHeight = Math.max(document.documentElement.clientHeight, window.innerHeight || 0);" +
                                           "var elementTop = arguments[0].getBoundingClientRect().top;" +
                                           "window.scrollBy(0, elementTop-(viewPortHeight/2));";

            ((IJavaScriptExecutor) Driver).ExecuteScript(centerOnElement, element);
        }


        protected void CenterOn(string descriptor)
        {
            Console.WriteLine($"centering on \"{descriptor}\"");
            Wait(descriptor);
            var element = GetElement(descriptor);
            CenterOn(element);
        }


        protected void Click(IWebElement element)
        {
            try
            {
                element.Click();
            }
            catch (WebDriverException wde)
            {
                if (wde.Message.Contains("is not clickable at point") &&
                    wde.Message.Contains("Other element would receive the click") ||
                    wde.Message.Contains("cannot interact with element"))
                {
                    var js = Driver as IJavaScriptExecutor;
                    js?.ExecuteScript("arguments[0].click();", element);
                }
                else
                {
                    throw;
                }
            }
        }


        protected void Click(string descriptor, int? milliseconds = null)
        {
            Console.WriteLine($"clicking \"{descriptor}\"");
            Wait(descriptor, milliseconds);
            var element = GetElement(descriptor);
            Click(element);
        }


        protected void EnterText(IWebElement element, string text)
        {
            element.SendKeys(text);
        }


        protected void EnterText(string descriptor, string text)
        {
            Console.WriteLine($"entering \"{text}\" on \"{descriptor}\"");
            Wait(descriptor);
            EnterText(GetElement(descriptor), text);
        }


        protected bool Exists(string descriptor)
        {
            try
            {
                GetElement(descriptor);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }


        protected static string FindBackstopPath()
        {
            var path = CodeBasePath;
            var backstopPath = Path.Combine(path, "backstop");
            while (!Directory.Exists(Path.Combine(backstopPath, "backstop_data")))
            {
                var dir = Directory.GetParent(path);

                // If dir is null, then we've climbed the directory tree
                // as far as we can go.  The backstop directory either
                // doesn't exist or it is somewhere out of reach.  Just
                // return the CodeBasePath.
                if (dir == null)
                    return CodeBasePath;

                path = dir.FullName;
                backstopPath = Path.Combine(path, "backstop");
            }

            return backstopPath;
        }


        protected IWebElement GetElement(string descriptor)
        {
            var element = GetElements(descriptor).FirstOrDefault();
            if (element == null)
                throw new NoSuchElementException($"Could not find element \"{descriptor}\"");
            return element;
        }


        protected IEnumerable<IWebElement> GetElements(string descriptor)
        {
            var elements = Driver.FindElements(By.PartialLinkText(descriptor));
            if (!elements.Any())
                elements = Driver.FindElements(By.CssSelector(descriptor));
            if (!elements.Any())
                elements = Driver.FindElements(By.Name(descriptor));
            return elements;
        }


        protected void GoTo(string url)
        {
            Console.WriteLine($"going to {url}");
            Driver.Navigate().GoToUrl(url);
            WaitForDocumentReady();
        }


        protected void HoverOn(IWebElement element)
        {
            var actions = new Actions(Driver);
            actions.MoveToElement(element);
            actions.Perform();
        }


        protected void HoverOn(string descriptor)
        {
            Console.WriteLine($"hovering on \"{descriptor}\"");
            Wait(descriptor);
            var element = GetElement(descriptor);
            HoverOn(element);
        }


        protected void ScrollTo(IWebElement element)
        {
            var actions = new Actions(Driver);
            actions.MoveToElement(element);
            actions.Perform();
        }


        protected void ScrollTo(string descriptor)
        {
            Console.WriteLine($"scrolling to \"{descriptor}\"");
            Wait(descriptor);
            var element = GetElement(descriptor);
            HoverOn(element);
        }


        public void TakeFullPageScreenshot(string name)
        {
            // Add html2canvas if it isn't already loaded.
            WaitForDocumentReady();
            var js = Driver as IJavaScriptExecutor;
            if (js != null && (bool) js?.ExecuteScript("return (typeof html2canvas == 'undefined')"))
                js.ExecuteScript(
                    "var script = document.createElement('script'); script.src = \"https://cdnjs.cloudflare.com/ajax/libs/html2canvas/0.4.1/html2canvas.min.js\"; document.body.appendChild(script);");

            // Wait for it to load
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(60));
            wait.IgnoreExceptionTypes(typeof(InvalidOperationException));
            wait.Until(wd => js != null && (bool) js.ExecuteScript("return (typeof html2canvas != 'undefined')"));

            // Take a full page screenshot using html2canvas
            // ReSharper disable once IdentifierTypo
            const string generateScreenshotJs = @"(function() {
	                html2canvas(document.body, {
 		                onrendered: function (canvas) {                                          
		                    window.canvasImgContentDecoded = canvas.toDataURL(""image/png"");
	                    }
                    });
                })();";
            if (js == null) return;
            {
                js.ExecuteScript(generateScreenshotJs);

                // Wait for the screenshot to complete
                wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(60));
                wait.IgnoreExceptionTypes(typeof(InvalidOperationException));
                wait.Until(wd => !string.IsNullOrEmpty(js.ExecuteScript("return canvasImgContentDecoded;") as string));

                // Convert to raw bytes
                var pngContent = (string) js.ExecuteScript("return canvasImgContentDecoded;");
                pngContent = pngContent.Replace("data:image/png;base64,", string.Empty);
                var data = Convert.FromBase64String(pngContent);

                // Save screenshot to BitmapsPath under backstop
                name = Path.Combine(BitmapsPath,
                    $"{GetType().Name}_{TestContext.CurrentContext.Test.Name}_{name}.png");
                Directory.CreateDirectory(BitmapsPath);
                File.WriteAllBytes(name, data);
            }
        }


        public void TakeScreenshot(string name, string language = "en")
        {
            TakeScreenshot(name, (IWebElement) null, language);
        }


        public void TakeScreenshot(string name, string onElement, string language)
        {
            TakeScreenshot(name, GetElement(onElement), language);
        }


        public void TakeScreenshot(string name, IWebElement onElement, string language)
        {
            if (onElement != null)
                CenterOn(onElement);

            var its = _driver as ITakesScreenshot;
            var s = its?.GetScreenshot();
            Directory.CreateDirectory(BitmapsPath);
            name = Path.Combine(BitmapsPath,
                $"{GetType().Name}_{TestContext.CurrentContext.Test.Name}_{name}-{language}.png");
            s?.SaveAsFile(name, ScreenshotImageFormat.Png);
        }


        [TearDown]
        public void TestCleanup()
        {
            if (_driver == null) return;
            _driver.Dispose();
            _driver = null;
        }


        [SetUp]
        public void TestInit()
        {
            ApplyConfig();
        }


        protected void Wait(int milliseconds)
        {
            Console.WriteLine($"waiting {milliseconds / 1000d} second{(milliseconds == 1000 ? "" : "s")}");
            Thread.Sleep(milliseconds);
        }


        protected IWebElement Wait(string descriptor, int? milliseconds = null)
        {
            Console.WriteLine($"waiting for element \"{descriptor}\"");

            var start = DateTime.Now;
            while (milliseconds == null || (DateTime.Now - start).TotalMilliseconds < milliseconds)
                try
                {
                    return GetElement(descriptor);
                }
                catch
                {
                    // Wait a tiny amount of time and try again
                    Thread.Sleep(200);
                }

            throw new TimeoutException(
                $"Element \"{descriptor}\" never appeared within {milliseconds / 1000d} second{(milliseconds == 1000 ? "" : "s")}.");
        }


        protected void WaitForDocumentReady()
        {
            var timeout = new TimeSpan(0, 0, 60);
            var wait = new WebDriverWait(Driver, timeout);

            if (!(Driver is IJavaScriptExecutor javascript))
                throw new ArgumentException("driver", $"Driver must support javascript execution");

            wait.Until(d =>
            {
                try
                {
                    var readyState = javascript.ExecuteScript("if (document.readyState) return document.readyState;")
                        .ToString();
                    return readyState.ToLower() == "complete";
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }
        protected static string Capitalize(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            if (text.Length == 1)
                return text.ToUpper();
            return text.Substring(0, 1).ToUpper() + text.Substring(1);
        }

        protected static char? GetUserNameDelimiter(string name)
        {
            char? delimiter = null;
            foreach (var c in "._-")
            {
                if (!name.Contains(c))
                {
                    continue;
                }

                delimiter = c;
                break;
            }

            return delimiter;
        }
    }
}