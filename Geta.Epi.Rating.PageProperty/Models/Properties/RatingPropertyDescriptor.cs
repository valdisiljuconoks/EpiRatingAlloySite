using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace Geta.Epi.Rating.PageProperty.Models.Properties
{
    [EditorDescriptorRegistration(
      TargetType = typeof(string),
      UIHint = "RatingProperty")]
    public class RatingPropertyDescriptor : EditorDescriptor
    {
        public RatingPropertyDescriptor()
        {
            ClientEditingClass = "ratingModule.RatingProperty";
        }
    }
}