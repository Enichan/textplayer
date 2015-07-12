using System;
using System.Collections.Generic;
using System.Text;

namespace TextPlayer.ABC {
    public struct ABCNote {
        public Note BaseNote;
        public int TokenIndex;

        public ABCNote(Note baseNote, int tokenIndex) {
            BaseNote = baseNote;
            TokenIndex = tokenIndex;
        }
    }
}
