#!/bin/sh

pandoc index.md -f markdown_github -s --self-contained -t html5 -c github.css -o index.html
