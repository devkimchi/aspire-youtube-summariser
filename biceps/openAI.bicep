@minLength(1)
@maxLength(64)
@description('Name of the environment that can be used as part of naming resource convention, the name of the resource group for your application will use this name, prefixed with rg-')
param environmentName string

var tags = {
  'azd-env-name': environmentName
}

var resourceToken = uniqueString(resourceGroup().id)
var location = 'eastus'

var openai = {
  name: 'aoai-${resourceToken}'
  location: location
  tags: tags
  skuName: 'S0'
  model: {
    name: 'gpt-4-32k'
    deploymentName: 'model-gpt432k'
    version: '0613'
    skuName: 'Standard'
    skuCapacity: 10
  }
}

resource aoai 'Microsoft.CognitiveServices/accounts@2023-05-01' = {
  name: openai.name
  location: openai.location
  kind: 'OpenAI'
  tags: openai.tags
  sku: {
    name: openai.skuName
  }
  properties: {
    customSubDomainName: openai.name
    publicNetworkAccess: 'Enabled'
  }
}

resource aoaiDeployment 'Microsoft.CognitiveServices/accounts/deployments@2023-05-01' = {
  name: openai.model.deploymentName
  parent: aoai
  sku: {
    name: openai.model.skuName
    capacity: openai.model.skuCapacity
  }
  properties: {
    model: {
      format: 'OpenAI'
      name: openai.model.name
      version: openai.model.version
    }
  }
}

output aoaiInstance object = {
  id: aoai.id
  name: aoai.name
  endpoint: aoai.properties.endpoint
  apiKey: listKeys(aoai.id, '2023-05-01').key1
  deploymentName: aoaiDeployment.name
}
