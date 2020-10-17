using Microsoft.VisualStudio.TestTools.UnitTesting;
using Codist.Google.Entities;
using Codist.Google;

namespace TestProject
{
    [TestClass]
    public class GoogleTranslatorTest
    {
        private readonly ITranslator _googleTranslator = new GoogleTranslator();
        [TestMethod]
        public void Translate()
        {
            string sourceText = "TDD completely turned to traditional development.";
            TranslationResult transResult = _googleTranslator.Translate(sourceText, "en", "ru");
            Assert.AreEqual("TDD полностью превратился в традиционную разработку.", transResult.TargetText);

            sourceText = "How are you doing today?";
            transResult = _googleTranslator.Translate(sourceText, "ru", "en");
            Assert.AreEqual("How are you doing today?", transResult.TargetText);

            sourceText = "hello\"";
            transResult = _googleTranslator.Translate(sourceText, "en", "ru");
            Assert.AreEqual("Здравствуйте\"", transResult.TargetText);

            sourceText = "hello";
            transResult = _googleTranslator.Translate(sourceText, "en", "ru");
            Assert.AreEqual("Здравствуйте", transResult.TargetText);

            sourceText = "It's a very small project and may be fairly self explanatory if you are familiar with Visual Studio editor extensions. There are two components to the extension:";
            transResult = _googleTranslator.Translate(sourceText, "en", "ru");
            Assert.AreEqual("Это очень небольшой проект, и он может быть достаточно понятным, если вы знакомы с расширениями редактора Visual Studio. Расширение состоит из двух компонентов:", transResult.TargetText);


            sourceText = "<result>";
            transResult = _googleTranslator.Translate(sourceText, "en", "ru");
            Assert.AreEqual("<результат>", transResult.TargetText);
        }
    }
}