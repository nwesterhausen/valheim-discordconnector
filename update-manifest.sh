#!/bin/bash

# Check if both arguments are provided
if [ $# -ne 2 ]; then
    echo "Usage: $0 <manifest_file> <version_string>"
    exit 1
fi

MANIFEST_FILE=$1
VERSION_STRING=$2

# Check if file exists
if [ ! -f "$MANIFEST_FILE" ]; then
    echo "Error: File $MANIFEST_FILE does not exist."
    exit 1
fi

# Update the version in the manifest file
if ! sed -i "s/\"version_number\":\s*\"[^\"]*\"/\"version_number\": \"$VERSION_STRING\"/" "$MANIFEST_FILE"; then
    echo "Error: Failed to update version in $MANIFEST_FILE"
    exit 1
fi

echo "Successfully updated version to $VERSION_STRING in $MANIFEST_FILE"