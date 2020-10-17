using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using VisualStudioTranslator.Entities;
using VisualStudioTranslator.Enums;
using VisualStudioTranslator.Google.Entities;

namespace VisualStudioTranslator.Google
{
    using static GoogleUtils;

    public class GoogleTranslator : ITranslator
    {

        private static readonly List<TranslationLanguage> TargetLanguages;
        private static readonly List<TranslationLanguage> SourceLanguages;


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
        private GoogleTransResult TranslateByHttp(string text, string from = "en", string to = "ru")
        {
            if (!(text.Length > 0 && text.Length < 5000))
            {
                return null;
            }
            text = text.Replace("\\", "");

            var tk = GetTk(text);

            var uri = $"https://translate.google.cn/translate_a/single?client=webapp&sl={from}&tl={to}&hl=zh-CN&dt=t&ie=UTF-8&oe=UTF-8&ssel=6&tsel=3&kc=0&tk={tk}&q={HttpUtility.UrlEncode(text)}";
            var html = string.Empty;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            using (var response = httpWebRequest.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    if (stream == null)
                    {
                        return null;
                    }
                    using (var sr = new StreamReader(stream))
                    {
                        html = sr.ReadToEnd();
                    }
                }
            }

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

            dynamic tempResult = Newtonsoft.Json.JsonConvert.DeserializeObject(html);
            var resarry = Newtonsoft.Json.JsonConvert.DeserializeObject(tempResult[0].ToString());
            var length = (resarry.Count);
            var str = new System.Text.StringBuilder();
            for (int i = 0; i < length; i++)
            {
                var res = Newtonsoft.Json.JsonConvert.DeserializeObject(resarry[i].ToString());
                str.Append(res[0].ToString());
            }
            return new GoogleTransResult()
            {
                From = tempResult[2].ToString(),
                TargetText = str.ToString()
            };
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
        public TranslationResult Translate(string text, string @from = "en", string to = "ru")
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
                    GoogleTransResult googleTransResult = TranslateByHttp(text, from, to);
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