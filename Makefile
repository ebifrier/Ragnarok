#
# Makefile
#

SUBDIRS = Ragnarok Ragnarok.Shogi

all:
	@for d in $(SUBDIRS); do \
		(cd $$d; make all)   \
	done

hg:
	hg pull
	hg update

clean:
	@for d in $(SUBDIRS); do \
		(cd $$d; make clean) \
	done
