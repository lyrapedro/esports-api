using System.Collections.Generic;
using HtmlAgilityPack;

namespace EsportsApi.Core.Helpers.Extensions;

public static class HtmlNodeExtensions
{
    /// <summary>
    /// Retorna todos os nós irmãos seguintes (next siblings) do nó atual
    /// </summary>
    public static IEnumerable<HtmlNode> NextSiblings(this HtmlNode node)
    {
        while (node.NextSibling != null)
        {
            node = node.NextSibling;
            yield return node;
        }
    }

    /// <summary>
    /// Retorna todos os nós irmãos seguintes (next siblings) do nó atual que são elementos (não text nodes)
    /// </summary>
    public static IEnumerable<HtmlNode> NextElementSiblings(this HtmlNode node)
    {
        return node.NextSiblings().Where(n => n.NodeType == HtmlNodeType.Element);
    }
}