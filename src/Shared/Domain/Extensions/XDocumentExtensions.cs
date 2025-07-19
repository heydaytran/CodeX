namespace Domain.Extensions;

public static partial class XDocumentExtensions
{
    private static readonly char XPathSeparator = '/';

    public static ErrorOr<XDocument> TryToXDocument(this string xml)
    {
        try
        {
            return XDocument.Parse(xml);
        }
        catch (Exception e)
        {
            return Error.Validation(code: "InvalidXml", $"Invalid xml. {e.Message}.", metadata: new Dictionary<string, object> { { "Exception", e } });
        }
    }

    public static string ToString(this XDocument doc, bool includeDeclaration = false)
    {
        StringBuilder sb = new();

        if (includeDeclaration && doc.Declaration is not null)
        {
            sb.Append(doc.Declaration.ToString());
            sb.AppendLine();
        }

        sb.Append(doc.ToString());

        return sb.ToString();
    }

    public static XAttribute? CreateAndSetXPathAttribute(this XDocument doc, string xpath, string value)
    {
        var elementsRegex = ElementsRegex();
        var attributeRegex = AttributesRegex();

        var elementMatches = elementsRegex.Matches(xpath);
        if (elementMatches.Count == 0)
        {
            return null;
        }

        // Create the element path.
        var childElement = doc
            .GetOrCreateElementPath(elementMatches.Select(m => m.Value));

        if (childElement is null)
        {
            return null;
        }

        var attributeMatch = attributeRegex.Match(xpath);
        if (!attributeMatch.Success)
        {
            return null;
        }

        var attributeName = attributeMatch.Groups[1].Value;

        var attribute = childElement.Attribute(attributeName);
        if (attribute is not null)
        {
            attribute.Value = value;
        }
        else
        {
            attribute = new XAttribute(attributeName, value);

            childElement.Add(attribute);
        }

        return attribute;
    }

    public static void AddElements(this XDocument doc, string xpath, IEnumerable<XElement> elements)
    {
        var xpathParts = xpath.Split(XPathSeparator, StringSplitOptions.RemoveEmptyEntries);

        if (xpathParts.Length == 0)
        {
            throw new ArgumentException($"Invalid XPath provided '{xpath}'.");
        }

        var lastElementName = xpathParts.Last();

        var parentXPath = string.Join(XPathSeparator.ToString(), xpathParts.Take(xpathParts.Length - 1));
        var parentElements = doc.XPathSelectElements(parentXPath).ToList();

        if (parentElements.Count == 0)
        {
            var newParentElement = new XElement(lastElementName);
            doc.Root?.Add(newParentElement);
            parentElements.Add(newParentElement);
        }

        foreach (var element in elements)
        {
            parentElements.Last().Add(element);
        }
    }

    public static XElement? GetOrCreateElementPath(this XDocument doc, IEnumerable<string> elements)
    {
        if (!elements.Any())
        {
            return null;
        }

        XElement? currentElement = doc.Root;
        if (currentElement is null)
        {
            currentElement = new XElement(elements.First());
            doc.Add(currentElement);
        }

        foreach (var element in elements.Skip(1))
        {
            var existingElement = currentElement.Elements(element).FirstOrDefault();
            if (existingElement is not null)
            {
                currentElement = existingElement;
                continue;
            }

            existingElement = new XElement(element);
            currentElement.Add(existingElement);

            currentElement = existingElement;
        }

        return currentElement;
    }

    [GeneratedRegex(@"(?<=/)([^/@]+)")]
    private static partial Regex ElementsRegex();

    [GeneratedRegex(@"string\([^)]*@(\w+)\)")]
    private static partial Regex AttributesRegex();
}
