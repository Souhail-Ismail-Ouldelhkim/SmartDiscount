# Module oci-k3s

Crée 1 VM OCI ARM (Free Tier) avec K3s installé automatiquement.

## Composants

- 1 VM ARM (4 OCPU, 24 GB RAM) - gratuit permanent
- K3s installé via cloud-init
- Helm installé
- Kubeconfig configuré pour l'IP publique

## Usage

```hcl
module "k3s" {
  source              = "../../modules/oci-k3s"
  compartment_id      = var.compartment_id
  environment         = "test"
  availability_domain = var.availability_domain
  subnet_id           = module.network.subnet_public_id
  ssh_public_key      = file("~/.ssh/id_rsa.pub")
  vm_image_id         = "ocid1.image..."
}
```
