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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using TextPlayer;
using System.Threading;

namespace MidiPlayer {
    public enum SongFormat {
        MML, ABC
    }

    public partial class Form1 : Form {
        private volatile IMidiPlayer player;
        private object playerLock = new object();
        private volatile bool stopPlaying = false;
        private Thread backgroundThread;
        private int filterIndex = 1;

        public Form1() {
            InitializeComponent();
        }

        delegate void SetScrollValueDelegate(int val);

        private void SetScrollValue(int val) {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (scrSeek.InvokeRequired) {
                try {
                    SetScrollValueDelegate d = new SetScrollValueDelegate(SetScrollValue);
                    this.Invoke(d, new object[] { val });
                }
                catch { }
            }
            else {
                scrSeek.Value = Math.Max(scrSeek.Minimum, Math.Min(scrSeek.Maximum, val));
                SetTimeText();
            }
        }

        private void SetTimeText() {
            var playerRef = player;
            if (playerRef != null) {
                lblTime.Text = playerRef.Elapsed.ToString("mm':'ss") + " / " + playerRef.Duration.ToString("mm':'ss");
            }
            lblTime.Left = scrSeek.Right - lblTime.Width;
        }

        private void LoadFile(StreamReader reader, SongFormat format) {
            StopPlaying();
            stopPlaying = false;

            if (format == SongFormat.MML) {
                var mml = new PlayerMML();
                mml.Settings.MaxDuration = TimeSpan.MaxValue;
                mml.Settings.MaxSize = int.MaxValue;
                mml.Load(reader);
                player = mml;
            }
            else {
                var abc = new PlayerABC();
                abc.Settings.MaxDuration = TimeSpan.MaxValue;
                abc.Settings.MaxSize = int.MaxValue;
                abc.Load(reader);
                player = abc;
            }

            player.SetInstrument((Midi.Instrument)Enum.Parse(typeof(Midi.Instrument), cmbInstruments.SelectedItem.ToString()));
            player.Normalize = chkNormalize.Checked;
            player.Loop = chkLoop.Checked;
            player.CalculateNormalization();
            SetTimeText();
            scrSeek.Maximum = (int)Math.Ceiling(player.Duration.TotalSeconds);
            scrSeek.Minimum = 0;
            scrSeek.Value = 0;
            backgroundThread = new Thread(Play);
            backgroundThread.Start();
        }

        private void StopPlaying() {
            stopPlaying = true;
            if (backgroundThread != null) {
                backgroundThread.Join(TimeSpan.FromSeconds(1));
            }
        }

        protected override void OnClosed(EventArgs e) {
            StopPlaying();
            base.OnClosed(e);
        }

        private void Play() {
            try {
                TimeSpan now = new TimeSpan(DateTime.Now.Ticks);
                player.Play(now);

                while (player != null && !stopPlaying) {
                    lock (playerLock) {
                        if (player.Playing) {
                            player.Update(now);
                            int scrollVal = (int)player.Elapsed.TotalSeconds;
                            if (scrollVal != scrSeek.Value) {
                                SetScrollValue(scrollVal);
                            }
                        }
                        else {
                            if (scrSeek.Value != 0) {
                                SetScrollValue(0);
                            }
                        }
                    }
                    Thread.Sleep(1);
                    now = new TimeSpan(DateTime.Now.Ticks);
                }

                lock (playerLock) {
                    if (player.Playing) {
                        player.Stop();
                        //Console.WriteLine("Closed while playing, stopPlaying: " + stopPlaying + ", player was null: " + (player == null));
                    }
                    else {
                        //Console.WriteLine("Closed due to done playing");
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine("Background thread terminated: " + e.ToString());
            }
            finally {
                player.CloseDevice();
                player = null;
            }
        }

        private void btnOpen_Click(object sender, EventArgs e) {
            var diag = new OpenFileDialog();

            diag.InitialDirectory = ".";
            diag.Filter = "Song files|*.mml;*.abc|MML files (*.mml)|*.mml|ABC files (*.abc)|*.abc|All files (*.*)|*.*";
            diag.FilterIndex = filterIndex;
            diag.RestoreDirectory = true;

            Stream stream = null;
            if (diag.ShowDialog() == DialogResult.OK) {
                try {
                    if ((stream = diag.OpenFile()) != null) {
                        using (stream) {
                            using (var reader = new StreamReader(stream)) {
                                if (Path.GetExtension(diag.FileName).ToLowerInvariant() == ".abc")
                                    LoadFile(reader, SongFormat.ABC);
                                else
                                    LoadFile(reader, SongFormat.MML);
                                lblFile.Text = "File: " + Path.GetFileName(diag.FileName);
                            }
                        }
                    }
                }
                catch (Exception ex) {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message + System.Environment.NewLine + ex.StackTrace);
                    if (player != null)
                        player.CloseDevice();
                }
            }

            filterIndex = diag.FilterIndex;
        }

        private void Form1_Load(object sender, EventArgs e) {
            foreach (var instrument in Enum.GetValues(typeof(Midi.Instrument))) {
                string s = instrument.ToString();
                cmbInstruments.Items.Add(s);
                if (s == default(Midi.Instrument).ToString()) {
                    cmbInstruments.SelectedItem = s;
                }
            }
        }

        private void cmbInstruments_SelectionChangeCommitted(object sender, EventArgs e) {
            lock (playerLock) {
                if (player != null)
                    player.SetInstrument((Midi.Instrument)Enum.Parse(typeof(Midi.Instrument), cmbInstruments.SelectedItem.ToString()));
            }
        }

        private void chkNormalize_CheckedChanged(object sender, EventArgs e) {
            var playerRef = player;
            if (playerRef != null)
                playerRef.Normalize = chkNormalize.Checked;
        }

        private void chkLoop_CheckedChanged(object sender, EventArgs e) {
            var playerRef = player;
            if (playerRef != null)
                playerRef.Loop = chkLoop.Checked;
        }

        private void btnPause_Click(object sender, EventArgs e) {
            lock (playerLock) {
                if (player != null) {
                    if (player.Paused)
                        player.Unpause();
                    else if (player.Playing)
                        player.Pause();
                }
            }
        }

        private void chkMute_CheckedChanged(object sender, EventArgs e) {
            lock (playerLock) {
                if (player != null) {
                    player.Muted = chkMute.Checked;
                }
            }
        }

        private void scrSeek_MouseDown(object sender, MouseEventArgs e) {
            lock (playerLock) {
                if (player != null) {
                    double perc = e.X / (double)scrSeek.Width;
                    int seconds = (int)(perc * player.Duration.TotalSeconds);
                    player.Seek(new TimeSpan(DateTime.Now.Ticks), TimeSpan.FromSeconds(seconds));
                    SetScrollValue(seconds + 1);
                    SetScrollValue(seconds);
                }
            }
        }

        private void btnPlay_Click(object sender, EventArgs e) {
            lock (playerLock) {
                if (player != null) {
                    if (player.Playing && !player.Paused)
                        player.Stop();
                    player.Play(new TimeSpan(DateTime.Now.Ticks));
                }
            }
        }

        private void btnStop_Click(object sender, EventArgs e) {
            lock (playerLock) {
                if (player != null) {
                    player.Stop();
                }
            }
        }
    }
}
