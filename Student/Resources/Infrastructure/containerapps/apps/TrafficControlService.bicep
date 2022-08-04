param location string

param containerAppsEnvironmentId string
param registry string
param registryUsername string
@secure()
param registryPassword string
param image string

resource containerApp 'Microsoft.App/containerApps@2022-03-01' = {
  name: 'trafficcontrolservice'
  location: location
  properties: {
    managedEnvironmentId: containerAppsEnvironmentId
    template: {
      containers: [
        {
          name: 'trafficcontrolservice'
          image: image
          env: [
            {
              name: 'ASPNETCORE_URLS'
              value: 'http://*:6000'
            }
          ]
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 1
      }
    }
    configuration: {
      activeRevisionsMode: 'single'
      dapr: {
        enabled: true
        appId: 'trafficcontrolservice'
        appPort: 6000
      }
      ingress: {
        external: false
        targetPort: 6000
        allowInsecure: true
      }
      secrets: [
        {
          name: 'container-registry-password'
          value: registryPassword
        }      
      ]
      registries: [
        {
          server:registry
          username:registryUsername
          passwordSecretRef: 'container-registry-password'
        }
      ]      
    }
  }
}
