//using ArchServiceReference;
//using Newtonsoft.Json.Linq;
//using Nop.Core.Domain.Catalog;
//using Nop.Core.Domain.Media;
//using Nop.Core;
//using Nop.Data;
//using Nop.Services.Catalog;
//using Nop.Services.Configuration;
//using Nop.Services.Helpers;
//using Nop.Services.Media;
//using Nop.Services.Seo;
//using Nop.Services.Stores;
//using Nop.Services.Tasks;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Net.Http.Headers;
//using System.Net.Http;
//using System.Net;
//using System.Text;
//using Microsoft.Extensions.Logging;
//using ILogger = Nop.Services.Logging.ILogger;
//using System.Linq;
//using Arch.Core.Models.MasterData;
//using Newtonsoft.Json;

//namespace Arch.Core.Infrastructure.JobSchedules.Jobs
//{
//    public class ArchMasterDataJob : ArchScheduledJobBase<Asset>
//    {
//        protected override Type TaskType => typeof(ArchMasterDataJob);

//        #region Fields
//        private readonly IProductService _productService;
//        private readonly IPictureService _pictureService;
//        private readonly IScheduleTaskService _scheduleTaskService;
//        private readonly IArchStoreProductInfoService _archStoreProductInfoService;
//        private readonly ICategoryService _categoryService;
//        private readonly IRepository<ProductPicture> _productPictureRepository;
//        private readonly IRepository<PictureBinary> _pictureBinaryRepository;
//        private readonly IRepository<ArchProductDetail> _productDetailRepository;
//        private readonly IRepository<ArchStoreProductInfo> _productStoreProductInfoRepository;

//        private const string LastUpdateSettingParam = "ArchMasterDataTask_LastUpdate";
//        #endregion

//        #region Ctor
//        public ArchMasterDataJob(ECommerceBOClient archClient,
//            ISettingService settingService,
//            IProductService productService,
//            IPictureService pictureService,
//            IStoreContext storeContext,
//            IStoreService storeService,
//            IStoreMappingService storeMappingService,
//            IScheduleTaskService scheduleTaskService,
//            IArchStoreProductInfoService archStoreProductInfoService,
//            IUrlRecordService urlRecordService,
//            ICategoryService categoryService,
//            IRepository<ProductPicture> productPictureRepository,
//            IRepository<PictureBinary> pictureBinaryRepository,
//            IRepository<ArchProductDetail> productDetailRepository,
//            IRepository<ArchStoreProductInfo> productStoreProductInfoRepository,
//            ILogger logger,
//            ILogger<ScheduledJobBase<object>> jobLogger,
//            IObjectConverter objectConverter)
//            : base(settingService,
//                archClient,
//                storeService,
//                storeContext,
//                storeMappingService,
//                urlRecordService,
//                logger,
//                objectConverter,
//                jobLogger)
//        {
//            _productService = productService;
//            _pictureService = pictureService;
//            _productPictureRepository = productPictureRepository;
//            _pictureBinaryRepository = pictureBinaryRepository;
//            _scheduleTaskService = scheduleTaskService;
//            _archStoreProductInfoService = archStoreProductInfoService;
//            _categoryService = categoryService;
//            _productDetailRepository = productDetailRepository;
//            _productStoreProductInfoRepository = productStoreProductInfoRepository;
//        }
//        #endregion

//        #region Methods
//        protected override void Produce()
//        {
//            var stores = _storeService.GetAllStores();
//            var globalStore = stores.FirstOrDefault(p => p.IsGlobalStore);
//            if (globalStore == null || RunningOnStoreId != globalStore.Id)
//                return;

//            var lastUpdate = GetLastUpdate(LastUpdateSettingParam);

//            Debug($"Fetching pictures for products since {lastUpdate}");
//            FetchImageData(lastUpdate);
//        }


//        private void FetchImageData(DateTime lastUpdate)
//        {
//            var result = CallWebAPI(lastUpdate);
//            if (result != null)
//            {

//                string errorMessage = result.errorMessage;
//                if (!string.IsNullOrEmpty(errorMessage))
//                {
//                    Error(errorMessage, new Exception());
//                }

//                if (result.assets.Any())
//                {

//                    Debug($"Processing {result.assets.Count} items");
//                    foreach (var picture in result.assets)
//                    {
//                        EnqueueItem(picture);
//                    }
//                }
//            }
//        }

//        private void ProcessAsset(Asset picture)
//        {

//            string productCode = picture.barcode;
//            if (!string.IsNullOrEmpty(productCode))
//            {
//                string articleDescription = string.Empty;
//                Debug($"Loading Product: {productCode}");
//                var product = _productService.GetProductBySku(productCode, true);
//                if (product != null)
//                {
//                    UpdateProductMetaData(picture, product);
//                }
//            }
//        }

//        private void UpdateProductMetaData(Asset picture, Product product)
//        {
//            Debug($"Processing product: {product.Name}");
//            string awards = picture.awards;
//            if (!string.IsNullOrEmpty(awards))
//            {
//                product.Awards = awards;
//            }

//            string origin = picture.origin;
//            if (!string.IsNullOrEmpty(origin))
//            {
//                product.Origin = origin;
//            }

//            string abv = picture.abv;
//            if (!string.IsNullOrEmpty(abv))
//            {
//                product.AlcoholByVolume = decimal.Parse(abv);
//            }

//            if (!string.IsNullOrEmpty(picture.tastingNotes))
//                product.FullDescription = picture.tastingNotes;

//            //Picture Upsert
//            byte[] imageBytes = null;
//            try
//            {
//                using (var webClient = new WebClient())
//                {
//                    imageBytes = webClient.DownloadData(picture.downloadURL);
//                }
//            }
//            catch (Exception ex)
//            {
//                this.Log($"Unable download image for {product.ProductCodeField} - {product.Name} {picture.downloadURL}", ex, LogLevel.Error);
//            }

//            string filename = picture.filename;
//            string mimeType = picture.mimeType;

//            if (string.IsNullOrEmpty(mimeType))
//            {
//                mimeType ??= MimeTypes.ImageJpeg;
//            }


//            if (imageBytes != null && imageBytes.Length > 0 && !string.IsNullOrEmpty(filename) && !string.IsNullOrEmpty(product.ProductCodeField) && mimeType != "application/octet-stream")
//            {
//                try
//                {
//                    using (MemoryStream memoryStream = new MemoryStream())
//                    {
//                        memoryStream.Write(imageBytes, 0, imageBytes.Length);

//                        var newPicture = _pictureService.InsertPicture(imageBytes, mimeType, _pictureService.GetPictureSeName(product.Name));
//                        if (newPicture != null)
//                        {
//                            _pictureService.SetSeoFilename(newPicture.Id, _pictureService.GetPictureSeName(product.Name));
//                            _productService.InsertProductPicture(new ProductPicture
//                            {
//                                PictureId = newPicture.Id,
//                                ProductId = product.Id,
//                                DisplayOrder = 0
//                            });

//                            var pictures = _pictureService.GetPicturesByProductId(product.Id);
//                            foreach (var productPicture in pictures.Where(x => x.Id != newPicture.Id))
//                            {
//                                if (productPicture.SeoFilename == newPicture.SeoFilename)
//                                {
//                                    var productPictureRecord = (from item in _productPictureRepository.Table where productPicture.Id == item.PictureId select item).FirstOrDefault();
//                                    var pictureBinaryRecord = (from item in _pictureBinaryRepository.Table where item.PictureId == productPicture.Id select item).FirstOrDefault();

//                                    if (productPictureRecord != null)
//                                    {
//                                        _productPictureRepository.Delete(productPictureRecord);
//                                    }

//                                    _pictureService.DeletePicture(productPicture);

//                                    if (pictureBinaryRecord != null)
//                                    {
//                                        _pictureBinaryRepository.Delete(pictureBinaryRecord);
//                                    }

//                                    break;
//                                }
//                            }
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    this.Log($"Unable to save image for {product.ProductCodeField} - {product.Name} - Mimetype: {mimeType}", ex, LogLevel.Error);
//                }
//            }
//            _productService.UpdateProduct(product);
//        }

//        private MasterDataResponseModel CallWebAPI(DateTime lastUpdate)
//        {
//            const string methodURL = "api/ArcheStoreMasterData/GetAssetsChangedSince";
//            string errorMessage = string.Empty;
//            var result = new MasterDataResponseModel();
//            try
//            {
//                //Required to upload records to Azure in some environments
//                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

//                using (HttpClient client = new HttpClient())
//                {
//                    if (string.IsNullOrEmpty(_archSettings.ApiArchMasterDataAddress))
//                        return result;

//                    client.BaseAddress = new Uri(_archSettings.ApiArchMasterDataAddress);
//                    client.DefaultRequestHeaders.Accept.Clear();
//                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
//                    client.Timeout = new TimeSpan(0, 30, 0);

//                    var response = client.GetAsync(methodURL + "?changedDate=" + lastUpdate.ToString("yyyy-MM-ddTHH:mm:ss")).Result;
//                    if (response != null)
//                    {
//                        if (response.IsSuccessStatusCode)
//                        {
//                            var responseContent = response.Content.ReadAsStringAsync().Result;

//                            if (!string.IsNullOrEmpty(responseContent))
//                            {
//                                result = JsonConvert.DeserializeObject<MasterDataResponseModel>(responseContent);
//                            }
//                        }
//                        else { errorMessage = string.Format("Failed to get image data: {0}", errorMessage); }
//                    }
//                    else { errorMessage = "Failed to upload document: No response from service"; }
//                }

//                return result;
//            }
//            catch (Exception ex)
//            {
//                errorMessage = ex.Message;
//                throw new Exception(errorMessage);
//            }
//        }

//        private int FetchOrderMaximumQuantity(Product product)
//        {
//            var maxQuantity = _archSettings.OrderMaximumQuantityDefault;

//            var sharedBaseCodeProducts = _productService.GetProductsByCodes(product.BaseCodeField).ToList();
//            sharedBaseCodeProducts.Add(product); // include the new product being processed in the collection

//            foreach (var currentProduct in sharedBaseCodeProducts)
//            {
//                // fetch the product that is the next pack size up from the current item
//                var nextPackSizeUpProduct = sharedBaseCodeProducts
//                    .Where(nextProduct => nextProduct.PackSizeField.HasValue &&
//                                          currentProduct.PackSizeField.HasValue &&
//                                          currentProduct.PackSizeField < nextProduct.PackSizeField.Value)
//                    .OrderBy(i => i.PackSizeField)
//                    .FirstOrDefault();

//                if (nextPackSizeUpProduct != null && currentProduct.PackSizeField == 1)
//                {
//                    // if the product has a unit pack size and there is another product with the same base code with a larger pack size
//                    // then ensure that the unit pack size is limited to 1 less than larger pack size.
//                    currentProduct.OrderMaximumQuantity = (int)nextPackSizeUpProduct.PackSizeField.Value - 1;

//                    var isNewProduct = product.Id == currentProduct.Id;
//                    if (isNewProduct)
//                    {
//                        // update the max quantity to return as the product has not been created yet
//                        maxQuantity = currentProduct.OrderMaximumQuantity;
//                    }
//                    else
//                    {
//                        // update existing product
//                        _productService.UpdateProduct(currentProduct);
//                    }
//                }
//            }

//            return maxQuantity;
//        }

//        protected override void CollectData() { }

//        protected override void Consume(Asset item)
//        {
//            ProcessAsset(item);
//        }

//        protected override void OnCompleting()
//        {
//            base.OnCompleting();
//            Debug($"Completed producing");
//            SetLastUpdate(LastUpdateSettingParam);
//        }
//        #endregion
//    }
//}
