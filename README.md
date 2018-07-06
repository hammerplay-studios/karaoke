# Karaoke plugin for Unity
Karaoke is a neat plugin to parse and display karaoke subtitle files such as SSA/ASS inside Unity.

If you are looking for a SRT parser have a look at (https://github.com/roguecode/Unity-Simple-SRT), this project is a derivate of it.

Use Aegisub to create subtitles/karaoke and generate SSA/ASS files, it's opensource (http://www.aegisub.org/)

## How to
- Add `Karaoke` component to a `Text` component.
- Rename the `.ass` file to `.txt`, then drag and drop it to the `Subtitle File` property.
- You can either use `Play On Awake` or call the method `StartSubtitle()` or `StartSubtitle(callback)`.
- Add your custom tag to `Highlight Start Tag` and `Highlight End Tag`.

![Component](https://github.com/hammerplay-studios/karaoke/blob/master/ReadmeFiles/Karaoke-Component.PNG?raw=true)

## Preview
![Preview](https://github.com/hammerplay-studios/karaoke/blob/master/ReadmeFiles/preview.gif?raw=true)
