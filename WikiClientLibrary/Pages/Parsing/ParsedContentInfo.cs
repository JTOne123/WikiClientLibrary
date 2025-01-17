using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WikiClientLibrary.Pages.Queries.Properties;
using WikiClientLibrary.Sites;

namespace WikiClientLibrary.Pages.Parsing
{
    /// <summary>
    /// Contains parsed content of specific page or wikitext.
    /// </summary>
    /// <remarks>Use <see cref="WikiSiteExtensions.ParsePageAsync(WikiSite,string)"/> or other related methods to get parsed content.</remarks>
    [JsonObject(MemberSerialization.OptIn)]
    public class ParsedContentInfo
    {
        /// <summary>
        /// The title of the page.
        /// </summary>
        [JsonProperty]
        public string Title { get; private set; }

        /// <summary>
        /// The displayed title HTML.
        /// </summary>
        /// <remarks>The actual displayed title can be reformatted by DISPLAYTITLE magic word, and language variant conversions may be applied.</remarks>
        [JsonProperty]
        public string DisplayTitle { get; private set; }

        [JsonProperty]
        public int PageId { get; private set; }

        [JsonProperty("revid")]
        public int RevisionId { get; private set; }

        /// <summary>
        /// Parsed content, in HTML form.
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// Parsed summary, if exists, in HTML form.
        /// </summary>
        public string Summary { get; private set; }

        [JsonProperty("text")]
        private JToken DummyText
        {
            set { Content = (string)value["*"]; }
        }

        [JsonProperty("parsedsummary")]
        private JToken DummySummary
        {
            set { Summary = (string)value["*"]; }
        }

        [JsonProperty("langlinks")]
        public IReadOnlyCollection<LanguageLinkInfo> LanguageLinks { get; private set; }

        [Obsolete("Use LanguageLinks instead of this property.")]
        public IReadOnlyCollection<LanguageLinkInfo> Interlanguages => LanguageLinks;

        [JsonProperty]
        public IReadOnlyCollection<ContentCategoryInfo> Categories { get; private set; }

        [JsonProperty]
        public IReadOnlyCollection<ContentSectionInfo> Sections { get; private set; }

        [JsonProperty]
        public IReadOnlyCollection<ContentPropertyInfo> Properties { get; private set; }

        /// <summary>
        /// Gets a list of templates transcluded in this page.
        /// Available if <see cref="ParsingOptions.TranscludedPages"/> is specified.
        /// </summary>
        [JsonProperty("templates")]
        public IReadOnlyCollection<ContentTransclusionInfo> TranscludedPages { get; private set; }

        /// <summary>
        /// Gets the limit reports generated by the parser.
        /// Available if <see cref="ParsingOptions.LimitReport"/> is specified.
        /// </summary>
        [JsonProperty("limitreportdata")]
        public IReadOnlyCollection<ParserLimitReport> ParserLimitReports { get; private set; }

        /// <summary>
        /// Determines the redirects that has been followed to reach the page.
        /// </summary>
        [JsonProperty]
        public IReadOnlyCollection<ContentRedirectInfo> Redirects { get; private set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class ContentRedirectInfo
    {
        [JsonProperty]
        public string From { get; private set; }

        [JsonProperty]
        public string To { get; private set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class ContentPropertyInfo
    {
        [JsonProperty]
        public string Name { get; private set; }

        [JsonProperty("*")]
        public string Value { get; private set; }

        /// <inheritdoc />
        public override string ToString()
            => Name + "=" + Value;
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class ContentSectionInfo
    {
        /// <summary>
        /// Index of the section.
        /// </summary>
        /// <remarks>
        /// This value is usually a number.
        /// For titles in transcluded templates, this property may have a value like "T-1", "T-2", etc.
        /// </remarks>
        [JsonProperty]
        public string Index { get; private set; }

        /// <summary>
        /// Heading text.
        /// </summary>
        [JsonProperty("line")]
        public string Heading { get; private set; }

        /// <summary>
        /// Anchor name of the heading.
        /// </summary>
        [JsonProperty]
        public string Anchor { get; private set; }

        /// <summary>
        /// Heading number. E.g. 3.2 .
        /// </summary>
        [JsonProperty]
        public string Number { get; private set; }

        /// <summary>
        /// Level of the heading.
        /// </summary>
        [JsonProperty]
        public int Level { get; private set; }

        /// <summary>
        /// Toc level of the heading. This is usually <see cref="Level"/> - 1.
        /// </summary>
        [JsonProperty]
        public int TocLevel { get; private set; }

        /// <summary>
        /// Title of the page.
        /// </summary>
        [JsonProperty("fromtitle")]
        public string PageTitle { get; private set; }

        /// <summary>
        /// Byte offset of the section.
        /// </summary>
        /// <remarks>
        /// Note that sometimes this property is not available,
        /// especially when the heading is included in a template.
        /// </remarks>
        public int? ByteOffset { get; private set; }

        /// <summary>
        /// 返回表示当前对象的字符串。
        /// </summary>
        /// <returns>
        /// 表示当前对象的字符串。
        /// </returns>
        public override string ToString()
        {
            return PageTitle + "#" + Heading;
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class ContentCategoryInfo
    {
        /// <summary>
        /// Title of the category.
        /// </summary>
        [JsonProperty("*")]
        public string CategoryName { get; private set; }

        [JsonProperty]
        public string SortKey { get; private set; }

        [JsonProperty]
        public bool IsHidden { get; private set; }

        /// <inheritdoc />
        public override string ToString()
            => string.IsNullOrEmpty(SortKey) ? CategoryName : (CategoryName + "|" + SortKey);
    }

    /// <summary>
    /// Represents a transcluded page/template/module in the parsed page.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ContentTransclusionInfo
    {
        /// <summary>
        /// Title of the transcluded page.
        /// </summary>
        [JsonProperty("*")]
        public string Title { get; private set; }

        /// <summary>
        /// Namespace id of the transcluded page.
        /// </summary>
        [JsonProperty("ns")]
        public int NamespaceId { get; private set; }

        /// <summary>
        /// Whether the transcluded page exists.
        /// </summary>
        [JsonProperty]
        public bool Exists { get; private set; }

        /// <inheritdoc />
        public override string ToString() => Title;
    }

    /// <summary>
    /// Represents a group in the limit report generated by parser.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ParserLimitReport
    {
        /// <summary>
        /// Limit report name.
        /// </summary>
        /// <remarks>E.g. smw-limitreport-intext-parsertime, limitreport-templateargumentsize .</remarks>
        [JsonProperty]
        public string Name { get; private set; }

        /// <summary>
        /// Current value of the report, if available.
        /// </summary>
        public double? Value { get; private set; }

        /// <summary>
        /// Value limit of the report, if available.
        /// </summary>
        public double? Limit { get; private set; }

        private static double? TryParseAsDouble(JToken token)
        {
            var s = (string)token;
            double v;
            if (double.TryParse(s, out v)) return v;
            return null;
        }

#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
        /// <summary>
        /// All the content of the report.
        /// </summary>
        [JsonExtensionData] private IDictionary<string, JToken> _Content;
#pragma warning restore CS0649

        public IReadOnlyDictionary<string, JToken> Content { get; private set; }

        private static readonly IReadOnlyDictionary<string, JToken> EmptyContent =
            new ReadOnlyDictionary<string, JToken>(new Dictionary<string, JToken>(0));

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Value = Limit = null;
            if (_Content != null)
            {
                JToken jt;
                if (_Content.TryGetValue("0", out jt))
                    Value = TryParseAsDouble(jt);
                if (_Content.TryGetValue("1", out jt))
                    Limit = TryParseAsDouble(jt);
                Content = new ReadOnlyDictionary<string, JToken>(_Content);
            }
            else
            {
                Content = EmptyContent;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var s = Name + ": " + Value;
            if (Limit != null) s += "/" + Limit;
            return s;
        }
    }
}
