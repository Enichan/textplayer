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

namespace MidiPlayer {
    public class PlayerMML : MultiTrackMMLPlayer, IMidiPlayer {
        private MidiDevice midi;
        private TimeSpan elapsed;
        private TimeSpan curTime;
        private float normalizeScalar;
        private volatile bool loop;
        private volatile bool normalize;
        private volatile bool paused;

        public PlayerMML()
            : base() {
            midi = new MidiDevice();
            midi.SetInstrument(default(Midi.Instrument));
        }

        public void RecalculateDuration() {
            CalculateDuration();
        }

        public void CalculateNormalization() {
            int maxVol = 0;

            foreach (var track in Tracks) {
                int maxTrackVol = 0;
                foreach (var cmd in track.Commands) {
                    if (cmd.Type == MMLCommandType.Volume) {
                        var vol = Convert.ToInt32(cmd.Args[0]);
                        if (Mode == MMLMode.ArcheAge) {
                            vol = (int)Math.Round(Math.Min(1.0, Math.Max(0.0, vol / 127.0)) * 15);
                        }
                        vol = Math.Max(Settings.MinVolume, Math.Min(Settings.MaxVolume, vol));
                        maxTrackVol = Math.Max(vol, maxTrackVol);
                    }
                    else if (cmd.Type == MMLCommandType.Note || cmd.Type == MMLCommandType.NoteNumber) {
                        if (maxTrackVol == 0)
                            maxTrackVol = 8;
                    }
                }
                maxVol = Math.Max(maxVol, maxTrackVol);
            }

            if (maxVol == 0)
                maxVol = 8;

            normalizeScalar = 15.0f / maxVol;
        }

        public void SetInstrument(Midi.Instrument instrument) {
            midi.SetInstrument(instrument);
        }

        public void CloseDevice() {
            midi.Close();
        }

        public void StopNotes() {
            midi.StopNotes();
        }

        public override void Play(TimeSpan currentTime) {
            if (!paused) {
                curTime = currentTime;
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

        public override void Update(TimeSpan currentTime) {
            if (currentTime == TimeSpan.Zero)
                currentTime = curTime;

            if (paused) {
                curTime = currentTime;
                return;
            }

            elapsed += currentTime - curTime;
            curTime = currentTime;

            midi.HandleTimeOuts(elapsed);

            base.Update(elapsed);

            if (!Playing && loop) {
                Stop();
                Play(currentTime);
                Update(currentTime);
            }
        }

        protected override void PlayNote(Note note, int channel, TimeSpan time) {
            if (normalize)
                note.Volume = Math.Min(Math.Max(note.Volume * normalizeScalar, 0), 1);

            midi.PlayNote(channel, note, elapsed + note.Length); 
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
