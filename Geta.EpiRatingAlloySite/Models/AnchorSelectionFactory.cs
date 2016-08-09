using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer.Shell.ObjectEditing;

namespace Geta.EpiRatingAlloySite.Models
{
    public class AnchorSelectionFactory : ISelectionFactory
    {
        public IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
        {
            return new LinkItemSelection[]
            {
                new LinkItemSelection() { AnchorName = "Anchor1" },
                new LinkItemSelection() { AnchorName = "anchor2" }
            };
        }
    }

    public class LinkItemSelection: ISelectItem
    {
        public string AnchorName { get; set; }

        public object Value
        {
            get
            {
                return AnchorName;
            }
        }

        public string Text
        {
            get
            {
                return AnchorName;
            }
        }
    }

}