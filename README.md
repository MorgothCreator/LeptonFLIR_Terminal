# XmegaBootLd
A fast bootloader in hex data transfer mode.

This is the first public release.

Is a very fast botloader that receive intel hex data format, 
it is developed in C language using the flash and eeprom driver delivered from Atmel and modified to match to this application,
with a size of about 3100 bytes for ATxmega E5 devices, 3700bytes for devices with les than 64KB flash 
and 4050bytes for devices with more than 64KB flash memory.


This bootloader receive intel hex lines from a intel hex file with status response of every received line, 
this bootloader can work with a serial adapter or throught a bluetooth or Xbee transceivers.

Aditional this bootloader can receive several commands:

1) "BootInit" this is the expected word to enter in bootloader, the bootloader wait for this word for about two seconds until hi jump to aplication.

2) "FlashW" this is the wort that indicate to bootloader that the next received data will be put in flash app memory.

3) "EEPromW" this is the word that indicate to bootloader that the next received data will be put in eeprom memory.

4) "Exit" this is the word that indicate to bootloader that all data has been transmited and to jump to loaded application.

The boot loader report on every line the status chars:

1) 'a' line definition error.

2) 'b' second hex char not found.

3) 'c' checksum error.

4) 'd' line mismach.

5) 'e' no memory selected.

6) 'k' received line is OK and has been writed on buffer.


This bootloader work with flash memory with pages, 
all received data will be writed into a buffer until are received a request to write in another page, 
when is received a write to another page the bootloader will write the buffer data into flash page 
and will load to buffer the request write page, in this mode the bootloader not write directly to flash 
but append data on flash.


The same principle is with eeprom for all devices except E5 devices that is nonbuffered.
