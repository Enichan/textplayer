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
using TextPlayer;

namespace PitchSample {
    class SoundSource {
        private Source source;
        private TimeSpan timeOut;
        private TimeSpan fadeStart;
        private TimeSpan fadeEnd;
        private float startingVolume;

        public SoundSource(Source source) {
            this.source = source;
            startingVolume = source.Volume;
        }

        public void Fade() {
            fadeStart = new TimeSpan(MusicPlayer.Time.Ticks);
            fadeEnd = fadeStart + TimeSpan.FromMilliseconds(100);
        }

        public void Stop() {
            if (source.State == OpenTK.Audio.OpenAL.ALSourceState.Playing) {
                source.Stop();
            }
            source.Dispose();
        }

        public TimeSpan TimeOut { get { return timeOut; } set { timeOut = value; } }
        public TimeSpan FadeStart { get { return fadeStart; } set { fadeStart = value; } }
        public TimeSpan FadeEnd { get { return fadeEnd; } set { fadeEnd = value; } }
        public float Volume { get { return source.Volume; } set { source.Volume = Math.Max(0f, Math.Min(1f, value)); } }
        public float StartingVolume { get { return startingVolume; } }
    }
}
