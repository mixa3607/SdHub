#export DOCKER_HOST=ssh://192.168.1.100
export DC_ARGS=('--env-file' './dc/env-prod')
docker-compose "${DC_ARGS[@]}" config
docker-compose "${DC_ARGS[@]}" build sdhub
