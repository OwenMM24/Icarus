#!/bin/bash

# Define the output file
OUTPUT_FILE="combined.json"

# Start the JSON array
echo "[" > "$OUTPUT_FILE"

# Use find with -print0 to handle spaces in filenames
FIRST_FILE=true
while IFS= read -r -d '' FILE; do
    # Extract the filename without path and extension
    BASENAME=$(basename -- "$FILE" .json)
    
    # Remove ".suki" at the end of the filename if present
    BASENAME=${BASENAME%.suki}

    # Skip empty files to prevent jq errors
    if [ ! -s "$FILE" ]; then
        echo "Skipping empty file: $FILE"
        continue
    fi

    # Validate JSON file before processing
    if ! jq empty "$FILE" >/dev/null 2>&1; then
        echo "Skipping invalid JSON file: $FILE"
        continue
    fi

    # Modify the JSON file:
    # 1. Update the "SchemaName" field with the cleaned filename
    # 2. Convert `_id` into `{ "$oid": "<original_id>" }` if `_id` exists
    MODIFIED_JSON=$(jq --arg schema "$BASENAME" '
        .SchemaName = $schema |
        if ._id then ._id = { "$oid": ._id } else . end
    ' "$FILE" 2>/dev/null)

    # Ensure the modified JSON is not empty before appending
    if [ -z "$MODIFIED_JSON" ]; then
        echo "Skipping file due to processing error: $FILE"
        continue
    fi

    # Ensure compact JSON format
    MODIFIED_JSON=$(echo "$MODIFIED_JSON" | jq -c '.')

    # Append modified JSON content, handling commas correctly
    if [ "$FIRST_FILE" = true ]; then
        FIRST_FILE=false
vb        echo "," >> "$OUTPUT_FILE"
    fi

    echo "$MODIFIED_JSON" >> "$OUTPUT_FILE"

done < <(find . -type f -name "*.json" -print0)

# Close the JSON array
echo "]" >> "$OUTPUT_FILE"

echo "Combined JSON saved to $OUTPUT_FILE"
