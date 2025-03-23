#!/bin/bash

# Check if both arguments are provided
if [ $# -ne 2 ]; then
    echo "Usage: $0 <toml_file> <version_string>"
    exit 1
fi

TOML_FILE=$1
VERSION_STRING=$2

# Check if file exists
if [ ! -f "$TOML_FILE" ]; then
    echo "Error: File $TOML_FILE does not exist."
    exit 1
fi

# Update the version in the TOML file
if ! sed -i "s/versionNumber = \"[0-9.]*\"/versionNumber = \"$VERSION_STRING\"/" "$TOML_FILE"; then
    echo "Error: Failed to update version in $TOML_FILE"
    exit 1
fi

echo "Successfully updated version to $VERSION_STRING in $TOML_FILE"