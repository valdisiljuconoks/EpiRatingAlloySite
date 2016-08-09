using EPiServer.Core;
using EPiServer.Framework.Serialization;
using EPiServer.PlugIn;
using EPiServer.ServiceLocation;
using Geta.EpiRatingAlloySite.Models.ViewModels;

namespace Geta.EpiRatingAlloySite.Models
{
    [PropertyDefinitionTypePlugIn]
    public class LinkListProperty : PropertyList<LinkModel>
    {
        private readonly IObjectSerializer _objectSerializer;
        private Injected<ObjectSerializerFactory> _objectSerializerFactory;

        public LinkListProperty()
        {
            _objectSerializer = _objectSerializerFactory.Service.GetSerializer("application/json");
        }

        public override PropertyData ParseToObject(string value)
        {
            ParseToSelf(value);
            return this;
        }

        protected override LinkModel ParseItem(string value)
        {
            return _objectSerializer.Deserialize<LinkModel>(value);
        }
    }
}
