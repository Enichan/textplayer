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
            { "AMaj", "A" }, { "BMaj", "B" }, { "CMaj", "C" }, { "DMaj", "D" }, { "EMaj", "E" }, { "FMaj", "F" }, { "GMaj", "G" },
            { "A#Maj", "A" }, { "B#Maj", "B" }, { "C#Maj", "C" }, { "D#Maj", "D" }, { "E#Maj", "E" }, { "F#Maj", "F" }, { "G#Maj", "G" }
        };
    }
}
