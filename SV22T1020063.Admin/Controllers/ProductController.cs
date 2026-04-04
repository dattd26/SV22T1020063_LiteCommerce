using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020063.BusinessLayers;
using SV22T1020063.Models.Catalog;
using SV22T1020063.Models.Common;

namespace SV22T1020063.Admin.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private const string PRODUCT_SEARCH = "ProductSearchInput";
        public async Task<IActionResult> Index()
        {
            var input = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH);
            if (input == null)
            {
                input = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = ApplicationContext.PageSize,
                    SearchValue = string.Empty,
                    CategoryID = 0,
                    SupplierID = 0,
                    MinPrice = 0,
                    MaxPrice = 0
                };
            }
            ViewBag.Categories = (await CatalogDataService.ListCategoriesAsync(new PaginationSearchInput() { Page = 1, PageSize = 0 })).DataItems;
            ViewBag.Suppliers = (await PartnerDataService.ListSuppliersAsync(new PaginationSearchInput() { Page = 1, PageSize = 0 })).DataItems;
            return View(input);
        }
        public async Task<IActionResult> Search(ProductSearchInput input)
        {
            var result = await CatalogDataService.ListProductsAsync(input);
            ApplicationContext.SetSessionData(PRODUCT_SEARCH, input);
            return View(result);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Title = "Bổ sung mặt hàng";
            var model = new Product()
            {
                ProductID = 0,
                IsSelling = true,
                Photo = "nophoto.png"
            };
            await LoadDataForView();
            return View("Edit", model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "Chỉnh sửa mặt hàng";
            var model = await CatalogDataService.GetProductAsync(id);
            if (model == null)
                return RedirectToAction("Index");

            await LoadDataForView();
            ViewBag.Photos = await CatalogDataService.ListPhotosAsync(id);
            ViewBag.Attributes = await CatalogDataService.ListAttributesAsync(id);
            return View(model);
        }

        private async Task LoadDataForView()
        {
            ViewBag.Categories = (await CatalogDataService.ListCategoriesAsync(new PaginationSearchInput() { Page = 1, PageSize = 0 })).DataItems;
            ViewBag.Suppliers = (await PartnerDataService.ListSuppliersAsync(new PaginationSearchInput() { Page = 1, PageSize = 0 })).DataItems;
        }

        [HttpPost]
        public async Task<IActionResult> Save(Product data, IFormFile? uploadPhoto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data.ProductName))
                    ModelState.AddModelError(nameof(data.ProductName), "Tên mặt hàng không được để trống");
                if (data.CategoryID <= 0)
                    ModelState.AddModelError(nameof(data.CategoryID), "Vui lòng chọn loại hàng");
                if (data.SupplierID <= 0)
                    ModelState.AddModelError(nameof(data.SupplierID), "Vui lòng chọn nhà cung cấp");
                if (string.IsNullOrWhiteSpace(data.Unit))
                    ModelState.AddModelError(nameof(data.Unit), "Đơn vị tính không được để trống");
                if (data.Price < 0)
                    ModelState.AddModelError(nameof(data.Price), "Giá bán không hợp lệ");

                if (!ModelState.IsValid)
                {
                    ViewBag.Title = data.ProductID == 0 ? "Bổ sung mặt hàng" : "Chỉnh sửa mặt hàng";
                    await LoadDataForView();
                    return View("Edit", data);
                }

                if (uploadPhoto != null)
                {
                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(uploadPhoto.FileName)}";
                    string filePath = Path.Combine(ApplicationContext.WWWRootPath, "images/products", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await uploadPhoto.CopyToAsync(stream);
                    }
                    data.Photo = fileName;
                }

                if (data.ProductID == 0)
                {
                    int id = await CatalogDataService.AddProductAsync(data);
                    return RedirectToAction("Edit", new { id });
                }
                else
                {
                    await CatalogDataService.UpdateProductAsync(data);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Title = data.ProductID == 0 ? "Bổ sung mặt hàng" : "Chỉnh sửa mặt hàng";
                await LoadDataForView();
                return View("Edit", data);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (Request.Method == "POST")
            {
                await CatalogDataService.DeleteProductAsync(id);
                return RedirectToAction("Index");
            }
            var model = await CatalogDataService.GetProductAsync(id);
            if (model == null)
                return RedirectToAction("Index");
            ViewBag.CanDelete = !await CatalogDataService.IsUsedProductAsync(id);
            return View(model);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var model = await CatalogDataService.GetProductAsync(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

        public async Task<IActionResult> ListAttributes(int pid)
        {
            var model = await CatalogDataService.ListAttributesAsync(pid);
            return View(model);
        }

        public IActionResult CreateAttribute(int pid)
        {
            ViewBag.Title = "Bổ sung thuộc tính";
            var model = new ProductAttribute()
            {
                AttributeID = 0,
                ProductID = pid
            };
            return View("EditAttribute", model);
        }

        public async Task<IActionResult> EditAttribute(int pid, int attributeId)
        {
            ViewBag.Title = "Thay đổi thuộc tính";
            var model = await CatalogDataService.GetAttributeAsync(attributeId);
            if (model == null)
                return RedirectToAction("Edit", new { id = pid });
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAttribute(ProductAttribute data)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data.AttributeName))
                    ModelState.AddModelError(nameof(data.AttributeName), "Tên thuộc tính không được để trống");
                if (string.IsNullOrWhiteSpace(data.AttributeValue))
                    ModelState.AddModelError(nameof(data.AttributeValue), "Giá trị thuộc tính không được để trống");
                if (data.DisplayOrder <= 0)
                    ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị không hợp lệ");

                if (!ModelState.IsValid)
                {
                    ViewBag.Title = data.AttributeID == 0 ? "Bổ sung thuộc tính" : "Thay đổi thuộc tính";
                    return View("EditAttribute", data);
                }

                if (data.AttributeID == 0)
                    await CatalogDataService.AddAttributeAsync(data);
                else
                    await CatalogDataService.UpdateAttributeAsync(data);

                return RedirectToAction("Edit", new { id = data.ProductID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("EditAttribute", data);
            }
        }

        public async Task<IActionResult> DeleteAttribute(int pid, int attributeId)
        {
            await CatalogDataService.DeleteAttributeAsync(attributeId);
            return RedirectToAction("Edit", new { id = pid });
        }

        public async Task<IActionResult> ListPhotos(int pid)
        {
            var model = await CatalogDataService.ListPhotosAsync(pid);
            return View(model);
        }

        public IActionResult CreatePhoto(int pid)
        {
            ViewBag.Title = "Bổ sung ảnh";
            var model = new ProductPhoto()
            {
                PhotoID = 0,
                ProductID = pid,
                Photo = "nophoto.png"
            };
            return View("EditPhoto", model);
        }

        public async Task<IActionResult> EditPhoto(int pid, int photoId)
        {
            ViewBag.Title = "Thay đổi ảnh";
            var model = await CatalogDataService.GetPhotoAsync(photoId);
            if (model == null)
                return RedirectToAction("Edit", new { id = pid });
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SavePhoto(ProductPhoto data, IFormFile? uploadPhoto)
        {
            try
            {
                if (data.DisplayOrder <= 0)
                    ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị không hợp lệ");

                if (uploadPhoto == null && data.PhotoID == 0)
                    ModelState.AddModelError(nameof(data.Photo), "Vui lòng chọn ảnh");

                if (!ModelState.IsValid)
                {
                    ViewBag.Title = data.PhotoID == 0 ? "Bổ sung ảnh" : "Thay đổi ảnh";
                    return View("EditPhoto", data);
                }

                if (uploadPhoto != null)
                {
                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(uploadPhoto.FileName)}";
                    string filePath = Path.Combine(ApplicationContext.WWWRootPath, "images/products", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await uploadPhoto.CopyToAsync(stream);
                    }
                    data.Photo = fileName;
                }

                if (data.PhotoID == 0)
                    await CatalogDataService.AddPhotoAsync(data);
                else
                    await CatalogDataService.UpdatePhotoAsync(data);

                return RedirectToAction("Edit", new { id = data.ProductID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("EditPhoto", data);
            }
        }

        public async Task<IActionResult> DeletePhoto(int pid, int photoId)
        {
            await CatalogDataService.DeletePhotoAsync(photoId);
            return RedirectToAction("Edit", new { id = pid });
        }
    }
}
