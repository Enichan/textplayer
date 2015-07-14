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

namespace TextPlayer.MML {
    /// <summary>
    /// A concrete implementation of MMLPlayer for MultiTrackMMLPlayer.
    /// </summary>
    public class MMLPlayerTrack : MMLPlayer {
        private MultiTrackMMLPlayer parent;

        public MMLPlayerTrack(MultiTrackMMLPlayer parent)
            : base() {
            this.parent = parent;
        }

        protected override void PlayNote(Note note, int channel, TimeSpan time) {
            parent.PlayNote(note, channel, this, time);
        }

        protected override void SetTempo(MMLCommand cmd) {
            if (Mode == MMLMode.Mabinogi) { // tempo changes in Mabinogi apply to all tracks
                parent.SetTempo(Convert.ToInt32(cmd.Args[0]));
            }
            else { // tempo changes in ArcheAge only apply to the current track
                base.SetTempo(cmd);
            }
        }

        protected override void CalculateDuration() {
        }

        public MultiTrackMMLPlayer Parent { get { return parent; } }
    }
}
