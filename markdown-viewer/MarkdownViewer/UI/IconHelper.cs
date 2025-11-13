using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace MarkdownViewer.UI
{
    /// <summary>
    /// Helper class for creating simple icons using GDI+ graphics.
    /// Much more reliable than SVG rendering.
    /// </summary>
    public static class IconHelper
    {
        /// <summary>
        /// Creates icons by drawing them with GDI+ graphics
        /// </summary>
        public static Bitmap? CreateIcon(string iconName, int size = 20, Color? color = null)
        {
            try
            {
                var iconColor = color ?? Color.FromArgb(64, 64, 64);
                var bitmap = new Bitmap(size, size);

                using (var g = Graphics.FromImage(bitmap))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.Clear(Color.Transparent);

                    using (var pen = new Pen(iconColor, 2))
                    {
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;
                        pen.LineJoin = LineJoin.Round;

                        float margin = size * 0.15f;
                        float center = size / 2f;
                        float radius = (size - margin * 2) / 2f;

                        switch (iconName.ToLower())
                        {
                            case "refresh-cw":
                                // Circular arrow
                                g.DrawArc(pen, margin, margin, size - margin * 2, size - margin * 2, 45, 270);
                                // Arrow head
                                g.DrawLine(pen, size - margin - 3, margin + 2, size - margin, margin);
                                g.DrawLine(pen, size - margin, margin, size - margin - 2, margin + 3);
                                break;

                            case "check-circle":
                                // Circle
                                g.DrawEllipse(pen, margin, margin, size - margin * 2, size - margin * 2);
                                // Check mark
                                pen.Width = 2.5f;
                                g.DrawLine(pen, center - radius * 0.4f, center, center - radius * 0.1f, center + radius * 0.4f);
                                g.DrawLine(pen, center - radius * 0.1f, center + radius * 0.4f, center + radius * 0.5f, center - radius * 0.3f);
                                break;

                            case "alert-circle":
                                // Circle
                                g.DrawEllipse(pen, margin, margin, size - margin * 2, size - margin * 2);
                                // Exclamation mark
                                pen.Width = 2.5f;
                                g.DrawLine(pen, center, center - radius * 0.5f, center, center + radius * 0.1f);
                                g.FillEllipse(new SolidBrush(iconColor), center - 1.5f, center + radius * 0.3f, 3, 3);
                                break;

                            case "folder":
                                // Folder shape
                                var folderRect = new RectangleF(margin, margin + size * 0.25f, size - margin * 2, size * 0.6f);
                                g.DrawRectangle(pen, folderRect.X, folderRect.Y, folderRect.Width, folderRect.Height);
                                // Folder tab
                                g.DrawLine(pen, folderRect.X, folderRect.Y, folderRect.X + folderRect.Width * 0.4f, folderRect.Y);
                                g.DrawLine(pen, folderRect.X + folderRect.Width * 0.4f, folderRect.Y, folderRect.X + folderRect.Width * 0.5f, margin);
                                g.DrawLine(pen, folderRect.X + folderRect.Width * 0.5f, margin, folderRect.X + folderRect.Width * 0.7f, margin);
                                break;

                            case "globe":
                                // Circle
                                g.DrawEllipse(pen, margin, margin, size - margin * 2, size - margin * 2);
                                // Vertical line
                                g.DrawLine(pen, center, margin, center, size - margin);
                                // Horizontal line
                                g.DrawLine(pen, margin, center, size - margin, center);
                                // Curved lines
                                g.DrawArc(pen, margin + radius * 0.3f, margin, radius * 1.4f, size - margin * 2, -90, 180);
                                break;

                            case "info":
                                // Circle
                                g.DrawEllipse(pen, margin, margin, size - margin * 2, size - margin * 2);
                                // i letter
                                g.FillEllipse(new SolidBrush(iconColor), center - 1.5f, center - radius * 0.5f, 3, 3);
                                pen.Width = 2.5f;
                                g.DrawLine(pen, center, center - radius * 0.2f, center, center + radius * 0.5f);
                                break;

                            case "help-circle":
                                // Circle
                                g.DrawEllipse(pen, margin, margin, size - margin * 2, size - margin * 2);
                                // Question mark
                                var qArc = new RectangleF(center - radius * 0.35f, center - radius * 0.5f, radius * 0.7f, radius * 0.7f);
                                g.DrawArc(pen, qArc, -180, 180);
                                g.DrawLine(pen, center, center + radius * 0.2f, center, center + radius * 0.3f);
                                g.FillEllipse(new SolidBrush(iconColor), center - 1.5f, center + radius * 0.45f, 3, 3);
                                break;

                            case "file-text":
                                // Document rectangle
                                var docRect = new RectangleF(margin + 2, margin, size - margin * 2 - 4, size - margin * 2);
                                g.DrawRectangle(pen, docRect.X, docRect.Y, docRect.Width, docRect.Height);
                                // Text lines inside document
                                pen.Width = 1.5f;
                                float lineY = docRect.Y + docRect.Height * 0.3f;
                                g.DrawLine(pen, docRect.X + 3, lineY, docRect.X + docRect.Width - 3, lineY);
                                lineY += docRect.Height * 0.2f;
                                g.DrawLine(pen, docRect.X + 3, lineY, docRect.X + docRect.Width - 3, lineY);
                                lineY += docRect.Height * 0.2f;
                                g.DrawLine(pen, docRect.X + 3, lineY, docRect.X + docRect.Width * 0.6f, lineY);
                                break;

                            default:
                                // Default: simple circle
                                g.DrawEllipse(pen, margin, margin, size - margin * 2, size - margin * 2);
                                break;
                        }
                    }
                }

                return bitmap;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Failed to create icon: {IconName}", iconName);
                return null;
            }
        }

        /// <summary>
        /// Legacy method name for compatibility
        /// </summary>
        public static Bitmap? LoadIcon(string iconName, int size = 20, Color? color = null)
        {
            return CreateIcon(iconName, size, color);
        }
    }
}
