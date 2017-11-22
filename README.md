# ImageProcessing
Image processing library

The library is built to allow you to use System.Drawing style classes in .NetStandard. There is no direct access to filesystem to reduce dependencies, but if you ever need to decode or save an image, you could simply create a FileStream and pass it to Bitmap.Save or BitmapFactory.Decode for reading.
