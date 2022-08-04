param containerAppsEnvironmentName string

param connectionString string
param consumerGroup string
param storageAccountName string
param storageAccountKey string
param storageContainerName string

resource containerAppsEnvironment 'Microsoft.App/managedEnvironments@2022-03-01' existing = {
  name: containerAppsEnvironmentName

  resource daprComponent 'daprComponents@2022-03-01' = {
    name: 'entrycam'
    properties: {
      componentType: 'bindings.azure.eventhubs'
      version: 'v1'
      metadata: [
        {
          name: 'connectionString'
          value: connectionString
        }
        {
          name: 'consumerGroup'
          value: consumerGroup
        }   
        {
          name: 'storageAccountName'
          value: storageAccountName
        }     
        {
          name: 'storageAccountKey'
          value: storageAccountKey
        }
        {
          name: 'storageContainerName'
          value: storageContainerName
        }
      ]
      scopes: [
        'trafficcontrolservice'
      ]
    }
  }
}

output daprEntryCamName string = containerAppsEnvironment::daprComponent.name
