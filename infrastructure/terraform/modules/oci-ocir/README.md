# Module oci-ocir

Crée les repositories OCI Container Registry (OCIR) pour les 13 microservices SmartDiscount.

## Usage

```hcl
module "ocir" {
  source         = "../../modules/oci-ocir"
  compartment_id = var.compartment_id
  environment    = "test"
}
```

## Repositories créés

Pour chaque microservice, un repo du type :

