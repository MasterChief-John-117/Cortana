using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Cortana.Utilities
{
    public class ImageUtils
    {
        public IEnumerable<Color> GetPixels(Bitmap bitmap, CommandHandler cmd, int inc)
        {
            for (int x = 0; x < bitmap.Width; x += inc)
            {
                for (int y = 0; y < bitmap.Height; y += inc)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    yield return pixel;
                    cmd.Iterator++;
                }
            }
        }

        public Color GetAverageColor(Bitmap bitmap, CommandHandler cmd, int tolerance)
        {
            {
                var colorsWithCount =
                    GetPixels(bitmap, cmd, 10)
                        .GroupBy(color => color)
                        .Select(grp =>
                            new
                            {
                                Color = grp.Key,
                                Count = grp.Count()
                            })
                        .OrderByDescending(x => x.Count)
                        .Where(c => c.Color.A > 100)
                        .Where(rgb => rgb.Color.R < (255 - tolerance - 2) || rgb.Color.G < (255 - tolerance - 2) || rgb.Color.B < (255 - tolerance - 2))
                        .Take(100);
                Dictionary<Color, int> colors = new Dictionary<Color, int>();
                Dictionary<Color, int> finalColors = new Dictionary<Color, int>();
                foreach (var color in colorsWithCount)
                {
                    colors.Add(color.Color, color.Count);
                    cmd.Iterator++;
                }
                foreach (var color in colors)
                {
                    finalColors.Add(color.Key, color.Value);
                    for (int i = 0; i < colors.Count; i++)
                    {
                        var c = colors.Keys.ElementAt(i);
                        if (Math.Abs(color.Key.R - c.R) < tolerance && Math.Abs(color.Key.G - c.G) < tolerance &&
                            Math.Abs(color.Key.B - c.B) < tolerance)
                        {
                            finalColors[color.Key] += colors.Values.ElementAt(i);
                        }
                        cmd.Iterator++;
                    }
                }
                var final = finalColors.OrderByDescending(pair => pair.Value).Take(1).First().Key;
                
                return final;
            }
        }
    }
}