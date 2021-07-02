using gov.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace gov.Services
{
    public class CrawlerService
    {
        private ChromeDriver driver = new ChromeDriver();
        private WebDriverWait webDriverWait;

        public CrawlerService()
        {
            webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(120));
        }

        public Task<bool> TryLogin(string userPw)
        {
            try
            {
                driver.Navigate().GoToUrl("https://www.gov.kr/nlogin/?Mcode=10003");

                driver.FindElementByXPath("/html/body/div[7]/ul/li[3]/a").Click();
                driver.FindElementById("userId").SendKeys(User.userId);
                driver.FindElementById("pwd").SendKeys(userPw);
                driver.FindElementById("genLogin").Click();

                return Task.FromResult(true);
            }

            catch
            {
                return Task.FromResult(false);
            }
            
        }

        public Task<bool> TryGetDocument(string bunzi, string ho)
        {
            try
            {
                driver.Navigate().GoToUrl("https://www.gov.kr/portal/main");
                // 토지(임야)대장 대기 후 클릭
                webDriverWait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id=\"container\"]/div/section[5]/div/div[2]/div[2]/ul/li[2]/div/a[3]")));
                driver.FindElementByXPath("//*[@id=\"container\"]/div/section[5]/div/div[2]/div[2]/ul/li[2]/div/a[3]").Click();

                // 신청 버튼 대기 후 클릭
                webDriverWait.Until(ExpectedConditions.ElementToBeClickable(By.Id("applyBtn")));
                driver.FindElementById("applyBtn").Click();

                // 토지 대장 열람 창 전환
                webDriverWait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("/html/body/div[6]/div/div[1]/div[1]/a[3]")));
                driver.FindElementByXPath("/html/body/div[6]/div/div[1]/div[1]/a[3]").SendKeys(Keys.Enter);

                // 주소 입력 창 띄우기
                var handles_before = driver.WindowHandles;
                webDriverWait.Until(ExpectedConditions.ElementToBeClickable(By.Id("btnAddress")));
                driver.FindElementById("btnAddress").Click();

                // 주소 입력 창으로 focus 전환         
                webDriverWait.Until(e => handles_before.Count != driver.WindowHandles.Count);
                driver.SwitchTo().Window(driver.WindowHandles[1]);

                // 주소 입력, 검색 후 대기
                driver.FindElementById("txtAddr").SendKeys(Config.firstAddress + " " + Config.secondAddress + " " + Config.thirdAddress);
                driver.FindElementByXPath("//*[@id=\"frm_popup\"]/fieldset/div/div/span/button").SendKeys(Keys.Enter);
                webDriverWait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id=\"resultList\"]/a[2]")));

                // 자식 요소 검색
                var children = driver.FindElementById("resultList").FindElements(By.TagName("a"));
                string tempAddress;

                // [0] 시, 도 [1] 시,군,구 [2]읍,면,동
                string[] addressArr = { };
                bool isSuccess = false;

                for (int i = 2; i <= children.Count; i++)
                {
                    tempAddress = driver.FindElementByXPath("//*[@id=\"resultList\"]/a[" + i.ToString() + "]/dl/dd/div").Text;
                    addressArr = tempAddress.Split(new string[] { " " }, StringSplitOptions.None);
                    addressArr[2] = addressArr[2].Split('(')[1].Replace(")", "");

                    if (addressArr[0] == Config.firstAddress && addressArr[1] == Config.secondAddress && addressArr[2] == Config.thirdAddress)
                    {
                        driver.FindElementByXPath("//*[@id=\"resultList\"]/a[" + i.ToString() + "]/dl/dd/div").Click();
                        isSuccess = true;
                        break;
                    }
                }

                // 맞는 주소 못찾으면 첫번째 주소 누르기
                if (!isSuccess)
                {
                    driver.FindElementByXPath("//*[@id=\"resultList\"]/a[2]").Click();
                }

                driver.SwitchTo().Window(driver.WindowHandles[0]);

                // 번지 입력
                driver.FindElementById("토지임야대장신청서_IN-토지임야대장신청서_신청토지소재지_주소정보_상세주소_번지").SendKeys(bunzi);
                driver.FindElementById("토지임야대장신청서_IN-토지임야대장신청서_신청토지소재지_주소정보_상세주소_호").SendKeys(ho);


                // 연혁인쇄 설정
                driver.FindElementById("토지임야대장신청서_IN-토지임야대장신청서_연혁인쇄선택_.라디오코드_1").Click();
                // 제출 버튼
                driver.FindElementById("btn_end").Click();

                webDriverWait.Until(ExpectedConditions.ElementToBeClickable(
                               By.XPath("//*[@id=\"EncryptionAreaID_0\"]/div[1]/table/tbody/tr/td[4]/p[2]/span/a")));

                return Task.FromResult(true);

            }

            catch
            {
                Debug.WriteLine("다큐먼트 에러");
                return Task.FromResult(false);
            }
            
        }
        public Task<bool> CheckValidation(string bunzi, string ho)
        {
            try
            {
                // 자식 요소 검색
                var children = driver.FindElement(By.XPath("//*[@id=\"EncryptionAreaID_0\"]/div[1]/table/tbody")).FindElements(By.TagName("tr"));
                string temp;
                string tempAddress;
                if(string.IsNullOrEmpty(ho))
                {
                    tempAddress = bunzi;
                }

                else
                {
                    tempAddress = bunzi + "-" + ho;
                }

                for (int i = 1; i <= children.Count; i++)
                {
                    var handles_before = driver.WindowHandles;

                    // 열람 버튼 클릭
                    driver.FindElementByXPath("//*[@id=\"EncryptionAreaID_0\"]/div[1]/table/tbody/tr[" + i.ToString() + "]/td[4]/p[2]/span/a").Click();
                    webDriverWait.Until(e => handles_before.Count != driver.WindowHandles.Count);

                    driver.SwitchTo().Window(driver.WindowHandles[1]);
                    
                    // 지번 로딩 대기
                    webDriverWait.Until(ExpectedConditions.ElementExists(
                              By.XPath("//*[@id=\"EncryptionAreaID_0\"]/div[1]/table[2]/tbody/tr[1]/td[1]/table/tbody/tr[3]/td[2]")));
                    // 지번 일치 확인
                    temp = driver.FindElementByXPath("//*[@id=\"EncryptionAreaID_0\"]/div[1]/table[2]/tbody/tr[1]/td[1]/table/tbody/tr[3]/td[2]").Text;
                    
                    if (temp == tempAddress)
                    {
                        Debug.WriteLine("일치");
                        driver.SwitchTo().Window(driver.WindowHandles[1]);
                        return Task.FromResult(true);
                    }

                    Debug.WriteLine("조뺑이");
                    driver.Close();
                    driver.SwitchTo().Window(driver.WindowHandles[0]);

                }

                return Task.FromResult(false);
            }

            catch
            {
                Debug.WriteLine("밸리데이션 에러");
                return Task.FromResult(false);
            }


        }

        public Task<string> TryGetArea()
        {
            try
            {
                string area;
                string areaStore = string.Empty;
                var children = driver.FindElementById("EncryptionAreaID_0").FindElements(By.TagName("div"));

                Debug.WriteLine(children.Count);

                // 면적 구하기
                for (int i = 1; i <= children.Count; i++)
                {
                    for (int j = 4; j <= 10; j += 2)
                    {
                        area = driver.FindElementByXPath("//*[@id=\"EncryptionAreaID_0\"]/div[" + i.ToString() + "]/table[2]/tbody/tr[2]/td/table/tbody/tr[" + j.ToString() + "]/td[2]/span").Text;
                        Debug.WriteLine(area);
                        if (string.IsNullOrEmpty(area))
                        {
                            area = areaStore;
                            return Task.FromResult(area);
                        }

                        areaStore = area;
                    }
                }

                return Task.FromResult(string.Empty);
            }

            catch
            {
                Debug.WriteLine("토지 에러");
                return Task.FromResult(string.Empty);
            }
        }

        public Task<string> TryGetOwner()
        {
            try
            {
                string owner;
                string ownerStore = string.Empty;
                var children = driver.FindElementById("EncryptionAreaID_0").FindElements(By.TagName("div"));

                // 소유자 구하기
                for (int i = 1; i <= children.Count; i++)
                {
                    for (int j = 5; j <= 11; j += 2)
                    {
                        owner = driver.FindElementByXPath("//*[@id=\"EncryptionAreaID_0\"]/div[" + i.ToString() + "]/table[2]/tbody/tr[2]/td/table/tbody/tr[" + j.ToString() + "]/td[2]").Text;

                        if (string.IsNullOrEmpty(owner))
                        {
                            owner = ownerStore;
                            return Task.FromResult(owner);
                        }

                        ownerStore = owner;
                    }
                }

                return Task.FromResult(string.Empty);
            }

            catch
            {
                Debug.WriteLine("소유자 에러");
                return Task.FromResult(string.Empty);
            }
        }

        public Task<bool> Capture(string bunzi, string ho)
        {
            try
            {
                Dictionary<string, Object> metrics = new Dictionary<string, Object>();
                metrics["width"] = driver.ExecuteScript("return Math.max(window.innerWidth,document.body.scrollWidth,document.documentElement.scrollWidth)");
                metrics["height"] = driver.ExecuteScript("return Math.max(window.innerHeight,document.body.scrollHeight,document.documentElement.scrollHeight)");
                metrics["deviceScaleFactor"] = driver.ExecuteScript("return window.devicePixelRatio");
                metrics["mobile"] = driver.ExecuteScript("return typeof window.orientation !== 'undefined'");

                driver.ExecuteChromeCommand("Emulation.setDeviceMetricsOverride", metrics);

                string path_to_save_screenshot = Config.savePath + @"/" + bunzi + "-" + ho + ".png";
                driver.GetScreenshot().SaveAsFile(path_to_save_screenshot, ScreenshotImageFormat.Png);
               
                return Task.FromResult(true);
            }

            catch
            {
                Debug.WriteLine("캡쳐 에러");
                return Task.FromResult(false);
            }
        }

        public Task<bool> CloseTab()
        {
            try
            {
                driver.Close();
                driver.SwitchTo().Window(driver.WindowHandles[0]);

                return Task.FromResult(true);
            }

            catch
            {
                return Task.FromResult(false);
            }
            
        }
    }
}
