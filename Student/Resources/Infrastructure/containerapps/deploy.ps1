$RG_NAME = 'container-traffic'
$REGISTRY = 'mycontapp.azurecr.io'

$REGISTRY_USERNAME = az acr credential show -n mycontapp --query username
$REGISTRY_PASSWORD = az acr credential show -n mycontapp --query passwords[0].value

az group create -n $RG_NAME -l westeurope
az deployment group create -n container-traffic-app -g $RG_NAME --template-file .\main.bicep -p registry=$REGISTRY registryUsername=$REGISTRY_USERNAME registryPassword=$REGISTRY_PASSWORD