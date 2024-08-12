REGISTRY=$1
IMAGE_NAME=$2
TAG=$3
DOCKERFILE_PATH=$4

# Build the Docker image
docker build -t $IMAGE_NAME:$TAG -f $DOCKERFILE_PATH .

# Tag the image with the registry
docker tag $IMAGE_NAME:$TAG $REGISTRY/$IMAGE_NAME:$TAG

# Push the image to the Azure Container Registry
docker push $REGISTRY/$IMAGE_NAME:$TAG