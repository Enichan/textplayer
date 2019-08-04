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
using System.Diagnostics;

namespace TextPlayer {
    /// <summary>
    /// Abstract base music playing class.
    /// </summary>
    public abstract class MusicPlayer : IMusicPlayer {
        #region Static
        private static Stopwatch time;
        private static object timeLock = new object();

        static MusicPlayer() {
            time = new Stopwatch();
            time.Start();
        }

        /// <summary>
        /// Get default time used for timing music players, thread-safe
        /// </summary>
        public static TimeSpan Time {
            get {
                lock (timeLock) {
                    return time.Elapsed;
                }
            }
        }
        #endregion

        protected bool playing = false;
        protected TimeSpan lastTime;
        protected TimeSpan startTime;
        private bool muted;

        public MusicPlayer() {
        }

        /// <summary>
        /// Loads from file.
        /// </summary>
        /// <param name="file">The full path containing the file.</param>
        public void FromFile(string file) {
            using (StreamReader stream = new StreamReader(file)) {
                Load(stream);
            }
        }

        /// <summary>
        /// Loads from string.
        /// </summary>
        /// <param name="str">A string containing the song's code.</param>
        public abstract void Load(string str);

        /// <summary>
        /// Loads from stream.
        /// </summary>
        /// <param name="stream">A stream containing the song's code.</param>
        public void Load(StreamReader stream) {
            var strBuilder = new StringBuilder();
            char[] buffer = new char[1024];
            while (!stream.EndOfStream) {
                int bytesRead = stream.ReadBlock(buffer, 0, buffer.Length);
                if (strBuilder.Length + bytesRead > validationSettings.MaxSize) {
                    throw new SongSizeException("Song exceeded maximum length of " + validationSettings.MaxSize);
                }
                strBuilder.Append(buffer, 0, bytesRead);
            }
            Load(strBuilder.ToString());
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
            if (playing)
                throw new InvalidOperationException(this.GetType().ToString() + " was already playing.");

            playing = true;
            lastTime = currentTime;
            startTime = currentTime;
        }

        /// <summary>
        /// Stops this music player.
        /// </summary>
        public virtual void Stop() {
            playing = false;
            startTime = TimeSpan.Zero;
            lastTime = TimeSpan.Zero;
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
            if (Playing) {
                lastTime = currentTime;
            }
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

        /// <summary>
        /// Plays a note on a given channel.
        /// </summary>
        /// <param name="note">Note to play.</param>
        /// <param name="channel">Zero-based channel to play the note on.</param>
        /// <param name="time">Current playback time.</param>
        protected abstract void PlayNote(Note note, int channel, TimeSpan time);

        /// <summary>
        /// Modifies referenced note up or down in tone by given number of steps. There are 12 steps per octave.
        /// </summary>
        /// <param name="note">Note to modify.</param>
        /// <param name="steps">Steps up or down to modify the note.</param>
        protected void Step(ref Note note, int steps) {
            if (steps == 0)
                return;

            if (steps > 0) {
                for (int i = 0; i < steps; i++) {
                    switch (note.Type) {
                        case 'a':
                            if (!note.Sharp)
                                note.Sharp = true;
                            else {
                                note.Type = 'b';
                                note.Sharp = false;
                            }
                            break;
                        case 'b':
                            note.Type = 'c';
                            note.Octave++;
                            break;
                        case 'c':
                            if (!note.Sharp)
                                note.Sharp = true;
                            else {
                                note.Type = 'd';
                                note.Sharp = false;
                            }
                            break;
                        case 'd':
                            if (!note.Sharp)
                                note.Sharp = true;
                            else {
                                note.Type = 'e';
                                note.Sharp = false;
                            }
                            break;
                        case 'e':
                            note.Type = 'f';
                            break;
                        case 'f':
                            if (!note.Sharp)
                                note.Sharp = true;
                            else {
                                note.Type = 'g';
                                note.Sharp = false;
                            }
                            break;
                        case 'g':
                            if (!note.Sharp)
                                note.Sharp = true;
                            else {
                                note.Type = 'a';
                                note.Sharp = false;
                            }
                            break;
                    }
                }
            }
            else {
                for (int i = 0; i < Math.Abs(steps); i++) {
                    switch (note.Type) {
                        case 'a':
                            if (note.Sharp)
                                note.Sharp = false;
                            else {
                                note.Type = 'g';
                                note.Sharp = true;
                            }
                            break;
                        case 'b':
                            note.Type = 'a';
                            note.Sharp = true;
                            break;
                        case 'c':
                            if (note.Sharp)
                                note.Sharp = false;
                            else {
                                note.Type = 'b';
                                note.Octave--;
                            }
                            break;
                        case 'd':
                            if (note.Sharp)
                                note.Sharp = false;
                            else {
                                note.Type = 'c';
                                note.Sharp = true;
                            }
                            break;
                        case 'e':
                            note.Type = 'd';
                            note.Sharp = true;
                            break;
                        case 'f':
                            if (note.Sharp)
                                note.Sharp = false;
                            else {
                                note.Type = 'e';
                            }
                            break;
                        case 'g':
                            if (note.Sharp)
                                note.Sharp = false;
                            else {
                                note.Type = 'f';
                                note.Sharp = true;
                            }
                            break;
                    }
                }
            }
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

        #region Properties
        /// <summary>
        /// Boolean value indicating whether the player is still playing music.
        /// </summary>
        public bool Playing {
            get {
                return playing;
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
        internal abstract ValidationSettings validationSettings { get; }
        /// <summary>
        /// Duration of the song.
        /// </summary>
        public abstract TimeSpan Duration { get; }
        /// <summary>
        /// Time elapsed since start of the song.
        /// </summary>
        public virtual TimeSpan Elapsed { get { return lastTime - startTime; } }
        #endregion
    }
}
