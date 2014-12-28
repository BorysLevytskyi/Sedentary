using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using Sedentary.Model;

namespace Sedentary.Framework
{
	public static class IconProvider
	{
		private static readonly Brush WhiteBrush = new SolidBrush(Color.White);
		private static readonly Dictionary<IconConfig, Bitmap> IconCache = new Dictionary<IconConfig, Bitmap>();

		public static Bitmap GetIconImage(IconConfig cfg)
		{
			Bitmap icon;
			if (IconCache.TryGetValue(cfg, out icon))
			{
				Tracer.Write("Icon used from cache");
				return icon;
			}

			IconCache.Add(cfg, icon = DrawIcon(cfg));
			return icon;
		}

		private static Bitmap DrawIcon(IconConfig config)
		{
			string path = GetIconPath(config.WorkState);
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path))
			{
				if (stream == null)
				{
					throw new Exception(string.Format("{0} embedded resource wasn't found", path));
				}

				using (var iconOrig = new Bitmap(stream))
				{
					var icon = new Bitmap(16, 16);

					using (Graphics graphics = Graphics.FromImage(icon))
					{
						graphics.FillRectangle(WhiteBrush, 0, 0, 16, 16);
						graphics.DrawImage(iconOrig, 0, 0, 16, 16);
						
						if (config.OverlayHeight > 0)
						{
							int height = Math.Min(Math.Max(0, config.OverlayHeight), 16);
							var rectangle = new Rectangle(0, 16 - height, 16, height);
							graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, config.OverlayColor)), rectangle);
						}
						
						graphics.Flush();

						Tracer.Write("New icon drawn");

						return icon;
					}
				}
			}
		}

		private static string GetIconPath(WorkState state)
		{
			switch (state)
			{
				case WorkState.Away:
					return "Sedentary.away.png";
				case WorkState.Standing:
					return "Sedentary.standing.jpg";
				default:
					return "Sedentary.sitting.png";
			}
		}
	}
}