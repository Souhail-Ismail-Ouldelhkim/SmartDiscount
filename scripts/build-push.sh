#!/usr/bin/env bash
set -euo pipefail

NS="${NS:?Definis NS (namespace OCI)}"
ENVIRONMENT="${ENVIRONMENT:-test}"
TAG="${TAG:-v1}"
REG="af-casablanca-1.ocir.io/${NS}/smartdiscount/${ENVIRONMENT}"
SRC="$(cd "$(dirname "$0")/../src" && pwd)"

# nom-du-repo-OCIR : dossier-du-projet
SERVICES=(
  "identity-api:SmartDiscount.Identity.API"
  "catalog-api:SmartDiscount.Catalog.API"
  "ordering-api:SmartDiscount.Ordering.API"
  "webhooks-api:SmartDiscount.Webhooks.API"
  "notification-api:SmartDiscount.Notification.API"
  "discount-api:SmartDiscount.Discount.API"
  "wishlist-api:SmartDiscount.Wishlist.API"
  "order-processor:SmartDiscount.OrderProcessor"
  "payment-processor:SmartDiscount.PaymentProcessor"
  "webapp:SmartDiscount.WebApp"
  "webhooksclient:SmartDiscount.WebhookClient"
)

cd "$SRC"
for entry in "${SERVICES[@]}"; do
  repo="${entry%%:*}"
  project="${entry##*:}"
  echo "=== $repo ($project) ==="
  docker build --provenance=false --sbom=false --build-arg SERVICE="$project" -t "$REG/$repo:$TAG" .
  docker push "$REG/$repo:$TAG"
done
echo "Terminé : toutes les images sont dans $REG"