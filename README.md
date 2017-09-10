# MASHCo-Holo

This is the MASHCo holographic App for the Microsoft Hololens. It uses the RESTful HTTP-API of Eclipse-Smarthome to show smarthome items to the user and send commands to them. The App uses QR-Codes to create holograms and bind them to a real item. You can find a JavaFX-Tool to generate the QR-Codes [here](https://github.com/SeelabFhdo/SmarthomeLabeler). After creating the QR-Codes just say `scan` while the app is running, get close to the QR-Code and look on it.

## Building
Clone the project, open it with Unity 2017 or newer, build it for Windows (UWP) and build the final app with Visual Studio (best approach for debugging). Alternatively you can build and install the App directly with the HoloToolkit which is included in the project (easiest and fastest approach)


## Testing without QR-Codes
If you don't want to generate QR-Codes, you can insert an Eclipse Smarthome item REST-URL like
`http://foo/rest/items` in the PersistenceManager script which is attached to the MainCamera. The App shows all items which are tagged with the `holo` tag in front of the users position. Make sure that you are looking to an open area while launching the app. After initialization you can move the holograms as you want.  

## Notice

Please notice that some other scenes (like the Microsoft origami example) are included too for testing because this project is still under development. You don't have to add this scenes to your UWP-Build. Sometimes they are useful to test your build configuration.

Thanks to [this](https://github.com/mtaulty/QrCodes) Project for making it so easy to read QR-Codes with the hololens. 
