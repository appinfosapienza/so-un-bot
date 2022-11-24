#!/bin/bash

SEARCHDIR="./Ingegneria del Software/"

echo "Invalid URLs:"

for img in `grep "img" "$SEARCHDIR" -R | sed -E 's/^(.*?)\.txt\:img\=//'`; do
    img=$(echo "$img"|tr -d '\n'|tr -d '\r')
    curl -I "$img" 2>/dev/null | \
        grep -i "Content-Type: image/" >/dev/null
    if [ "$?" -ne 0 ]; then
        (
            cd "$SEARCHDIR"
            grep "$img" . -R
        )
    fi
done
