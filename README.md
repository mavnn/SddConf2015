# Property Based Testing

This repository has several dependencies managed via Paket. Run restore.bat
or restore.sh before opening the code for best results.

These code examples were created for a talk at
[SDDConf 2015](http://sddconf.com/agenda/?p=243) if you want to know more
about what's going on!

Enterprise.fsx holds some examples around creating XML docs. This is a
stand alone F# script file (no associated project or solution) for you
to play with in the editor of your choice.

Serialization.fsx contains a few tests from the Chiron Json serialization
library, and loads a checkout of a commit of the Chiron code that
contained a bug. Again, this is a stand alone script file.

And finally the PhoneNumbers directory contains a Visual Studio solution
and some C# code for property testing a class for representing
E.164 formatted international phone numbers.

The presentation briefly touched on the property based testing of
the Sproc.Lock open source project - that code isn't included
here, but is available [on github](http://github.com/15below/Sproc.Lock).
