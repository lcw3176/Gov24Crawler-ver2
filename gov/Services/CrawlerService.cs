using gov.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace gov.Services
{
    public class CrawlerService
    {
        private ChromeDriver driver = new ChromeDriver();
        private WebDriverWait webDriverWait;

        public CrawlerService()
        {
            webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
        }

        public Task<bool> TryLogin(string userPw)
        {
            try
            {
                driver.Navigate().GoToUrl("https://www.gov.kr/nlogin/?Mcode=10003");
               
                driver.FindElement(By.XPath("/html/body/div[8]/ul/li[3]/span/a")).Click();
                driver.FindElement(By.Id("userId")).SendKeys(User.userId);
                driver.FindElement(By.Id("pwd")).SendKeys(userPw);
                driver.FindElement(By.Id("genLogin")).Click();

                return Task.FromResult(true);
            }

            catch
            {
                return Task.FromResult(false);
            }
            
        }

        public Task<bool> TryGetDocument(string bunzi, string ho, bool isSan)
        {
            try
            {
                driver.Navigate().GoToUrl("https://www.gov.kr/portal/main");

                // 임시 팝업 제거 코드
                // driver.ExecuteScript("document.getElementsByClassName('popWrap img_pop')[0].style.display='none';");
                
                // 토지(임야)대장 대기 후 클릭
                webDriverWait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("/html/body/div[9]/section[3]/div/div[3]/div/div[1]/div[1]/a[2]")));
                driver.FindElement(By.XPath("/html/body/div[9]/section[3]/div/div[3]/div/div[1]/div[1]/a[2]")).Click();

                // 신청 버튼 대기 후 클릭
                webDriverWait.Until(ExpectedConditions.ElementToBeClickable(By.Id("applyBtn")));
                driver.FindElement(By.Id("applyBtn")).Click();

                // 토지 대장 열람 창 전환
                webDriverWait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("/html/body/div[8]/div/div[1]/div[1]/a[3]")));
                driver.FindElement(By.XPath("/html/body/div[8]/div/div[1]/div[1]/a[3]")).SendKeys(Keys.Enter);

                // 주소 입력 창 띄우기
                var handles_before = driver.WindowHandles;
                webDriverWait.Until(ExpectedConditions.ElementToBeClickable(By.Id("btnAddress")));
                driver.FindElement(By.Id("btnAddress")).Click();

                // 주소 입력 창으로 focus 전환         
                webDriverWait.Until(e => handles_before.Count != driver.WindowHandles.Count);
                driver.SwitchTo().Window(driver.WindowHandles[1]);

                // 주소 입력, 검색 후 대기
                driver.FindElement(By.Id("txtAddr")).SendKeys(Config.firstAddress + " " + Config.secondAddress + " " + Config.thirdAddress);
                driver.FindElement(By.XPath("//*[@id=\"frm_popup\"]/fieldset/div/div/span/button")).SendKeys(Keys.Enter);
                webDriverWait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id=\"resultList\"]/a[2]")));

                // 자식 요소 검색
                var children = driver.FindElement(By.Id("resultList")).FindElements(By.TagName("a"));
                string tempAddress;

                // [0] 시, 도 [1] 시,군,구 [2]읍,면,동
                string[] addressArr = { };
                bool isSuccess = false;

                for (int i = 2; i <= children.Count; i++)
                {
                    tempAddress = driver.FindElement(By.XPath("//*[@id=\"resultList\"]/a[" + i.ToString() + "]/dl/dd/div")).Text;
                    addressArr = tempAddress.Split(new string[] { " " }, StringSplitOptions.None);
                    addressArr[2] = addressArr[2].Split('(')[1].Replace(")", "");

                    if (addressArr[0] == Config.firstAddress && addressArr[1] == Config.secondAddress && addressArr[2] == Config.thirdAddress)
                    {
                        driver.FindElement(By.XPath("//*[@id=\"resultList\"]/a[" + i.ToString() + "]/dl/dd/div")).Click();
                        isSuccess = true;
                        break;
                    }
                }

                // 맞는 주소 못찾으면 첫번째 주소 누르기
                if (!isSuccess)
                {
                    driver.FindElement(By.XPath("//*[@id=\"resultList\"]/a[2]")).Click();
                }

                driver.SwitchTo().Window(driver.WindowHandles[0]);

                // 대기
                webDriverWait.Until(ExpectedConditions.ElementExists(
                    By.XPath("//*[@id=\"토지임야대장신청서_IN-토지임야대장신청서_신청토지소재지_주소정보_상세주소_번지\"]")));


                // 번지 입력
                driver.FindElement(By.XPath("//*[@id=\"토지임야대장신청서_IN-토지임야대장신청서_신청토지소재지_주소정보_상세주소_번지\"]")).SendKeys(bunzi);
                driver.FindElement(By.XPath("//*[@id=\"토지임야대장신청서_IN-토지임야대장신청서_신청토지소재지_주소정보_상세주소_호\"]")).SendKeys(ho);


                // 연혁인쇄 설정
                driver.FindElement(By.Id("토지임야대장신청서_IN-토지임야대장신청서_연혁인쇄선택_.라디오코드_1")).Click();

                // '산' 들어가 있으면 임야대장으로 변경
                if (isSan)
                {
                    driver.FindElement(By.Id("토지임야대장신청서_IN-토지임야대장신청서_대장구분_.라디오코드_2")).Click();
                }

                // 제출 버튼
                driver.FindElement(By.Id("btn_end")).Click();

                return Task.FromResult(true);

            }

            catch
            {
                Console.WriteLine("다큐먼트 에러" + bunzi + "-" + ho);
                return Task.FromResult(false);
            }
            
        }

        public Task<bool> CheckValidation(string bunzi, string ho, bool isSan)
        {
            try
            {
                webDriverWait.Until(ExpectedConditions.ElementExists(
                   By.XPath("//*[@id=\"EncryptionAreaID_0\"]/div[1]/table/tbody")));

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

                if (isSan)
                {
                    tempAddress = "산 " + tempAddress;
                }

                for (int i = 1; i <= children.Count; i++)
                {
                    var handles_before = driver.WindowHandles;

                    // 열람 버튼 클릭
                    driver.FindElement(By.XPath("//*[@id=\"EncryptionAreaID_0\"]/div[1]/table/tbody/tr[" + i.ToString() + "]/td[4]/p[2]/span/a")).Click();
                    webDriverWait.Until(e => handles_before.Count != driver.WindowHandles.Count);

                    driver.SwitchTo().Window(driver.WindowHandles[1]);
                    
                    // 지번 로딩 대기
                    webDriverWait.Until(ExpectedConditions.ElementExists(
                              By.XPath("/html/body/div/div/div[2]/div[1]/table[2]/tbody/tr[1]/td[1]/table/tbody/tr[3]/td[2]")));
                    // 지번 일치 확인
                    temp = driver.FindElement(By.XPath("/html/body/div/div/div[2]/div[1]/table[2]/tbody/tr[1]/td[1]/table/tbody/tr[3]/td[2]")).Text;
                    
                    if (temp == tempAddress)
                    {
                        driver.SwitchTo().Window(driver.WindowHandles[1]);
                        return Task.FromResult(true);
                    }

                    driver.Close();
                    driver.SwitchTo().Window(driver.WindowHandles[0]);

                }

                return Task.FromResult(false);
            }

            catch(Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("밸리데이션 에러" + bunzi + "-" + ho);
                return Task.FromResult(false);
            }


        }

        public Task<string> TryGetArea()
        {
            try
            {
                string area;
                string areaStore = string.Empty;
                // 표 한개당 div 3개, 공유지 연명부 div 1개
                var children = driver.FindElement(By.XPath("/html/body/div/div/div[2]")).FindElements(By.TagName("div"));

                // 면적 구하기
                for (int i = 1; i <= children.Count / 3; i++)
                {
                    for (int j = 4; j <= 10; j += 2)
                    {
                        if(!IsElementPresent(By.XPath("/html/body/div/div/div[2]/div[" + i.ToString() + "]/table[2]/tbody/tr[2]/td/table/tbody/tr[" + j.ToString() + "]/td[2]/span")))
                        {
                            continue;
                        }

                        area = driver.FindElement(By.XPath("/html/body/div/div/div[2]/div[" + i.ToString() + "]/table[2]/tbody/tr[2]/td/table/tbody/tr[" + j.ToString() + "]/td[2]/span")).Text;

                        if (string.IsNullOrEmpty(area))
                        {
                            // 면적 분할, 합병에 의한 빈칸 체크
                            string merge = driver.FindElement(By.XPath("/html/body/div/div/div[2]/div[" + i.ToString() + "]/table[2]/tbody/tr[2]/td/table/tbody/tr[" + j.ToString() + "]/td[3]")).Text;
                            
                            if (string.IsNullOrEmpty(merge))
                            {
                                area = areaStore;
                                return Task.FromResult(area);
                            }
                        } 
                        else

                        {
                            areaStore = area;
                        }

                        
                    }
                }

                return Task.FromResult(areaStore);
            }

            catch(Exception ex)
            {
                // 에러 수정 필요
                Console.WriteLine(ex);
                Console.WriteLine("토지 에러");
                return Task.FromResult(string.Empty);
            }
        }

        private bool IsElementPresent(By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public Task<string> TryGetOwner()
        {
            try
            {
                string owner;
                string ownerStore = string.Empty;
                // 표 한개당 div 3개, 공유지 연명부 div 1개
                var children = driver.FindElement(By.XPath("/html/body/div/div/div[2]")).FindElements(By.TagName("div"));

                // 소유자 구하기
                for (int i = 1; i <= children.Count / 3; i++)
                {
                    for (int j = 5; j <= 11; j += 2)
                    {
                        owner = driver.FindElement(By.XPath("/html/body/div/div/div[2]/div[" + i.ToString() + "]/table[2]/tbody/tr[2]/td/table/tbody/tr[" + j.ToString() + "]/td[2]")).Text;

                        if (string.IsNullOrEmpty(owner))
                        {
                            // 소유자 말소로 인해 빈칸인 상황 대비
                            string malso = driver.FindElement(By.XPath("/html/body/div/div/div[2]/div[" + i.ToString() + "]/table[2]/tbody/tr[2]/td/table/tbody/tr[" + j.ToString() + "]/td[1]")).Text;

                            if (string.IsNullOrEmpty(malso))
                            {
                                owner = ownerStore;
                                return Task.FromResult(owner);
                            }
                            
                        }

                        ownerStore = owner;
                    }
                }

                return Task.FromResult(ownerStore);
            }

            catch
            {
                Console.WriteLine("소유자 에러");
                return Task.FromResult(string.Empty);
            }
        }

        public Task<bool> CaptureImage(string saveFileName)
        {
            try
            {
                Dictionary<string, Object> metrics = new Dictionary<string, Object>();
                metrics["width"] = driver.ExecuteScript("return Math.max(window.innerWidth,document.body.scrollWidth,document.documentElement.scrollWidth)");
                metrics["height"] = driver.ExecuteScript("return Math.max(window.innerHeight,document.body.scrollHeight,document.documentElement.scrollHeight)");
                metrics["deviceScaleFactor"] = driver.ExecuteScript("return window.devicePixelRatio");
                metrics["mobile"] = driver.ExecuteScript("return typeof window.orientation !== 'undefined'");

                driver.ExecuteCdpCommand("Emulation.setDeviceMetricsOverride", metrics);


                string path_to_save_screenshot = Config.savePath + @"/" + saveFileName + ".png";
                driver.GetScreenshot().SaveAsFile(path_to_save_screenshot, ScreenshotImageFormat.Png);
               
                return Task.FromResult(true);
            }

            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("캡쳐 에러" + saveFileName);
                return Task.FromResult(false);
            }
        }

        public Task<bool> CloseCompletedTab()
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
