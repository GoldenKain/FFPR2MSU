# FFPR2MSU (Final Fantasy Pixel Remaster to MSU-1)

FFPR2MSU is a tool that extracts the audio files from the PC version of the Final Fantasy Pixel Remaster games and converts them to be used with existing MSU-1 patches:

- [kurrono's MSU-1 hack](https://www.zeldix.net/t1952-final-fantasy-iv-j-final-fantasy-ii-us) for Final Fantasy II/IV.
- [Conn's MSU-1 hack](https://www.zeldix.net/t2070-final-fantasy-v) for Final Fantasy V.
- [Dizzy611](../../../../Dizzy611)'s [DancingMadFF6](../../../../Dizzy611/DancingMadFF6) for Final Fantasy III/VI.

> [!NOTE]
> I will <b><i>NOT</i></b> share the audio files themselves. The files are under copyright, thus sharing them around would be illegal. You will need to supply your own music asset file (in this case, a Unity bundle file) from the PC version of the Final Fantasy Pixel Remaster game in question.

## SETUP AND HOW TO USE

1. First off, Purchase and install the PC version of your [Final Fantasy Pixel Remaster of choice](https://store.steampowered.com/bundle/21478/FINAL_FANTASY_IVI_Bundle/) if not already done.
2. Follow these steps depending on the game you're looking to use:
   <details>
   <summary>Final Fantasy II/IV</summary>

     1. Download the [files for the MSU-1 hack](http://bszelda.zeldalegends.net/stuff/Con/ff2j_msu1.zip). ([Here](https://www.zeldix.net/t1952-final-fantasy-iv-j-final-fantasy-ii-us) is the link to the page on the Zeldix forums if the other link is broken)
     2. Extract the archive.
     3. Retrieve the .ips and .msu files.

   </details>

   <details>
   <summary>Final Fantasy V</summary>

     1. Download the [files for the MSU-1 hack](https://drive.google.com/open?id=1o2XQfHcLWnFp6c8KJsa6HOKgLL51tQO5). ([Here](https://www.zeldix.net/t2070-final-fantasy-v) is the link to the page on the Zeldix forums if the other link is broken)
     2. Extract the archive.
     3. Retrieve the .ips and .msu files.

   </details>

   <details>
   <summary>Final Fantasy III/VI</summary>

     1. From user's [Dizzy611](../../../../Dizzy611)'s [DancingMadFF6](../../../../Dizzy611/DancingMadFF6) repository here on Github, download the [.ips file](../../../../Dizzy611/DancingMadFF6/tree/master/patch/ff3msu.ips) and the [.msu file](../../../../Dizzy611/DancingMadFF6/tree/master/patch/ff3msu.msu). If the links are broken, at the time of writing this, they are located in the ["patch" folder on the master branch](../../../../Dizzy611/DancingMadFF6/tree/master/patch).

   </details>
3. Patch your rom with the corresponding .ips patch file with the help of some rom patching program like [Flips](https://www.romhacking.net/utilities/1040/) or [Lunar IPS](https://www.romhacking.net/utilities/240/).
4. To save yourself the headache of having to rename dozens of files later on, make sure that the msu file and the pached rom have the same name (be careful not to inadvertently change the file extensions).
5. From the corresponding Final Fantasy Pixel Remaster Steam install folder, find the .bundle background music asset file and keep it close, you'll need it soon. It should be named something like: "ff`corresponding-game-number`\_bgm\_assets_all\_`series-of-numbers-and-letters`.bundle".
6. Download the [latest release](releases/latest) of the program or compile it from source. Make sure to keep the ini file in the same directory as the executable.
7. Drag-and-drop the .bundle file you found earlier on the executable and follow the instructions written in the console that will pop up. Alternatively, you can run the following commands in a terminal:

   On Windows:

   ```bat
   cd "path\to\the\program\directory"
   FF6PR2MSU.exe "path\to\the\bundle\file"
   ```

   On Linux:

   ```bash
   cd "path/to/the/program/directory"
   FF6PR2MSU "path/to/the/bundle/file"
   ```

8. You should now have a bunch of .pcm files in a folder named "output" located in the same directory as the program. Now, simply place all of those and the rom you patched in your location of choice (either an FX Pak Pro or some emulator that supports the MSU-1).

## FAQ

### Why is there no GUI?

Because I want to make it work on Windows and Linux... I prefered leaving it as a CLI application for simplicity's sake. Seeing as it's pretty simple and only requires drag-and-dropping a single file and then enter 2 pieces of information, I think this will be more than sufficient.

### I encountered an issue or a bug... what should I do?

Look up the [open issues](../../issues). If it has already been reported, comment on it if you have any more information to add. If it hasn't been reported, you can [create a new issue](../../issues/new). Any help is greatly appreciated! :slightly_smiling_face:

- Track milestones [here](../../milestones?state=open).
- Track open issues [here](../../issues?q=is%3Aissue+is%3Aopen).

## CREDITS

This was made possible by the incredible work of user [Dizzy611](../../../../Dizzy611) on his tool [DancingMadFF6](../../../../Dizzy611/DancingMadFF6) for Final Fantasy III/VI, [kurrono's MSU-1 hack](https://www.zeldix.net/t1952-final-fantasy-iv-j-final-fantasy-ii-us) for Final Fantasy II/IV and [Conn's MSU-1 hack](https://www.zeldix.net/t2070-final-fantasy-v) for Final Fantasy V on the [Zeldix forums](https://www.zeldix.net/), the [VGAudio](../../../../Thealexbarney/VGAudio) library by user [Thealexbarney](../../../../Thealexbarney), [wav2msu](../../../../jbaiter/wav2msu) which was originally written by a user named Kawa (whose page and original C# source code I could not find online) and then [reimplemented in C](../../../../jbaiter/wav2msu) by [jbaiter](../../../../jbaiter)'s and [AudioMog](../../../../Yoraiz0r/AudioMog) by [Yoraiz0r](../../../../Yoraiz0r).

Full credits and a copy of the copyright notice for [jbaiter](../../../../jbaiter)'s [wav2msu](../../../../jbaiter/wav2msu) and [Yoraiz0r](../../../../Yoraiz0r)'s [AudioMog](../../../../Yoraiz0r/AudioMog) can be viewed at the top of the files [Wav2Msu.cs](FF6PR2MSU/Wav2Msu.cs) and [AudioMogHcaDecoder.cs](FF6PR2MSU/AudioMogHcaDecoder.cs) respectively.
