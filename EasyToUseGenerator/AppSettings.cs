//
// MIT License
//
// Copyright(c) 2019-2021 Benjamin Ellett
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
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasyToUseGenerator
{
    public interface IAppSettingService
    {
        public int DefaultPasswordLengthInChars { get; set; }

        public PasswordType DefaultPasswordType { get; set; }

        public void Save();
    }

    public class AppSettings : IAppSettingService
    {
        private readonly ITextFileService textFileService;

        private readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
        {
            MaxDepth = 2,
            Encoder = null, // Use the default encoder

            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            Converters =
                {
                    new JsonStringEnumConverter(namingPolicy: null, allowIntegerValues: false)
                },

            IgnoreReadOnlyFields = true,
            IgnoreReadOnlyProperties = true,
            IncludeFields = false,

            AllowTrailingCommas = false,
            NumberHandling = JsonNumberHandling.Strict,
            ReadCommentHandling = JsonCommentHandling.Disallow,
            PropertyNameCaseInsensitive = false,

            WriteIndented = true,
        };

        public AppSettings(ITextFileService textFileService)
        {
            this.textFileService = textFileService;
            this.InitializeSettings();
        }

        public int DefaultPasswordLengthInChars { get; set; }

        public PasswordType DefaultPasswordType { get; set; }

        public void Save()
        {
            SerializedSettings serializedSettings = new SerializedSettings(this);
            string jsonSerializedSettings = JsonSerializer.Serialize<SerializedSettings>(serializedSettings, jsonSerializerOptions);
            this.textFileService.CreateDirectoryIfItDoesNotExist(this.SettingsFileDirectoryPath);
            this.textFileService.WriteTextFile(this.SettingsFileNamePath, jsonSerializedSettings);
        }

        private string SettingsFileDirectoryPath
        {
            get
            {
                const string ApplicationName = "Generator";

                string roamingAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                return Path.Combine(roamingAppDataFolder, ApplicationName);
            }
        }

        private string SettingsFileNamePath
        {
            get
            {
                const string SettingsFileName = "GeneratorSettings.json";

                return Path.Combine(this.SettingsFileDirectoryPath, SettingsFileName);
            }
        }

        private void InitializeSettings()
        {
            this.DefaultPasswordType = Constants.InitialDefaultPasswordType;
            this.DefaultPasswordLengthInChars = Constants.InitialDefaultPasswordLengthInChars;

            string jsonSerializedSettings;

            if (this.textFileService.TryReadTextFile(this.SettingsFileNamePath, out jsonSerializedSettings))
            {
                SerializedSettings serializedSettings = JsonSerializer.Deserialize<SerializedSettings>(jsonSerializedSettings, jsonSerializerOptions);
                
                // Ignore invalid settings stored in the configuration file.
                
                // The GUI version of this application does not support passwords with spaces.
                if (Password.IsValidPasswordType(serializedSettings.DefaultPasswordType) &&
                    (serializedSettings.DefaultPasswordType != PasswordType.AnyKeyOnAnEnglishKeyboard))
                {
                    this.DefaultPasswordType = serializedSettings.DefaultPasswordType;
                }

                if (Password.IsValidPasswordLength(serializedSettings.DefaultPasswordLengthInChars))
                {
                    this.DefaultPasswordLengthInChars = serializedSettings.DefaultPasswordLengthInChars;
                }
            }
        }

        /// <summary>
        /// This class is used to convert the app's settings to and from JSON.
        /// </summary>
        public class SerializedSettings
        {
            public SerializedSettings()
            {
                // Set the initial values in case they are not in the configuration file.  JsonSerializer.Deserialize<SerializedSettings>()
                // does not throw an error if a JSON file does not contain an expected property.  Instead, it leaves the property's value as
                // the default value.
                this.DefaultPasswordType = Constants.InitialDefaultPasswordType;
                this.DefaultPasswordLengthInChars = Constants.InitialDefaultPasswordLengthInChars;
            }

            public SerializedSettings(AppSettings appSettings)
            {
                this.DefaultPasswordType = appSettings.DefaultPasswordType;
                this.DefaultPasswordLengthInChars = appSettings.DefaultPasswordLengthInChars;
            }

            public PasswordType DefaultPasswordType { get; set; } 

            public int DefaultPasswordLengthInChars { get; set; }
        }
    }
}
