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
using System.IO;

namespace TextPlayer {
    /// <summary>
    /// A ready made class that supports multiple simultaneously playing MML tracks.
    /// </summary>
    public abstract class MultiTrackMMLPlayer : IMusicPlayer {
        private List<MMLPlayerTrack> tracks;
        private bool muted;

        public MultiTrackMMLPlayer() {
            tracks = new List<MMLPlayerTrack>();
        }

        /// <summary>
        /// Plays a note on a given channel.
        /// </summary>
        /// <param name="note">Note to play.</param>
        /// <param name="channel">Zero-based channel to play the note on.</param>
        protected abstract void PlayNote(Note note, int channel);

        internal virtual void PlayNote(Note note, int channel, MMLPlayerTrack track) {
            if (Muted)
                return;

            int index = tracks.IndexOf(track);
            if (index < 0)
                return;
            PlayNote(note, index);
        }

        /// <summary>
        /// Sets the tempo on all tracks.
        /// </summary>
        /// <param name="tempo">Tempo in bpm.</param>
        public virtual void SetTempo(int tempo) {
            foreach (var track in tracks) {
                track.Tempo = tempo;
            }
        }

        /// <summary>
        /// Plays the song. Uses DateTime.Now as the starting time.
        /// </summary>
        public virtual void Play() {
            Play(new TimeSpan(DateTime.Now.Ticks));
        }

        /// <summary>
        /// Plays the song using a custom starting time.
        /// </summary>
        /// <param name="currentTime">Time the playing started.</param>
        public virtual void Play(TimeSpan currentTime) {
            foreach (var track in tracks)
                track.Play(currentTime);

            Update(currentTime);
        }

        /// <summary>
        /// Update this music player. Uses DateTime.Now as the current time.
        /// </summary>
        public virtual void Update() {
            Update(new TimeSpan(DateTime.Now.Ticks));
        }

        /// <summary>
        /// Update this music player using a custom current time.
        /// </summary>
        /// <param name="currentTime">Current player time.</param>
        public virtual void Update(TimeSpan currentTime) {
            while (currentTime >= nextTick && Playing) {
                foreach (var track in tracks) {
                    track.Update(track.NextTick);
                }
            }

            if (!Playing) {
                Stop();
            }
        }

        /// <summary>
        /// Stops this music player.
        /// </summary>
        public virtual void Stop() {
            foreach (var track in tracks)
                track.Stop();
        }

        /// <summary>
        /// Seeks to position within the song (relative to TimeSpan.Zero). Uses DateTime.Now as the current time.
        /// </summary>
        /// <param name="position">Position relative to TimeSpan.Zero to seek to.</param>
        public virtual void Seek(TimeSpan position) {
            Seek(new TimeSpan(DateTime.Now.Ticks), position);
        }

        /// <summary>
        /// Seeks to position within the song (relative to TimeSpan.Zero) using a custom current time.
        /// </summary>
        /// <param name="currentTime">Current player time.</param>
        /// <param name="position">Position relative to TimeSpan.Zero to seek to.</param>
        public virtual void Seek(TimeSpan currentTime, TimeSpan position) {
            bool storedMute = Muted;

            Stop();
            Mute();
            Play(currentTime - position);
            Update(currentTime);

            if (!storedMute)
                Unmute();
        }

        /// <summary>
        /// Load MML from a file containing code starting with 'MML@' and ending in ';'
        /// with tracks separated by ','
        /// </summary>
        /// <param name="file">Path to read from.</param>
        /// <param name="maxTracks">Maximum number of tracks allowed, 0 for infinite.</param>
        public void FromFile(string file, int maxTracks) {
            using (StreamReader stream = new StreamReader(file)) {
                Load(stream, maxTracks);
            }
        }

        /// <summary>
        /// Load MML from a file containing code starting with 'MML@' and ending in ';'
        /// with tracks separated by ','
        /// </summary>
        /// <param name="file">Path to read from.</param>
        public void FromFile(string file) {
            FromFile(file, 0);
        }

        /// <summary>
        /// Load MML from a string of code starting with 'MML@' and ending in ';'
        /// with tracks separated by ','
        /// </summary>
        /// <param name="code">MML collection string to load from.</param>
        /// <param name="maxTracks">Maximum number of tracks allowed, 0 for infinite.</param>
        public void Load(string code, int maxTracks) {
            string trimmedCode = code.Trim().TrimEnd('\n', '\r').TrimStart('\n', '\r');

            if (!trimmedCode.StartsWith("MML@", StringComparison.InvariantCultureIgnoreCase))
                throw new MalformedMMLException("Mabinogi-format MML code should start with 'MML@'");
            if (!trimmedCode.EndsWith(";", StringComparison.InvariantCultureIgnoreCase))
                throw new MalformedMMLException("Mabinogi-format MML code should end with ';'");

            trimmedCode = trimmedCode.Replace("MML@", "");
            trimmedCode = trimmedCode.Remove(trimmedCode.Length - 1);

            var tokens = code.Split(',');
            if (tokens.Length > maxTracks && maxTracks > 0)
                throw new MalformedMMLException("Maximum number of tracks exceeded. Count: " + tokens.Length + ", max: " + maxTracks);

            tracks = new List<MMLPlayerTrack>();
            foreach (var mml in tokens) {
                var track = new MMLPlayerTrack(this);
                track.Load(mml);
                tracks.Add(track);
            }
        }

        /// <summary>
        /// Load MML from a string of code starting with 'MML@' and ending in ';'
        /// with tracks separated by ','
        /// </summary>
        /// <param name="code">MML collection string to load from.</param>
        public void Load(string code) {
            Load(code, 0);
        }

        /// <summary>
        /// Load MML from a stream containing code starting with 'MML@' and ending in ';'
        /// with tracks separated by ','
        /// </summary>
        /// <param name="stream">TextReader object to read from.</param>
        /// <param name="maxTracks">Maximum number of tracks allowed, 0 for infinite.</param>
        public void Load(TextReader stream, int maxTracks) {
            Load(stream.ReadToEnd(), maxTracks);
        }

        /// <summary>
        /// Load MML from a stream containing code starting with 'MML@' and ending in ';'
        /// with tracks separated by ','
        /// </summary>
        /// <param name="stream">TextReader object to read from.</param>
        public void Load(TextReader stream) {
            Load(stream.ReadToEnd(), 0);
        }

        /// <summary>
        /// Mutes this player.
        /// </summary>
        public virtual void Mute() {
            muted = true;
        }

        /// <summary>
        /// Unmutes this player.
        /// </summary>
        public virtual void Unmute() {
            muted = false;
        }

        public List<MMLPlayerTrack> Tracks { get { return tracks; } }
        /// <summary>
        /// Boolean value indicating whether the player is still playing music.
        /// </summary>
        public bool Playing {
            get {
                foreach (var track in tracks) {
                    if (track.Playing)
                        return true;
                }
                return false;
            }
        }
        /// <summary>
        /// Boolean value indicating whether the player is muted.
        /// </summary>
        public bool Muted {
            get {
                return muted;
            }
            set {
                if (muted == value)
                    return;
                if (value)
                    Mute();
                else
                    Unmute();
            }
        }
        private TimeSpan nextTick {
            get {
                long max = 0;
                foreach (var track in tracks) {
                    max = Math.Max(max, track.NextTick.Ticks);
                }
                return new TimeSpan(max);
            }
        }
    }

    /// <summary>
    /// A concrete implementation of MMLPlayer for MultiTrackMMLPlayer.
    /// </summary>
    public class MMLPlayerTrack : MMLPlayer {
        private MultiTrackMMLPlayer parent;

        public MMLPlayerTrack(MultiTrackMMLPlayer parent) {
            this.parent = parent;
        }

        protected override void PlayNote(Note note, int channel) {
            parent.PlayNote(note, channel, this);
        }

        protected override void SetTempo(MMLCommand cmd) {
            parent.SetTempo(Convert.ToInt32(cmd.Args[0]));
        }

        public MultiTrackMMLPlayer Parent { get { return parent; } }
    }

    public class MalformedMMLException : Exception {
        public MalformedMMLException()
            : base() {
        }

        public MalformedMMLException(string message)
            : base(message) {
        }

        public MalformedMMLException(string message, Exception innerException)
            : base(message, innerException) {
        }
    }
}
