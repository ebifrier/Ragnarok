#
#
#

OUTDIR = bin
TARGET = $(OUTDIR)/Ragnarok.dll

PROTOBUF_PATH = ../externals/mono

CSFLAGS = -target:library -debug+ -warn:4 -define:__LINUX__ \
          -o+ -define:MONO -define:RGN_NOT_USE_COOKIEGETTERSHARP

LDFLAGS = -keyfile:Ragnarok.snk -lib:$(OUTDIR) -lib:$(PROTOBUF_PATH)
LIBS    = -r:System.Xml.Linq.dll -r:System.Runtime.Serialization.dll \
          -r:System.Web.dll -r:WindowsBase.dll -r:System.Data.SQLite.dll \
	  -r:Mono.Posix.dll -r:protobuf-net.dll -r:NLog.dll

all:
	@if test ! -d $(OUTDIR); then \
		mkdir $(OUTDIR); \
	fi
	@cp $(PROTOBUF_PATH)/* $(OUTDIR)
	@gmcs -recurse:*.cs $(CSFLAGS) -out:$(TARGET) $(LDFLAGS) $(LIBS) $^
	chmod a-x $(TARGET)

clean:
	rm -rf $(OUTDIR)
