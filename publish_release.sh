#!/bin/bash
#
# publish_release.sh — Build & release script for real-bean-boy
#
# Usage:
#   ./publish_release.sh <version-tag> [description-file] [webhook-url]
#
#   <version-tag>     Required. e.g. v0.0.8
#   [description-file]  Optional. Path to a text file with release notes.
#   [webhook-url]       Optional. Discord webhook URL (overrides env var).
#
# To avoid passing the webhook URL every time, export it in your ~/.bashrc:
#   export DISCORD_WEBHOOK_URL="https://discord.com/api/webhooks/..."
#
# The Godot executable path defaults to the version in the script.
# Override it if needed:
#   export GODOT_EXEC="/path/to/Godot_v4.x.x_linux.x86_64"
#
# Exit immediately if a command fails
set -e

# Check if a version number was provided
if [ -z "$1" ]; then
  echo "Usage: ./publish_release.sh <version-tag> [description-file] [webhook-url]"
  exit 1
fi

VERSION=$1
DESCRIPTION_FILE=$2

if [ -n "$DISCORD_WEBHOOK_URL" ]; then
  :  # already set from environment
elif [ -n "$3" ]; then
  DISCORD_WEBHOOK_URL=$3
else
  echo "Error: DISCORD_WEBHOOK_URL is not set. Export it in ~/.bashrc or pass as the third argument." >&2
  exit 1
fi

# Check required dependencies
for cmd in gh curl python3 git zip; do
  if ! command -v "$cmd" &>/dev/null; then
    echo "Error: $cmd is not installed. Please install it and try again." >&2
    exit 1
  fi
done

# Godot executable path (override via GODOT_EXEC env var)
GODOT_EXEC=${GODOT_EXEC:-$HOME/Godot_v4.6.3-stable_mono_linux_x86_64/Godot_v4.6.3-stable_mono_linux.x86_64}

echo "🚀 Starting build process for release $VERSION..."

# 1. Create build directories
rm -rf build && mkdir -p build/windows/debug build/linux/debug

# 2. Export Linux (Headless)
echo "🐧 Exporting Linux build..."
"$GODOT_EXEC" --headless --export-release "Linux (debug)" build/linux/debug/real-bean-boy.x86_64

# 3. Export Windows (Headless)
echo "🪟 Exporting Windows build..."
"$GODOT_EXEC" --headless --export-release "Windows (debug)" build/windows/debug/real-bean-boy.exe

# 4. Zip the builds and the source code
echo "📦 Zipping builds..."
cd build
zip -rq real-bean-boy-linux-${VERSION}.zip linux/debug/
zip -rq real-bean-boy-windows-${VERSION}.zip windows/debug/
cd ..

echo "🗄️ Zipping source code..."
git ls-files | zip -q build/real-bean-boy-source-${VERSION}.zip -@

# 5. Create GitHub Release
echo "🌐 Uploading to GitHub..."
if [ -n "$DESCRIPTION_FILE" ] && [ -f "$DESCRIPTION_FILE" ]; then
  echo "📝 Using release notes from $DESCRIPTION_FILE"
  NOTES_ARG="--notes-file $DESCRIPTION_FILE"
else
  NOTES_ARG="--generate-notes"
fi
gh release create "$VERSION" \
  build/real-bean-boy-windows-${VERSION}.zip \
  build/real-bean-boy-linux-${VERSION}.zip \
  build/real-bean-boy-source-${VERSION}.zip \
  --title "Release $VERSION" \
  $NOTES_ARG

# 6. Send Custom Message to Discord
echo "💬 Pinging Discord with direct links..."

# Read release description if provided
if [ -n "$DESCRIPTION_FILE" ] && [ -f "$DESCRIPTION_FILE" ]; then
  DESCRIPTION=$(python3 -c "import sys,json; print(json.dumps(sys.stdin.read()))" < "$DESCRIPTION_FILE")
else
  DESCRIPTION="Click the links below to download the latest files directly."
fi

# Get the repository name automatically (e.g., "YourUsername/YourGame")
REPO_NAME=$(gh repo view --json nameWithOwner --jq '.nameWithOwner')

# Construct the direct download URLs
RELEASE_URL="https://github.com/${REPO_NAME}/releases/tag/${VERSION}"
WINDOWS_URL="https://github.com/${REPO_NAME}/releases/download/${VERSION}/real-bean-boy-windows-${VERSION}.zip"
LINUX_URL="https://github.com/${REPO_NAME}/releases/download/${VERSION}/real-bean-boy-linux-${VERSION}.zip"
SOURCE_URL="https://github.com/${REPO_NAME}/releases/download/${VERSION}/real-bean-boy-source-${VERSION}.zip"

# Create a JSON payload for a nice Discord embed
JSON_PAYLOAD=$(cat <<EOF
{
  "content": "🎉 **New Build Ready!** (${VERSION})",
  "embeds": [{
    "title": "Release ${VERSION}",
    "url": "${RELEASE_URL}",
    "color": 3447003,
    "description": ${DESCRIPTION},
    "fields": [
      {
        "name": "🪟 Windows Build",
        "value": "[Download](${WINDOWS_URL})",
        "inline": true
      },
      {
        "name": "🐧 Linux Build",
        "value": "[Download](${LINUX_URL})",
        "inline": true
      },
      {
        "name": "📦 Project Files",
        "value": "[Download](${SOURCE_URL})",
        "inline": false
      }
    ]
  }]
}
EOF
)

# Send the payload to Discord via curl
curl -s -H "Content-Type: application/json" -d "$JSON_PAYLOAD" "$DISCORD_WEBHOOK_URL" > /dev/null

# 6. Cleanup
echo "🧹 Cleaning up local build files..."
rm -rf build/*

echo "✅ Success! Release $VERSION published and Discord notified!"
