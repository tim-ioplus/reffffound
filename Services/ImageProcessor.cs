using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;

public static class ImageProcessingSync
{
    // 1. Fetch image from URL or local path (synchronous)
    public static Image FetchImage(string imageSource)
    {
        if (imageSource.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            // Fetch from URL synchronously
            using (var webClient = new WebClient())
            {
                byte[] imageData = webClient.DownloadData(imageSource);
                using (var stream = new MemoryStream(imageData))
                {
                    return Image.FromStream(stream);
                }
            }
        }
        else
        {
            // Load from local file
            if (!File.Exists(imageSource))
            {
                throw new FileNotFoundException($"Image file not found: {imageSource}");
            }
            return Image.FromFile(imageSource);
        }
    }

    // 2. Resize image to 500px width while maintaining aspect ratio
    public static Image ResizeImage(Image originalImage, int targetWidth = 500)
    {
        if (originalImage.Width <= targetWidth)
        {
            return new Bitmap(originalImage); // Return copy to avoid modifying original
        }

        int originalWidth = originalImage.Width;
        int originalHeight = originalImage.Height;
        int targetHeight = (int)((float)originalHeight / originalWidth * targetWidth);

        var resizedImage = new Bitmap(targetWidth, targetHeight);
        using (var graphics = Graphics.FromImage(resizedImage))
        {
            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graphics.DrawImage(originalImage, 0, 0, targetWidth, targetHeight);
        }

        return resizedImage;
    }

    // 3. Convert image to Data URL
    public static string ConvertToDataUrl(Image image, ImageFormat format = null)
    {
        format ??= ImageFormat.Png; // Default to PNG for transparency support

        using (var memoryStream = new MemoryStream())
        {
            image.Save(memoryStream, format);
            var imageBytes = memoryStream.ToArray();
            var base64String = Convert.ToBase64String(imageBytes);
            return $"data:image/{format.ToString().ToLower()};base64,{base64String}";
        }
    }

    // New method to combine multiple images vertically
    public static Image CombineImagesVertically(IEnumerable<Image> images)
    {
        if (!images.Any())
            throw new ArgumentException("No images provided to combine");

        // Calculate total height
        int totalHeight = images.Sum(img => img.Height);
        int maxWidth = images.Max(img => img.Width);

        // Create the combined image
        var combinedImage = new Bitmap(maxWidth, totalHeight);

        using (var graphics = Graphics.FromImage(combinedImage))
        {
            int currentY = 0;
            foreach (var image in images)
            {
                graphics.DrawImage(image, 0, currentY);
                currentY += image.Height;
            }
        }

        return combinedImage;
    }

    // Combined function for easy use with comma-separated URLs
    public static string ProcessImageListToDataUrl(string commaSeparatedImageUrls, int targetWidth = 500)
    {
        if (string.IsNullOrWhiteSpace(commaSeparatedImageUrls))
            throw new ArgumentException("No image URLs provided");

        // Split the comma-separated string into individual URLs
        var imageUrls = commaSeparatedImageUrls.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(url => url.Trim())
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .ToList();

        if (imageUrls.Count == 0)
            throw new ArgumentException("No valid image URLs found in the input string");

        // Download and resize all images
        var images = new List<Image>();
        foreach (var url in imageUrls)
        {
            try
            {
                using (var originalImage = FetchImage(url))
                {
                    var resizedImage = targetWidth > 0 && originalImage.Width != targetWidth
                        ? ResizeImage(originalImage, targetWidth)
                        : new Bitmap(originalImage); // Return copy if no resize needed

                    images.Add(resizedImage);
                }
            }
            catch (Exception ex)
            {
                // Log error but continue with other images
                Console.WriteLine($"Error processing image {url}: {ex.Message}");
            }
        }

        if (images.Count == 0)
			{
				throw new Exception("Failed to download any images");
			}
            

        // Combine all images vertically
        using (var combinedImage = CombineImagesVertically(images))
        {
            // Convert to Data URL
            return ConvertToDataUrl(combinedImage);
        }
    }
}
