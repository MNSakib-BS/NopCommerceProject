using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Arch.Core.Domains.Catalog;
public class CategoryAdditional:BaseEntity
{
    public int CategoryId { get; set; }
    public int BannerHeroPictureTopId { get; set; }
    public int BannerHeroPictureLowerId { get; set; }
    /// <summary>
    /// Gets or sets the category type
    /// </summary>
    public int CategoryTypeId { get; set; }

    public bool ArchManaged { get; set; }
    public bool DisableZoom { get; set; }

    /// <summary>
    /// Gets or sets the Category type
    /// </summary>
    public CategoryType CategoryType
    {
        get => (CategoryType)CategoryTypeId;
        set => CategoryTypeId = (int)value;
    }

    public int ExternalId { get; set; }
    public int ExternalParentId { get; set; }
    public int StoreTypeId { get; set; }
}
