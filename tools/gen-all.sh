#!/usr/bin/env bash
set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

echo "[gen-all] Starting full generation pipeline..."
echo ""

echo "[gen-all] Step 1/2: gen-data"
node "$SCRIPT_DIR/gen-data.js"

echo ""
echo "[gen-all] Step 2/2: gen-orm"
node "$SCRIPT_DIR/gen-orm.js"

echo ""
echo "[gen-all] All steps completed successfully."
