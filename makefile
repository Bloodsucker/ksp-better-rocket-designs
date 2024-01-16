.PHONY: all release debug clean

dlls_abs_path := $(abspath $(DLLS_PATH))

OUTPUT_PATH := "./build/bin"
CONFIG_OUTPUT_PATH := "$(OUTPUT_PATH)/$(CONFIG)"
CONFIG_PACKAGE_OUTPUT_PATH := "$(OUTPUT_PATH)/$(CONFIG)/package"
ZIP_PATH := $(abspath $(CONFIG_OUTPUT_PATH)/BetterRocketDesigns.zip)

all: clean release

release:
	msbuild /p:Configuration=Release /p:ReferencePath="$(dlls_abs_path)" BetterRocketDesigns.sln
	$(MAKE) package CONFIG=Release

debug:
	msbuild /p:Configuration=Debug /p:ReferencePath="$(dlls_abs_path)" BetterRocketDesigns.sln
	$(MAKE) package CONFIG=Debug

package:
	mkdir -p "$(CONFIG_PACKAGE_OUTPUT_PATH)"
	cp "./BetterRocketDesigns/bin/$(CONFIG)/BetterRocketDesigns.dll" "$(CONFIG_PACKAGE_OUTPUT_PATH)/BetterRocketDesigns.dll"
	cp -R "./BetterRocketDesigns/Textures" "$(CONFIG_PACKAGE_OUTPUT_PATH)/Textures"
	cp "./README.md" "$(CONFIG_PACKAGE_OUTPUT_PATH)/README.md"
	cd "$(CONFIG_PACKAGE_OUTPUT_PATH)" && zip -r $(ZIP_PATH) *

clean:
	msbuild /t:Clean /p:Configuration=Release BetterRocketDesigns.sln
	msbuild /t:Clean /p:Configuration=Debug BetterRocketDesigns.sln
	rm -rf $(OUTPUT_PATH)
