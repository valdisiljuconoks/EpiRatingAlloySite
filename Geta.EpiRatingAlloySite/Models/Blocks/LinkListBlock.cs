using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EPiServer;
using EPiServer.Cms.Shell.UI.ObjectEditing.EditorDescriptors;
using EPiServer.DataAbstraction;
using EPiServer.Shell.ObjectEditing;
using Geta.EpiRatingAlloySite.Models.ViewModels;

namespace Geta.EpiRatingAlloySite.Models.Blocks
{
    [SiteContentType(
        GUID = "8E05D62E-52F9-4993-BD1D-42C167D299FA",
        GroupName = SystemTabNames.Content)]
    public class LinkListBlock: SiteBlockData
    {
        
        //[EditorDescriptor(EditorDescriptorType = typeof(CollectionEditorDescriptor<LinkModel>))]
        //public virtual IList<LinkModel> LinkList { get; set; }

        public virtual  string Title { get; set; }

        public virtual Url Link { get; set; }

        [SelectOne(SelectionFactoryType = typeof(AnchorSelectionFactory))]
        public virtual  string Anchor { get; set; }

    }
}