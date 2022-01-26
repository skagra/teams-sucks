# Teams Sucks

I often find myself on mute during Teams meetinga and struggle to find the relevant window to be able to unmute.

This project solves the problem by providing an external physical device with buttons to bring Teams windows to the front/cycle them, to toggle mute and to toggle the camera.

The project consists of:

* Hardware
   * Based around a Microcontroller, with status LCD and sound signals.   
   * Connectivity to a PC running via Bluetooth or USB.
   * Powerable via a battery or USB.
   * C++ software uploaded to the Microtroller to control the funcion of the device and to comunicate with the PC.
* Protcol
   * A simple network protocol between the hardware device and the PC.
* PC software
   * A .Net application which communicates with the hardware device, taking instructions and manipulating Teams appropriately.

 
