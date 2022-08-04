param location string
param containerAppsEnvironmentId string
param registry string
param registryUsername string
@secure()
param registryPassword string
@secure()
param licensekey string
param image string

resource containerApp 'Microsoft.App/containerApps@2022-03-01' = {
  name: 'finecollectionservice'
  location: location
  properties: {
    managedEnvironmentId: containerAppsEnvironmentId
    template: {
      containers: [
        {
          name: 'finecollectionservice'
          image: image
          env: [
            {
              name: 'ASPNETCORE_URLS'
              value: 'http://*:6001'
            }
            {
              name: 'finecalculator.licensekey'
              secretRef: 'finecalculator-licensekey'
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
        appId: 'finecollectionservice'
        appPort: 6001
      }
      ingress: {
        external: false
        targetPort: 6001
        allowInsecure: true
      }
      secrets: [
        {
          name: 'container-registry-password'
          value: registryPassword
        }
        {
          name: 'finecalculator-licensekey'
          value: licensekey
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
