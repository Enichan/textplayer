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

namespace TextPlayer.MML {
    /// <summary>
    /// A ready made class that supports multiple simultaneously playing MML tracks.
    /// </summary>
    public abstract class MultiTrackMMLPlayer : IMusicPlayer {
        private List<MMLPlayerTrack> tracks;
        private bool muted;
        private MMLSettings settings;
        private TimeSpan duration;
        protected TimeSpan startTime;
        protected TimeSpan lastTime;
        private MMLMode mmlMode;

        public MultiTrackMMLPlayer() {
            tracks = new List<MMLPlayerTrack>();
            settings = new MMLSettings();
            settings.MaxSize = 1024 * 4 * 3;
        }

        /// <summary>
        /// Plays a note on a given channel.
        /// </summary>
        /// <param name="note">Note to play.</param>
        /// <param name="channel">Zero-based channel to play the note on.</param>
        /// <param name="time">Current playback time.</param>
        protected abstract void PlayNote(Note note, int channel, TimeSpan time);

        internal virtual void PlayNote(Note note, int channel, MMLPlayerTrack track, TimeSpan time) {
            if (Muted)
                return;

            int index = tracks.IndexOf(track);
            if (index < 0)
                return;
            PlayNote(note, index, time);
        }

        /// <summary>
        /// Sets the tempo on all tracks.
        /// </summary>
        /// <param name="tempo">Tempo in bpm.</param>
        public virtual void SetTempo(int tempo) {
            if (mmlMode == MMLMode.ArcheAge) // ArcheAge tempo changes only apply to the track they occur in
                return;

            foreach (var track in tracks) {
                track.Tempo = tempo;
            }
        }

        /// <summary>
        /// Plays the song. Uses MusicPlayer.Time as the starting time.
        /// </summary>
        public virtual void Play() {
            Play(MusicPlayer.Time);
        }

        /// <summary>
        /// Plays the song using a custom starting time.
        /// </summary>
        /// <param name="currentTime">Time the playing started.</param>
        public virtual void Play(TimeSpan currentTime) {
            foreach (var track in tracks)
                track.Play(currentTime);

            startTime = currentTime;
            //Update(currentTime);
        }

        /// <summary>
        /// Update this music player. Uses MusicPlayer.Time as the current time.
        /// </summary>
        public virtual void Update() {
            Update(MusicPlayer.Time);
        }

        /// <summary>
        /// Update this music player using a custom current time.
        /// </summary>
        /// <param name="currentTime">Current player time.</param>
        public virtual void Update(TimeSpan currentTime) {
            if (mmlMode == MMLMode.Mabinogi) {
                while (currentTime >= nextTick && Playing) {
                    foreach (var track in tracks) {
                        track.Update(track.NextTick);
                    }
                }
            }
            else {
                foreach (var track in tracks) {
                    track.Update(currentTime);
                }
            }

            lastTime = currentTime;

            if (!Playing) {
                Stop();
            }
        }

        protected virtual void CalculateDuration() {
            bool storedMute = Muted;

            Stop();
            Mute();
            Play(TimeSpan.Zero);

            while (Playing) {
                foreach (var track in tracks) {
                    track.Update(track.NextTick);
                }

                if (nextTick > settings.MaxDuration) {
                    throw new SongDurationException("Song exceeded maximum duration " + settings.MaxDuration);
                }
            }

            duration = nextTick;

            if (!storedMute)
                Unmute();
        }

        /// <summary>
        /// Stops this music player.
        /// </summary>
        public virtual void Stop() {
            foreach (var track in tracks)
                track.Stop();

            lastTime = TimeSpan.Zero;
            startTime = TimeSpan.Zero;
        }

        /// <summary>
        /// Seeks to position within the song (relative to TimeSpan.Zero). Uses MusicPlayer.Time as the current time.
        /// </summary>
        /// <param name="position">Position relative to TimeSpan.Zero to seek to.</param>
        public virtual void Seek(TimeSpan position) {
            Seek(MusicPlayer.Time, position);
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

#pragma warning disable 0618
        #region Loading methods
        /// <summary>
        /// Load MML from a file containing code starting with 'MML@' and ending in ';'
        /// with tracks separated by ','
        /// </summary>
        /// <param name="file">Path to read from.</param>
        /// <param name="maxTracks">Maximum number of tracks allowed, 0 for infinite.</param>
        /// <param name="strict">Accepts files that don't start with 'MML@' and end with a semi-colon if false, throws MalformedMMLException if true</param>
        public void FromFile(string file, int maxTracks, bool strict) {
            using (StreamReader stream = new StreamReader(file)) {
                Load(stream, maxTracks, strict);
            }
        }

        /// <summary>
        /// Load MML from a file containing code starting with 'MML@' and ending in ';'
        /// with tracks separated by ','
        /// </summary>
        /// <param name="file">Path to read from.</param>
        /// <param name="strict">Accepts files that don't start with 'MML@' and end with a semi-colon if false, throws MalformedMMLException if true</param>
        public void FromFile(string file, bool strict) {
            FromFile(file, 0, strict);
        }

        /// <summary>
        /// Load MML from a string of code starting with 'MML@' and ending in ';'
        /// with tracks separated by ','
        /// </summary>
        /// <param name="code">MML collection string to load from.</param>
        /// <param name="maxTracks">Maximum number of tracks allowed, 0 for infinite.</param>
        /// <param name="strict">Accepts files that don't start with 'MML@' and end with a semi-colon if false, throws MalformedMMLException if true</param>
        public void Load(string code, int maxTracks, bool strict) {
            if (code.Length > settings.MaxSize) {
                throw new SongSizeException("Song exceeded maximum length of " + settings.MaxSize);
            }

            string trimmedCode = code.Trim(new char[] { ' ', '\t', '\n', '\r' });

            if (strict) {
                if (!trimmedCode.StartsWith("MML@", StringComparison.InvariantCultureIgnoreCase))
                    throw new MalformedMMLException("Mabinogi-format MML code should start with 'MML@'");
                if (!trimmedCode.EndsWith(";", StringComparison.InvariantCultureIgnoreCase))
                    throw new MalformedMMLException("Mabinogi-format MML code should end with ';'");

                trimmedCode = trimmedCode.Replace("MML@", "");
                trimmedCode = trimmedCode.Remove(trimmedCode.Length - 1);
            }
            else {
                if (trimmedCode.StartsWith("MML@", StringComparison.InvariantCultureIgnoreCase))
                    trimmedCode = trimmedCode.Replace("MML@", "");
                if (trimmedCode.EndsWith(";", StringComparison.InvariantCultureIgnoreCase))
                    trimmedCode = trimmedCode.Remove(trimmedCode.Length - 1);
            }

            var tokens = code.Split(',');
            if (tokens.Length > maxTracks && maxTracks > 0)
                throw new MalformedMMLException("Maximum number of tracks exceeded. Count: " + tokens.Length + ", max: " + maxTracks);

            tracks = new List<MMLPlayerTrack>();
            for (int i = 0; i < tokens.Length; ++i) {
                var track = new MMLPlayerTrack(this);
                track.Settings = settings;
                track.Load(tokens[i]);
                track.Mode = mmlMode;
                tracks.Add(track);
            }

            CalculateDuration();
        }

        /// <summary>
        /// Load MML from a string of code starting with 'MML@' and ending in ';'
        /// with tracks separated by ','
        /// </summary>
        /// <param name="code">MML collection string to load from.</param>
        /// <param name="strict">Accepts files that don't start with 'MML@' and end with a semi-colon if false, throws MalformedMMLException if true</param>
        public void Load(string code, bool strict) {
            Load(code, 0, strict);
        }

        /// <summary>
        /// Load MML from a stream containing code starting with 'MML@' and ending in ';'
        /// with tracks separated by ','
        /// </summary>
        /// <param name="stream">StreamReader object to read from.</param>
        /// <param name="maxTracks">Maximum number of tracks allowed, 0 for infinite.</param>
        /// <param name="strict">Accepts files that don't start with 'MML@' and end with a semi-colon if false, throws MalformedMMLException if true</param>
        public void Load(StreamReader stream, int maxTracks, bool strict) {
            var strBuilder = new StringBuilder();
            char[] buffer = new char[1024];
            while (!stream.EndOfStream) {
                int bytesRead = stream.ReadBlock(buffer, 0, buffer.Length);
                if (strBuilder.Length + bytesRead > settings.MaxSize) {
                    throw new SongSizeException("Song exceeded maximum length of " + settings.MaxSize);
                }
                strBuilder.Append(buffer, 0, bytesRead);
            }
            Load(strBuilder.ToString(), maxTracks, strict);
        }

        /// <summary>
        /// Load MML from a stream containing code starting with 'MML@' and ending in ';'
        /// with tracks separated by ','
        /// </summary>
        /// <param name="stream">StreamReader object to read from.</param>
        /// <param name="strict">Accepts files that don't start with 'MML@' and end with a semi-colon if false, throws MalformedMMLException if true</param>
        public void Load(StreamReader stream, bool strict) {
            Load(stream, 0, strict);
        }
        #endregion

        #region Obsolete loading methods
        /// <summary>
        /// Load MML from a file containing code starting with 'MML@' and ending in ';'
        /// with tracks separated by ','
        /// </summary>
        /// <param name="file">Path to read from.</param>
        /// <param name="maxTracks">Maximum number of tracks allowed, 0 for infinite.</param>
        [Obsolete("This function allows reading a file that doesn't start with 'MML@' or ends in ';', use FromFile(string, int, bool) instead")]
        public void FromFile(string file, int maxTracks) {
            FromFile(file, maxTracks, false);
        }

        /// <summary>
        /// Load MML from a file containing code starting with 'MML@' and ending in ';'
        /// with tracks separated by ','
        /// </summary>
        /// <param name="file">Path to read from.</param>
        [Obsolete("This function allows reading a file that doesn't start with 'MML@' or ends in ';', use FromFile(string, bool) instead")]
        public void FromFile(string file) {
            FromFile(file, false);
        }

        /// <summary>
        /// Load MML from a string of code starting with 'MML@' and ending in ';'
        /// with tracks separated by ','
        /// </summary>
        /// <param name="code">MML collection string to load from.</param>
        /// <param name="maxTracks">Maximum number of tracks allowed, 0 for infinite.</param>
        [Obsolete("This function allows reading a file that doesn't start with 'MML@' or ends in ';', use Load(string, int, bool) instead")]
        public void Load(string code, int maxTracks) {
            Load(code, maxTracks, false);
        }

        /// <summary>
        /// Load MML from a string of code starting with 'MML@' and ending in ';'
        /// with tracks separated by ','
        /// </summary>
        /// <param name="code">MML collection string to load from.</param>
        [Obsolete("This function allows reading a file that doesn't start with 'MML@' or ends in ';', use Load(string, bool) instead")]
        public void Load(string code) {
            Load(code, false);
        }

        /// <summary>
        /// Load MML from a stream containing code starting with 'MML@' and ending in ';'
        /// with tracks separated by ','
        /// </summary>
        /// <param name="stream">StreamReader object to read from.</param>
        /// <param name="maxTracks">Maximum number of tracks allowed, 0 for infinite.</param>
        [Obsolete("This function allows reading a file that doesn't start with 'MML@' or ends in ';', use Load(StreamReader, int, bool) instead")]
        public void Load(StreamReader stream, int maxTracks) {
            Load(stream, maxTracks, false);
        }

        /// <summary>
        /// Load MML from a stream containing code starting with 'MML@' and ending in ';'
        /// with tracks separated by ','
        /// </summary>
        /// <param name="stream">StreamReader object to read from.</param>
        [Obsolete("This function allows reading a file that doesn't start with 'MML@' or ends in ';', use Load(StreamReader, bool) instead")]
        public void Load(StreamReader stream) {
            Load(stream, false);
        }
        #endregion
#pragma warning restore 0618

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
                for (int i = 0; i < tracks.Count; ++i){
                    if (tracks[i].Playing)
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
        /// <summary>
        /// Duration of the song.
        /// </summary>
        public TimeSpan Duration { get { return duration; } }

        private TimeSpan nextTick {
            get {
                long max = 0;
                for (int i = 0; i < tracks.Count; ++i){
                    max = Math.Max(max, tracks[i].NextTick.Ticks);
                }
                return new TimeSpan(max);
            }
        }
        public MMLSettings Settings { get { return settings; } set { settings = value; } }
        public virtual TimeSpan Elapsed { get { return lastTime - startTime; } }
        public MMLMode Mode {
            get {
                return mmlMode;
            }
            set {
                mmlMode = value;
                for (int i = 0; i < tracks.Count; ++i){
                    tracks[i].Mode = mmlMode;
                }
            }
        }
    }
}
