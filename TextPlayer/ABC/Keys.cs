#region License
// The MIT License (MIT)
// 
// Copyright (c) 2014 Emma 'Eniko' Maassen
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TextPlayer.ABC {
    /// <summary>
    /// Contains a set of accidentals for every key, as well as a dictionary of aliases for keys.
    /// </summary>
    public static class Keys {
        public static Dictionary<string, Dictionary<char, int>> Accidentals = new Dictionary<string, Dictionary<char, int>>() {
            { "C", new Dictionary<char, int>() },
            { "C#", new Dictionary<char, int>() { { 'F', 1 }, { 'C', 1 }, { 'G', 1 }, { 'D', 1 }, { 'A', 1 }, { 'E', 1 }, { 'B', 1 } } },
            { "F#", new Dictionary<char, int>() { { 'F', 1 }, { 'C', 1 }, { 'G', 1 }, { 'D', 1 }, { 'A', 1 }, { 'E', 1 } } },
            { "B", new Dictionary<char, int>() { { 'F', 1 }, { 'C', 1 }, { 'G', 1 }, { 'D', 1 }, { 'A', 1 } } },
            { "E", new Dictionary<char, int>() { { 'F', 1 }, { 'C', 1 }, { 'G', 1 }, { 'D', 1 } } },
            { "A", new Dictionary<char, int>() { { 'F', 1 }, { 'C', 1 }, { 'G', 1 } } },
            { "D", new Dictionary<char, int>() { { 'F', 1 }, { 'C', 1 } } },
            { "G", new Dictionary<char, int>() { { 'F', 1 } } },
            { "Cb", new Dictionary<char, int>() { { 'F', -1 }, { 'C', -1 }, { 'G', -1 }, { 'D', -1 }, { 'A', -1 }, { 'E', -1 }, { 'B', -1 } } },
            { "Gb", new Dictionary<char, int>() { { 'C', -1 }, { 'G', -1 }, { 'D', -1 }, { 'A', -1 }, { 'E', -1 }, { 'B', -1 } } },
            { "Db", new Dictionary<char, int>() { { 'G', -1 }, { 'D', -1 }, { 'A', -1 }, { 'E', -1 }, { 'B', -1 } } },
            { "Ab", new Dictionary<char, int>() { { 'D', -1 }, { 'A', -1 }, { 'E', -1 }, { 'B', -1 } } },
            { "Eb", new Dictionary<char, int>() { { 'A', -1 }, { 'E', -1 }, { 'B', -1 } } },
            { "Bb", new Dictionary<char, int>() { { 'E', -1 }, { 'B', -1 } } },
            { "F", new Dictionary<char, int>() { { 'B', -1 } } }
        };

        public static Dictionary<string, string> KeyAliases = new Dictionary<string, string>() {
            { "A#m", "C#" }, { "G#Mix", "C#" }, { "D#Dor", "C#" }, { "E#Phr", "C#" }, { "F#Lyd", "C#" }, { "B#Loc", "C#" }, 
            { "D#m", "F#" }, { "C#Mix", "F#" }, { "G#Dor", "F#" }, { "A#Phr", "F#" }, { "BLyd", "F#" }, { "E#Loc", "F#" }, 
            { "G#m", "B" }, { "F#Mix", "B" }, { "C#Dor", "B" }, { "D#Phr", "B" }, { "ELyd", "B" }, { "A#Loc", "B" }, 
            { "C#m", "E" }, { "BMix", "E" }, { "F#Dor", "E" }, { "G#Phr", "E" }, { "ALyd", "E" }, { "D#Loc", "E" }, 
            { "F#m", "A" }, { "EMix", "A" }, { "BDor", "A" }, { "C#Phr", "A" }, { "DLyd", "A" }, { "G#Loc", "A" }, 
            { "Bm", "D" }, { "AMix", "D" }, { "EDor", "D" }, { "F#Phr", "D" }, { "GLyd", "D" }, { "C#Loc", "D" }, 
            { "Em", "G" }, { "DMix", "G" }, { "ADor", "G" }, { "BPhr", "G" }, { "CLyd", "G" }, { "F#Loc", "G" }, 
            { "Am", "C" }, { "GMix", "C" }, { "DDor", "C" }, { "EPhr", "C" }, { "FLyd", "C" }, { "BLoc", "C" }, 
            { "Dm", "F" }, { "CMix", "F" }, { "GDor", "F" }, { "APhr", "F" }, { "BbLyd", "F" }, { "ELoc", "F" },
            { "Gm", "Bb" }, { "FMix", "Bb" }, { "CDor", "Bb" }, { "DPhr", "Bb" }, { "EbLyd", "Bb" }, { "ALoc", "Bb" },
            { "Cm", "Eb" }, { "BbMix", "Eb" }, { "FDor", "Eb" }, { "GPhr", "Eb" }, { "AbLyd", "Eb" }, { "DLoc", "Eb" },
            { "Fm", "Ab" }, { "EbMix", "Ab" }, { "BbDor", "Ab" }, { "CPhr", "Ab" }, { "DbLyd", "Ab" }, { "GLoc", "Ab" },
            { "Bbm", "Db" }, { "AbMix", "Db" }, { "EbDor", "Db" }, { "FPhr", "Db" }, { "GbLyd", "Db" }, { "CLoc", "Db" },
            { "Ebm", "Gb" }, { "DbMix", "Gb" }, { "AbDor", "Gb" }, { "BbPhr", "Gb" }, { "CbLyd", "Gb" }, { "FLoc", "Gb" },
            { "Abm", "Cb" }, { "GbMix", "Cb" }, { "DbDor", "Cb" }, { "EbPhr", "Cb" }, { "FbLyd", "Cb" }, { "BbLoc", "Cb" },
        };

        public static Dictionary<char, int> ModifyAccidentals(string key, Dictionary<char, int> accidentalArgs) {
            var accModified = new Dictionary<char, int>(Accidentals[key]);
            foreach (var kvp in accidentalArgs) {
                accModified[kvp.Key] = kvp.Value;
            }
            return accModified;
        }

        public static Dictionary<char, int> GetAccidentals(string s) {
            try {
                string key;
                Dictionary<char, int> accidentalArgs;
                bool exp;

                Parse(s, out key, out accidentalArgs, out exp);

                if (key == null) {
                    // return default key
                    return Accidentals["C"];
                }

                if (accidentalArgs == null || accidentalArgs.Count == 0) {
                    // no custom accidentals, no copy
                    return Accidentals[key];
                }
                else {
                    if (exp) {
                        // explicit accidentals, so actually, the key doesn't matter for functional purposes
                        return accidentalArgs;
                    }
                    else {
                        // additional accidentals, copy key accidentals and modify
                        return ModifyAccidentals(key, accidentalArgs);
                    }
                }
            }
            catch {
                return Accidentals["C"];
            }
        }

        /// <summary>
        /// Takes an ABC 'K' key field and returns three out values corresponding to the key+mode,
        /// the custom defined accidentals, and a value indicating whether the accidentals are explicit.
        /// </summary>
        public static void Parse(string s, out string key, out Dictionary<char, int> accidentals, out bool exp) {
            key = null;
            accidentals = null;
            exp = false;

            s = s.Trim();

            // shortcut
            if (Accidentals.ContainsKey(s)) {
                key = s;
                return;
            }
            if (KeyAliases.ContainsKey(s)) {
                key = KeyAliases[s];
                return;
            }

            if (!IsNullOrWhiteSpace.String(s)) {
                try {
                    // this will capture:
                    //   group 1: tonic (E, C#, Bb)
                    //   group 2: mode argument OR explicit accidentals argument (only first 3 letters of word are considered in matching)
                    //   group 3: rest of string containing accidental tokens
                    var argsMatch = Regex.Match(s,
                        @"^(?:\s*)([a-g][#b]?)(?:(?:\s*)(maj|min|ion|aeo|mix|dor|phr|lyd|loc|m|exp))?(?:\S*(.*))?$",
                        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

                    if (argsMatch.Success) {
                        var tonicMatch = argsMatch.Groups[1];
                        var modeMatch = argsMatch.Groups[2];
                        var restMatch = argsMatch.Groups[3];

                        var keyBuilder = new StringBuilder(5);

                        // append tonic
                        keyBuilder.Append(tonicMatch.Value.Substring(0, 1).ToUpperInvariant());
                        keyBuilder.Append(tonicMatch.Value.Substring(1).ToLowerInvariant());

                        // append mode
                        if (modeMatch.Success) {
                            if (modeMatch.Value.ToLowerInvariant().StartsWith("exp")) {
                                // if the exp keyword is found, we explicitly define all accidentals
                                exp = true;
                            }
                            else {
                                // otherwise we're defining the mode
                                if (modeMatch.Length > 2) {
                                    // get first three characters of mode
                                    var type = modeMatch.Value.Substring(0, 1).ToUpperInvariant() + modeMatch.Value.Substring(1, 2).ToLowerInvariant();

                                    // convert from aliased modes to main mode
                                    switch (type) {
                                        default:
                                            keyBuilder.Append(modeMatch.Value.Substring(0, 1).ToUpperInvariant());
                                            keyBuilder.Append(modeMatch.Value.Substring(1, 2).ToLowerInvariant());
                                            break;
                                        case "Maj":
                                        case "Ion":
                                            break;
                                        case "Min":
                                        case "Aeo":
                                            keyBuilder.Append('m');
                                            break;
                                    }
                                }
                                else {
                                    keyBuilder.Append(modeMatch.Value.ToLowerInvariant());
                                }
                            }
                        }

                        // parse defined accidentals
                        if (restMatch.Success && !IsNullOrWhiteSpace.String(restMatch.Value)) {
                            accidentals = new Dictionary<char, int>();

                            // get all accidentals matches, accidentals in group 1, note in group 2
                            var matches = Regex.Matches(restMatch.Value, @"(__|_|=|\^\^|\^)([A-Ga-g])", RegexOptions.CultureInvariant);
                            for (int i = 0; i < matches.Count; ++i) {
                                var noteMatch = matches[i];

                                var note = noteMatch.Groups[2].Value.ToUpperInvariant()[0];
                                int accValue;

                                switch (noteMatch.Groups[1].Value) {
                                    case "__":
                                        accValue = -2;
                                        break;
                                    case "_":
                                        accValue = -1;
                                        break;
                                    case "^^":
                                        accValue = 2;
                                        break;
                                    case "^":
                                        accValue = 1;
                                        break;
                                    case "=":
                                    default:
                                        accValue = 0;
                                        break;
                                }

                                accidentals[note] = accValue;
                            }
                        }

                        key = keyBuilder.ToString();
                    }
                    else {
                        key = null;
                    }
                }
                catch {
                    key = null;
                }
            }
            else {
                key = null;
            }

            if (key != null) {
                string alias;
                if (Keys.KeyAliases.TryGetValue(key, out alias)) {
                    key = alias;
                }

                if (!Keys.Accidentals.ContainsKey(key)) {
                    key = null;
                }
            }
        }
    }
}
