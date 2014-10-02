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
    /// Represents a single field of information in an ABC header.
    /// </summary>
    public struct ABCInfo {
        public char Identifier;
        public string Text;

        public ABCInfo(char ident, string text) {
            Identifier = ident;
            Text = text;
        }

        public static ABCInfo? Parse(string line) {
            if (line.Length < 2)
                return null;

            char id = line.ToUpperInvariant()[0];
            if ((int)id < 65 || (int)id > 90 || line.Substring(1, 1) != ":")
                return null;

            string text;
            if (line.Length > 2)
                text = line.Substring(2);
            else
                text = "";

            return new ABCInfo(id, text);
        }
    }
}
