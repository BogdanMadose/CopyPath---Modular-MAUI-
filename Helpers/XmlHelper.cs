using CommunityToolkit.Mvvm.ComponentModel;
using System.Xml.Linq;

namespace CopyPath___Modular_MAUI_.Helpers
{
    public static class XmlHelper
    {
        private static readonly string XmlFilePath = "options.xml";

        public static List<FileTransferOptions> ReadOptions()
        {
            var optionsList = new List<FileTransferOptions>();
            if (!File.Exists(XmlFilePath))
                return optionsList;

            var doc = XDocument.Load(XmlFilePath);
            foreach (var element in doc.Root.Elements("Option"))
            {
                var options = new FileTransferOptions
                {
                    Name = element.Element("Name")?.Value,
                    Source = element.Element("Source")?.Value,
                    Destination = element.Element("Destination")?.Value
                };
                optionsList.Add(options);
            }
            return optionsList;
        }

        public static void WriteOptions(List<FileTransferOptions> optionsList)
        {
            var doc = new XDocument(
                new XElement("FileTransferOptions",
                    optionsList.Select(options =>
                        new XElement("Option",
                            new XElement("Name", options.Name),
                            new XElement("Source", options.Source),
                            new XElement("Destination", options.Destination)
                        )
                    )
                )
            );
            doc.Save(XmlFilePath);
        }
    }

    public partial class FileTransferOptions : ObservableObject
    {
        [ObservableProperty]
        private bool _isSelected;

        public string Name { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
    }
}
