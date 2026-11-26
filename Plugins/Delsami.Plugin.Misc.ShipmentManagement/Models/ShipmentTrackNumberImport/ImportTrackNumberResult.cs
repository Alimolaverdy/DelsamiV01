namespace Delsami.Plugin.Misc.ShipmentManagement.Models.ShipmentTrackNumberImport;

public enum ImportTrackNumberResult
{
    Success=1,
    TrackingNumberIsTheSame=2,
    TrackingNumberIsNotTheSame=3,
    ShipmentNotFound = 4,
    ShipmentIdIsNotValid = 5,

}
