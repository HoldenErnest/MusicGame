## Theory

Using the input PCM data represented by an array of floats(R[0-1023] - L[1024-2047]), I can get a live representation in the form of an oscillogram.
With this I can try to detect patterns between the frequencies to determine when the audio is 'impactful'.


### Facts for PCM:
* Low sounds are represented by few wide waves/groups
* High sounds are represented by many skinny waves/groups

### Facts for FreqDomain:
* start of a note cant be denoted by a peaked high frequency (some notes may drag out and will stay high/slowly get lower)
* the raw data float[2048] = index: 0-2047, freq: 20-20k (tested this). Each record is about 9.75hz higher than the previous

### Patterns to detect/use:
* BPM
* Total number of groups
* How wide each group is
* Average height of each group
* How many outliers in the group
* Which frequencies does this group span
* total volume/energy (see below)

### Impactful audio:
* Should be inline with BPM
* More frequent for faster music
* Base drops

### Total Energy/Loudness: [link](https://dsp.stackexchange.com/questions/2951/loudness-of-pcm-stream/2953#2953)
* Use RMS to find total energy


### FFT(Fourier Transform): [link](https://www.codeproject.com/KB/audio-video/SoundViewer.aspx)
* converts time domain data into seperate frequencies. This can then be converted into a frequency domain
* Unity has a built in FFT: `AudioListener.GetSpectrumData(audioInfoRaw, 0, FFTWindow.Rectangular);`
![fft](Images/Testing/fft.png)

```
// PCM DATA
private void OnAudioFilterRead(float[] data, int channels) {
    if (!fetchRawAudio) return;
    audioInfoRaw = data;
    fetchRawAudio = false;
}
```


[Should I use fmod audio](https://www.fmod.com/docs/2.01/unity/user-guide.html)? seems like it may be more of an audio editor

[Maybe this](https://discussions.unity.com/t/how-to-do-a-fft-in-unity/139527/3)

### Random thoughts
 * use a better fft instead of unitys
 * "The Mel scale represents frequency space linearly up to 700 Hz and logarithmically above that, mimicking how humans perceive pitch space"

## From the top:

 * I get frequency amplitude per each frame. We'll call each of these a `frequencyFrame`
 * FOR EACH frequencyFrame:
    * divide the frequencyFrame into the 9 `octaves` according to [this chart](https://mixbutton.com/music-tools/frequency-and-pitch/music-note-to-frequency-chart). (you might be able to scrap some of the edge octaves)
    * FOR EACH of the octaves:
        * divide the octave into 7 `notes`, once again according to the chart
        * each of these notes will hold the `highest frequency` in its frequency range, as well as the `average`
        * FOR EACH note:
            * compare it with the equivilent notes in a stored `noteArrayBuffer` (a few previous slices of frequencyFrames)
            * this is an `activeNote` IF:
                * this note is much higher than the buffered ones. (whether it be highest freq or higher average)
                * this note is higher than both the neighboring notes ( if the average is about the same there could be 2 active notes right next to eachother)
        * store this new note array in the buffer
 *** Try showing this information directly first, but it might have timing problems in which case do the below ***

 * Once you have all active notes you can determine if theyre a `shownNote` IF:
    * //it lines up with the bpm (for major notes)
    * a note from this frequency hasnt been played in n ms
    * there arent many other shown notes in this `shownNotesSection`
 * a shown notes section is a row of notes which hold the tiles you will physically click ( ultimately the notes have to have some kind of spacing so the player isnt overwhelmed notes at nearly the same time) (these sections should move somewhat with the bpm). If there are ever consecutive active notes, choose only the middle one to be displayed?

.
### Note activating algorithm ---------------
* [Some inspirational research](https://pdf.sciencedirectassets.com/280203/1-s2.0-S1877050912X00061/1-s2.0-S1877050912006424/main.pdf?X-Amz-Security-Token=IQoJb3JpZ2luX2VjEIz%2F%2F%2F%2F%2F%2F%2F%2F%2F%2FwEaCXVzLWVhc3QtMSJGMEQCIHFKGmB7qPDHPIVFJo3133c%2FnSgWhvW%2BHYDK0FAfLZtUAiA1V4pAqVeW7JTm%2B6Q1eV6dytzPhypjtnVBNVRSJOPKlCq8BQj1%2F%2F%2F%2F%2F%2F%2F%2F%2F%2F8BEAUaDDA1OTAwMzU0Njg2NSIMhHCt%2FOdf%2F65MgQe2KpAF%2BulUdicUeB94XKNKzoedaPnhMvICLhMbriWLCn4%2FoO0X951%2FuqM9z3H%2BaKNzOzUhw2e70ZcPq6vXHrDCFIir42OL6pa%2B1KLdvJaRCCIUHPuto5A4rSvomB38Z2Yuussmh78SSbvPN3srITw6fE8EMREl2%2Fmu4pRLRhK4bI%2F6ohMZum8RjwU0G9PvLNllXvfpv1FrrHBIVMKgyCNLMpcDjE2sxpMIDCOkKA7aAO%2FME1I6QvN09XRdZKXUI3AK5fOEIONnqptgfsWRK7Z3VEcttzRTqAZ4TdpjrmrLY3MyiFNXdrprU%2BXD3PXUPmfR%2BYNxMwy59J%2Fu9Ll2MAdGYca5WMiMDIMWZEITPRsnEtSKdjcc5yEAbTOpn%2Ba8hxC5qbKwUuOTS9hvPf4CZK4%2Bd9MMNgojtac2Gw2w0zXkJ4vVg14wxfOJEJywTslTU8yEW1EmMoZD8JwaTJm63TitkHk3HJtzZyMvKeprdOCFe1BoxGHv7rk8zK4aCN7bcL3svYDxIKAfJkOcF36t%2Bqs%2Bud%2BhKiYm0p1fnR1Qh9zmKDGbwP8GI9PIIJ8M25D%2FRvxgeUJT7ndZoCV3VTj4m4u6OPlNIm3hDn%2FJSU1x%2BU80AcmX5rrwZ5MqtY4CVoG3VN32Gixko6EMrKU721e0RaPqGXCuN%2BembKlu3lD%2FKkVqQgMLjd8yp6ZF1y8H2zL%2F3Hyis24Nmun8gHNkDMVXwnorE5nm%2FwncFJJbwMI9Uzqm%2FFXMdlJ06NeF0lZ2LwlncpUr22z5q44%2FQOuFvg7ZFu8W31N%2Broj27q9tomUlt%2F54B0WaZFCmXm8R5rs4as6WlvjlQ4w9coZsze8JkIVX1HklADDqs9nrE%2FUN6q7284H%2FtEmTuxAwjdO7vwY6sgE0kONVF2fpWHtEzMKzZaux6gl4vpgVED1OzzjJ3J%2FgsH26ZcSqEEBSfmjjZmvKSJtjjdfej3N%2BW5SUsNxnfJLQ2LjiQ0YQjDICx9eCcA4CjXT69nbN%2F4okrawRzr19bQvH3%2F1Ep9t%2B6NcWvceeKmJXmGF%2F7t8%2BJ5LRJ5jctspJ%2BkThKqrb4DcWosrgVwE%2B8We%2F6gsOlALtz%2FixKHy5OJfHepn2Mw1bgi4j%2BddTZvL46MPs&X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Date=20250403T203130Z&X-Amz-SignedHeaders=host&X-Amz-Expires=300&X-Amz-Credential=ASIAQ3PHCVTYV4DCNLOD%2F20250403%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Signature=bb89ddb1a8eff6634a93fe601714bef89f704c26bccbdb35f6c707f64f94219e&hash=3df6565f5949c209327398560b54f28a7c7c526462fedaa8275fdf0283802f59&host=68042c943591013ac2b2430a89b270f6af2c76d8dfd086a07176afe7c76c2c61&pii=S1877050912006424&tid=spdf-8b3fc122-6053-45f4-8acb-a2580f9a8d0b&sid=dbac595d3de10148c95b2e34934491b098a9gxrqa&type=client&tsoh=d3d3LnNjaWVuY2VkaXJlY3QuY29t&rh=d3d3LnNjaWVuY2VkaXJlY3QuY29t&ua=0f15575101025755535501&rr=92ab5473ed765113&cc=us)
* interpolate the data to the [Bark scale](https://en.wikipedia.org/wiki/Bark_scale)

#### Background Note:  (maybe a bass (loud and there but not as prevelant to what we percieve))
 * maxAmp is within X% of relativeMaxAmp(frequencyFrame highest)
 * ?? some kind of weighing: lower octaves are heard less by our ears
#### Forground Note:  (the main notes)
 * has buildup: previous notes should be lower amplitude (?? avg or max)
 * compare neighbors: highest peak neighbor wins.
 
.

#### Notes to be generated over time:
 * green bars represent different note sections
 * black lines represent active notes (grey is potentially active)

![example of notes](./Images/Theory/noteExample.png)

#### Straight forward notes to detect
 ![good notes](./Images/Theory/specNotes.png)

#### Though some notes may be active, they might not be played if its not lined up with bpm
 ![Bad notes](./Images/Theory/specNotes2.png)



#### My spectrogram progress (Audacity's vs Mine)
 ![Bad notes](./Images/Theory/customSpectrograph.png)
 ![Bad notes](./Images/Theory/customSpectrograph2.png)

#### Note seperation on the spectrogram
 ![Note Seperation](./Images/Theory/noteSeperation.png)
 ![Note Seperation working](./Images/Theory/noteSeperation2.png)

#### Note distinction:
 * You can find notes by crossing multiple tests

**Note Beginnings**
 ![note beginnings](./Images/Theory/noteBeginExample.png)
**Note Beginnings + High Amplitude**
 ![note Highest](./Images/Theory/Note%20begin%20X%20highestAmp.png)