namespace Helios.Core.Domains.Elements.Interfaces
{
    public interface IHtmlOutput
    {
        string GetHtml(bool isOperation, int locked = 0, int freezed = 0, bool panelElement = false, bool firstElement = false);
        string GetHtml(bool isOperation, bool IsPreview, bool IsPdf = false);
    }
}
