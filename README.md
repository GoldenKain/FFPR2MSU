# FF6PR2MSU (Final Fantasy VI Pixel Remaster to MSU-1)
FF6PR2MSU is a tool for Linux (not yet, but it's coming!) and Windows that extracts the audio files from the PC version of Final Fantasy VI Pixel Remaster (which will be referred to as FF6PR from now on) and converts them to be used with [Dizzy611](https://github.com/Dizzy611)'s [DancingMadFF6](https://github.com/Dizzy611/DancingMadFF6), a Final Fantasy III/VI (SNES) MSU-1 hack.

## STILL A WORK IN PRORESS!
The application is incomplete and unoptimized. As such, it might still be buggy and the code is in dire need of organizing and cleaning up.
<br /><br />
Right now, users will need to have [Yoraiz0r](https://github.com/Yoraiz0r)'s [AudioMog](https://github.com/Yoraiz0r/AudioMog) alongside this program for it to work.

### TO-DO
1. Find a way to rip assets to wave audio files instead of having to require the use of an external program (AudioMog).
2. (TBD) Repurpose and open up the app to allow converting FF4 and FF5PR audio files for FF4/5 MSU-1 hacks (if those even exist).

## DISCLAIMER
I will <b><i>NOT</i></b> share the Pixel Remaster audio files themselves. The files are under copyright, thus sharing them around would be illegal. You will need to supply your own music asset file (in this case, a Unity bundle file) from the PC version of Final Fantasy VI Pixel Remaster.
<br /><br />
Full credits and a copy of the copyright notice for [jbaiter](https://github.com/jbaiter)'s [wav2msu](https://github.com/jbaiter/wav2msu) can be viewed at the top of the file with my C# reimplementation/modifications ([Wav2Msu.cs](https://github.com/GoldenKain/FF6PR2MSU/blob/main/FF6PR2MSU/Wav2Msu.cs)).

## SETUP AND HOW TO USE
<b>The app is a work in progress and I intend on streamlining the whole process and make it so that it doesnt require any external tools/programs/etc.<br />
This process is bound to change, but I will do my best to update the steps when it changes. If there are errors in the guide, I'm sorry. I'll definitely come back to write a more detailed and final one once I'm done with all the streamlining I intend on doing.</b><br /><br />

1. First off, Purchase the Steam version of Final Fantasy VI Pixel Remaster if you don't already have it.
2. You will need to patch your Final Fantasy III/VI (SNES) rom file. For that, head over to the [DancingMadFF6](https://github.com/Dizzy611/DancingMadFF6)'s repository page, and patch your rom with the .ips file with the help of some ips patch program like Flips or Lunar IPS.
3. In your FFVIPR's install folder, find the .bundle background music asset file and keep it close, you'll need it soon. It should be named "ff6_bgm_assets_all_181eb630118efa8542dab51f7e8d2795.bundle".
4. This part should be eliminated eventually... You'll need to go and download [AudioMog](https://github.com/Yoraiz0r/AudioMog), a program that can rip or modify resource files for various Square Enix games on PC. You'll need to download the latest version (v2021.06.05.02 at the time of writing this guide) and extract it in the same folder as the FF6PR2MSU executable.
5. Drag-and-drop the .bundle file you found early on the FF6PR2MSU executable and follow the instructions written in the console that will pop up.
6. You should now have a bunch of .pcm files (around 75) in a folder named "output" that was created in the same directory as FF6PR2MSU. Now, simply place all of those and the rom you patched in your location of choice (either an FX Pak Pro or some emulator that supports the MSU-1).

## CREDITS
This was made possible by the incredible work of user [Dizzy611](https://github.com/Dizzy611) on his tool [DancingMadFF6](https://github.com/Dizzy611/DancingMadFF6) which allows for MSU-1 streamed audio in Final Fantasy III/VI (SNES), the [VGAudio](https://github.com/Thealexbarney/VGAudio) library by user [Thealexbarney](https://github.com/Thealexbarney) and [wav2msu](https://github.com/jbaiter/wav2msu) which was originally written by a user named Kawa (whose page and original C# source code I could not find online) and then [reimplemented in C](https://github.com/jbaiter/wav2msu) by [jbaiter](https://github.com/jbaiter)'s.
<br /><br />
FF6PR2MSU also requires the use of [Yoraiz0r](https://github.com/Yoraiz0r)'s [AudioMog](https://github.com/Yoraiz0r/AudioMog), a tool to extract/modify Square Enix game assets.

## FAQ
#### - I encountered an issue or a bug... what should I do?
Look up the [open issues](https://github.com/GoldenKain/FF6PR2MSU/issues). If it has already been reported, comment on it if you have any more information to add (this could make fixing it simpler and could get fixed sooner!). If it hasn't been reported, then I would greatly appreciate it if you could [create a new issue](https://github.com/GoldenKain/FF6PR2MSU/issues/new). :)

#### - But... where's the GUI?
Because I want to make it work on Windows and Linux (in the future)... I prefered leaving it as a CLI application for simplicity's sake. Seeing as it's pretty simple and only requires drag-and-dropping a single file and then enter 2 pieces of information, I think this will be more than sufficient.
