using System;
using System.Collections.Generic;
using ProjectLink.Data.Generated;
using ProjectLink.Utils;
using UnityEngine;

namespace ProjectLink.Core
{
    public enum LanguageCode
    {
        EN,
        KO,
        ZH_CN,
        ZH_TW,
        TH
    }

    public sealed class LocalizationManager : MonoBehaviour
    {
        const string PrefsKey = "LanguageCode";
        const LanguageCode FallbackLanguage = LanguageCode.EN;

        static readonly IReadOnlyDictionary<string, LanguageCode> CountryToLanguage = new Dictionary<string, LanguageCode>
        {
            ["US"] = LanguageCode.EN,
            ["GB"] = LanguageCode.EN,
            ["AU"] = LanguageCode.EN,
            ["CA"] = LanguageCode.EN,
            ["KR"] = LanguageCode.KO,
            ["CN"] = LanguageCode.ZH_CN,
            ["SG"] = LanguageCode.ZH_CN,
            ["TW"] = LanguageCode.ZH_TW,
            ["HK"] = LanguageCode.ZH_TW,
            ["MO"] = LanguageCode.ZH_TW,
            ["TH"] = LanguageCode.TH,
        };

        readonly Dictionary<LanguageCode, Dictionary<string, string>> _strings = new();
        readonly Dictionary<string, (string en, string ko)> _errors = new();

        public static LocalizationManager Instance { get; private set; }
        public static event Action LanguageChanged;

        public LanguageCode CurrentLanguage { get; private set; } = FallbackLanguage;

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadStrings();
            CurrentLanguage = ResolveInitialLanguage();
        }

        public static string Get(string stringId)
        {
            if (Instance == null)
                return stringId;

            return Instance.GetText(stringId);
        }

        public static string GetError(string errorCode)
        {
            if (string.IsNullOrEmpty(errorCode))
                return "";

            if (Instance == null)
                return errorCode;

            return Instance.GetErrorText(errorCode);
        }

        public static void SetLanguage(LanguageCode language)
        {
            if (Instance == null)
                return;

            Instance.ApplyLanguage(language);
        }

        public static bool TryParseLanguage(string value, out LanguageCode language)
        {
            return Enum.TryParse(value?.Replace('-', '_'), true, out language);
        }

        public string GetText(string stringId)
        {
            if (TryGetText(CurrentLanguage, stringId, out string text))
                return text;

            if (TryGetText(FallbackLanguage, stringId, out text))
                return text;

            return stringId;
        }

        public string GetErrorText(string errorCode)
        {
            if (!_errors.TryGetValue(errorCode, out var row))
                return errorCode;

            return CurrentLanguage == LanguageCode.KO && !string.IsNullOrEmpty(row.ko)
                ? row.ko
                : !string.IsNullOrEmpty(row.en) ? row.en : errorCode;
        }

        public void ApplyLanguage(LanguageCode language)
        {
            if (!_strings.ContainsKey(language))
                language = FallbackLanguage;

            if (CurrentLanguage == language)
                return;

            CurrentLanguage = language;
            if (DataManager.Instance != null)
                DataManager.Instance.LanguageCode = language.ToString();

            PlayerPrefs.SetString(PrefsKey, language.ToString());
            PlayerPrefs.Save();
            LanguageChanged?.Invoke();
        }

        void LoadStrings()
        {
            _strings.Clear();
            foreach (LanguageCode lang in Enum.GetValues(typeof(LanguageCode)))
                _strings[lang] = new Dictionary<string, string>();

            var rows = CsvLoader.Load<Clientstring>(Clientstring.ResourcePath);
            for (int i = 0; i < rows.Length; i++)
            {
                var row = rows[i];
                _strings[LanguageCode.EN][row.stringId]    = row.EN;
                _strings[LanguageCode.KO][row.stringId]    = row.KO;
                _strings[LanguageCode.ZH_CN][row.stringId] = row.ZH_CN;
                _strings[LanguageCode.ZH_TW][row.stringId] = row.ZH_TW;
                _strings[LanguageCode.TH][row.stringId]    = row.TH;
            }

            _errors.Clear();
            var errors = CsvLoader.Load<ErrorMessages>(ErrorMessages.ResourcePath);
            for (int i = 0; i < errors.Length; i++)
            {
                var row = errors[i];
                if (!string.IsNullOrEmpty(row.errorCode))
                    _errors[row.errorCode] = (row.en, row.ko);
            }
        }

        LanguageCode ResolveInitialLanguage()
        {
            string saved = DataManager.Instance != null && !string.IsNullOrEmpty(DataManager.Instance.LanguageCode)
                ? DataManager.Instance.LanguageCode
                : PlayerPrefs.GetString(PrefsKey, string.Empty);
            if (TryParseLanguage(saved, out var savedLanguage) && _strings.ContainsKey(savedLanguage))
                return savedLanguage;

            string country = GetDeviceCountryCode();
            if (CountryToLanguage.TryGetValue(country, out var countryLanguage) && _strings.ContainsKey(countryLanguage))
                return countryLanguage;

            return FallbackLanguage;
        }

        static string GetDeviceCountryCode()
        {
            try
            {
                var region = new System.Globalization.RegionInfo(System.Globalization.CultureInfo.CurrentCulture.Name);
                return region.TwoLetterISORegionName.ToUpperInvariant();
            }
            catch (ArgumentException)
            {
                return string.Empty;
            }
        }

        bool TryGetText(LanguageCode language, string stringId, out string text)
        {
            text = null;
            return _strings.TryGetValue(language, out var languageStrings)
                && languageStrings.TryGetValue(stringId, out text);
        }
    }
}
