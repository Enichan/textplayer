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
using TextPlayer;
using TextPlayer.MML;
using Cireon.Audio;

namespace PitchSample {
    class OggMMLPlayer : MultiTrackMMLPlayer {
        private List<SoundSource> fadingSources;
        private List<SoundSource> activeSources;
        private TimeSpan now;

        public OggMMLPlayer()
            : base() {
            activeSources = new List<SoundSource>();
            fadingSources = new List<SoundSource>();
        }

        protected override void PlayNote(Note note, int channel) {
            // get the pitch relative to the C note in the same octave
            var pitch = GetPitch(note);
            // get the appropriate C note sound
            var sound = SoundBank.Sounds[note.Octave];
            // play it pitched up to match the note we're looking for
            var source = sound.Play(note.Volume, pitch);

            // set the sound source's time out so it'll be ended when the 
            // note should end instead of playing through to completion
            source.TimeOut = now + note.Length;
            activeSources.Add(source);
        }

        private float GetPitch(Note note) {
            // get relative pitch, this is the constant below to the power of the number of semitones between the notes
            var c = new Note() { Type = 'c', Octave = note.Octave };
            var dist = note.GetStep() - c.GetStep();
            return (float)Math.Pow(1.0594630943592952645618252949463, dist);
        }

        public override void Update(TimeSpan currentTime) {
            now = currentTime;

            // check which notes should have ended and fade them out (fading out before stopping prevents popping sounds)
            for (int i = activeSources.Count - 1; i >= 0; i--) {
                var src = activeSources[i];
                if (currentTime >= src.TimeOut) {
                    src.Fade();
                    activeSources.RemoveAt(i);
                    fadingSources.Add(src);
                }
            }

            // fade out sources and remove when done
            for (int i = fadingSources.Count - 1; i >= 0; i--) {
                var src = fadingSources[i];
                var volScale = 1f - (float)((now.TotalSeconds - src.FadeStart.TotalSeconds) / (src.FadeEnd.TotalSeconds - src.FadeStart.TotalSeconds));
                src.Volume = volScale * src.StartingVolume;
                if (currentTime > src.FadeEnd) {
                    src.Stop();
                    fadingSources.RemoveAt(i);
                }
            }

            base.Update(currentTime);
        }
    }
}
