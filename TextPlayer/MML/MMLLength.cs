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

namespace TextPlayer.MML {
    /// <summary>
    /// Represents an MML note length with an integer length (1, 2, 4, 8, 16, 32, 64) and a dot which indicates it's half again as long.
    /// </summary>
    public struct MMLLength {
        public int Length;
        public bool Dotted;

        public MMLLength(int length, bool dotted) {
            Length = length;
            Dotted = dotted;
        }

        public TimeSpan ToTimeSpan(double secondsPerMeasure) {
            double length = 1.0 / (double)Length;
            if (Dotted)
                length *= 1.5;

            return new TimeSpan((long)(secondsPerMeasure * length * TimeSpan.TicksPerSecond));
        }
    }
}
