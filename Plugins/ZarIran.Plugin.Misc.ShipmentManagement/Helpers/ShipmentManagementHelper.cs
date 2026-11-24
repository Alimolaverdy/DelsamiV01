using System;

namespace ZarIran.Plugin.Misc.ShipmentManagement.Helpers
{
    public class ShipmentManagementHelper
    {
        public static int GenerateDeliveryCode()
        {
            Random random = new Random();

            int randomNumber = random.Next(100000, 1000000);

            return randomNumber;
        }

        public static int GetShipmentIdFromShipmentCode(string shipmentCode)
        {
            try
            {
                shipmentCode = NormalizeHelper.NormalizeNumbers(shipmentCode);

                var shipmentId = int.Parse(shipmentCode.Substring(1).TrimStart(new[] { '0' }));

                return shipmentId;
            }
            catch (Exception ex)
            {
            }
            return 0;
        }
    }
}
