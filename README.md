# My Osmand Client

With this project you can create a UWP app to transmit the GPS position of your device to a server of your choice via the OsmAnd protocol.  
I needed a simple app to regularly communicate the location of my Windows devices to my [Traccar](https://www.traccar.org) server (yes, it is compatible with the ***Traccar** GPS tracking system*) and I couldn't find anything on the Store, so I created my own, and here it is.

##  Building and installation

Currently the app is not published on the *Microsoft Store*, nor is it planned to be published, so if you want to use it you have to build it on your own.  
Clone the repository on your PC and open the solution in *Visual Studio*: from here you can create a new package according to your needs. Once you have the package, you need to install it on your Windows 10 device, desktop or mobile (remember it's a UWP app, so it's compatible with all): double-click the package on PC or deploy it on your mobile device via wifi or usb connection - that's it, you're done.  
Please note: as it is not a published app on the *Store*, you need to enable *Developer* mode to allow installation from other sources such as your package.

## Settings and usage

By opening the app, once installed on the device, you'll have only one screen available, which is the configuration screen: fill in the fields as needed and save to start tracking the location.

<img src="https://user-images.githubusercontent.com/35368854/160000597-d7478783-b7ab-4793-94e4-0d947a334ed7.png" height="350">

- **Enable**: Check this box to enable periodic transmission of the location - if this is not checked, no data will be sent.
- **Device ID**: Enter a unique numeric id here to identify your device on the platform to which you'll send the data.
- **Server URL**: The full url of the server you want to transmit your location to. Remember to also enter the protocol (e.g. http or https).
- **Server port**: The port on which the server is listening and on which sending will be done.
- **Precision (meter)**: How accurate can be, in meters, the detection of the position done by your device. Please note that in any case the accuracy may vary depending on the device and environmental conditions.
- **Update frequency (seconds)**: How often, in seconds, a location detection and its subsequent sending to the given server will be done.

Tap or click *Save* when you're done to run the app with the new settings provided.

The line below the button gives you useful information about the status of the detection or any problems with the app.

## Final informations

If you want to contribute to the project, you're welcome: open an issue explaining your idea and its development will be evaluated, or create the code changes yourself and open a pull request when you've done.

Please note that this code is provided "as is", and it's not guaranteed to be bug free - I do my best to keep it up to date in my spare time. Also note that it was created in a personal capacity and is in no way affiliated with Microsoft Corporation, Traccar, Osmand or any other company.

Thanks for taking an interest in **My Osmand Client**!
