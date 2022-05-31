# FF6PR2MSU (Final Fantasy VI Pixel Remaster to MSU-1)
FF6PR2MSU is a tool for Linux and Windows to take the ripped .wav audio files from Final Fantasy VI Pixel Remaster (which will be referred to as FF6PR from now on) and make them work with [Dizzy611](https://github.com/Dizzy611)'s [DancingMadFF6](https://github.com/Dizzy611/DancingMadFF6) Final Fantasy III (SNES) MSU-1 hack.

## STILL A WORK IN PRORESS!
The application is incomplete, unoptimized, is untested on Windows, still requires a script to batch the operations and was put together in a day... As such, it might still be buggy and the code is in dire need of organizing and cleaning up.
<br /><br />
Right now, users will need to find a wav2msu executable or simply compile [jbaiter](https://github.com/jbaiter)'s [wav2msu](https://github.com/jbaiter/wav2msu). For Linux users, compiling it as an executable .out file seemed to work great.

### TO-DO
1. Streamline how to load all necessary files. Maybe I should just ask a path to a folder instead of recquiring all files as arguments?
2. Either use wav2msu.c as a native C library or re-write it in C# in the app so it doesn't require an external executable to convert the .wav to .pcm files.
3. See if it's possible to only require the single FF6PR resource file instead of the ripped audio files... It would certainly make this whole process way more simple and user-friendly.
4. (TBD) Repurpose and open up the app to allow converting FF4 and FF5PR audio files for FF4/5 MSU-1 hacks (if those even exist).

## DISCLAIMER
I will <b><i>NOT</i></b> share the Pixel Remaster audio files themselves. The files are under copyright, thus sharing them around would be illegal. You will need to supply your own. To do so, I recommend you look around how to rip the audio from resource files of the Steam release of the game.
<br /><br />
FF6PR2MSU was made under the LGPL license, as such, you may use it and redistribute it. However, any modification to the program has to be made available under the same license.

## SETUP AND HOW TO USE
<b>The app is a work in progress and I intend on streamlining the whole process and require less external tools and everything.<br />
This process is bound to change and as such has been written pretty quickly... If there are errors in the guide, I'm sorry. I'll definitely come back to write a more detailed and final one once I'm done with all the streamlining I intend on doing.</b><br /><br />

1. Get FF6PR's .wav audio files ready and in a folder. They should be named something along those lines "SWAV_BGM_FF6_\[some number\]\_000.wav" (it might look a <i>little</i> different than this, but as long as it has the "FF6\_\[some number\]" part and the .wav extension, it should work fine).
2. Get wav2msu.exe or wav2msu.out and put it in the same folder as the FF6PR2MSU executable. However, due to most EXEs I've found online being outdated, you might have to compile your own from the [wav2msu repository](https://github.com/jbaiter/wav2msu). I <i>am</i> working on a way to eliminate this step, but, for now, this is the way it's gotta be...
3. Time for another part I need to streamline... For Windows users, you're in luck, you can simply select all the .wav files to convert and drag on drop them on the executable. On Linux, unless your Desktop Environment or file exporer allows you to drag and drop, you'll most need to write a quick command. You can use the find command to search for all files in a folder and pipe the result for the executable's parameters. Something along those lines I believe: `$(find folder/*.wav -print0) | FF6PR2MSU -`.
4. Assuming you managed to follow my poorly written steps... you should now have a bunch (around 75) .pcm files. Now, you need to patch your rom file and place all the files in the right location for your use (either an FX Pak Pro or an emulator). For that, head over to the [DancingMadFF6](https://github.com/Dizzy611/DancingMadFF6)'s repository page, and patch your FF3 rom with the .ips file.

## CREDITS
This was made possible by the incredible work of user [Dizzy611](https://github.com/Dizzy611) on his tool [DancingMadFF6](https://github.com/Dizzy611/DancingMadFF6) which allows for MSU-1 streamed audio in Final Fantasy III and the [VGAudio](https://github.com/Thealexbarney/VGAudio) library by user [Thealexbarney](https://github.com/Thealexbarney).
<br /><br />
FF6PR2MSU also requires the use of wav2msu, a tool to convert wave audio files to the PCM format used by the MSU-1. The original version of this tool was made by a user named Kawa whose page or original C# source code I could not find online... As such, I'll link you to [jbaiter](https://github.com/jbaiter)'s [reimplementation in C](https://github.com/jbaiter/wav2msu).

## FAQ
#### - I encountered an issue or a bug... what should I do?
Look up the [open issues](https://github.com/GoldenKain/FF6PR2MSU/issues). If it has already been reported, comment on it if you have any more information to add (this could make fixing it simpler and could get fixed sooner!). If it hasn't been reported, then I would greatly appreciate it if you could [create a new issue](https://github.com/GoldenKain/FF6PR2MSU/issues/new). :)

#### - But... where's the GUI?
Because I want to make it work on Windows and Linux... I prefered leaving it as a CLI application for simplicity's sake. I might try and make one later, but it would most likely require maintaining 2 different GUIs to keep it multi-platform and I don't know if I really want to do that. TBD.
