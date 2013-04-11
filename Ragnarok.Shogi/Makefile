#
#
#

OUTDIR = bin
TARGET = $(OUTDIR)/Ragnarok.Shogi.dll

LIB_PATH = ../externals/mono

CSFLAGS = -target:library -debug+ -warn:4 -define:__LINUX__ \
          -o+ -define:MONO

LDFLAGS = -keyfile:Ragnarok.Shogi.snk -lib:$(OUTDIR) -lib:$(LIB_PATH)
LIBS    = -r:System.Runtime.Serialization.dll -r:protobuf-net.dll \
          -r:Ragnarok

all:
	cd ../Ragnarok; make all
	@if test ! -d $(OUTDIR); then \
		mkdir $(OUTDIR); \
	fi
	cp ../Ragnarok/$(OUTDIR)/*.dll $(OUTDIR)
	@gmcs -recurse:*.cs $(CSFLAGS) -out:$(TARGET) $(LDFLAGS) $(LIBS) $^
	chmod a-x $(TARGET)

clean:
	cd ../Ragnarok; make clean
	rm -rf $(OUTDIR)
