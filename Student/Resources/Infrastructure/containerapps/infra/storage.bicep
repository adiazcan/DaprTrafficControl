param uniqueSeed string
param location string
param entryCamContainerName string
param exitCamContainerName string

resource storageAccount 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: toLower('sa${uniqueString(uniqueSeed)}')
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'BlobStorage'
  properties: {
    accessTier: 'Hot'
  }
}

resource storageAccountEntryCamContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-04-01' = {
  name: '${storageAccount.name}/default/${entryCamContainerName}'
}

resource storageAccountExitCamContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-04-01' = {
  name: '${storageAccount.name}/default/${exitCamContainerName}'
}

output storageAccountName string = storageAccount.name
output storageAccountEntryCamContainerName string = entryCamContainerName
output storageAccountExitCamContainerName string = exitCamContainerName
output storageAccountContainerKey string = listKeys(storageAccount.id, storageAccount.apiVersion).keys[0].value
