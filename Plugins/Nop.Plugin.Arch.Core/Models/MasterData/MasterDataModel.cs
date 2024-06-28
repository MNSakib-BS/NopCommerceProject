namespace Nop.Plugin.Arch.Core.Models.MasterData
{
    #region "MasterDataResponseModel"
    public class MasterDataResponseModel
    {
        public List<Asset> Assets { get; set; }
        public string? ErrorMessage { get; set; }
        public MasterDataResponseModel()
        {
            Assets = new List<Asset>();
        }
    }

    #endregion

    #region "Asset"

    public class Asset
    {
        public string? AssetsID { get; set; }
        public string? Filename { get; set; }
        public string? DownloadURL { get; set; }
        public string? MimeType { get; set; }
        public string? Barcode { get; set; }
        public string? Brand { get; set; }
        public string? Category { get; set; }
        public string? ArticleCode { get; set; }
        public string? ArticleDescription { get; set; }
        public string? ArticleVariantCode { get; set; }
        public string? ArticleVariantDescription { get; set; }
        public string? UnitOfMeasure { get; set; }
        public string? PackSize { get; set; }
        public string? PackDescription { get; set; }
        public string? PackClassification { get; set; }
        public string? Angle { get; set; }
        public string? Origin { get; set; }
        public string? Abv { get; set; }
        public string? Awards { get; set; }
        public string? TastingNotes { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    #endregion

}
