# GSMComm
GSM SMS library for C#

This is a fork of welly87/GSMComm, which itself is a decompilation of the GSMComm library (http://www.scampers.org/steve/sms/libraries.htm).

Since this is not the original code, it's quite buggy when it comes to PDU conversion. But the communication part is working OK.

I made the implementation compatible to the mono framework, which does not support the DataReceived callback of the serialport class.
