param containerAppsEnvironmentName string

param host string
param port string
param smtpuser string
@secure()
param smtppassword string

resource containerAppsEnvironment 'Microsoft.App/managedEnvironments@2022-03-01' existing = {
  name: containerAppsEnvironmentName

  resource daprComponent 'daprComponents@2022-03-01' = {
    name: 'sendmail'
    properties: {
      componentType: 'bindings.smtp'
      version: 'v1'
      metadata: [
        {
          name: 'host'
          value: host
        }
        {
          name: 'port'
          value: port
        }        
        {
          name: 'user'
          value: smtpuser
        }        
        {
          name: 'password'
          value: smtppassword //llevar a secret
        }      
      ]
      scopes: [
        'finecollectionservice'
      ]
    }
  }
}

output daprEmailName string = containerAppsEnvironment::daprComponent.name
