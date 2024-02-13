.PHONY: all release debug clean

dlls_abs_path := $(abspath $(DLLS_PATH))

OUTPUT_PATH := "./build/bin"
CONFIG_OUTPUT_PATH := "$(OUTPUT_PATH)/$(CONFIG)"
PACKAGE_ZIP_ROOT_FOLDER := "$(OUTPUT_PATH)/$(CONFIG)/package"
PACKAGE_FILE_DESTINATION := "$(PACKAGE_ZIP_ROOT_FOLDER)/GameData/BetterRocketDesigns"
ZIP_PATH := $(abspath $(CONFIG_OUTPUT_PATH)/BetterRocketDesigns.zip)

all: clean release

release:
	msbuild /p:Configuration=Release /p:ReferencePath="$(dlls_abs_path)" BetterRocketDesigns.sln
	$(MAKE) package CONFIG=Release

debug:
	msbuild /p:Configuration=Debug /p:ReferencePath="$(dlls_abs_path)" BetterRocketDesigns.sln
	$(MAKE) package CONFIG=Debug

package:
	mkdir -p "$(PACKAGE_FILE_DESTINATION)"
	cp "./BetterRocketDesigns/bin/$(CONFIG)/BetterRocketDesigns.dll" "$(PACKAGE_FILE_DESTINATION)/BetterRocketDesigns.dll"
	cp -R "./BetterRocketDesigns/Textures" "$(PACKAGE_FILE_DESTINATION)/Textures"
	cp "./README.md" "$(PACKAGE_FILE_DESTINATION)/README.md"
	cp "./LICENSE" "$(PACKAGE_FILE_DESTINATION)/LICENSE"
	python3 update-version.py get-version > "$(PACKAGE_FILE_DESTINATION)/VERSION"
	cd "$(PACKAGE_ZIP_ROOT_FOLDER)" && zip -r $(ZIP_PATH) *

clean:
	msbuild /t:Clean /p:Configuration=Release BetterRocketDesigns.sln
	msbuild /t:Clean /p:Configuration=Debug BetterRocketDesigns.sln
	rm -rf $(OUTPUT_PATH)
