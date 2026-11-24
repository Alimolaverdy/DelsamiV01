using System.Collections.Generic;
using SevenSpikes.Nop.Conditions.Domain;
using SevenSpikes.Nop.Framework.Domain.Enums;

namespace SevenSpikes.Nop.Plugins.HtmlWidgets.Infrastructure.Constants;

public class Plugin
{
    public const string Name = "Nop Html Widgets";

    public const string ResourceName = "SevenSpikes.Plugins.HtmlWidgets.Admin.Menu.MenuName";

    public const string SystemName = "SevenSpikes.Nop.Plugins.HtmlWidgets";

    public const string FolderName = "SevenSpikes.Nop.Plugins.HtmlWidgets";

    public const string UrlInStore = "http://www.nop-templates.com/html-widgets-plugin-for-nopcommerce";

    public const string HtmlWidgetsTableName = "SS_HW_HtmlWidget";

    public static EntityType EntityType = (EntityType)25;

    public static IDictionary<ConditionType, IList<object>> AvailableConditionTypes = new Dictionary<ConditionType, IList<object>>
    {
        {
            (ConditionType)4,
            new List<object>
            {
                (CustomerConditionProperty)0,
                (CustomerConditionProperty)2,
                (CustomerConditionProperty)4,
                (CustomerConditionProperty)3,
                (CustomerConditionProperty)5,
                (CustomerConditionProperty)1
            }
        },
        {
            (ConditionType)3,
            new List<object>
            {
                (ProductConditionProperty)1,
                (ProductConditionProperty)0,
                (ProductConditionProperty)4,
                (ProductConditionProperty)5,
                (ProductConditionProperty)11,
                (ProductConditionProperty)12,
                (ProductConditionProperty)2,
                (ProductConditionProperty)8,
                (ProductConditionProperty)10,
                (ProductConditionProperty)6,
                (ProductConditionProperty)7,
                (ProductConditionProperty)3,
                (ProductConditionProperty)9,
                (ProductConditionProperty)13,
                (ProductConditionProperty)14,
                (ProductConditionProperty)15,
                (ProductConditionProperty)16,
                (ProductConditionProperty)19
            }
        },
        {
            (ConditionType)1,
            new List<object>
            {
                (ManufacturerConditionProperty)0,
                (ManufacturerConditionProperty)1
            }
        },
        {
            (ConditionType)2,
            new List<object>
            {
                (CategoryConditionProperty)0,
                (CategoryConditionProperty)1
            }
        },
        {
            (ConditionType)5,
            new List<object>()
        },
        {
            (ConditionType)6,
            new List<object>
            {
                (CategoryConditionProperty)0,
                (CategoryConditionProperty)1
            }
        },
        {
            (ConditionType)7,
            new List<object>
            {
                (VendorConditionProperty)0,
                (VendorConditionProperty)1
            }
        }
    };
}
