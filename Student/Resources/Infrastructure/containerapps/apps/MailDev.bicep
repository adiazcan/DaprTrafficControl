param location string

param containerAppsEnvironmentId string

resource containerApp 'Microsoft.App/containerApps@2022-03-01' = {
  name: 'maildev'
  location: location
  properties: {
    managedEnvironmentId: containerAppsEnvironmentId
    template: {
      containers: [
        {
          name: 'maildev'
          image: 'maildev/maildev:latest'
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 1
      }
    }
    configuration: {
      activeRevisionsMode: 'single'
      ingress: {
        external: false
        targetPort: 4025
        allowInsecure: true
      }
    }
  }
}

output fqdn string = containerApp.properties.configuration.ingress.fqdn
