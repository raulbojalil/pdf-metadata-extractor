
using System;
using System.Xml.Serialization;

namespace PDFMetadataExtractor
{
    [Serializable]
    public class Metadata
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Value { get; set; }
    }

    [Serializable]
    public class MetadataCoordinate
    {
        [XmlAttribute]
        public string MetadataName { get; set; }

        [XmlAttribute]
        public int X { get; set; }

        [XmlAttribute]
        public int Y { get; set; }

        [XmlAttribute]
        public int Width { get; set; }

        [XmlAttribute]
        public int Height { get; set; }

        [XmlAttribute]
        public int Page { get; set; }

        [XmlIgnore()]
        public MetadataTag MetadataTag { get; set; }

        [XmlIgnore()]
        public TransparentPanel HoverPanel { get; set; }

        [XmlIgnore()]
        public string TestString { get; set; }

    }

    
}