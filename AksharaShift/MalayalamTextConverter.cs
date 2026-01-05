using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AksharaShift
{
    /// <summary>
    /// Handles conversion of Unicode Malayalam to ML (Legacy) and FML fonts.
    /// Uses character mapping and reordering logic compatible with ISM/Kuttipencil style conversion.
    /// </summary>
    public static class MalayalamTextConverter
    {
        // Unicode Constants
        private const char CH_VIRAMA = '\u0D4D';
        
        // Vowel Signs that need to be moved to the LEFT of the consonant cluster
        // s=െ, t=േ, ss=ൈ
        // and Ra Subscript (mapped to {) which also moves left in this legacy font (\u0D4D\u0D30)
        // We handle Ra by replacing it with a placeholder \uE000 which we add to this list.
        private static readonly char[] LeftVowelSigns = { '\u0D46', '\u0D47', '\u0D48', '\uE000' };

        /// <summary>
        /// Converts Unicode Malayalam to ML-TTKarthika (ML Series) format.
        /// </summary>
        public static string ConvertToML(string unicodeText)
        {
            if (string.IsNullOrEmpty(unicodeText)) return "";
            
            // 1. Pre-process text (handle split vowels, canonicalize)
            string processed = PreProcess(unicodeText);

            // 2. Reorder for legacy (move left vowel signs before consonant clusters)
            processed = ReorderForLegacy(processed);

            // 3. Map to font glyphs (Greedy matching)
            return MapToLegacy(processed, ML_Karthika_Map);
        }

        /// <summary>
        /// Converts Unicode Malayalam to FML-Revathi (FML Series) format.
        /// </summary>
        public static string ConvertToFML(string unicodeText)
        {
            if (string.IsNullOrEmpty(unicodeText)) return "";

            // 1. Pre-process
            string processed = PreProcess(unicodeText);

            // 2. Reorder
            processed = ReorderForLegacy(processed);

            // 3. Map
            return MapToLegacy(processed, FML_Revathi_Map);
        }

        private static string PreProcess(string text)
        {
            // Normalizer rules provided by user
            return text
                .Replace("ൻറ", "ന്റ")
                .Replace("ന്‍പ", "മ്പ")
                .Replace("ററ", "റ്റ")
                .Replace("റ്‍", "ർ")
                .Replace("ണ്‍", "ൺ")
                .Replace("ന്‍", "ൻ")
                .Replace("ര്‍", "ർ")
                .Replace("ല്‍", "ൽ")
                .Replace("ള്‍", "ൾ")
                .Replace("ക്‍", "ൿ")
                .Replace("െെ", "ൈ")
                .Replace("ൊ", "ൊ")
                .Replace("ാെ", "ൊ")
                .Replace("ോ", "ോ")
                .Replace("ാേ", "ോ")
                .Replace("ൌ", "ൌ")
                .Replace("ൗെ", "ൌ")
                .Replace("എെ", "ഐ")
                .Replace("ഇൗ", "ഈ")
                .Replace("ഉൗ", "ഊ")
                .Replace("ഒൗ", "ഔ")
                
                // Split Vowel Decomposition (Standard legacy requirement)
                .Replace("ൊ", "\u0D46\u0D3E") // o -> e + aa
                .Replace("ോ", "\u0D47\u0D3E") // O -> E + aa
                .Replace("ൌ", "\u0D46\u0D57"); // au -> e + au_length_mark
        }

        private static string ReorderForLegacy(string text)
        {
            // Use placeholder for primary Ra-conjunct (Virama + Ra)
            // This treats it as a single character that needs to move left, like 'e' sign.
            StringBuilder sb = new StringBuilder(text.Replace("\u0D4D\u0D30", "\uE000"));
            
            // Iterate and find Left Vowel Signs
            for (int i = 0; i < sb.Length; i++)
            {
                if (LeftVowelSigns.Contains(sb[i]))
                {
                    char vowelSign = sb[i];
                    // Find the start of the "Consonant Cluster" preceding this sign.
                    // A cluster is (Consonant + Virama) * n + Consonant.
                    
                    int clusterStart = i - 1;
                    if (clusterStart < 0) continue; 

                    int currentScan = i - 1;
                    while (currentScan > 0)
                    {
                        if (currentScan >= 1 && sb[currentScan] == CH_VIRAMA)
                        {
                            // Continue past virama
                        }
                        else
                        {
                            // Not a virama, check if we found a consonant base
                            break; 
                        }
                        currentScan--; // Consonant
                        currentScan--; // Virama
                    }
                    
                    int moveDest = i - 1;
                    while (moveDest > 0 && sb[moveDest - 1] == CH_VIRAMA)
                    {
                         moveDest -= 2;
                    }
                    
                    sb.Remove(i, 1);
                    sb.Insert(moveDest, vowelSign);
                }
            }
            return sb.ToString().Replace("\uE000", "\u0D4D\u0D30");
        }

        private static string MapToLegacy(string text, Dictionary<string, string> map)
        {
            StringBuilder result = new StringBuilder();
            int i = 0;
            
            // Get keys sorted by length descending to ensure greedy matching
            // (In production we should cache this sorted list)
            var sortedKeys = map.Keys.OrderByDescending(k => k.Length).ToList();
            int maxKeyLen = sortedKeys.Any() ? sortedKeys.First().Length : 1;

            while (i < text.Length)
            {
                bool matched = false;
                
                // Try to match longest key first
                // Optimization: We could limit max key length check
                for (int len = maxKeyLen; len > 0; len--)
                {
                    if (i + len > text.Length) continue;
                    
                    string sub = text.Substring(i, len);
                    if (map.TryGetValue(sub, out string? val))
                    {
                        result.Append(val);
                        i += len;
                        matched = true;
                        break;
                    }
                }

                if (!matched)
                {
                    // If no match, keep char (e.g. English/Numbers)
                    result.Append(text[i]);
                    i++;
                }
            }
            return result.ToString();
        }

        // --- MAPPINGS ---
        // Populated from User Request (Step 335)

        private static readonly Dictionary<string, string> ML_Karthika_Map = new Dictionary<string, string>
        {
            {"ം", "w"}, {"ഃ", "x"}, {"അ", "A"}, {"ആ", "B"}, {"ഇ", "C"},
            {"ഈ", "Cu"}, {"ഉ", "D"}, {"ഊ", "Du"}, {"ഋ", "E"}, {"ഌ", "\\p"},
            {"എ", "F"}, {"ഏ", "G"}, {"ഐ", "sF"}, {"ഒ", "H"}, {"ഓ", "Hm"},
            {"ഔ", "Hu"}, {"ക", "I"}, {"ഖ", "J"}, {"ഗ", "K"}, {"ഘ", "L"},
            {"ങ", "M"}, {"ച", "N"}, {"ഛ", "O"}, {"ജ", "P"}, {"ഝ", "Q"},
            {"ഞ", "R"}, {"ട", "S"}, {"ഠ", "T"}, {"ഡ", "U"}, {"ഢ", "V"},
            {"ണ", "W"}, {"ത", "X"}, {"ഥ", "Y"}, {"ദ", "Z"}, {"ധ", "["},
            {"ന", "\\"}, {"പ", "]"}, {"ഫ", "^"}, {"ബ", "_"}, {"ഭ", "`"},
            {"മ", "a"}, {"യ", "b"}, {"ര", "c"}, {"റ", "d"}, {"ല", "e"},
            {"ള", "f"}, {"ഴ", "g"}, {"വ", "h"}, {"ശ", "i"}, {"ഷ", "j"},
            {"സ", "k"}, {"ഹ", "l"}, 
            
            // Matras - Note: PreProcessor logic handles splitting of o/O/au
            // These map the INDIVIDUAL components
            {"ാ", "m"}, {"ി", "n"}, {"ീ", "o"}, {"ു", "p"}, {"ൂ", "q"},
            {"ൃ", "r"}, {"െ", "s"}, {"േ", "t"}, {"ൈ", "ss"}, 
            {"്", "v"}, {"ൗ", "u"},

            // Conjuncts / Special
            {"ക്ക", "¡"}, {"ക്ല", "¢"}, {"ക്ഷ", "£"}, {"ഗ്ഗ", "€"}, 
            {"ഗ്ല", "¥"}, {"ങ്ക", "Š"}, {"ങ്ങ", "§"}, {"ച്ച", "š"}, 
            {"ഞ്ച", "©"}, {"ഞ്ഞ", "ª"}, {"ട്ട", "«"}, {"ണ്‍", "¬"},
            {"ണ്ട", "ï"}, {"ണ്ണ", "®"}, {"ത്ത", "¯"}, {"ത്ഥ", "°"}, 
            {"ദ്ദ", "±"}, {"ദ്ധ", "²"}, {"ന്‍", "³"}, {"ന്ദ", "µ"}, 
            {"ന്ന", "¶"}, {"ന്മ", "·"}, {"പ്ല", "¹"}, {"ബ്ബ", "º"}, 
            {"ബ്ല", "»"}, {"മ്പ", "Œ"}, {"മ്മ", "œ"}, {"മ്ല", "Ÿ"}, 
            {"യ്യ", "¿"}, {"ര്‍", "À"}, {"റ്റ", "ä"}, {"ല്‍", "Â"}, 
            {"ല്ല", "Ã"}, {"ള്‍", "Ä"}, {"ള്ള", "Å"}, {"വ്വ", "Æ"}, 
            {"ശ്ല", "Ç"}, {"ശ്ശ", "È"}, {"സ്ല", "É"}, {"സ്സ", "Ê"}, 
            {"ഹ്ല", "Ë"}, {"സ്റ്റ", "Ì"}, {"ഡ്ഡ", "Í"}, {"ക്ട", "Î"}, 
            {"ബ്ധ", "Ï"}, {"ബ്ദ", "Ð"}, {"ച്ഛ", "Ñ"}, {"ഹ്മ", "Ò"}, 
            {"ഹ്ന", "Ó"}, {"ന്ധ", "Ô"}, {"ത്സ", "Õ"}, {"ജ്ജ", "Ö"}, 
            {"ണ്മ", "×"}, {"സ്ഥ", "Ø"}, {"ന്ഥ", "Ù"}, {"ജ്ഞ", "Ú"}, 
            {"ത്ഭ", "Û"}, {"ഗ്മ", "Ü"}, {"ശ്ച", "Ý"}, {"ണ്ഡ", "Þ"}, 
            {"ത്മ", "ß"}, {"ക്ത", "à"}, {"ഗ്ന", "á"}, {"ന്റ", "â"}, 
            {"ഷ്ട", "ã"}, {"ന്", "å"}, {"്യ", "y"}, {"്വ", "z"}, 
            {"്ര", "{"}, {"ന്ത", "´"}, {"പ്പ", "¸"},
            {"കു", "æ"}, {"രു", "ê"}, {"ക്കു", "ç"},
            
            // Normalized variants
            {"ൺ", "¬"}, {"ൻ", "³"}, {"ർ", "À"}, {"ൽ", "Â"}, {"ൾ", "Ä"}, {"ൿ", "Î"} 
        };

        private static readonly Dictionary<string, string> FML_Revathi_Map = new Dictionary<string, string>
        {
            // Consonants (Keeping as defined previously)
            {"ക", "k"}, {"ഖ", "K"}, {"ഗ", "g"}, {"ഘ", "G"}, {"ങ", "M"},
            {"ച", "c"}, {"ഛ", "C"}, {"ജ", "j"}, {"ഝ", "J"}, {"ഞ", "N"},
            {"ട", "t"}, {"ഠ", "T"}, {"ഡ", "d"}, {"ഢ", "D"}, {"ണ", "n"},
            {"ത", "q"}, {"ഥ", "Q"}, {"ദ", "w"}, {"ധ", "W"}, {"ന", "m"}, 
            {"പ", "p"}, {"ഫ", "P"}, {"ബ", "b"}, {"ഭ", "B"}, {"മ", "y"},
            {"യ", "v"}, {"ര", "r"}, {"റ", "R"}, {"ല", "l"}, {"ള", "L"},
            {"ഴ", "z"}, {"വ", "V"}, {"ശ", "S"}, {"ഷ", "x"}, {"സ", "s"}, {"ഹ", "h"},
            {"അ", "A"}, {"ആ", "B"}, {"ഇ", "C"}, {"ഉ", "D"}, {"എ", "E"}, 
            {"ാ", "a"}, {"ി", "i"}, {"ീ", "I"}, {"ു", "u"}, {"ൂ", "U"},
            {"െ", "e"}, {"േ", "E"}, 
            {"ക്ക", "¡"}, {"ണ്ട", " "},
        };

        /// <summary>
        /// Gets statistics about the conversion (for debugging).
        /// </summary>
        public static ConversionStats GetStats(string input, ConversionType type)
        {
            var stats = new ConversionStats
            {
                InputLength = input.Length,
                OutputText = type == ConversionType.ML ? ConvertToML(input) : ConvertToFML(input)
            };
            stats.OutputLength = stats.OutputText?.Length ?? 0;
            stats.MalayalamCharsFound = input.Count(c => (c >= '\u0D00' && c <= '\u0D7F'));
            return stats;
        }
    }

    public enum ConversionType { ML, FML }

    public class ConversionStats
    {
        public int InputLength { get; set; }
        public int OutputLength { get; set; }
        public string? OutputText { get; set; }
        public int MalayalamCharsFound { get; set; }
    }
}
