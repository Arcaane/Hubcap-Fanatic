using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HubcapLocalisation {
    public static class LocalizationManager {
        private static Dictionary<string, Dictionary<string, string>> localisationDictionnary = new();

        public delegate void LocalizationUpdate();
        public static event LocalizationUpdate OnLocalizationChanged;
        
        private static string language = "French";
        private static string key = "";
        
        /// <summary>
        /// Read all data from all files
        /// </summary>
        /// <param name="localisationAssets"></param>
        public static void ReadData(List<TextAsset> localisationAssets) {
            List<string> addedKeys = new();
            
            foreach (TextAsset text in localisationAssets) {
                List<string> lines = GetLines(text.text);
                List<string> languages = lines[0].Split(',').Select(i => i.Trim()).ToList();
                
                for (int lang = 1; lang < languages.Count; lang++) {
                    if (localisationDictionnary.ContainsKey(languages[lang])) continue;
                    localisationDictionnary.Add(languages[lang], new Dictionary<string, string>());
                }

                foreach (string line in lines) {
                    if (line == lines[0]) continue;
                    
                    List<string> columns = line.Split(',').ToList();
                    key = columns[0];

                    if (key == "") continue;
                    if (addedKeys.Contains(key)) {
                        Debug.LogError($"Duplicated key `{key}` found in `{text.name}`. This key is not loaded.");
                        continue;
                    }
                    addedKeys.Add(key);
                    
                    for (var lang = 1; lang < languages.Count; lang++) {
                        localisationDictionnary[languages[lang]].Add(key, columns[lang]);
                    }
                }
            }
        }

        /// <summary>
        /// Change the current language used
        /// </summary>
        /// <param name="newLanguage"></param>
        public static void ChangeLanguage(string newLanguage) {
            language = newLanguage;
            OnLocalizationChanged?.Invoke();
        }

        /// <summary>
        /// Localise a text based on its key
        /// </summary>
        /// <param name="localizationKey"></param>
        /// <returns></returns>
        public static string Localize(string localizationKey) {
            if (localisationDictionnary.Count == 0) return "Dictionnary wasn't loaded";
            if (!localisationDictionnary.ContainsKey(language)) return $"There is no dictionnary assiociated to the current language : {language}";
            if (!localisationDictionnary[language].ContainsKey(localizationKey)) return $"The dictionnary {language} doesn't contain the key {localizationKey}";
            if (localisationDictionnary[language][localizationKey] == "") return $"The dictionnary {language} contains the key {localizationKey} but it's value is null";
            
            return localisationDictionnary[language][localizationKey];
        }

        /// <summary>
        /// Return all lines from a text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static List<string> GetLines(string text) {
            text = text.Replace("\r\n", "\n");
            return text.Split("\n").Where(i => i != "").ToList();
        }
    }
}
