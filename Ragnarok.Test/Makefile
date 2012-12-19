#
#
#

OUTDIR = bin
TARGET = $(OUTDIR)/Ragnarok.Test.exe

LIB_PATH = ../externals/mono

CSFLAGS = -target:exe -debug+ -warn:4 -define:__LINUX__ \
          -o+ -define:MONO

LDFLAGS = -lib:$(OUTDIR) -lib:$(LIB_PATH)
LIBS    = -r:System.Runtime.Serialization.dll -r:protobuf-net.dll \
          -r:Ragnarok

all:
	cd ../Ragnarok; make all
	@if test ! -d $(OUTDIR); then \
		mkdir $(OUTDIR); \
	fi
	cp ../Ragnarok/$(OUTDIR)/*.dll $(OUTDIR)
	@gmcs Program.cs Properties/*.cs Net/*.cs $(CSFLAGS) -out:$(TARGET) $(LDFLAGS) $(LIBS) $^
	chmod a-x $(TARGET)

clean:
	cd ../Ragnarok; make clean
	rm -rf $(OUTDIR)
