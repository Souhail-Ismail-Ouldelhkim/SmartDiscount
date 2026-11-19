@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param sku string = 'Standard'

resource eventbus 'Microsoft.ServiceBus/namespaces@2024-01-01' = {
  name: take('eventbus-${uniqueString(resourceGroup().id)}', 50)
  location: location
  properties: {
    disableLocalAuth: true
    publicNetworkAccess: 'Enabled'
  }
  sku: {
    name: sku
  }
  tags: {
    'aspire-resource-name': 'eventbus'
  }
}

resource smartdiscount_event_bus 'Microsoft.ServiceBus/namespaces/topics@2024-01-01' = {
  name: 'smartdiscount-event-bus'
  parent: eventbus
}

resource Basket 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2024-01-01' = {
  name: 'Basket'
  parent: smartdiscount_event_bus
}

resource Catalog 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2024-01-01' = {
  name: 'Catalog'
  parent: smartdiscount_event_bus
}

resource Notification 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2024-01-01' = {
  name: 'Notification'
  parent: smartdiscount_event_bus
}

resource Ordering 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2024-01-01' = {
  name: 'Ordering'
  parent: smartdiscount_event_bus
}

resource OrderProcessor 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2024-01-01' = {
  name: 'OrderProcessor'
  parent: smartdiscount_event_bus
}

resource PaymentProcessor 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2024-01-01' = {
  name: 'PaymentProcessor'
  parent: smartdiscount_event_bus
}

resource Ordering_webapp 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2024-01-01' = {
  name: 'Ordering-webapp'
  parent: smartdiscount_event_bus
}

resource Webhooks 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2024-01-01' = {
  name: 'Webhooks'
  parent: smartdiscount_event_bus
}

output serviceBusEndpoint string = eventbus.properties.serviceBusEndpoint

output serviceBusHostName string = split(replace(eventbus.properties.serviceBusEndpoint, 'https://', ''), ':')[0]

output name string = eventbus.name

output id string = eventbus.id