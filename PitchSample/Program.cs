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
using Cireon.Audio;

namespace PitchSample {
    class Program {
        static void Main(string[] args) {
            // load our sounds
            SoundBank.Init();

            // load and play our song
            OggMMLPlayer player = new OggMMLPlayer();
            player.FromFile("stones.mml", true);
            player.Play();

            while (player.Playing) {
                // update the audio device
                AudioManager.Instance.SourceManager.Update();

                // update our song
                player.Update();

                // be nice to the CPU
                System.Threading.Thread.Sleep(1);
            }

            // clean up
            AudioManager.Instance.Dispose();
        }
    }
}
