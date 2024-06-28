using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using Nop.Core;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Helpers;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Text;
using ILogger = Nop.Services.Logging.ILogger;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

using Nop.Arch.Services.Helpers;

namespace Nop.Arch.Infrastructure.JobSchedules.Jobs
{
    public partial class ProductImageUpdateJob : ScheduledJobBase<Product>
    {
        protected override Type TaskType => typeof(ProductImageUpdateJob);

        #region Fields

        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IPictureService _pictureService;
        private readonly INopFileProvider _fileProvider;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IRepository<ProductPicture> _productPictureRepository;
        private readonly IRepository<PictureBinary> _pictureBinaryRepository;
        private readonly Dictionary<string, string> _productCodeFileNamePaths;
        private readonly Dictionary<string, string> _multipleProductCodes;
        private readonly Regex _productCodeRegex;
        private readonly Regex _filePathRegex;

        #endregion

        #region Ctor

        public ProductImageUpdateJob(ISettingService settingService,
            ICategoryService categoryService,
            IProductService productService,
            IPictureService pictureService,
            INopFileProvider fileProvider,
            IStoreContext storeContext,
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService,
            IWebHostEnvironment webHostEnvironment,
            IRepository<ProductPicture> productPictureRepository,
            IRepository<PictureBinary> pictureBinaryRepository,
            ILogger logger,
            IObjectConverter objectConverter,
            ILogger<ScheduledJobBase<object>> jobLogger)
            : base(settingService,
                storeService,
                storeContext,
                storeMappingService,
                urlRecordService,
                logger,
                objectConverter,
                jobLogger)
        {
            _categoryService = categoryService;
            _productService = productService;
            _pictureService = pictureService;
            _fileProvider = fileProvider;
            _webHostEnvironment = webHostEnvironment;
            _productPictureRepository = productPictureRepository;
            _pictureBinaryRepository = pictureBinaryRepository;

            _productCodeFileNamePaths = new Dictionary<string, string>();
            _multipleProductCodes = new Dictionary<string, string>();
            _productCodeRegex =  new Regex(@"^\d+-\d+$");
            _filePathRegex = new Regex(@"([0-9]+(-[0-9]+)?)\.[a-zA-Z0-9]+$");
        }

        #endregion

        #region Methods

        protected override void Produce()
        {
            FetchImageData();

            Debug($"Processing pictures for products in holding");
            
            InternalProduce(true);

            Debug($"Completed producing");
        }

        private void FetchImageData()
        {
            _productCodeFileNamePaths.Clear();

            var path = string.IsNullOrWhiteSpace(_archSettings.ProductImagesFilePath)
                ? $"{_webHostEnvironment.WebRootPath}/images/products"
                : _archSettings.ProductImagesFilePath;

            _fileProvider.CreateDirectory(path);

            var allFiles = _fileProvider.EnumerateFiles(path, "*.*", false)
                .Select(i => new FileInfo(i))
                .OrderByDescending(i => i.CreationTime);

            foreach (var file in allFiles)
            {
                var name = file.Name.Substring(0, file.Name.Length - file.Extension.Length);

                if (!_productCodeFileNamePaths.ContainsKey(name))
                {
                    _productCodeFileNamePaths.Add(name, file.FullName);
                }
            }
        }

        private void InternalProduce(bool isInHolding)
        {
            var take = 1000;
            var page = 0;
            bool hasRecords;
            try
            {
                do
                {
                    Debug($"Fetching products / take:{take} / page:{page}");
                    var productCodes = new List<string>();

                    foreach (KeyValuePair<string, string> entry in _productCodeFileNamePaths)
                    {
                        var isMatch = _productCodeRegex.IsMatch(entry.Key);

                        if (isMatch)
                        {
                            string[] productCode = entry.Key.Split('-');
                            if (productCode.Length > 0) productCodes.Add(productCode[0]);

                            if (!_multipleProductCodes.ContainsKey(entry.Key)) 
                                _multipleProductCodes.Add(entry.Key, entry.Value);
                        }
                        else
                        {
                            productCodes.Add(entry.Key);
                        }

                    }

                    var productsPaged = _productService.GetProductsByProductCodes(productCodes, page, take);
                    var count = productsPaged.Count;

                    for (var i = 0; i < count; i++)
                    {
                        var item = productsPaged[i];
                        Consume(item);
                    }

                    page++;
                    hasRecords = page < productsPaged.TotalPages;
                } while (hasRecords);
            }
            catch (Exception ex) 
            {
                Log("ProductImageUpdateJob Exception", ex, LogLevel.Error);
            }
            
        }

        private bool ProductHasPicture(int productId, string productName)
        {
            var pictures = _pictureService.GetPicturesByProductId(productId);
            var productPicture = pictures.FirstOrDefault(i => i.TitleAttribute == productName);
            var hasPicture = productPicture != null;
            return hasPicture;
        }

        private void UploadImageFromSharedBaseCodeProduct(Product product, string path, int displayOrder = 0)
        {
            var picture = LoadPicture(product, path);

            _pictureService.UpdatePicture(picture.Id,
                _pictureService.LoadPictureBinary(picture),
                picture.MimeType,
                picture.SeoFilename,
                product.Name,
                product.Name);

            _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(product.Name));

            _productService.InsertProductPicture(new ProductPicture
            {
                PictureId = picture.Id,
                ProductId = product.Id,
                DisplayOrder = displayOrder
            });

            _productService.CalculateIsInHolding(product);
            _productService.UpdateProduct(product);
        }

        private void UploadImageForProduct(Product product, string path, int displayOrder = 0)
        {
            var picture = LoadPicture(product, path);

            Picture productPicture = null;
            var pictures = _pictureService.GetPicturesByProductId(product.Id);
            productPicture = pictures.FirstOrDefault(i => i.TitleAttribute == product.Name);
            if (productPicture != null)
            {
                _pictureService.UpdatePicture(picture.Id,
                    _pictureService.LoadPictureBinary(picture),
                    picture.MimeType,
                    picture.SeoFilename,
                    product.Name,
                    product.Name);

                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(product.Name));

                _productService.InsertProductPicture(new ProductPicture
                {
                    PictureId = picture.Id,
                    ProductId = product.Id,
                    DisplayOrder = displayOrder
                });
            }
            else
            {
                _pictureService.UpdatePicture(picture.Id,
                    _pictureService.LoadPictureBinary(picture),
                    picture.MimeType,
                    picture.SeoFilename,
                    product.Name,
                    product.Name);

                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(product.Name));

                _productService.InsertProductPicture(new ProductPicture
                {
                    PictureId = picture.Id,
                    ProductId = product.Id,
                    DisplayOrder = displayOrder
                });
            }

            _productService.CalculateIsInHolding(product);
            _productService.UpdateProduct(product);
        }

        protected override void Consume(Product item)
        {
            Debug($"Consuming {item.Name}");

            if (_productCodeFileNamePaths.ContainsKey(item.ProductCodeField))
                UploadImageForProduct(item, _productCodeFileNamePaths[item.ProductCodeField]);

            foreach (KeyValuePair<string, string> entry in _multipleProductCodes)
            {
                if (entry.Key.Split('-')[0] == item.ProductCodeField)
                {
                    var displayOrder = int.Parse(entry.Key.Split('-')[1]);
                    UploadImageForProduct(item, _productCodeFileNamePaths[entry.Key], displayOrder);
                }
            }

                // if no picture is found, then try load from a shared base code product that has a picture
                if (!ProductHasPicture(item.Id, item.Name))
            {
                // fetch shared products with the same base code that have pictures
                // with priority set from smallest pack size to largest
                var sharedBaseCodeProductWithPicture = _productService.GetProductsByCodes(item.BaseCodeField)
                    .Where(i => i.Id != item.Id && ProductHasPicture(i.Id, i.Name))
                    .OrderBy(i => i.PackSizeField)
                    .FirstOrDefault();

                if (sharedBaseCodeProductWithPicture != null &&
                    _productCodeFileNamePaths.ContainsKey(sharedBaseCodeProductWithPicture.ProductCodeField))
                {
                    UploadImageFromSharedBaseCodeProduct(item, _productCodeFileNamePaths[sharedBaseCodeProductWithPicture.ProductCodeField]);
                }

                foreach (KeyValuePair<string, string> entry in _multipleProductCodes)
                {
                    if (sharedBaseCodeProductWithPicture != null && entry.Key.Split('-')[0] == sharedBaseCodeProductWithPicture.ProductCodeField)
                    {
                        var displayOrder = int.Parse(entry.Key.Split('-')[1]);
                        UploadImageFromSharedBaseCodeProduct(item, _productCodeFileNamePaths[entry.Key], displayOrder);
                    }
                }
            }

            // if still no picture is found, then try load matching the base code as the picture name
            if (!ProductHasPicture(item.Id, item.Name) && _productCodeFileNamePaths.ContainsKey(item.BaseCodeField))
            {
                UploadImageForProduct(item, _productCodeFileNamePaths[item.BaseCodeField]);
            }

            foreach (KeyValuePair<string, string> entry in _multipleProductCodes)
            {
                if (!ProductHasPicture(item.Id, item.Name) && entry.Key.Split('-')[0] == item.BaseCodeField)
                {
                    var displayOrder = int.Parse(entry.Key.Split('-')[1]);
                    UploadImageForProduct(item, _productCodeFileNamePaths[entry.Key], displayOrder);
                }
            }

            Debug($"Completed processing {item.Name}");
        }

        private Picture LoadPicture(Product product, string picturePath, int? picId = null)
        {
            var pictureName = "";
            var match = _filePathRegex.Match(picturePath);
            if (match.Success) { pictureName = match.Groups[1].Value; }

            if (string.IsNullOrEmpty(picturePath) || !_fileProvider.FileExists(picturePath) || product == null)
                return null;

            //set to jpeg in case mime type cannot be found
            new FileExtensionContentTypeProvider().TryGetContentType(picturePath, out var mimeType);
            mimeType ??= MimeTypes.ImageJpeg;

            var picturePathBinary = _fileProvider.ReadAllBytes(picturePath);
            var pictureAlreadyExists = false;
            if (picId != null)
            {
                //compare with existing product pictures
                var existingPicture = _pictureService.GetPictureById(picId.Value);
                if (existingPicture != null)
                {
                    var existingBinary = _pictureService.LoadPictureBinary(existingPicture);
                    //picture binary after validation (like in database)
                    var validatedPictureBinary = _pictureService.ValidatePicture(picturePathBinary, mimeType);
                    if (existingBinary.SequenceEqual(validatedPictureBinary) ||
                        existingBinary.SequenceEqual(picturePathBinary))
                    {
                        pictureAlreadyExists = true;
                    }
                }
            }

            if (pictureAlreadyExists)
                return null;

            var newPicture = _pictureService.InsertPicture(picturePathBinary, mimeType, _pictureService.GetPictureSeName(product.Name));

            if (newPicture != null && !_productCodeRegex.IsMatch(pictureName))
            {
                var pictures = _pictureService.GetPicturesByProductId(product.Id);
                foreach (var picture in pictures.Where(x => x.Id != newPicture.Id))
                {
                    if (picture.SeoFilename == newPicture.SeoFilename)
                    {
                        var productPictureRecord = (from item in _productPictureRepository.Table where picture.Id == item.PictureId select item).FirstOrDefault();
                        var pictureBinaryRecord = (from item in _pictureBinaryRepository.Table where item.PictureId == picture.Id select item).FirstOrDefault();

                        if (productPictureRecord != null)
                        {
                            _productPictureRepository.Delete(productPictureRecord);
                        }

                        _pictureService.DeletePicture(picture);

                        if (pictureBinaryRecord != null)
                        {
                            _pictureBinaryRepository.Delete(pictureBinaryRecord);
                        }

                        break;
                    }
                }
            }

            return newPicture;
        }

        protected override void CollectData() { }
        #endregion
    }
}
