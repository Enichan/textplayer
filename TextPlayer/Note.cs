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

namespace TextPlayer {
    /// <summary>
    /// Represents a note to be played.
    /// </summary>
    public struct Note {
        /// <summary>
        /// The octave of the note. A 4 here refers to the octave of Middle-C (C4).
        /// </summary>
        public int Octave;
        /// <summary>
        /// Duration of this note.
        /// </summary>
        public TimeSpan Length;
        /// <summary>
        /// The base type of the note, abcdefg.
        /// </summary>
        public char Type;
        /// <summary>
        /// Indicates whether this note is a normal note, or a sharp note.
        /// </summary>
        public bool Sharp;
        /// <summary>
        /// The volume of this note, ranges between 0.0 (inaudible) and 1.0 (loudest).
        /// </summary>
        public float Volume;

        /// <summary>
        /// Calculates the note's frequency in Hz.
        /// </summary>
        /// <param name="tuningNote">Optional note to use for tuning, this is Note.A4 (440 Hz) by default.</param>
        /// <returns>Frequency in Hz.</returns>
        public double GetFrequency(Note? tuningNote = null) {
            if (!tuningNote.HasValue)
                tuningNote = Note.A4;

            int dist = GetStep() - tuningNote.Value.GetStep();
            double freq = 440 * Math.Pow(1.0594630943592952645618252949463, dist);

            return freq;
        }

        /// <summary>
        /// Calculates the step of this note. Steps increase by 1 for every semitone, resulting in 12 steps per octave.
        /// </summary>
        /// <returns>Note step as an integer.</returns>
        public int GetStep() {
            int step;

            switch (Type) {
                case 'a':
                    if (!Sharp)
                        step = 9;
                    else
                        step = 10;
                    break;
                case 'b':
                    step = 11;
                    break;
                case 'c':
                    if (!Sharp)
                        step = 0;
                    else
                        step = 1;
                    break;
                case 'd':
                    if (!Sharp)
                        step = 2;
                    else
                        step = 3;
                    break;
                case 'e':
                    step = 4;
                    break;
                case 'f':
                    if (!Sharp)
                        step = 5;
                    else
                        step = 6;
                    break;
                case 'g':
                default:
                    if (!Sharp)
                        step = 7;
                    else
                        step = 8;
                    break;
            }

            step += Octave * 12;
            return step;
        }

        private static bool a4initialized = false;
        private static Note a4;
        /// <summary>
        /// Note A in octave 4 (A4), commonly used as the pitch standard for calculating frequencies.
        /// </summary>
        public static Note A4 {
            get {
                if (a4initialized)
                    return a4;
                a4.Octave = 4;
                a4.Type = 'a';
                a4initialized = true;
                return a4;
            }
        }
    }
}
