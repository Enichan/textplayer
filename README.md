<h1>TextPlayer Framework<h1>
<h4>An MML and ABC song playing framework.</h4>

This .NET 2.0 compatible framework is intended for the parsing and timing of songs written in the plaintext MML or ABC formats used for instrument-playing in MMOs like Mabinogi, Archeage or Lord of the Rings Online. The framework will parse songs and time them, notifying derived classes of when notes need to be played, on what channel, at what pitch, etc. The framework itself does not generate audio; this is an implementation detail left up to the user.

<h2>Basic Usage</h2>

To implement a player, derive one of the following classes: _ABCPlayer_, _MMLPlayer_, or _MultiTrackMMLPlayer_. Because _MultiTrackMMLPlayer_ doesn't derive from the base _MusicPlayer_ class and the other two classes do, all of these implement a common _IMusicPlayer_ interface. A basic implementation for any of these classes would look like this:

```C#
public class ImplementedPlayer : MultiTrackMMLPlayer {
    public ImplementedPlayer()
        : base() {
    }

    protected override void PlayNote(Note note, int channel) {
        // audio implementation goes here
    }
}
```

Examples are provided with the Framework for a basic single-track MML and basic ABC player which plays notes on channel 0 in a console using Console.Beep.

<h2>Notes</h2>

The _Note_ struct comes with the following properties:
<ul>
<li>Octave: octave of the note, normally ranging from 1 up to 8. The fourth octave corresponds to the octave of middle-C.
<li>Length: a _TimeSpan_ containing the length or duration of the note.
<li>Type: a lower-case character in the form of 'abcdefg' that denotes the base type of the note.
<li>Sharp: a boolean value which indicates whether or not the note is sharp or not.
<li>Volume: a floating point value indicating the volume of the note, between 0.0 and 1.0.
</ul>
This information can then be used to implement audio. Notes have an additional function _GetFrequency(Note? tuningNote = null)_ method. This will calculate the frequency of the _Note_ based on a specified tuning note, or A4 = 440 Hz if unspecified. This can be then used for synth implementations. 

There is also a _GetStep()_ function, which calculates the semitone index as an integer, starting at 12 for C1 and going up by 1 for every semitone. This can be used to pitch audio samples up or down to create the desired tone, in order to avoid the 100 or so samples of audio required otherwise. The below example assumes that a pitch of 1.0 is default, going up to 2.0 for one octave up, 0.5 for one octave down, and an audio sample corresponding to middle-C (C4).

```C#
var c4 = new Note() { Type = 'c', Octave = 4 };
var note = new Note() { Type = 'f', Octave = 4 };
var dist = note.GetStep() - c4.GetStep();
var pitch = Math.Pow(1.0594630943592952645618252949463, dist);
```

While this code works for Unity, it will not work for XNA in which a pitch of 0.0 is default, a pitch of 1.0 is one octave up and a pitch of -1.0 is one octave down. In this case the following code can be used.

```C#
var c4 = new Note() { Type = 'c', Octave = 4 };
var note = new Note() { Type = 'f', Octave = 4 };
var dist = note.GetStep() - c4.GetStep();
var pitch = Math.Pow(1.0594630943592952645618252949463, Math.Abs(dist)) - 1;
if (dist < 0)
    pitch *= -1;
```

It is recommended to create at least one sample per octave, due to limits on pitching up or down in most audio libraries, as well as distortion.

<h2>MML Implementation</h2>

MML is fully supported through the _MultiTrackMMLPlayer_ class, with the following caveats:
<ul>
<li>The version of MML supported is the non-verbose version used by Mabinogi and Archeage, with code starting with 'MML@' and ending in a semi-colon ';', with tracks split up by a comma. This means that the extended markup available to more tradition usage of MML is not parsed.
<li>Mabinogi's note command (ex 'N60') would allow musicians in that game to play notes in octaves above or below the maximum. All notes in the TextPlayer Framework are validated for maximum and minimum values, including the note command.
<li>Default values correspond to the following commands: 'O4', 'L4', 'T120', V8'.
<li>When using the single-track _MMLPlayer_ class _only_ the code for that track should be provided. This should not be preceeded by 'MML@' or end in a semi-colon ';'.
</ul>

<h2>ABC Implementation</h2>

The framework attempts to implement the ABC implementation but for security and reasons of complexity really only supports the features supported by Lord of the Rings Online, with the following caveats:
<ul>
<li>ABC files that contain multiple tunes (denoted by the header command 'X: <track>') are parsed as separate tracks and can be played separately. By default the first tune is played. The TextPlayer Framework will not play multiple tunes in sequence as Lord of the Rings Online does.
<li>Many advanced features such as repetitions and tuples are unsupported. The currently supported featureset is:
<ul>
<li>Commented lines (lines starting with the percent sign '%') are supported.
<li>A global header may be specified, its values are set before the header values specific to the currently playing tune are set.
<li>The information fields for key ('K'), meter ('M'), default note length ('L') and tempo ('Q') are supported.
<li>The 'Q', 'L', and 'K' commands are also supported as inline information fields inside a tune body.
<li>Chords are supported. Chord duration is set to the lowest duration note contained inside the chord.
<li>Rests inside chords are supported, and can modify the duration of the chord.
<li>Measures are supported and will clear accidentals.
<li>The following dynamics for setting volume are supported: '+pppp+', '+ppp+', '+pp+', '+p+', '+mp+', '+mf+', '+f+', '+ff+', '+fff+', '+ffff+'.
<li>Notes are supported and default to octave 4 for upper case notes and octave 5 for lower case. The shortest possible note length is 1/64 and the longest is 4 measures.
<li>Rests ('x', 'z' or 'Z') are supported and subject to the above length restrictions.
<li>Notes can be tied using the & symbol but only notes of the same pitch and octave can be tied in such a manner. In all other cases the tie is simply ignored.
<li>Octave can be changed, and any number of accidental symbols will compute correctly when attached to a note. If an accidentals reset symbol is found anywhere ('=') the accidental is reset and all other modifiers ignored.
<li>Accidentals can be set to propagate to apply to all notes of the same pitch in the same octave (default as per Lord of the Rings Online), all notes of the same pitch regardless of octave, or set to only apply to the one note and not propagate at all.
</ul>
<li>When ABC v2.1 strict is indicated an error will be thrown if files do not start with a version string in the form of '%abc-<majorVersion>.<minorVersion>', or '%abc-2.1'.
<li>In strict mode an error will be thrown if the 'T: <title>' information field is not preceeded by the 'X: <track>' information field.
<li>In strict mode an error will be thrown if the 'Q: <tempo>' information field is not in the form of 'Q: <noteLength> = <bpm>', for example 'Q: 1/4 = 120'.
<li>When not in strict mode the tempo field will be parsed as best as possible, where simple numbers are allowed (note length is assumed to be 1/4 or inferred from the meter), and a reverse notation of '<bpm> = <noteLength>' is also allowed.
</ul>
