using Nop.Core.Configuration;

namespace Nop.Plugin.NopStation.Theme.Arch;
public class ArchThemeSettings : ISettings
{
    public string CustomThemeColor { get; set; }
    public string AccentThemeColor { get; set; }

    public bool EnableImageLazyLoad { get; set; }

    public int LazyLoadPictureId { get; set; }

    public bool ShowSupportedCardsPictureAtPageFooter { get; set; }

    public int SupportedCardsPictureId { get; set; }

    public bool ShowLoginPictureAtLoginPage { get; set; }

    public int LoginPictureId { get; set; }

    public bool ShowLogoAtPageFooter { get; set; }

    public int FooterLogoPictureId { get; set; }

    public string FooterEmail { get; set; }

    public bool EnableDescriptionBoxOne { get; set; }

    public string DescriptionBoxOneTitle { get; set; }

    public int DescriptionBoxOnePictureId { get; set; }

    public string DescriptionBoxOneText { get; set; }

    public string DescriptionBoxOneUrl { get; set; }

    public bool EnableDescriptionBoxTwo { get; set; }

    public string DescriptionBoxTwoTitle { get; set; }

    public int DescriptionBoxTwoPictureId { get; set; }

    public string DescriptionBoxTwoText { get; set; }

    public string DescriptionBoxTwoUrl { get; set; }

    public bool EnableDescriptionBoxThree { get; set; }

    public string DescriptionBoxThreeTitle { get; set; }

    public int DescriptionBoxThreePictureId { get; set; }

    public string DescriptionBoxThreeText { get; set; }

    public string DescriptionBoxThreeUrl { get; set; }

    public bool EnableDescriptionBoxFour { get; set; }

    public string DescriptionBoxFourTitle { get; set; }

    public int DescriptionBoxFourPictureId { get; set; }

    public string DescriptionBoxFourText { get; set; }

    public string DescriptionBoxFourUrl { get; set; }

    public string CustomCss { get; set; }
}
