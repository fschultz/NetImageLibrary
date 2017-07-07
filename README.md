# .Net Image Library

.NET library for easy image handling like:
  * Filters
  * Cropping
  * Thumbnail creation
  * Saving as JPG/GIF/PNG files or streams
  * Opening images from streams or URLs

You'll find information on how to get started at http://kaliko.com/image-library/get-started/ and full API documentation at http://kaliko.com/image-library/api/

<a href="https://www.nuget.org/packages/ImageLibrary/">Get .Net Image Library from NuGet</a>

Using only safe code making this library possible to use on web hosts with medium trust.

When having access to full trust make sure to use ImageLibrary.FastFilters to greatly improve performance!

Current build contains the following filters:
  * Gaussian blur filter
  * Unsharpen mask filter
  * Chroma key filter
  * Contrast filter
  * Brightness filter
  * Invert filter
  * Desaturnation filter

If you plan using this library with WPF or simular, read this post on <a href="http://labs.kaliko.com/2011/03/convert-to-bitmapimage.html">how to convert an KalikoImage object to System.Windows.Media.Imaging.BitmapImage and System.Windows.Controls.Image</a>.

## History
**3.0.2**
* Fixed bug where BlitFill doesn't properly fill portrait sized images

**3.0.1**
* Added option to prevent upscaling when resizing images

**3.0.0**
* Added faster filter alternatives for full trust environmnets
* Added SetResolution functions

**2.0.6**
* Added support to store original or set new resolution (DPI) 
* Added RotateFlip method
* Added access to bitmap data

**2.0.5**
* Rewritten file loader to prevent file locks
* Fixed image loading to ignore pixel-per-inch resolutions of original images
* Fixed constructor using System.Drawing.Image to support indexed palettes

**2.0.4**
* Added new TextField class for better text support
* Fixed scaling bug and updated test program 

**2.0.0**
  * Replaced Gaussian blur filter with better implementation (affects unsharpen masks)
  * Added chroma key filter
  * Rewritten API for Scaling
  * Added color space handling

**1.2.4**
  * Updated to Visual Studio 2010.
  * Code clean-up.<br>
  * Unwanted-border-artifact-problem fixed (thanks Richard!)<br>
  * IDisponable has been implemented.<br>

**1.2.3**
  * Minor changes.
  * First API documentation uploaded. Still missing a whole lot, but it's a start :)

**1.2.2**
  * Minor changes

**1.2.1**
  * Bug in thumbnail function fixed.
  * Code cleaned up.
