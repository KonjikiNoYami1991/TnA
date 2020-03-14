# TnA - Tanoshimi no Autohardsubber

Simple, yet almost automatic, autohardsubber from [Tanoshimi no Sekai Fansub](https://tnsfansub.com/), written in C# .NET 4.8.

```bash
THIS IS STILL IN BETA VERSION. USE IT CAREFULLY.
```

## Building

This solution requires a C# builder for Windows.\
For example, [Visual Studio Community 2019](https://visualstudio.microsoft.com/vs/community/), even without registration.\
No need to use Professional or Enterprise edition, but you can.

## Requirements for building

- [FFmpeg by Zeranoe](https://ffmpeg.zeranoe.com/builds/) static for Windows (automatically downloaded)
- [SevenZipSharp](https://www.nuget.org/packages/SevenZipSharp.Net45/) (no need to install 7z)
- [MediaInfoNet](https://www.nuget.org/packages/MediaInfoNet/) (no need to install MediaInfo)
- [FFmpeg Output Wrapper DLL](https://github.com/KonjikiNoYami1991/FFmpegOutputWrapperNET) (for encoding progress)
- [mkvmerge.exe from MKVtoolnix for Windows](https://mkvtoolnix.download/) (preferably x86 for compatibility with x86 systems)

## Features
- Only italian language, for now.
- No administrator rights required.
- Can be placed in a netfolder and used with multiple PCs (not recommended).
- Supported file formats:
  - MKV (this is the only supported format for autohardsubbing)
  - MP4 
  - M2TS (only for MKV remux)
  - TS (only for MKV remux)
  - AVI
  - MOV 
  - RMVB (not tested)
  - OGM (not tested)
  - FLV (not tested)
  - VOB
  - MPG/MPEG
  - 3GP (not tested)
  - M4V (not tested)
- Automatic extraction of fonts and subtitle track from MKV files (no need to install fonts).
- Integrated updater.
- Autodeinterlacing when needed with [YADIF](https://ffmpeg.org/ffmpeg-filters.html#yadif-1), mode=0 for spatial check without bobbing (only if TFF or BFF are detected, otherwise disabled).\
This is not the best way for deinterlacing, but it's very fast and simple to use.
- Instead of xyVSfilter (not included in FFmpeg), it uses [LIBASS](https://github.com/libass/libass) for Advanced Substation Alpha subtitles and generally works well.
- Format selection when adding an entire folder.
- Clipboard monitoring, similar to JDownloader.\
By activating this, you can just copy files and folder from File Explorer and they will be automatically added, following the formats choices.
- Settings save in .ini file.
- Automatic removing of source file after a correct conversion (this feature has to be tested, because files seem removed, but they're not).
- Pause/resume conversion.
- SendTo link in Windows SendTo menù can be created or removed.
- Possibility to choose one action after conversion of all listed files.
- Can be restored to initial settings.
- Audio track copying instead of re-encoding if destination audio is equal to source.
- Remuxing files to MKV (with mkvmerge, ignoring all other settings) and to MP4 files.\
MP4 remux only supports:
  - H.264/H.265/XviD/DivX/MPEG 1-2-4 video streams
  - AAC/AC-3/MP3/ALAC audio streams
  - No subtitles of any kind, except TimedText (very rare and only used for 3GP).
  - Any other stream not listed above will be re-encoded according to the other parameters.
- All videos will be resized if selected resolution of destination is lower than source, considering aspect ratio. It'll never do any upscale.\
For example, a video source with resolution 1280x720 will never be upscaled to 1600x900, even selecting 900p for resolution.\
So, don't try to.
- Because there are videos with odd resolution, such as 1555x813 or one between width and height, because every compatibility profile is designed to reach standard encoding, this application will recalculate the destination resolution.\
For example, if width is odd, it will be lowered by 1, the same for height.
- While aspect ratio is greater than or equal to 1,333 (4:3), width will be used for resizing.\
Instead, height will be used.


## Hardware requirements
### Minimum
 - OS: Windows 7/8/8.1/10 x86/x64.
 - CPU: Intel Core 2 Quad, 2,00 GHz x 4.
 - RAM: 4 GB DDR2 (better have DDR3).
 - GPU: any.
 - SSD/HDD: HDD, better yet with SSD.

### Recommended
 - OS: Windows 7/8/8.1/10 x86/x64.
 - CPU: the faster, the better.
 - RAM: 8 GB DDR4.
 - GPU: any.
 - SSD/HDD: HDD, better yet with SSD.

Actually, any hardware will be fine (excepted RAM) because it's just a matter of how much time is needed to process video files. Obviously, process will be slow with slow PCs.
For example, a video 1080p with 23,976 fps, duration of 24 mins, encoded at below-medium quality, reaches 10-12 fps during conversion: PC is slow or not suitable for video encoding, but time needed is still acceptable.
Keep in mind that deinterlacing slows down encoding speed.

## Explanation of parameters
There are many parameters that can be setted before starting encoding, described below.

### Compatibility
#### - Bluray AAC
This profile suits compatibilty for bluray players witch plays AAC audio stream.
- VIDEO CODEC: H.264 8bit YUV420p
  - Parameters
    - Profile: High
    - Level: 4.1
    - Bluray compat: enabled
    - Maxrate: 50000k
    - Bufsize: 70000k
    - Pixel format: yuv420p
- AUDIO CODEC: AAC-LC, 1 to 6 channels (copied if audio source is the same to avoid quality loss)
#### - Bluray AC-3
This profile suits compatibilty for bluray players witch plays AC-3 audio stream (almost all).
- VIDEO CODEC: H.264 8bit YUV420p
  - Parameters
    - Profile: High
    - Level: 4.1
    - Bluray compat: enabled
    - Maxrate: 50000k
    - Bufsize: 70000k
    - Pixel format: yuv420p
- AUDIO CODEC: AC-3 (not fixed, not E-AC-3), 1 to 6 channels (copied if audio source is the same to avoid quality loss)
#### - Remux MP4
This profile remux any source file, re-encoding every stream not suitable, to MP4 container.\
Be sure to select destination quality to avoid wrong re-encoding parameters.
- VIDEO CODEC: copied if source video stream is suitable with MP4 container, otherwise video H.264 8bit YUV420p
  - Parameters
    - Profile: High
    - Level: 4.1
    - Bluray compat: disabled
    - Maxrate: none
    - Bufsize: none
    - Pixel format: yuv420p
- AUDIO CODEC
  - Lossy case
    - Copied if audio source is one among allowed formats, otherwise AAC-LC, 1 to 6 channels
  - Lossless case
    - Re-encoded to ALAC to avoid quality loss.
#### - Remux MKV
  - Copies all streams and metadata from source to destination file, without any stream conversion.
  - Ignores all other presets.
#### - Streaming HTML5 H.264
This profile is suitable for video streaming in HTML5 web pages.
- VIDEO CODEC: H.264 8bit YUV420p
  - Parameters
    - Profile: High
    - Level: 4.1
    - Bluray compat: enabled (maybe it should be disabled, because b-pyramid=0 is not compatible with many BD players and it could be useless to be enabled)
    - Maxrate: 20000k
    - Bufsize: 20000k
    - Pixel format: yuv420p
    - Bluray compat: disabled
    - x264opts 
      - Cabac: disabled
      - Weightp: disabled
      - Weightb: disabled
      - Sync lookahead: disabled
      - Sliced threads: 1
      - B pyramid: 0
      - Keyint min: framerate casted to integer and multiplied by 10
      - GOP: same as Keyint min to make constant GOP
- AUDIO CODEC: AAC-LC, 1 to 2 channels (copied if audio source is the same to avoid quality loss and has at most 2 channels)
#### - Streaming HTML5 H.265
This profile is named "Streaming", but it's a simple profile to convert to H.265 any file you want.
- VIDEO CODEC: H.265 8bit YUV420p
  - Parameters
    - Profile: main
    - Level: 4.1
    - Bluray compat: enabled (maybe it should be disabled, because b-pyramid=0 is not compatible with many BD players and it could be useless to be enabled)
    - Maxrate: 50000k
    - Bufsize: 70000k
    - Pixel format: yuv420p
    - Bluray compat: none
    - UHD bluray compat: disabled
    - x265params 
      - Pmode: enabled
      - Pme: enabled
      - Psy-rd: 4
      - Rdoq-level: 1
      - Psy-rdoq: 10
      - Weightb: 1
      - Me: uneven multi-hexagon
      - Subme: 4
      - Rdpenalty: 1
      - Open-gop: disabled
      - Rc-lookahead: 40
      - Bframes: 4
      - Rd: 4
      - B-adapt: 2
- AUDIO CODEC: AAC-LC, 1 to 2 channels (copied if audio source is the same to avoid quality loss and has at most 2 channels)
#### - XviD MP3
This profile is suitable for old players with DivX/XviD compatibility and PCs, of course.
- VIDEO CODEC: H.263 8bit YUV420p
  - Parameters
    - Profile: MPEG4 Advanced Simple Profile
    - Level: 5
    - Macroblock decision algorithm: RD
    - Sub pel motion estimation: 7
    - Bframes: 2
    - Limit motion vectors range: 1023
    - Use Odml: autodetermined, based on destination filesize
    - Pre motion estimation compare: 7
    - Macroblock compare: 7
    - Sub pel me compare: 7
    - Quantizer curve compression factor: 1,0
    - Minimum quantizer scale: 1,0
    - Maximum quantizer scale: 7,0
    - Flags
      - Four motion vector: enabled
      - Loop: enabled
      - Quarter-pixel motion compensation: enabled
      - High quality AC prediction: enabled
    - FFlags
      - Generate missing PTS if DTS is present: enabled
- AUDIO CODEC: MP3, 1 to 2 channels (copied if audio source is the same to avoid quality loss)
#### - Workraw
This profile generates a file with only video and audio streams, excluding all the others, with very poor quality.
- VIDEO CODEC: H.264 8bit YUV420p
  - Parameters
    - Profile: High
    - Level: 4.1
    - Bluray compat: disabled
    - Maxrate: 20000k
    - Bufsize: 20000k
    - Pixel format: yuv420p
    - Bluray compat: disabled
    - Partitions
      - i4x4
      - i8x8
      - p8x8
      - b8x8
    - Subtitles: exluded
    - Tune: fast decode
    - x264opts 
      - B pyramid: 0
- AUDIO CODEC: Vorbis, 1 channel (copied if audio source is the same to avoid quality loss)

### Resolution
There's not so much to explain.\
Resolution is setted by height, but used according to the case explained above:
 - Case AR greater than or equal to 1,333 (4:3): width is used.
 - Case AR lower than 1,333 (4:3): heigth is used.

Possible values are:
 - 1080p
 - 900p
 - 720p
 - 576p
 - 480p (actually it's 486p)
 - 396p

### Quality
Quality is the parameter that determines the final quality of destination file.\
Possible values are:
- Altissima (Highest)
  - Mostly useless.
  - Very slow.
  - Very high bitrate.
  - Very large filesize.
- Alta (High)
  - Useful to keep high quality.
  - Not as slow as Highest quality but slower than Above medium quality.
  - Large filesize.
- Medio-alta (Above medium)
  - Good quality.
  - Medium filesize (sometimes large, but it rarely happens).
  - Slower than Medium quality but faster than Above medium.
- Media (Medium)
  - Good compromise between quality and filesize.
  - Medium speed.
  - Normal filesize.
- Medio-bassa (Below medium)
  - Resonable quality.
  - Fast.
  - Quite small filesize.
  - Suitable for streaming.
- Bassa (Low)
  - Low quality.
  - Small filesize.
  - Suitable for streaming.
  - Faster.
- Bassissima (Lowest)
  - Very low quality.
  - Very small filesize.
  - Suitable for streaming, but no, do not use this preset for streaming.
  - Very fast.
- Bozza (Draft)
  - THIS PRESET IS FOR TESTING ONLY.
  - Useful to make conversion tests.
  - Fastest.
  - Very bad quality.
  - Smallest filesize

Audio bitrate is determinted using bitrate for stereo and multiplied by audio channel number, then divided by 2 (excepted for AC-3, not divided by 2).

### Subtitle modes

 - Hardsub
   1) Demux all the fonts inside MKV, if any.
   2) Scan for forced subtitle track, if any. Otherwise, default subtitle track, if any. If not found default of forced subtitles, it picks the first track. If there's no subtitles, it just skips demux and jumps to encode process.
 - Softsub
   - Copies all fonts and subtitle tracks inside MKV file and just re-encode video and audio, according to compatibility and quality presets.
   - It forces MKV container for destination file.
 - No sottotitoli (No subtitles)
   - Ignores all subtitle tracks and fonts

Hardsub mode will choose only one subtitle track, never two or more.


## Usage

- Drag&drop files and folders, use buttons or paste files and folders from system clipboard.
- Choose one among compatibility profiles, resolutions (height), quality profiles, aubtitle modes.
- Click on Play button and wait. Destination files will be in the same folder of source files, never overwritten.

## Hints
 - You can convert the same file twice or more, just be sure to change compatibility, quality or resolution.
 - Filelist is a Datagridview, similar to Excel or Calc. Clicking on the row header will select all lines in the grid.
 - Selection can be done using Shift or CTRL key, too.

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[GPLv3](https://choosealicense.com/licenses/gpl-3.0/)
