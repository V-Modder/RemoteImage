# RemoteImage

## Genaral

These applications displays images on the servers display from various clients.
I was inspired to wrote these application by various crime-series on TV, especially by NavyCIS.

The whole project is written in C# .Net 4.0 and Visual Studio 2010.

## Client

The RemoteImage client takes some pictures, serializes them and sends the images to the server.
The images could be taken by taking a existing picture or use the snipping options.
The snipping options are like the "Windows 7 Snipping Tool".

## Server

The RemoteImage server listens to connenctions and recieves the images.
The images will be saved in relationship to the client who send them, and will be displayed on the screen.
If the client leaves, all the pictures he send will be removed from the screen.

## License

[Openwindowgetter](http://blog.tcx.be/2006/05/getting-list-of-all-open-windows.html)<br />
[Snippingtool](http://stackoverflow.com/questions/3123776/net-equivalent-of-snipping-tool/3124252#3124252)<br />
[Fullscreen](http://www.codeproject.com/Articles/16618/How-To-Make-a-Windows-Form-App-Truly-Full-Screen-a)<br />
[Nito Asynchronous Library](https://nitoasync.codeplex.com/)<br />