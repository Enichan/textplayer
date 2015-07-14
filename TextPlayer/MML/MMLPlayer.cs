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
using System.Text.RegularExpressions;

namespace TextPlayer.MML {
    /// <summary>
    /// Abstract player which parses and plays MML code. This class represents only a single-track player, which takes
    /// a single track's worth of code (not prefixed by 'MML@' or ending in ';').
    /// </summary>
    public abstract class MMLPlayer : MusicPlayer {
        /// <summary>
        /// The smallest part of a measure in MML, this is 1/128th, because the smallest length indicator is 64,
        /// but a dotted 64 length note adds half its length, so we need a lower resolution than 1/64.
        /// </summary>
        public const double Tick = 1.0 / 128;

        /// <summary>
        /// Time when the next smallest increment occurs.
        /// </summary>
        protected TimeSpan nextTick;

        /// <summary>
        /// The measure location the next note needs to be played.
        /// </summary>
        protected double nextNote;

        /// <summary>
        /// Current measure location.
        /// </summary>
        protected double curMeasure;

        private int octave;
        private MMLLength length;
        private int tempo;
        /// <summary>
        /// Seconds per measure
        /// </summary>
        protected double spm;
        private int volume;

        private MMLSettings settings;
        private TimeSpan duration;

        protected List<MMLCommand> commands;
        protected int cmdIndex;

        protected List<string> mmlPatterns = new List<string> {
                @"[tT]\d{1,3}",
                @"[lL](16|2|4|8|1|32|64)\.?",
                @"[vV]\d+",
                @"[oO]\d",
                @"<",
                @">",
                @"[a-gA-G](\+|#|-)?(16|2|4|8|1|32|64)?\.?",
                @"[rR](16|2|4|8|1|32|64)?\.?",
                @"[nN]\d+\.?",
                @"&"
        };

        public MMLPlayer()
            : base() {
            settings = new MMLSettings();
            SetDefaultValues();
            settings.MaxSize = 1024 * 4;
        }

        private void SetDefaultValues() {
            octave = 4;
            length = new MMLLength(4, false);
            Tempo = 120;
            volume = 8;
            cmdIndex = 0;
        }

        public override void Load(string str) {
            if (str.Length > settings.MaxSize) {
                throw new SongSizeException("Song exceeded maximum length of " + settings.MaxSize);
            }

            commands = new List<MMLCommand>();
            string code = str.Replace("\n", "").Replace("\r", "");

            string compoundPattern = "";
            foreach (string exp in mmlPatterns) {
                if (!string.IsNullOrEmpty(compoundPattern))
                    compoundPattern += "|";
                compoundPattern += exp;
            }

            MatchCollection matches = Regex.Matches(code, compoundPattern);
            foreach (Match match in matches) {
                commands.Add(MMLCommand.Parse(match.Value));
            }

            CalculateDuration();
            SetDefaultValues();
        }

        protected virtual void CalculateDuration() {
            TimeSpan dur = TimeSpan.Zero;
            double measureLength;

            for (int i = 0; i < commands.Count; i++) {
                var cmd = commands[i];
                switch (cmd.Type) {
                    case MMLCommandType.Tempo:
                        SetTempo(cmd);
                        break;
                    case MMLCommandType.Length:
                        SetLength(cmd);
                        break;
                    case MMLCommandType.Rest:
                        var len = GetRest(cmd, out measureLength);
                        dur += len.ToTimeSpan(spm);
                        break;
                    case MMLCommandType.NoteNumber:
                    case MMLCommandType.Note:
                        var note = GetNote(cmd, out measureLength);
                        dur += note.Length;
                        break;
                }

                if (dur > settings.MaxDuration) {
                    throw new SongDurationException("Song exceeded maximum duration " + settings.MaxDuration);
                }
            }

            duration = dur;
        }

        public override void Play(TimeSpan currentTime) {
            base.Play(currentTime);

            nextTick = lastTime;
            nextNote = 0;
            curMeasure = 0;
            //Update(lastTime);
        }

        public override void Stop() {
            base.Stop();
            SetDefaultValues();
        }

        protected virtual void ProcessCommands() {
            bool noteFound = false;
            MMLCommand cmd;

            while (!noteFound && cmdIndex < commands.Count) {
                cmd = commands[cmdIndex];

                if (cmd.Type == MMLCommandType.Note || cmd.Type == MMLCommandType.NoteNumber) {
                    return;
                }
                else if (cmd.Type == MMLCommandType.Rest) {
                    return;
                }
                else {
                    ProcessCommand(cmd);
                }

                cmdIndex++;
            }
        }

        public override void Update(TimeSpan currentTime) {
            if (!playing)
                return;

            // We want to wait until the current tick's time has elapsed before incrementing the next ticks
            while (currentTime >= nextTick) {
                // process commands first
                ProcessCommands();

                // Play a note if the current measure location is at the next note location
                if (curMeasure >= nextNote) {
                    if (cmdIndex < commands.Count) {
                        NextNote();
                    }
                    else
                        Stop();
                }

                // Increment the current measure location, increment the time for the next tick to occur based on current seconds per measure.
                nextTick += new TimeSpan((long)(Tick * spm * TimeSpan.TicksPerSecond));
                curMeasure += Tick;
            }
            base.Update(currentTime);
        }

        private void NextNote() {
            bool noteFound = false;
            Note note;
            MMLCommand cmd;

            while (!noteFound && cmdIndex < commands.Count) {
                cmd = commands[cmdIndex];

                if (cmd.Type == MMLCommandType.Note || cmd.Type == MMLCommandType.NoteNumber) {
                    double measureLength;
                    note = GetNote(cmd, out measureLength);

                    while (cmdIndex + 2 < commands.Count &&
                        commands[cmdIndex + 1].Type == MMLCommandType.Tie &&
                        (commands[cmdIndex + 2].Type == MMLCommandType.Note || commands[cmdIndex + 2].Type == MMLCommandType.NoteNumber)) {
                        double tiedLength;
                        Note tiedNote = GetNote(commands[cmdIndex + 2], out tiedLength);
                        if (tiedNote.Sharp == note.Sharp && tiedNote.Type == note.Type && tiedNote.Octave == note.Octave) {
                            cmdIndex += 2;
                            note.Length += tiedNote.Length;
                            measureLength += tiedLength;
                        }
                    }
                    noteFound = true;
                    nextNote += measureLength;
                    ValidateAndPlayNote(note);
                }
                else if (cmd.Type == MMLCommandType.Rest) {
                    double measureLength;
                    GetRest(cmd, out measureLength);
                    nextNote += measureLength;
                    noteFound = true;
                }
                else {
                    ProcessCommand(cmd);
                }

                cmdIndex++;
            }
        }

        private void ValidateAndPlayNote(Note note) {
            if (note.Octave < settings.MinOctave)
                note.Octave = settings.MinOctave;
            else if (note.Octave > settings.MaxOctave)
                note.Octave = settings.MaxOctave;
            note.Volume = Math.Max(0f, Math.Min(note.Volume, 1f));
            if (!Muted)
                PlayNote(note, 0, nextTick);
        }

        private MMLLength GetRest(MMLCommand cmd, out double measureLength) {
            MMLLength rest = GetLength(cmd.Args[0], cmd.Args[1]);
            measureLength = 1.0 / rest.Length * (rest.Dotted ? 1.5 : 1.0);
            return rest;
        }

        private Note GetNote(MMLCommand cmd, out double measureLength) {
            if (cmd.Type == MMLCommandType.Note) {
                return GetNoteNormal(cmd, out measureLength);
            }
            else {
                return GetNoteNumber(cmd, out measureLength);
            }
        }

        private Note GetNoteNormal(MMLCommand cmd, out double measureLength) {
            Note note = new Note();

            note.Volume = volume / 15f;
            note.Octave = octave;
            note.Type = cmd.Args[0].ToLowerInvariant()[0];

            switch (cmd.Args[1]) {
                case "#":
                case "+":
                    Step(ref note, +1);
                    break;
                case "-":
                    Step(ref note, -1);
                    break;
                default:
                    note.Sharp = false;
                    break;
            }

            var mmlLen = GetLength(cmd.Args[2], cmd.Args[3]);
            note.Length = mmlLen.ToTimeSpan(spm);

            measureLength = 1.0 / mmlLen.Length;
            if (mmlLen.Dotted)
                measureLength *= 1.5;

            return note;
        }

        private Note GetNoteNumber(MMLCommand cmd, out double measureLength) {
            Note note = new Note();

            note.Volume = volume / 15f;
            note.Octave = 1;
            note.Type = 'c';

            // 12 == C1
            int noteNumber = Convert.ToInt32(cmd.Args[0]) - 12;
            int octavesUp = noteNumber / 12;

            int steps;
            if (octavesUp != 0)
                steps = noteNumber % (octavesUp * 12);
            else
                steps = noteNumber;

            note.Octave += octavesUp;
            Step(ref note, steps);

            var mmlLen = GetLength("", cmd.Args[1]);
            note.Length = mmlLen.ToTimeSpan(spm);

            measureLength = 1.0 / mmlLen.Length;
            if (mmlLen.Dotted)
                measureLength *= 1.5;

            return note;
        }

        protected virtual void ProcessCommand(MMLCommand cmd) {
            switch (cmd.Type) {
                case MMLCommandType.Length:
                    SetLength(cmd);
                    break;
                case MMLCommandType.Octave:
                    SetOctave(Convert.ToInt32(cmd.Args[0]));
                    break;
                case MMLCommandType.OctaveDown:
                    SetOctave(octave - 1);
                    break;
                case MMLCommandType.OctaveUp:
                    SetOctave(octave + 1);
                    break;
                case MMLCommandType.Tempo:
                    SetTempo(cmd);
                    break;
                case MMLCommandType.Volume:
                    SetVolume(Convert.ToInt32(cmd.Args[0]));
                    break;
            }
        }

        protected virtual void SetLength(MMLCommand cmd) {
            length = new MMLLength(Convert.ToInt32(cmd.Args[0]), cmd.Args[1] != "");
        }

        protected virtual void SetOctave(int newOctave) {
            octave = newOctave;
            if (octave < settings.MinOctave)
                octave = settings.MinOctave;
            else if (octave > settings.MaxOctave)
                octave = settings.MaxOctave;
        }

        protected virtual void SetTempo(MMLCommand cmd) {
            Tempo = Convert.ToInt32(cmd.Args[0]);
        }

        protected virtual void SetVolume(int vol) {
            // Compress from 1-127 range (ArcheAge volume) to 1-15 (Mabinogi volume)
            if (Mode == MMLMode.ArcheAge) {
                vol = (int)Math.Round(Math.Min(1.0, Math.Max(0.0, vol / 127.0)) * 15);
            }

            volume = vol;
            if (volume < settings.MinVolume)
                volume = settings.MinVolume;
            else if (volume > settings.MaxVolume)
                volume = settings.MaxVolume;
        }

        private MMLLength GetLength(string number, string dot) {
            MMLLength l = length;
            if (!string.IsNullOrEmpty(number))
                l = new MMLLength(Convert.ToInt32(number), dot != "");
            else if (!string.IsNullOrEmpty(dot))
                l.Dotted = true;
            return l;
        }

        #region Properties
        public int Tempo {
            get {
                return tempo;
            }
            set {
                tempo = Math.Max(settings.MinTempo, Math.Min(settings.MaxTempo, value));
                spm = 60d / ((double)tempo / 4);
            }
        }
        public List<MMLCommand> Commands { get { return commands; } }
        public TimeSpan NextTick { get { return nextTick; } }
        public MMLSettings Settings { get { return settings; } set { settings = value; } }
        internal override ValidationSettings validationSettings { get { return settings; } }
        public override TimeSpan Duration { get { return duration; } }
        public MMLMode Mode { get; set; }
        #endregion
    }
}
