using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace Geta.EpiRatingAlloySite.PropertyFiles
{
    [EditorDescriptorRegistration(
       TargetType = typeof(string),
       UIHint = "RatingProperty")]
    public class RatingPropertyDescriptor: EditorDescriptor
    {
        public RatingPropertyDescriptor()
        {
            ClientEditingClass = "ratingModule.RatingProperty";
        }
    }
}