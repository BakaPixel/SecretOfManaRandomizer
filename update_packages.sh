poetry lock --regenerate
poetry sync
poetry export -f requirements.txt --output requirements.txt --without-hashes