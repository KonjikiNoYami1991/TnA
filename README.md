# TnA - Tanoshimi no Autohardsubber

Simple, yet almost automatic, autohardsubber from Tanoshimi no Sekai Fansub in .NET 4.8.

## Building

This solution requires a C# builder for Windows.\
For example, [Visual Studio Community 2019](https://visualstudio.microsoft.com/vs/community/), even without registration.\
No need to use Professional or Enterprise edition, but you can.

## Requirements

- [FFmpeg by Zeranoe](https://ffmpeg.zeranoe.com/builds/) static for Windows (automatically downloaded)
- [SevenZipSharp](https://www.nuget.org/packages/SevenZipSharp.Net45/) (no need 7z installed)
- [MediaInfoNet](https://www.nuget.org/packages/MediaInfoNet/) (no need MediaInfo installed)
- [FFmpeg Output Wrapper DLL](https://github.com/KonjikiNoYami1991/FFmpegOutputWrapperNET) (for encoding progress)
- [mkvmerge.exe from MKVtoolnix for Windows](https://mkvtoolnix.download/) (preferably x86 for better compatibility with x86 systems)

## Features
- Supported file formats
  - MKV
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
- 


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
