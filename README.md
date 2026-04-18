# todo-api

📋 API Server for the Totally Organized Disorganization Organizer.

## Local development with Docker Compose

### Prerequisites

- [Docker](https://docs.docker.com/get-docker/) with the Compose plugin.

### Example

```bash
# pull required images
docker compose pull

# build project
docker compose build

# start the server (creates a database and runs migrations automagically)
docker compose up -d

# stop the server
docker compose down

# stop the server and also wipe the database
docker compose down -v
```
