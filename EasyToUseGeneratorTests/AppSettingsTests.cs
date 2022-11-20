//
// MIT License
//
// Copyright(c) 2019-2022 Benjamin Ellett
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//

using CommonGeneratorCode;
using EasyToUseGenerator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Language.Flow;
using System.Text.RegularExpressions;

namespace EasyToUseGenerator.Tests
{
    [TestClass]
    public class AppSettingsTests
    {
        [TestMethod]
        public void AppSettingsShouldUseTheDefaultSettingsValuesIfNoSettingsFileExists()
        {
            // This test tests the case where the settings file does not exist.
            (AppSettingsService appSettings, _) = CreateAppSettingsClass(expectedSettingsFileContent: null);

            // The AppSetting class should use the following values if the settings file does not exist.
            AssertAppSettingsMatchExpectedValues(
                appSettings,
                expectedDefaultPasswordType: Constants.InitialDefaultPasswordType,
                expectedDefaultPasswordLength: Constants.InitialDefaultPasswordLengthInChars);
        }

        [TestMethod]
        public void AppSettingsShouldLoadValidSettings()
        {
            // The AppSetting class should use the values which were in the settings file's JSON
            TestAppSettingsCorrectlyParsesSettingsInSettingsFile(
                expectedDefaultPasswordType: Constants.InitialDefaultPasswordType,
                expectedDefaultPasswordLength: Constants.InitialDefaultPasswordLengthInChars);

            TestAppSettingsCorrectlyParsesSettingsInSettingsFile(
                expectedDefaultPasswordType: PasswordType.AnyKeyOnAnEnglishKeyboardExceptASpace,
                expectedDefaultPasswordLength: 22);

            TestAppSettingsCorrectlyParsesSettingsInSettingsFile(
                expectedDefaultPasswordType: PasswordType.AlphaNumeric,
                expectedDefaultPasswordLength: 10);

            TestAppSettingsCorrectlyParsesSettingsInSettingsFile(
                expectedDefaultPasswordType: PasswordType.Numeric,
                expectedDefaultPasswordLength: 6);
        }

        [TestMethod]
        public void AppSettingsShouldChangeInvalidPasswordTypesToValidPasswordTypes()
        {
            // The PasswordType.AnyKeyOnAnEnglishKeyboard is not supported because spaces are not allowed
            // in passwords created by the GUI version of Generator.
            TestAppSettingsCorrectlyParsesSettingsInSettingsFile(
                expectedDefaultPasswordType: PasswordType.AnyKeyOnAnEnglishKeyboardExceptASpace,
                expectedDefaultPasswordLength: 8,
                defaultPasswordTypeInConfigurationFile: PasswordType.AnyKeyOnAnEnglishKeyboard);
        }

        [TestMethod]
        public void SaveShouldPersistTheSpecifiedSettings()
        {
            (AppSettingsService appSettings, Mock <ITextFileService> textFileServiceMock) = 
                TestAppSettingsCorrectlyParsesSettingsInSettingsFile(
                    expectedDefaultPasswordType: PasswordType.AlphaNumeric,
                    expectedDefaultPasswordLength: 10);

            appSettings.Save();

            textFileServiceMock.Verify(
                tfs => tfs.CreateDirectoryIfItDoesNotExist(It.IsAny<string>()),
                Times.Once,
                "The AppSettings class must create the directory which holds the settings file before it writes the settings file.");

            textFileServiceMock.Verify(
                tfs => tfs.WriteTextFile(
                    It.IsAny<string>(), 
                    It.Is<string>(json => IsSettingsFileContentValid(json, PasswordType.AlphaNumeric, 10))),
                Times.Once,
                "The AppSettings class must correctly write its current settings.");
        }

        private static bool IsSettingsFileContentValid(string savedSettingsFileJsonContent, PasswordType expectedPasswordType, int expectedPasswordLength)
        {
            Regex matchDefaultPasswordTypeRegEx = CreateJsonPropertyMatcherRegEx(
                jsonProperyName: "DefaultPasswordType",
                jsonProperyValue: $"\"{expectedPasswordType}\"");

            Regex matchDefaultPasswordLengthRegEx = CreateJsonPropertyMatcherRegEx(
                jsonProperyName: "DefaultPasswordLengthInChars",
                jsonProperyValue: expectedPasswordLength);

            return matchDefaultPasswordTypeRegEx.IsMatch(savedSettingsFileJsonContent) &&
                   matchDefaultPasswordLengthRegEx.IsMatch(savedSettingsFileJsonContent);



            static Regex CreateJsonPropertyMatcherRegEx(string jsonProperyName, object jsonProperyValue)
            {
                string matchJsonPropetyRegExString = $"\"{jsonProperyName}\":\\s*{jsonProperyValue}";
                return new Regex(matchJsonPropetyRegExString, RegexOptions.Multiline | RegexOptions.CultureInvariant);
            }
        }

        private static (AppSettingsService, Mock<ITextFileService>) CreateAppSettingsClass(string? expectedSettingsFileContent)
        {
            Mock<ITextFileService> textFileServiceMock = CreateTextServiceMock(expectedSettingsFileContent);
            AppSettingsService newAppSettings = new AppSettingsService(textFileServiceMock.Object);
            return (newAppSettings, textFileServiceMock);
        }

        private static Mock<ITextFileService> CreateTextServiceMock(string? expectedTextFileContent)
        {
            Mock<ITextFileService> textFileServiceMock = new Mock<ITextFileService>(MockBehavior.Strict);

            ISetup<ITextFileService, bool> textFileServiceMockSetup;

            // TFS stands for Text File Service.
            textFileServiceMockSetup = textFileServiceMock.Setup(tfs => tfs.TryReadTextFile(It.IsAny<string>(), out expectedTextFileContent));

            if (expectedTextFileContent != null)
            {
                textFileServiceMockSetup.Returns(true);
            }
            else
            {
                textFileServiceMockSetup.Returns(false);
            }

            textFileServiceMock.Setup(tfs => tfs.CreateDirectoryIfItDoesNotExist(It.IsAny<string>()));
            textFileServiceMock.Setup(tfs => tfs.WriteTextFile(It.IsAny<string>(), It.IsAny<string>()));

            return textFileServiceMock;
        }

        private static (AppSettingsService, Mock<ITextFileService>) TestAppSettingsCorrectlyParsesSettingsInSettingsFile(
            PasswordType expectedDefaultPasswordType, 
            int expectedDefaultPasswordLength, 
            PasswordType? defaultPasswordTypeInConfigurationFile = null)
        {
            if (!defaultPasswordTypeInConfigurationFile.HasValue)
            {
                defaultPasswordTypeInConfigurationFile = expectedDefaultPasswordType;
            }

            string settingsFileContent =
                 "{                                                                                        \n" +
                $"    \"DefaultPasswordType\": \"{defaultPasswordTypeInConfigurationFile.ToString()}\",    \n" +
                $"    \"DefaultPasswordLengthInChars\": {expectedDefaultPasswordLength}                    \n" +
                 "}                                                                                        \n";

            (AppSettingsService appSettings, Mock<ITextFileService> textFileServiceMock) = CreateAppSettingsClass(settingsFileContent);

            AssertAppSettingsMatchExpectedValues(
                appSettings,
                expectedDefaultPasswordType,
                expectedDefaultPasswordLength);

            return (appSettings, textFileServiceMock);
        }

        private static void AssertAppSettingsMatchExpectedValues(AppSettingsService appSettings, PasswordType expectedDefaultPasswordType, int expectedDefaultPasswordLength)
        {
            Assert.IsTrue(appSettings.DefaultPasswordType == expectedDefaultPasswordType);
            Assert.IsTrue(appSettings.DefaultPasswordLengthInChars == expectedDefaultPasswordLength);
        }
    }
}