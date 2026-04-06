namespace SV22T1020063.Models.Common
{
    /// <summary>
    /// Represents a single item in a breadcrumb trail.
    /// </summary>
    public class BreadcrumbItem
    {
        public string Text { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public bool IsActive { get; set; } = false;
    }
}
