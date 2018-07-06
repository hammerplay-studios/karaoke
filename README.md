# Karaoke plugin for Unity
Karaoke is a neat plugin to parse and display karaoke subtitle files such as SSA/ASS inside Unity.

If you are looking for a SRT parser have a look at (https://github.com/roguecode/Unity-Simple-SRT), this project is a derivate of this project.

Use Aegisub to create subtitle/karaoke SSA/ASS files, it is opensource (http://www.aegisub.org/)

## How to
- Add the `Karaoke` component to Text component.
- Rename the `.ass` file to `.txt`, then drag and drop in `Subtitle File` property.
- You either use `Play On Awake` or call the method `StartSubtitle()` or `StartSubtitle(callback)`.
- Add your custom tag to `Highlight Start Tag` and `Highlight End Tag` to highlight words of the line.

![Component](https://github.com/hammerplay-studios/karaoke/blob/master/ReadmeFiles/Karaoke-Component.PNG?raw=true)

## Preview
![Preview](https://github.com/hammerplay-studios/karaoke/blob/master/ReadmeFiles/preview.gif?raw=true)
