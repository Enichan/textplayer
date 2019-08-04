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
    interface IMusicPlayer {
        /// <summary>
        /// Loads from file.
        /// </summary>
        /// <param name="file">The full path containing the file.</param>
        void FromFile(string file);
        /// <summary>
        /// Loads from string.
        /// </summary>
        /// <param name="str">A string containing the song's code.</param>
        void Load(string str);
        /// <summary>
        /// Loads from stream.
        /// </summary>
        /// <param name="stream">A stream containing the song's code.</param>
        void Load(StreamReader stream);
        /// <summary>
        /// Plays the song. Uses MusicPlayer.Time as the starting time.
        /// </summary>
        void Play();
        /// <summary>
        /// Plays the song using a custom starting time.
        /// </summary>
        /// <param name="currentTime">Time the playing started.</param>
        void Play(TimeSpan currentTime);
        /// <summary>
        /// Stops this music player.
        /// </summary>
        void Stop();
        /// <summary>
        /// Update this music player. Uses MusicPlayer.Time as the current time.
        /// </summary>
        void Update();
        /// <summary>
        /// Update this music player using a custom current time.
        /// </summary>
        /// <param name="currentTime">Current player time.</param>
        void Update(TimeSpan currentTime);
        /// <summary>
        /// Seeks to position within the song (relative to TimeSpan.Zero). Uses MusicPlayer.Time as the current time.
        /// </summary>
        /// <param name="position">Position relative to TimeSpan.Zero to seek to.</param>
        void Seek(TimeSpan position);
        /// <summary>
        /// Seeks to position within the song (relative to TimeSpan.Zero) using a custom current time.
        /// </summary>
        /// <param name="currentTime">Current player time.</param>
        /// <param name="position">Position relative to TimeSpan.Zero to seek to.</param>
        void Seek(TimeSpan currentTime, TimeSpan position);
        /// <summary>
        /// Mutes this player.
        /// </summary>
        void Mute();
        /// <summary>
        /// Unmutes this player.
        /// </summary>
        void Unmute();

        /// <summary>
        /// Boolean value indicating whether the player is still playing music.
        /// </summary>
        bool Playing { get; }
        /// <summary>
        /// Boolean value indicating whether the player is muted.
        /// </summary>
        bool Muted { get; }
        /// <summary>
        /// Duration of the song.
        /// </summary>
        TimeSpan Duration { get; }
        /// <summary>
        /// Current position inside song.
        /// </summary>
        TimeSpan Elapsed { get; }
    }
}
