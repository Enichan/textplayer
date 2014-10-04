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
using System.Linq;
using System.Text;

namespace PitchSample {
    static class SoundBank {
        private static Dictionary<int, Sound> sounds;

        static SoundBank() {
            // load the sound samples to use to play the song
            sounds = new Dictionary<int, Sound>();
            sounds[1] = new Sound(@"sounds\c1.ogg");
            sounds[2] = new Sound(@"sounds\c2.ogg");
            sounds[3] = new Sound(@"sounds\c3.ogg");
            sounds[4] = new Sound(@"sounds\c4.ogg");
            sounds[5] = new Sound(@"sounds\c5.ogg");
            sounds[6] = new Sound(@"sounds\c6.ogg");
            sounds[7] = new Sound(@"sounds\c7.ogg");
            sounds[8] = new Sound(@"sounds\c8.ogg");
        }

        public static void Init() {
        }

        public static Dictionary<int, Sound> Sounds { get { return sounds; } }
    }
}
