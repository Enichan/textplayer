#region License
// The MIT License (MIT)
// 
// Copyright (c) 2014-2015 Emma 'Eniko' Maassen
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

namespace TextPlayer.ABC {
    public static class LotroAutoDetect {
        private static List<string> normalizedMarkers;
        private static List<string> lotroMarkers = new List<string> {
            @"%%lotro-compatible",
            @"% using Firefern's ABC converter",
            @"Z: Transcribed by Firefern's ABC sequencer",
            @"%%abc-creator Maestro v2.3.0",
            @"%%abc-creator Maestro v2.2.1",
            @"%%abc-creator Maestro v2.2.0",
            @"%%abc-creator Maestro v2.1.1",
            @"%%abc-creator Maestro v2.1.0",
            @"%%abc-creator Maestro v2.0.0",
            @"%%abc-creator Maestro v1.4.1",
            @"%%abc-creator Maestro v1.4.0",
            @"%%abc-creator Maestro v1.0.0",
            @"% Produced with Bruzo's Transcoding Environment",
            @"Z: Transcribed by LotRO MIDI Player: http://lotro.acasylum.com/midi"
        };

        private static string maestroFuture_normalized;
        private static string maestroFuture = "%%abc-creator Maestro";

        static LotroAutoDetect() {
            UpdateMarkers();
        }

        public static void UpdateMarkers() {
            normalizedMarkers = new List<string>();
            if (lotroMarkers != null) {
                foreach (var line in lotroMarkers) {
                    normalizedMarkers.Add(NormalizeMarker(line));
                }
            }

            if (maestroFuture != null) {
                maestroFuture_normalized = NormalizeMarker(maestroFuture);
            }
            else {
                maestroFuture_normalized = null;
            }
        }

        private static string NormalizeMarker(string marker) {
            return marker.Trim().Replace(" ", "").ToLowerInvariant();
        }

        public static bool IsLotroMarker(string line) {
            var norm = NormalizeMarker(line);
            if (norm.StartsWith(maestroFuture_normalized)) {
                return true;
            }
            return normalizedMarkers.Contains(norm);
        }

        /// <summary>
        /// Call LotroAutoDetect.UpdateMarkers after changing the list of markers.
        /// </summary>
        public static List<string> LotroMarkers { get { return lotroMarkers; } set { lotroMarkers = value; } }
        /// <summary>
        /// Gets the marker to be used for future versions of Maestro, defaults to '%%abc-creator Maestro'.
        /// This value is not used in a straight up comparison, but instead uses string.StartsWith.
        /// Call LotroAutoDetect.UpdateMarkers after changing.
        /// </summary>
        public static string MaestroFutureMarker { get { return maestroFuture; } set { maestroFuture = value; } }
    }
}
