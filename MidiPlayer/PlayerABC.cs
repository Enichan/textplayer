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
using TextPlayer.ABC;

namespace MidiPlayer {
    public class PlayerABC : ABCPlayer, IMidiPlayer {
        private MidiDevice midi;
        private TimeSpan elapsed;
        private TimeSpan lastABCTime;
        private float normalizeScalar;
        private volatile bool loop;
        private volatile bool normalize;
        private volatile bool paused;

        public PlayerABC()
            : base(false) {
            midi = new MidiDevice();
            midi.SetInstrument(default(Midi.Instrument));
        }

        public override void Play(TimeSpan currentTime) {
            if (!paused) {
                lastABCTime = currentTime;
                elapsed = TimeSpan.Zero;
                StopNotes();
                base.Play(TimeSpan.Zero);
            }
            else {
                Unpause();
            }
        }

        public override void Stop() {
            elapsed = TimeSpan.Zero;
            paused = false;
            StopNotes();
            base.Stop();
        }

        protected override void PlayNote(Note note, int channel, TimeSpan time) {
            if (!Muted && channel == 0) {
                //Console.WriteLine(lastTime + ": " + note.Type + (note.Sharp ? "#" : "") + "[" + note.Octave + "] " + note.Length);
            }

            if (normalize) {
                note.Volume = Math.Min(Math.Max(note.Volume * normalizeScalar, 0), 1);
            }

            midi.PlayNote(channel, note, elapsed + note.Length);
        }

        protected override void PlayChord(List<Note> notes, TimeSpan time) {
            bool write = false;
            if (Muted)
                write = false;
            if (write)
                Console.Write(lastTime + ": Chord [");
            for (int i = 0; i < notes.Count; i++) {
                PlayNote(notes[i], i + 1, time);
                if (write) {
                    Console.Write(" " + notes[i].Type + (notes[i].Sharp ? "#" : "") + "[" + notes[i].Octave + "] ");
                }
            }
            if (write)
                Console.WriteLine(" ]");
        }

        public override void Update(TimeSpan currentTime) {
            if (currentTime == TimeSpan.Zero)
                currentTime = lastABCTime;

            if (paused) {
                lastABCTime = currentTime;
                return;
            }

            elapsed += currentTime - lastABCTime;
            lastABCTime = currentTime;

            midi.HandleTimeOuts(elapsed);

            base.Update(elapsed);

            if (!Playing && loop) {
                Stop();
                Play(currentTime);
                Update(currentTime);
            }
        }

        public void StopNotes() {
            midi.StopNotes();
        }

        public void SetInstrument(Midi.Instrument instrument) {
            midi.SetInstrument(instrument);
        }

        public void CalculateNormalization() {
            Mute();

            var storeLoop = loop;
            loop = false;

            TimeSpan length = TimeSpan.Zero;
            double maxVol = 0;
            Stop();
            Play(TimeSpan.Zero);
            while (Playing) {
                length += TimeSpan.FromMilliseconds(100);
                Update(length);
                maxVol = Math.Max(maxVol, Volume);
            }

            loop = storeLoop;

            Unmute();

            if (maxVol == 0)
                maxVol = 90.0 / 127;

            normalizeScalar = (float)(1.0 / maxVol);
        }

        public void CloseDevice() {
            midi.Close();
        }

        public void Unpause() {
            if (!paused)
                return;

            paused = false;
        }

        public void Pause() {
            if (paused)
                return;

            midi.StopNotes();
            paused = true;
        }

        public override void Mute() {
            midi.Muted = true;
            midi.StopNotes();
            base.Mute();
        }

        public override void Unmute() {
            midi.Muted = false;
            base.Unmute();
        }

        public override void Seek(TimeSpan currentTime, TimeSpan position) {
            bool storedPause = paused;

            base.Seek(currentTime, position);

            if (storedPause)
                Pause();
        }

        /// <summary>
        /// Thread-safe
        /// </summary>
        public bool Normalize { get { return normalize; } set { normalize = value; } }
        /// <summary>
        /// Thread-safe
        /// </summary>
        public bool Loop { get { return loop; } set { loop = value; } }
        public override TimeSpan Elapsed { get { return elapsed; } }
        public bool Paused {
            get {
                return paused;
            }
        }
    }
}
