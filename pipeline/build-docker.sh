REGISTRY=$1
IMAGE_NAME=$2
TAG=$3
DOCKERFILE_PATH=$4


# Build the Docker image
sudo -u AzDevOps docker build -t $IMAGE_NAME:$TAG -f $DOCKERFILE_PATH .

echo  $REGISTRY/$IMAGE_NAME:$TAG

# Tag the image with the registry
sudo -u AzDevOps docker tag $IMAGE_NAME:$TAG $REGISTRY/$IMAGE_NAME:$TAG

# Push the image to the Azure Container Registry
sudo -u AzDevOps docker push $REGISTRY/$IMAGE_NAME:$TAG