# MusicGame
A game made in unity that takes music input


## How does it work?
It uses a built in FFT to gather the frequency data of a song in real time. With that data you can determine which frequencies hold what could be consitered active notes. These are then displayed to the screen in the form of playable notes.

You can visit [my theories page](Theory.md) to view my whole thought process and notes.

## More tools
To work out magic numbers and make sure its all working properly I should construct an image from this real time frequency data showing it like a spectrograph, then I can draw marks on that to show where it thinks active notes are.