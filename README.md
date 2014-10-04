<h1>TextPlayer Framework</h1>
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
<li>Length: a <i>TimeSpan</i> containing the length or duration of the note.
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

<h2>Timing</h2>

All classes can be played using the _Play_ and _Update_ methods. The _Play_ method is called to prepare a song for playback. Then the song is played by repeated calls to _Update_ every frame until the song is done when the _Playing_ property is set to false.

These methods have two overloads, one which takes the current time and one which doesn't. If no time is specified, _DateTime.Now_ is used for the current time. Games which have a main loop which specifies the current game time as a _TimeSpan_ (such as XNA) can simply pass this value to these methods and playback will perform as expected. Songs store their elapsed time since starting in the _Elapsed_ property.

If specifying _TimeSpan.Zero_ as the starting time when calling the _Play_ method, users will have to keep track of the amount of time passed since calling _Play_ manually. This is not generally recommended.

<h2>Security and Validation</h2>

Because this framework is intended to be used inside games where player submitted content cannot be vetted ahead of time, the library comes with features which validate content ahead of time. These Settings can be accessed and customized via the <i>Settings</i> property of any of the player classes. This is always a subclass of the main _ValidationSettings_ class which contains settings common to both MML and ABC.

<h3>File Size, Song Length, Octaves and Tempo</h3>

The first thing validated when loading a song (using the _Load_ or _FromFile_ methods) is its file size. When loading from stream or file an error is immediately thrown when the number of bytes read exceeds _Settings.MaxSize_, preventing further reading of the file. When loading from a string, if the string length exceeds this property an exception is also thrown. This property defaults to the following:
<ul>
<li><i>ABCPlayer</i>: 12,288 bytes or 12 kilobytes.
<li><i>MultiTrackMMLPlayer</i>: 12,288 bytes or 12 kilobytes.
<li><i>MMLPlayer</i>: 4,096 bytes or 4 kilobytes.
</ul>
After this the song's duration is calculated. As soon as the duration exceeds _Settings.MaxDuration_ an exception is thrown and calculation stops to prevent client freezing. This property defaults to 5 minutes.

The allowed range of octaves is specified by the _Settings.MinOctave_ and _Settings.MaxOctave_ properties. These default to 1 and 8 respectively. Tones with octaves outside of this range are clamped to these values.

The final thing validated generally is the tempo, specified by _Settings.MinTempo_ and _Settings.MaxTempo_, defaulting to values of 32 and 255 respectively. This value specifies the tempo in beats per minute. For MML these values correspond to 'T32' and 'T255'. For ABC this corresponds to 'Q: 1/4 = 32' and 'Q: 1/4 = 255'.

<h3>MML Validation</h3>

MML validation settings also has the <i>MinVolume</i> and <i>MaxVolume</i> properties. These specify the minimum and maximum volume to accept. Volumes outside of this range are clamped to this range. Defaults to 1 and 15.

<h3>ABC Validation</h3>

ABC validation settings also have the following properties:
<ul>
<li><i>ShortestNote</i> and <i>LongestNote</i>: The shortest and longest notes allowable. These default to 1/64th of a measure and 4 measures long respectively. Shorter or longer notes are clamped to these values.
<li><i>MaxChordNotes</i>: The maximum number of notes in a chord which are actually played, excluding rests. Chords <i>can</i> contain more notes, and these notes will be used to determine the duration of the chord, but they will not result in any <i>PlayNote</i> calls.
</ul>

<h2>MML Implementation Details</h2>

MML is fully supported through the _MultiTrackMMLPlayer_ class, with the following caveats:
<ul>
<li>The version of MML supported is the non-verbose version used by Mabinogi and Archeage, with code starting with 'MML@' and ending in a semi-colon ';', with tracks split up by a comma. This means that the extended markup available to more tradition usage of MML is not parsed.
<li>Mabinogi's note command (ex 'N60') would allow musicians in that game to play notes in octaves above or below the maximum. All notes in the TextPlayer Framework are validated for maximum and minimum values, including the note command.
<li>Default values correspond to the following commands: 'O4', 'L4', 'T120', V8'.
<li>When using the single-track <i>MMLPlayer</i> class <i>only</i> the code for that track should be provided. This should not be preceeded by 'MML@' or end in a semi-colon ';'.
</ul>

<h2>ABC Implementation Details</h2>

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
<li>The default octave for ABC notated songs is never clearly specified. <i>ABCPlayer</i> uses octave 4 by default. This can be changed by setting <i>ABCPlayer.DefaultOctave</i>.
<li>The default method for propagating accidentals in ABC notation is specified to be <i>AccidentalPropagation.Pitch</i>. The <i>ABCPlayer</i> uses <i>AccidentalPropagation.Octave</i> by default however in order to be compatible with Lord of the Rings Online playback. This can be changed by setting <i>ABCPlayer.DefaultAccidentalPropagation</i>.
</ul>
