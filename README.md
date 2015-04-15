# Lepton FLIR thermal image sensor PC interface.
This is a interface to display thermal image captured by a Lepton FLIR image sensor.

The data from the sensor will be heandled by an adapter from SPI to UART (a bridge adapter) like project example that can be cloned from <a href="https://github.com/MorgothCreator/mSdk/tree/master/SDK_Probe_STM32-E407_olimex">here.</a>

The data send to this application will be raw 80x60x2 bytes, total of 9600 bytes, with a pause between pockets of about 25ms.
