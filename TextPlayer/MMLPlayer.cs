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

namespace TextPlayer {
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

        private const int minOctave = 1;
        private const int maxOctave = 8;

        protected List<MMLCommand> commands;
        protected int cmdIndex;

        protected List<string> mmlPatterns = new List<string> {
                @"[tT]\d+",
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
            SetDefaultValues();
        }

        private void SetDefaultValues() {
            octave = 4;
            length = new MMLLength(4, false);
            Tempo = 120;
            volume = 8;
            cmdIndex = 0;
        }

        public override void Load(TextReader stream) {
            commands = new List<MMLCommand>();
            string code = stream.ReadToEnd().Replace("\n", "").Replace("\r", "");

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
                    MMLLength rest = GetLength(cmd.Args[0], cmd.Args[1]);
                    nextNote += 1.0 / rest.Length * (rest.Dotted ? 1.5 : 1.0);
                    noteFound = true;
                }
                else {
                    ProcessCommand(cmd);
                }

                cmdIndex++;
            }
        }

        private void ValidateAndPlayNote(Note note) {
            if (note.Octave < minOctave)
                note.Octave = 1;
            else if (note.Octave > maxOctave)
                note.Octave = maxOctave;
            if (!Muted)
                PlayNote(note, 0);
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
            if (octave < minOctave)
                octave = minOctave;
            else if (octave > maxOctave)
                octave = maxOctave;
        }

        protected virtual void SetTempo(MMLCommand cmd) {
            Tempo = Convert.ToInt32(cmd.Args[0]);
        }

        protected virtual void SetVolume(int vol) {
            volume = vol;
            if (volume < 1)
                volume = 1;
            else if (volume > 15)
                volume = 15;
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
                tempo = value;
                spm = 60d / ((double)tempo / 4);
            }
        }
        public List<MMLCommand> Commands { get { return commands; } }
        public TimeSpan NextTick { get { return nextTick; } }
        #endregion
    }

    public struct MMLLength {
        public int Length;
        public bool Dotted;

        public MMLLength(int length, bool dotted) {
            Length = length;
            Dotted = dotted;
        }

        public TimeSpan ToTimeSpan(double secondsPerMeasure) {
            double length = 1.0 / (double)Length;
            if (Dotted)
                length *= 1.5;

            return new TimeSpan((long)(secondsPerMeasure * length * TimeSpan.TicksPerSecond));
        }
    }

    public struct MMLCommand {
        public MMLCommandType Type;
        public List<string> Args;

        public static MMLCommand Parse(string token) {
            MMLCommand cmd = new MMLCommand();
            MMLCommandType t = MMLCommandType.Unknown;
            List<string> args = new List<string>();

            switch (token.ToLowerInvariant()[0]) {
                case 't':
                    t = MMLCommandType.Tempo;
                    AddPart(args, token, @"\d+");
                    break;
                case 'l':
                    t = MMLCommandType.Length;
                    AddPart(args, token, @"(16|2|4|8|1|32|64)");
                    AddPart(args, token, @"\.");
                    break;
                case 'v':
                    t = MMLCommandType.Volume;
                    AddPart(args, token, @"\d+");
                    break;
                case 'o':
                    t = MMLCommandType.Octave;
                    AddPart(args, token, @"\d");
                    break;
                case '<':
                    t = MMLCommandType.OctaveDown;
                    break;
                case '>':
                    t = MMLCommandType.OctaveUp;
                    break;
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                    t = MMLCommandType.Note;
                    AddPart(args, token, @"[a-gA-G]");
                    AddPart(args, token, @"(\+|#|-)");
                    AddPart(args, token, @"(16|2|4|8|1|32|64)");
                    AddPart(args, token, @"\.");
                    break;
                case 'r':
                    t = MMLCommandType.Rest;
                    AddPart(args, token, @"(16|2|4|8|1|32|64)");
                    AddPart(args, token, @"\.");
                    break;
                case 'n':
                    t = MMLCommandType.NoteNumber;
                    AddPart(args, token, @"\d+");
                    AddPart(args, token, @"\.");
                    break;
                case '&':
                    t = MMLCommandType.Tie;
                    break;
                default:
                    t = MMLCommandType.Unknown;
                    args.Add(token);
                    break;
            }

            cmd.Type = t;
            cmd.Args = args;

            return cmd;
        }

        private static void AddPart(List<string> args, string token, string pattern) {
            string s = Regex.Match(token, pattern).Value;
            args.Add(s);
        }
    }

    public enum MMLCommandType {
        Tempo,
        Length,
        Volume,
        Octave,
        OctaveDown,
        OctaveUp,
        Note,
        Rest,
        NoteNumber,
        Tie,
        Unknown
    }
}
