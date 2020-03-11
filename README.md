# TnA - Tanoshimi no Autohardsubber

Simple, yet almost automatic, autohardsubber from [Tanoshimi no Sekai Fansub](https://tnsfansub.com/), written in C# .NET 4.8.

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
- Supported file formats
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
- Automatic extraction of fonts and subtitle track from MKV files (no need to install fonts)
- Integrated updater
- Autodeinterlacing when needed with [YADIF](https://ffmpeg.org/ffmpeg-filters.html#yadif-1), mode=0 for spatial check without bobbing (only if TFF or BFF are detected, otherwise disabled).\
This is not the best way for deinterlacing, but it's very fast and simple to use.
- Instead of xyVSfilter (not included in FFmpeg), it uses [LIBASS](https://github.com/libass/libass) for Advanced Substation Alpha subtitles and generally works well.
- Format selection when adding an entire folder.
- Clipboard monitoring, similar to JDownloader.\
By activating this, you can just copy files and folder from File Explorer and they will be automatically added, following the formats' choices.
- Settings save in .ini file.
- Automatic removing of source file after a correct conversion.
- Pause/resume conversion.
- SendTo link in Windows SendTo menù can be created or removed.
- Possibility to choose one action after conversion of all listed files.
- Can be restored to initial settings.


## Usage

- Drag&drop files and folders, use buttons or paste files and folders from system clipboard.
- Choose one among compatibility profiles, resolutions (height), quality profiles, aubtitle modes.
- Click on Play button and wait. Destination files will be in the same folder of source files, never overwritten.

```bash
This is a frontend 
```

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[GPLv3](https://choosealicense.com/licenses/gpl-3.0/)
