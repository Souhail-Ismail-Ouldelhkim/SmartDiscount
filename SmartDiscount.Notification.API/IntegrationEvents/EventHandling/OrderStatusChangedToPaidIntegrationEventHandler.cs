using SmartDiscount.EventBus.Abstractions;
using SmartDiscount.Notification.API.IntegrationEvents.Events;
using SmartDiscount.Notification.API.Services;

namespace SmartDiscount.Notification.API.IntegrationEvents.EventHandling;

public class OrderStatusChangedToPaidIntegrationEventHandler(
    IdentityClient identityClient,
    OrderingClient orderingClient,
    IEmailSender emailSender,
    ILogger<OrderStatusChangedToPaidIntegrationEventHandler> logger)
    : IIntegrationEventHandler<OrderStatusChangedToPaidIntegrationEvent>
{
    public async Task Handle(OrderStatusChangedToPaidIntegrationEvent @event)
    {
        logger.LogInformation("Commande #{OrderId} payee par {BuyerName}", @event.OrderId, @event.BuyerName);

        var user = await identityClient.GetUserByIdAsync(@event.BuyerIdentityGuid);

        if (user == null || string.IsNullOrEmpty(user.Email))
        {
            logger.LogWarning("Email introuvable pour {Guid}", @event.BuyerIdentityGuid);
            return;
        }
        var subject1 = $"Confirmation de votre commande #{@event.OrderId}";
        var html1 = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto;'>
                <h2 style='color: #1a2b4a;'>Merci pour votre commande !</h2>
                <p>Bonjour <strong>{user.Name} {user.LastName}</strong>,</p>
                <p>Votre commande <strong>#{@event.OrderId}</strong> a bien été payée.</p>
                <p style='color: #666;'>Cordialement,<br/>L'équipe SmartDiscount</p>
            </div>";

        try
        {
            await emailSender.SendEmailAsync(user.Email, subject1, html1);
            logger.LogInformation("EMAIL 1 (confirmation) envoye a {Email}", user.Email);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Echec email 1 pour commande #{OrderId}", @event.OrderId);
        }
        var order = await orderingClient.GetOrderByIdAsync(@event.OrderId);

        if (order == null)
        {
            logger.LogWarning("Details commande #{OrderId} introuvables, pas de facture", @event.OrderId);
            return;
        }

        var itemsRows = "";
        foreach (var item in order.OrderItems)
        {
            var lineTotal = item.UnitPrice * item.Units;
            itemsRows += $@"
                <tr>
                    <td style='padding: 8px; border-bottom: 1px solid #eee;'>{item.ProductName}</td>
                    <td style='padding: 8px; border-bottom: 1px solid #eee; text-align: center;'>{item.Units}</td>
                    <td style='padding: 8px; border-bottom: 1px solid #eee; text-align: right;'>{item.UnitPrice:F2} $</td>
                    <td style='padding: 8px; border-bottom: 1px solid #eee; text-align: right;'>{lineTotal:F2} $</td>
                </tr>";
        }

        var subject2 = $"Votre facture - commande #{@event.OrderId}";
        var html2 = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto;'>
                <h2 style='color: #1a2b4a;'>Votre facture</h2>
                <p>Commande <strong>#{order.OrderNumber}</strong></p>

                <h3 style='color: #1a2b4a;'>Adresse de livraison</h3>
                <p>
                    {order.Street}<br/>
                    {order.City}, {order.State} {order.Zipcode}<br/>
                    {order.Country}
                </p>

                <h3 style='color: #1a2b4a;'>Détail des produits</h3>
                <table style='width: 100%; border-collapse: collapse;'>
                    <thead>
                        <tr style='background: #1a2b4a; color: white;'>
                            <th style='padding: 8px; text-align: left;'>Produit</th>
                            <th style='padding: 8px; text-align: center;'>Qté</th>
                            <th style='padding: 8px; text-align: right;'>Prix unit.</th>
                            <th style='padding: 8px; text-align: right;'>Total</th>
                        </tr>
                    </thead>
                    <tbody>
                        {itemsRows}
                    </tbody>
                </table>

                <h3 style='text-align: right; color: #1a2b4a; margin-top: 20px;'>
                    Total payé : {order.Total:F2} $
                </h3>

                <p style='color: #666;'>Merci de votre confiance,<br/>L'équipe SmartDiscount</p>
            </div>";

        try
        {
            await emailSender.SendEmailAsync(user.Email, subject2, html2);
            logger.LogInformation("EMAIL 2 (facture) envoye a {Email} pour commande #{OrderId}", user.Email, @event.OrderId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Echec email 2 (facture) pour commande #{OrderId}", @event.OrderId);
        }
    }
}