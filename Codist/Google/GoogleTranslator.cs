using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Codist.Google.Entities;
using Codist.Google.Enums;

namespace Codist.Google
{

    public class GoogleTranslator : ITranslator
    {

        private static readonly List<TranslationLanguage> TargetLanguages;
        private static readonly List<TranslationLanguage> SourceLanguages;
        private static readonly HttpClient client = new HttpClient();


        static GoogleTranslator()
        {
            TargetLanguages = new List<TranslationLanguage>()
            {
                new TranslationLanguage ("af", "Afrikaans"),
                new TranslationLanguage ("sq", "Albanian"),
                new TranslationLanguage ("ar", "Arabic"),
                new TranslationLanguage ("be", "Belarusian"),
                new TranslationLanguage ("bg", "Bulgarian"),
                new TranslationLanguage ("ca", "Catalan"),
                new TranslationLanguage ("zh-CN", "Chinese (Simplified)"),
                new TranslationLanguage ("zh-TW", "Chinese (Traditional)"),
                new TranslationLanguage ("hr", "Croatian"),
                new TranslationLanguage ("cs", "Czech"),
                new TranslationLanguage ("da", "Danish"),
                new TranslationLanguage ("nl", "Dutch"),
                new TranslationLanguage ("en", "English"),
                new TranslationLanguage ("et", "Estonian"),
                new TranslationLanguage ("tl", "Filipino"),
                new TranslationLanguage ("fi", "Finnish"),
                new TranslationLanguage ("fr", "French"),
                new TranslationLanguage ("gl", "Galician"),
                new TranslationLanguage ("de", "German"),
                new TranslationLanguage ("el", "Greek"),
                new TranslationLanguage ("iw", "Hebrew"),
                new TranslationLanguage ("hi", "Hindi"),
                new TranslationLanguage ("hu", "Hungarian"),
                new TranslationLanguage ("is", "Icelandic"),
                new TranslationLanguage ("id", "Indonesian"),
                new TranslationLanguage ("ga", "Irish"),
                new TranslationLanguage ("it", "Italian"),
                new TranslationLanguage ("ja", "Japanese"),
                new TranslationLanguage ("ko", "Korean"),
                new TranslationLanguage ("lv", "Latvian"),
                new TranslationLanguage ("lt", "Lithuanian"),
                new TranslationLanguage ("mk", "Macedonian"),
                new TranslationLanguage ("ms", "Malay"),
                new TranslationLanguage ("mt", "Maltese"),
                new TranslationLanguage ("fa", "Persian"),
                new TranslationLanguage ("pl", "Polish"),
                new TranslationLanguage ("pt", "Portugese"),
                new TranslationLanguage ("ro", "Romanian"),
                new TranslationLanguage ("ru", "Russian"),
                new TranslationLanguage ("sr", "Serbian"),
                new TranslationLanguage ("sk", "Slovak"),
                new TranslationLanguage ("sl", "Slovenian"),
                new TranslationLanguage ("es", "Spanish"),
                new TranslationLanguage ("sw", "Swahili"),
                new TranslationLanguage ("sv", "Swedish"),
                new TranslationLanguage ("th", "Thai"),
                new TranslationLanguage ("tr", "Turkish"),
                new TranslationLanguage ("uk", "Ukranian"),
                new TranslationLanguage ("vi", "Vietnamese"),
                new TranslationLanguage ("cy", "Welsh"),
                new TranslationLanguage ("yi", "Yiddish"),
                new TranslationLanguage ("mn", "Mongolian"),
                new TranslationLanguage ("la", "Latin")
            };

            SourceLanguages = new List<TranslationLanguage>() { new TranslationLanguage("auto", "Auto-detect") };
            SourceLanguages.AddRange(TargetLanguages);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="text">text's length must between 0 and 5000</param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        private async Task<GoogleTransResult> TranslateByHttpAsync(string text, string from = "en", string to = "ru")
        {
            if (!(text.Length > 0 && text.Length < 5000))
            {
                return null;
            }
            text = text.Replace("\\", "");

            var tk = GoogleUtils.GetTk(text);
            var uri = $"https://translate.google.cn/translate_a/single?client=webapp&sl={from}&tl={to}&hl=zh-CN&dt=t&ie=UTF-8&oe=UTF-8&ssel=6&tsel=3&kc=0&tk={tk}&q={HttpUtility.UrlEncode(text)}";

            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                string html = await response.Content.ReadAsStringAsync();

                // Above three lines can be replaced with new helper method below
                // string responseBody = await client.GetStringAsync(uri);

                if (html.Contains("/sorry/index?continue=") && html.Contains("302 Moved"))
                {
                    return new GoogleTransResult()
                    {
                        From = "Unknown",
                        TargetText = "Current IP traffic anomaly, can not be translated!"
                    };
                }
                if (html.Contains("Error 403!"))
                {
                    return new GoogleTransResult()
                    {
                        From = "Unknown",
                        TargetText = "Error 403!"
                    };
                }

                JArray jArray = JArray.Parse(html);
                var resarry = jArray[0];
                var str = new System.Text.StringBuilder();
                foreach (JArray res in resarry)
                {
                    str.Append(res[0].ToString());
                }
                return new GoogleTransResult()
                {
                    From = jArray[2].ToString(),
                    TargetText = str.ToString()
                };
            }
            catch (HttpRequestException e)
            {
                return new GoogleTransResult()
                {
                    From = "Exception Caught!",
                    TargetText = $"Message :{e.Message}"
                };
            }
        }

        public string GetIdentity()
        {
            return "Google";
        }
        public static string GetName()
        {
            return "Google Translator";
        }

        public static string GetDescription()
        {
            return "place the single request control within 5000 bytes in length (One Chinese Is a byte), you can on the website translation https://translate.google.cn/";
        }
        public static string GetWebsite()
        {
            return "https://translate.google.cn/";
        }

        public List<TranslationLanguage> GetAllTargetLanguages()
        {
            return TargetLanguages;
        }

        public List<TranslationLanguage> GetAllSourceLanguages()
        {
            return SourceLanguages;
        }

        public static List<TranslationLanguage> GetTargetLanguages()
        {
            return TargetLanguages;
        }

        public static List<TranslationLanguage> GetSourceLanguages()
        {
            return SourceLanguages;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text">text's length must between 0 and 5000</param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<TranslationResult> TranslateAsync(string text, string @from = "en", string to = "ru")
        {
            TranslationResult result = new TranslationResult()
            {
                SourceLanguage = @from,
                TargetLanguage = to,
                SourceText = text,
                TargetText = "",
                FailedReason = ""
            };
            if (SourceLanguages.Count(sl => sl.Code == @from) <= 0)
            {
                result.TranslationResultTypes = TranslationResultTypes.Failed;
                result.FailedReason = "unrecognizable source language";
            }
            else if (TargetLanguages.Count(tl => tl.Code == to) <= 0)
            {
                result.TranslationResultTypes = TranslationResultTypes.Failed;
                result.FailedReason = "unrecognizable target language";
            }
            else
            {
                try
                {
                    result.TranslationResultTypes = TranslationResultTypes.Successed;
                    GoogleTransResult googleTransResult = await TranslateByHttpAsync(text, from, to);
                    result.SourceLanguage = googleTransResult.From;
                    result.TargetText = googleTransResult.TargetText;
                }
                catch (Exception exception)
                {
                    result.FailedReason = exception.Message;
                    result.TranslationResultTypes = TranslationResultTypes.Failed;
                }
            }
            return result;
        }
    }
}