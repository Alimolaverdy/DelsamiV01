using System;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using Nop.Core.Infrastructure;
using SevenSpikes.Nop.Plugins.AjaxFilters.Helpers;

public class WebConfigMangerHelper : IWebConfigMangerHelper
{
    private const string JsonDeserializerMembersKey = "aspnet:MaxJsonDeserializerMembers";

    private readonly INopFileProvider _fileProvider;

    public WebConfigMangerHelper(INopFileProvider fileProvider)
    {
        _fileProvider = fileProvider;
    }

    public void AddMaxJsonSetting()
    {
        var webConfigPath = _fileProvider.MapPath("~/Web.config");
        var document = XDocument.Load(webConfigPath);
        var hasChanged = false;

        document.Changed += (_, _) => hasChanged = true;

        var appSettings = document.XPathSelectElement("configuration/appSettings");
        var existingSetting = document.XPathSelectElement(
            $"configuration/appSettings/add[@key='{JsonDeserializerMembersKey}']");

        if (appSettings == null || existingSetting != null)
            return;

        appSettings.Add(new XElement(
            "add",
            new XAttribute("key", JsonDeserializerMembersKey),
            new XAttribute("value", "150000")));

        if (!hasChanged)
            return;

        try
        {
            document.Save(webConfigPath);
        }
        catch (IOException)
        {
            // Updating Web.config is best-effort; the host may keep this file read-only.
        }
        catch (UnauthorizedAccessException)
        {
            // Updating Web.config is best-effort; the application identity may lack write access.
        }
    }
}
