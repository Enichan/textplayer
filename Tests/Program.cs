using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TextPlayer.ABC;

namespace Tests {
    /// <summary>
    /// These are some unit tests for the framework.
    /// They're ugly but they work.
    /// </summary>
    class Program {
        static bool OutputOnSuccess = false;

        static void Main(string[] args) {
            var failedTests = new List<string>();

            var failedCount = TestKeyParser();
            if (failedCount > 0) {
                Console.WriteLine("!! ABC key parsing tests failed: " + failedCount + " count");
                failedTests.Add("ABC key parsing");
            }
            else {
                Console.WriteLine("ABC key parsing tests succeeded");
            }

            if (failedTests.Count > 0) {
                Console.WriteLine("!! " + failedTests.Count + " " + (failedTests.Count > 1 ? "tests" : "test") + " failed: " + string.Join(", ", failedTests));
            }
            else {
                Console.WriteLine("All tests succeeded");
            }

            Console.ReadKey();
        }

        /// <summary>
        /// Test key parsing for ABC files
        /// </summary>
        static int TestKeyParser() {
            var failed = 0;
            var random = new Random(605713895);

            var allKeys = Keys.Accidentals.Keys.ToList();
            allKeys.AddRange(Keys.KeyAliases.Keys);

            var majKeys = new List<string>();
            var minKeys = new List<string>();
            var mixKeys = new List<string>();
            var dorKeys = new List<string>();
            var phrKeys = new List<string>();
            var lydKeys = new List<string>();
            var locKeys = new List<string>();

            var keyTypes = new Dictionary<string, List<string>> {
                { "", majKeys }, { "major", majKeys }, { "maj", majKeys }, { "ionian", majKeys }, { "ion", majKeys },
                { "m", minKeys }, { "minor", minKeys }, { "min", minKeys }, { "aeolian", minKeys }, { "aeo", minKeys },
                { "mixolydian", mixKeys }, { "mix", mixKeys },
                { "dorian", dorKeys }, { "dor", dorKeys },
                { "phrygian", phrKeys }, { "phr", phrKeys },
                { "lydian", lydKeys }, { "lyd", lydKeys },
                { "locrian", locKeys }, { "loc", locKeys }
            };

            foreach (var key in allKeys) {
                if (key.Length == 1 || key.EndsWith("#") || key.EndsWith("b")) {
                    majKeys.Add(key);
                }
                else if (key.EndsWith("m")) {
                    minKeys.Add(key.Substring(0, key.Length - 1));
                }
                else {
                    var k = key.Substring(0, key.Length - 3);
                    var m = key.Substring(key.Length - 3);

                    switch (m) {
                        case "Mix":
                            mixKeys.Add(k);
                            break;
                        case "Dor":
                            dorKeys.Add(k);
                            break;
                        case "Phr":
                            phrKeys.Add(k);
                            break;
                        case "Lyd":
                            lydKeys.Add(k);
                            break;
                        case "Loc":
                            locKeys.Add(k);
                            break;
                        default:
                            throw new Exception("Unexpected mode " + m);
                    }
                }
            }

            foreach (var kvp in keyTypes) {
                var mode = kvp.Key;
                foreach (var tonic in kvp.Value) {
                    if (!TestKey(random, tonic, mode)) {
                        failed++;
                    }
                }
            }

            foreach (var kvp in keyTypes) {
                var mode = kvp.Key;
                foreach (var tonic in kvp.Value) {
                    if (!TestAcc(random, tonic, mode)) {
                        failed++;
                    }
                }
            }

            foreach (var tonic in majKeys) {
                if (!TestAcc(random, tonic, "exp", true)) {
                    failed++;
                }
            }

            return failed;
        }

        static string ModeToKey(string tonic, string mode) {
            mode = tonic + ModeToAlias(mode);
            if (Keys.KeyAliases.ContainsKey(mode)) {
                return Keys.KeyAliases[mode];
            }
            return mode;
        }

        static string ModeToAlias(string mode) {
            if (mode.Length == 0) {
                return "";
            }
            else if (mode.Length == 1) {
                return mode.ToLowerInvariant();
            }
            else if (mode == "ion" || mode == "ionian" || mode == "maj" || mode == "major") {
                return "";
            }
            else if (mode == "aeo" || mode == "aeolian" || mode == "min" || mode == "minor") {
                return "m";
            }
            else if (mode == "exp") {
                return "";
            }
            else {
                return mode.Substring(0, 1).ToUpperInvariant() + mode.Substring(1, 2).ToLowerInvariant();
            }
        }

        static bool TestKey(Random random, string tonic, string mode) {
            var expected = ModeToKey(tonic, mode);
            var input =
                RandomWhiteSpace(random, 0, 3) +
                RandomCase(random, tonic) +
                RandomWhiteSpace(random, 0, 3) +
                RandomCase(random, mode) +
                RandomWhiteSpace(random, 0, 3);

            string output;
            Dictionary<char, int> accidentals;
            bool exp;
            Keys.Parse(input, out output, out accidentals, out exp);

            if (output != expected || (accidentals != null && accidentals.Count > 0) || exp) {
                Console.WriteLine("  Key parse test failed: in '" + input + "', out '" + output + "', expected '" + expected + "'");
                return false;
            }
            else if (OutputOnSuccess) {
                Console.WriteLine("  Key parse test success: in '" + input + "', out '" + output + "'");
            }

            return true;
        }

        static bool TestAcc(Random random, string tonic, string mode, bool explicitMode = false) {
            Dictionary<char, int> accArgs;
            string accArgStr;
            RandomAccidentals(random, out accArgs, out accArgStr);

            var expected = ModeToKey(tonic, mode);
            var input =
                RandomWhiteSpace(random, 0, 3) +
                RandomCase(random, tonic) +
                RandomWhiteSpace(random, 0, 3) +
                RandomCase(random, mode) +
                RandomWhiteSpace(random) +
                accArgStr +
                RandomWhiteSpace(random, 0, 3);

            string output;
            Dictionary<char, int> accidentals;
            bool exp;
            Keys.Parse(input, out output, out accidentals, out exp);

            accidentals = Keys.GetAccidentals(input);
            var expectedAccidentals = explicitMode ? accArgs : Keys.ModifyAccidentals(expected, accArgs);

            if (output != expected || accidentals == null || accidentals.Except(expectedAccidentals).Count() > 0 || expectedAccidentals.Except(accidentals).Count() > 0 || exp != explicitMode) {
                Console.WriteLine("  Accidentals test failed:");
                Console.WriteLine("    in '" + input + "'");
                Console.WriteLine("    expected key '" + expected + "', got '" + output + "'");
                Console.WriteLine("    expected exp " + exp + ", got " + explicitMode);

                WriteKvpComparison(expectedAccidentals, accidentals, 4);

                return false;
            }
            else if (OutputOnSuccess) {
                Console.WriteLine("  Accidentals test succeeded:");
                Console.WriteLine("    in '" + input + "'");
                Console.WriteLine("    expected key '" + expected + "', got '" + output + "'");
                Console.WriteLine("    expected exp " + exp + ", got " + explicitMode);

                WriteKvpComparison(expectedAccidentals, accidentals, 4);
            }

            return true;
        }

        static void WriteKvpComparison(Dictionary<char, int> lhs, Dictionary<char, int> rhs, int spaces) {
            var indent = new string(' ', spaces);
            var maxLhsSize = (from kvp in lhs select kvp.ToString().Length).Max() + 1;
            var maxRhsSize = (from kvp in rhs select kvp.ToString().Length).Max() + 1;

            for (var i = 0; i < Math.Max(lhs.Count, rhs.Count); i++) {
                Console.Write(indent);

                if (i < lhs.Count) {
                    var str = lhs.ElementAt(i).ToString();
                    Console.Write(str);

                    var remaining = maxLhsSize - str.Length;
                    if (remaining > 0) {
                        Console.Write(new string(' ', remaining));
                    }
                }
                else {
                    Console.Write(new string(' ', maxLhsSize));
                }

                if (i < rhs.Count) {
                    var str = rhs.ElementAt(i).ToString();
                    Console.Write(str);

                    var remaining = maxRhsSize - str.Length;
                    if (remaining > 0) {
                        Console.Write(new string(' ', remaining));
                    }
                }
                else {
                    Console.Write(new string(' ', maxRhsSize));
                }

                Console.WriteLine("");
            }
        }

        static string RandomWhiteSpace(Random random, int min = 1, int max = 4) {
            return new string(' ', random.Next(min, max));
        }

        static string RandomCase(Random random, string input) {
            StringBuilder s = new StringBuilder();
            var lower = input.ToLowerInvariant();
            var upper = input.ToUpperInvariant();

            for (var i = 0; i < input.Length; i++) {
                s.Append(random.NextDouble() < 0.5 ? lower[i] : upper[i]);
            }

            return s.ToString();
        }

        static void RandomAccidentals(Random random, out Dictionary<char, int> accidentals, out string accidentalsStr) {
            accidentals = new Dictionary<char, int>();
            StringBuilder str = new StringBuilder();

            for (int i = 0; i < random.Next(1, 5); i++) {
                var note = (char)random.Next(65, 72);
                var count = random.Next(-2, 3);

                accidentals[note] = count;

                if (i > 0) {
                    str.Append(' ');
                }

                if (count < 0) {
                    str.Append(new string('_', Math.Abs(count)));
                }
                else if (count > 0) {
                    str.Append(new string('^', Math.Abs(count)));
                }
                else {
                    str.Append('=');
                }

                str.Append(note);
            }

            accidentalsStr = str.ToString();
        }
    }
}
