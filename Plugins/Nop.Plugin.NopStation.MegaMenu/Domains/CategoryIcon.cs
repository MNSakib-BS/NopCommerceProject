using Nop.Core;

namespace Nop.Plugin.NopStation.MegaMenu.Domains
{
    public partial class CategoryIcon : BaseEntity
    {
        public int CategoryId { get; set; }

        public int PictureId { get; set; }
    }
}